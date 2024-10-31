using SofarBMS.Helper;
using SofarBMS.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SofarBMS.UI
{
    public partial class CBSControl : UserControl
    {
        public CBSControl()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
        }

        string[] packSN = new string[3];

        int initCount = 0;
        RealtimeData_CBS5000S model = null;
        EcanHelper ecanHelper = EcanHelper.Instance;

        public static CancellationTokenSource cts = null;

        private void RTAControl_Load(object sender, EventArgs e)
        {
            //多语言翻译
            foreach (Control item in this.Controls)
            {
                GetControls(item);
            }

            Task.Run(async delegate
            {
                while (!cts.IsCancellationRequested)
                {
                    if (ecanHelper.IsConnection)
                    {
                        if (model != null && initCount >= 26)
                        {
                            var filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}//Log//CBS5000_{DateTime.Now.ToString("yyyy-MM-dd")}.csv";

                            if (!File.Exists(filePath))
                            {
                                File.AppendAllText(filePath, model.GetHeader() + "\r\n");
                            }
                            File.AppendAllText(filePath, model.GetValue() + "\r\n");
                            initCount = 0;
                            model = null;
                        }

                        while (EcanHelper._task.Count > 0
                            && !cts.IsCancellationRequested)
                        {
                            CAN_OBJ ch = (CAN_OBJ)EcanHelper._task.Dequeue();

                            this.Invoke(new Action(() => { AnalysisData(ch.ID, ch.Data); }));
                        }

                        //读取BMS序列号
                        ecanHelper.Send(new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                       , new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 });

                        await Task.Delay(500);

                        //获取实时数据指令
                        ecanHelper.Send(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                       , new byte[] { 0xE0, FrmMain.BMS_ID, 0x2C, 0x10 });

                        //定时一秒存储一次数据
                        await Task.Delay(500);
                    }
                }
            }, cts.Token);
        }


        public void AnalysisData(uint canID, byte[] data)
        {
            if ((canID & 0xff) != FrmMain.BMS_ID)
                return;

            if (model == null)
                model = new RealtimeData_CBS5000S();

            string[] strs;
            string[] controls;

            try
            {
                switch (canID | 0xff)
                {
                    case 0x1004FFFF:
                    case 0x1004E0FF:
                        initCount++;
                        strs = new string[4] { "0.1", "0.1", "0.01", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtBatteryVolt", "txtLoadVolt", "txtBatteryCurrent", "txtSOC" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.BatteryVolt = Convert.ToDouble(strs[0]);
                        model.LoadVolt = Convert.ToDouble(strs[1]);
                        model.BatteryCurrent = Convert.ToDouble(strs[2]);
                        model.SOC = Convert.ToDouble(strs[3]);
                        break;
                    case 0x1005FFFF:
                    case 0x1005E0FF:
                        initCount++;
                        strs = new string[5];
                        strs[0] = BytesToIntger(data[1], data[0]);
                        strs[1] = BytesToIntger(0x00, data[2]);
                        strs[2] = BytesToIntger(data[4], data[3]);
                        strs[3] = BytesToIntger(0x00, data[5]);
                        strs[4] = (Convert.ToInt32(strs[0]) - Convert.ToInt32(strs[2])).ToString();

                        controls = new string[5] { "txtBatMaxCellVolt", "txtBatMaxCellVoltNum", "txtBatMinCellVolt", "txtBatMinCellVoltNum", "txtBatDiffCellVolt" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.BatMaxCellVolt = Convert.ToUInt16(strs[0]);
                        model.BatMaxCellVoltNum = Convert.ToUInt16(strs[1]);
                        model.BatMinCellVolt = Convert.ToUInt16(strs[2]);
                        model.BatMinCellVoltNum = Convert.ToUInt16(strs[3]);
                        model.BatDiffCellVolt = Convert.ToUInt16(strs[4]);
                        break;
                    case 0x1006FFFF:
                    case 0x1006E0FF:
                        initCount++;
                        strs = new string[4] { "0.1", "1", "0.1", "1" };
                        strs[0] = BytesToIntger(data[1], data[0], 0.1);
                        strs[1] = BytesToIntger(0x00, data[2]);
                        strs[2] = BytesToIntger(data[4], data[3], 0.1);
                        strs[3] = BytesToIntger(0x00, data[5]);

                        controls = new string[4] { "txtBatMaxCellTemp", "txtBatMaxCellTempNum", "txtBatMinCellTemp", "txtBatMinCellTempNum" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.BatMaxCellTemp = Convert.ToDouble(strs[0]);
                        model.BatMaxCellTempNum = Convert.ToUInt16(strs[1]);
                        model.BatMinCellTemp = Convert.ToDouble(strs[2]);
                        model.BatMinCellTempNum = Convert.ToUInt16(strs[3]);
                        break;
                    case 0x1007FFFF:
                    case 0x1007E0FF:
                        initCount++;
                        txtTotalChgCap.Text = (Convert.ToDouble(((data[3] << 24) + (data[2] << 16) + (data[1] << 8) + (data[0] & 0xff)) * 0.001)).ToString();
                        txtTotalDsgCap.Text = (Convert.ToDouble(((data[7] << 24) + (data[6] << 16) + (data[5] << 8) + (data[4] & 0xff)) * 0.001)).ToString();

                        model.TotalChgCap = Convert.ToDouble(txtTotalChgCap.Text);
                        model.TotalDsgCap = Convert.ToDouble(txtTotalDsgCap.Text);
                        break;
                    case 0x1008FFFF:
                    case 0x1008E0FF:
                        initCount++;
                        richTextBox1.Clear(); richTextBox2.Clear(); richTextBox3.Clear();
                        analysisLog(data, 0);
                        break;
                    case 0x1009FFFF:
                    case 0x1009E0FF:
                        initCount++;
                        strs = new string[4];
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2]);
                        }

                        controls = new string[4] { "txtCellvoltage1", "txtCellvoltage2", "txtCellvoltage3", "txtCellvoltage4" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.CellVoltage1 = Convert.ToUInt32(strs[0]);
                        model.CellVoltage2 = Convert.ToUInt32(strs[1]);
                        model.CellVoltage3 = Convert.ToUInt32(strs[2]);
                        model.CellVoltage4 = Convert.ToUInt32(strs[3]);
                        break;
                    case 0x100AFFFF:
                    case 0x100AE0FF:
                        initCount++;
                        strs = new string[4];
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2]);
                        }

                        controls = new string[4] { "txtCellvoltage5", "txtCellvoltage6", "txtCellvoltage7", "txtCellvoltage8" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.CellVoltage5 = Convert.ToUInt32(strs[0]);
                        model.CellVoltage6 = Convert.ToUInt32(strs[1]);
                        model.CellVoltage7 = Convert.ToUInt32(strs[2]);
                        model.CellVoltage8 = Convert.ToUInt32(strs[3]);
                        break;
                    case 0x100BFFFF:
                    case 0x100BE0FF:
                        initCount++;
                        strs = new string[4];
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2]);
                        }

                        controls = new string[4] { "txtCellvoltage9", "txtCellvoltage10", "txtCellvoltage11", "txtCellvoltage12" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.CellVoltage9 = Convert.ToUInt32(strs[0]);
                        model.CellVoltage10 = Convert.ToUInt32(strs[1]);
                        model.CellVoltage11 = Convert.ToUInt32(strs[2]);
                        model.CellVoltage12 = Convert.ToUInt32(strs[3]);
                        break;
                    case 0x100CFFFF:
                    case 0x100CE0FF:
                        initCount++;
                        strs = new string[4];
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2]);
                        }

                        controls = new string[4] { "txtCellvoltage13", "txtCellvoltage14", "txtCellvoltage15", "txtCellvoltage16" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.CellVoltage13 = Convert.ToUInt32(strs[0]);
                        model.CellVoltage14 = Convert.ToUInt32(strs[1]);
                        model.CellVoltage15 = Convert.ToUInt32(strs[2]);
                        model.CellVoltage16 = Convert.ToUInt32(strs[3]);
                        break;
                    case 0x100DFFFF:
                    case 0x100DE0FF:
                        initCount++;
                        strs = new string[4] { "0.1", "0.1", "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtCelltemperature1", "txtCelltemperature2", "txtCelltemperature3", "txtCelltemperature4" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.CellTemperature1 = Convert.ToDouble(strs[0]);
                        model.CellTemperature2 = Convert.ToDouble(strs[1]);
                        model.CellTemperature3 = Convert.ToDouble(strs[2]);
                        model.CellTemperature4 = Convert.ToDouble(strs[3]);
                        break;
                    case 0x100EFFFF:
                    case 0x100EE0FF:
                        initCount++;
                        strs = new string[3] { "0.1", "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[3] { "txtMosTemperature", "txtEnvTemperature", "txtSOH" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        string strAdd = "[1~16]:";
                        for (int i = 6; i < 8; i++)
                        {
                            for (short j = 0; j < 8; j++)
                            {
                                if (GetBit(data[i], j) == 1)
                                {
                                    (this.Controls.Find(string.Format("txtCellvoltage{0}", j + 1 + ((i - 6) * 8)), true)[0] as TextBox).BackColor = Color.Aquamarine;
                                    strAdd += "1";
                                }
                                else
                                {
                                    (this.Controls.Find(string.Format("txtCellvoltage{0}", j + 1 + ((i - 6) * 8)), true)[0] as TextBox).BackColor = Color.White;
                                    strAdd = strAdd + "0";
                                }
                                if (j != 0)
                                {
                                    if (j + 1 % 8 == 0)
                                        strAdd += ",";
                                    else if (j + 1 % 4 == 0)
                                        strAdd += " ";
                                }
                            }
                        }

                        model.MosTemperature = Convert.ToDouble(strs[0]);
                        model.EnvTemperature = Convert.ToDouble(strs[1]);
                        model.SOH = Convert.ToDouble(strs[2]);
                        model.EquaState = strAdd;
                        break;
                    case 0x100FFFFF:
                    case 0x100FE0FF:
                        initCount++;
                        txtRemainCap.Text = BytesToIntger(data[1], data[0], 0.1);
                        txtFullCap.Text = BytesToIntger(data[3], data[2], 0.1);
                        txtCycleTIme.Text = BytesToIntger(data[5], data[4]);

                        model.RemainingCapacity = txtRemainCap.Text;
                        model.FullCapacity = txtFullCap.Text;
                        model.CycleTIme = Convert.ToUInt16(txtCycleTIme.Text);
                        break;
                    case 0x1040FFFF:
                    case 0x1040E0FF:
                        initCount++;
                        txtCumulative_discharge_capacity.Text = (((data[3] << 24) + (data[2] << 16) + (data[1] << 8) + (data[0] & 0xff))).ToString();
                        txtCumulative_charge_capacity.Text = (((data[7] << 24) + (data[6] << 16) + (data[5] << 8) + (data[4] & 0xff))).ToString(); ;

                        model.CumulativeDischargeCapacity = txtCumulative_discharge_capacity.Text;
                        model.CumulativeChargeCapacity = txtCumulative_charge_capacity.Text;
                        break;
                    case 0x1041FFFF:
                    case 0x1041E0FF:
                        initCount++;
                        txtBalance_temperature1.Text = BytesToIntger(data[1], data[0], 0.1);
                        txtBalance_temperature2.Text = BytesToIntger(data[3], data[2], 0.1);
                        txtDCDC_temperature1.Text = BytesToIntger(data[5], data[4], 0.1);
                        txtDCDC_temperature2.Text = BytesToIntger(data[7], data[6], 0.1);

                        model.BalanceTemperature1 = txtBalance_temperature1.Text;
                        model.BalanceTemperature2 = txtBalance_temperature2.Text;
                        model.DcdcTemperature1 = txtDCDC_temperature1.Text;
                        model.DcdcTemperature2 = txtDCDC_temperature2.Text;
                        break;
                    case 0x1042FFFF:
                    case 0x1042E0FF:
                        initCount++;
                        strs = new string[4] { "0.1", "0.1", "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtCelltemperature5", "txtCelltemperature6", "txtCelltemperature7", "txtCelltemperature8" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.CellTemperature5 = Convert.ToDouble(strs[0]);
                        model.CellTemperature6 = Convert.ToDouble(strs[1]);
                        model.CellTemperature7 = Convert.ToDouble(strs[2]);
                        model.CellTemperature8 = Convert.ToDouble(strs[3]);
                        break;
                    case 0x1045FFFF:
                    case 0x1045E0FF:
                        initCount++;
                        richTextBox4.Clear(); richTextBox5.Clear(); richTextBox6.Clear();
                        analysisLog(data, 1);
                        break;
                    case 0x104AFFFF:
                    case 0x104AE0FF:
                        initCount++;
                        strs = new string[2] { "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[2] { "txtPowerTemperture1", "txtPowerTemperture2" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.PowerTemperture1 = Convert.ToDouble(txtPowerTemperture1.Text);
                        model.PowerTemperture2 = Convert.ToDouble(txtPowerTemperture2.Text);
                        break;
                    case 0x1027E0FF:
                        string strSn = GetPackSN(data);

                        if (!string.IsNullOrEmpty(strSn))
                            txtSN.Text = strSn;
                        break;
                    case 0x102EE0FF:
                        initCount++;
                        //BMS软件版本
                        string[] bsm_soft = new string[3];
                        for (int i = 0; i < 3; i++)
                        {
                            bsm_soft[i] = data[i + 2].ToString().PadLeft(2, '0');
                        }
                        txtSoftware_Version_Bms.Text = Encoding.ASCII.GetString(new byte[] { data[1] }) + string.Join("", bsm_soft);
                        //BMS硬件版本
                        string[] bsm_HW = new string[2];
                        for (int i = 0; i < 2; i++)
                        {
                            bsm_HW[i] = data[i + 5].ToString().PadLeft(2, '0');
                        }
                        txtHardware_Version_Bms.Text = string.Join("", bsm_HW);

                        model.BMSSoftwareVersion = txtSoftware_Version_Bms.Text;
                        model.BMSHardwareVersion = txtHardware_Version_Bms.Text;
                        break;
                    case 0x1046FFFF:
                    case 0x1046E0FF:
                        initCount++;
                        //加热请求
                        string headRequest = "0:无请求无禁止 1:请求加热 2:禁止加热";
                        switch (data[1] & 0x03)
                        {
                            case 0:
                                headRequest = "NONE";
                                break;
                            case 1:
                                headRequest = "请求加热";
                                break;
                            case 2:
                                headRequest = "禁止加热";
                                break;
                            default:
                                break;
                        }
                        txtHeatRequest.Text = headRequest.ToString();

                        Dictionary<int, string> setContorls_Byte0 = new Dictionary<int, string>() {
                            {0,"pbChargeEnable" },
                            {1,"pbDischargeEnable" },
                            {2,"pbBmuCutOffRequest" },
                            {3,"pbBmuPowOffRequest" },
                            {4,"pbForceChrgRequest" }
                    };
                        Dictionary<int, string> setContorls_Byte2 = new Dictionary<int, string>() {
                            {0,"pbChagreStatus"},
                            {1,"pbDischargeStatus" }
                    };
                        Dictionary<int, string> setContorls_Byte6 = new Dictionary<int, string>() {
                            {0,"pbDiIO" },
                            {1,"pbChargeIO"}
                    };

                        for (short i = 0; i < setContorls_Byte0.Count; i++)
                        {
                            if (GetBit(data[0], i) == 1 && this.Controls.Find(setContorls_Byte0[i], true) != null)
                            {
                                (this.Controls.Find(setContorls_Byte0[i], true)[0] as PictureBox).BackColor = Color.Red;
                            }
                            else
                            {
                                (this.Controls.Find(setContorls_Byte0[i], true)[0] as PictureBox).BackColor = Color.Green;
                            }
                        }
                        for (short i = 0; i < setContorls_Byte2.Count; i++)
                        {
                            if (GetBit(data[2], i) == 1 && this.Controls.Find(setContorls_Byte2[i], true) != null)
                            {
                                (this.Controls.Find(setContorls_Byte2[i], true)[0] as PictureBox).BackColor = Color.Red;
                            }
                            else
                            {
                                (this.Controls.Find(setContorls_Byte2[i], true)[0] as PictureBox).BackColor = Color.Green;
                            }
                        }
                        for (short i = 0; i < setContorls_Byte6.Count; i++)
                        {
                            if (GetBit(data[6], i) == 1 && this.Controls.Find(setContorls_Byte6[i], true) != null)
                            {
                                (this.Controls.Find(setContorls_Byte6[i], true)[0] as PictureBox).BackColor = Color.Red;
                            }
                            else
                            {
                                (this.Controls.Find(setContorls_Byte6[i], true)[0] as PictureBox).BackColor = Color.Green;
                            }
                        }

                        model.ChargeEnable = (ushort)GetBit(data[0], 0);
                        model.DischargeEnable = (ushort)GetBit(data[0], 1);
                        model.BmuCutOffRequest = (ushort)GetBit(data[0], 2);
                        model.BmuPowOffRequest = (ushort)GetBit(data[0], 3);
                        model.ForceChrgRequest = (ushort)GetBit(data[0], 4);
                        model.ChagreStatus = (ushort)GetBit(data[2], 0);
                        model.DischargeStatus = (ushort)GetBit(data[2], 1);
                        model.DiIO = (ushort)GetBit(data[6], 0);
                        model.ChargeIO = (ushort)GetBit(data[6], 1);
                        model.HeatRequest = txtHeatRequest.Text;
                        break;
                    case 0x1047FFFF:
                    case 0x1047E0FF:
                        initCount++;
                        txtSyncFallSoc.Text = Convert.ToInt32(data[0].ToString("X2"), 16).ToString();
                        txtBMSStatus.Text = Enum.Parse(typeof(BMSState), (Convert.ToInt32(data[1].ToString("X2"), 16) & 0x0f).ToString()).ToString();
                        string balanceStatus = "";
                        switch ((Convert.ToInt32(data[2].ToString("X2"), 16) & 0x0f))
                        {
                            case 0: balanceStatus = "禁止"; break;
                            case 1: balanceStatus = "放电"; break;
                            case 2: balanceStatus = "充电"; break;
                            default:
                                break;
                        }
                        txtActiveBalanceStatus.Text = balanceStatus;
                        txtChargeCurrentLimitation.Text = BytesToIntger(data[5], data[4], 0.01);
                        txtDischargeCurrentLimitation.Text = BytesToIntger(data[7], data[6], 0.01);

                        model.SyncFallSoc = txtSyncFallSoc.Text;
                        model.BmsStatus = txtBMSStatus.Text;
                        model.ActiveBalanceStatus = txtActiveBalanceStatus.Text;
                        model.ChargeCurrentLimitation = Convert.ToDouble(txtChargeCurrentLimitation.Text);
                        model.DischargeCurrentLimitation = Convert.ToDouble(txtDischargeCurrentLimitation.Text);
                        break;
                    case 0x1048FFFF:
                    case 0x1048E0FF:
                        initCount++;
                        strs = new string[4] { "0.1", "0.1", "1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtBalanceBusVoltage", "txtBalanceCurrent", "txtActiveBalanceMaxCellVolt", "txtBatAverageTemp" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.BalanceBusVoltage = Convert.ToDouble(strs[0]);
                        model.BalanceCurrent = Convert.ToDouble(strs[1]);
                        model.ActiveBalanceMaxCellVolt = Convert.ToDouble(strs[2]);
                        model.BatAverageTemp = Convert.ToDouble(strs[3]);
                        break;
                    case 0x1049FFFF:
                    case 0x1049E0FF:
                        initCount++;
                        strs = new string[3] { "0.01", "1", "1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[3] { "txtActiveBalanceCellSoc", "txtActiveBalanceAccCap", "txtActiveBalanceRemainCap" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.ActiveBalanceCellSoc = Convert.ToDouble(strs[0]);
                        model.ActiveBalanceAccCap = txtActiveBalanceRemainCap.Text;
                        model.ActiveBalanceRemainCap = txtActiveBalanceRemainCap.Text;
                        break;
                    case 0x106AFFFF:
                    case 0x106AE0FF:
                        initCount++;
                        string[] softwareVersion = new string[3];
                        for (int i = 0; i < 3; i++)
                        {
                            softwareVersion[i] = data[i + 1].ToString().PadLeft(2, '0');
                        }
                        txtBMUSaftwareVersion.Text = Encoding.ASCII.GetString(new byte[] { data[0] }) + string.Join("", softwareVersion);
                        txtBMUCanVersion.Text = Encoding.ASCII.GetString(new byte[] { data[5], data[6] });

                        model.BMUSaftwareVersion = txtBMUSaftwareVersion.Text;
                        model.BMUCanVersion = txtBMUCanVersion.Text;
                        break;
                    case 0x106BFFFF:
                    case 0x106BE0FF:
                        initCount++;
                        strs = new string[3] { "0.1", "1", "1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[3] { "txtBatNominalCapacity", "txtRegisterID", "txtBatType" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.BatNominalCapacity = txtBatNominalCapacity.Text;
                        model.RegisterName = txtRegisterID.Text;
                        model.BatType = txtBatType.Text;
                        break;
                    case 0x106CFFFF:
                    case 0x106CE0FF:
                        initCount++;
                        //厂家信息
                        StringBuilder ManufacturerName = new StringBuilder();
                        ManufacturerName.Append(Encoding.ASCII.GetString(data));
                        txtManufacturerName.Text = ManufacturerName.ToString();

                        model.ManufacturerName = txtManufacturerName.Text;
                        break;
                    case 0x106DFFFF:
                    case 0x106DE0FF:
                        initCount++;
                        strs = new string[3] { "0.001", "0.001", "0.001" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[3] { "txtAuxVolt", "txtChgCurOffsetVolt", "txtDsgCurOffsetVolt" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        txtResetMode.Text = Enum.Parse(typeof(ResetMode), (Convert.ToInt32(data[6].ToString("X2"), 16) & 0x0f).ToString()).ToString();

                        model.ResetMode = txtResetMode.Text;
                        model.AuxVolt = txtAuxVolt.Text;
                        model.ChgCurOffsetVolt = txtChgCurOffsetVolt.Text;
                        model.DsgCurOffsetVolt = txtDsgCurOffsetVolt.Text;
                        break;
                }

                model.PackID = FrmMain.BMS_ID.ToString("X2");
            }
            catch (Exception)
            {

            }
        }

        private string GetPackSN(byte[] data)
        {
            //条形码解析
            switch (data[0])
            {
                case 0:
                    packSN[0] = Encoding.Default.GetString(data).Substring(1);
                    break;
                case 1:
                    packSN[1] = Encoding.Default.GetString(data).Substring(1);
                    break;
                case 2:
                    packSN[2] = Encoding.Default.GetString(data).Substring(1);
                    break;
            }

            //判断sn是否接收完成
            if (packSN[0] != null && packSN[1] != null && packSN[2] != null)
            {
                string strSN = String.Join("", packSN);
                return strSN.Substring(0, strSN.Length - 1);
            }
            else
            {
                return string.Empty;
            }
        }

        public List<int> GetEventList(byte[] responsedata)
        {
            List<int> lists = new List<int>();

            byte[] table = { 0x01, 0x02, 0x04, 0x8, 0x10, 0x20, 0x40, 0x80 };

            int cnt = 0;
            for (int i = 0; i < responsedata.Length; i++)
            {
                if (responsedata[i] != 0x00)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if ((table[j] & responsedata[i]) == table[j])
                        {
                            lists.Add(j + cnt + 1);
                        }
                    }
                }
                cnt += 8;
            }
            return lists;
        }

        private string BytesToIntger(byte high, byte low = 0x00, double unit = 1)
        {
            string value = Convert.ToInt16(high.ToString("X2") + low.ToString("X2"), 16).ToString();
            if (unit == 10)
            {
                value = (long.Parse(value) * 10).ToString();
            }
            else if (unit == 0.1)
            {
                value = (long.Parse(value) / 10.0f).ToString();
            }
            else if (unit == 0.01)
            {
                value = (long.Parse(value) / 100.0f).ToString();
            }
            else if (unit == 0.001)
            {
                value = (long.Parse(value) / 1000.0f).ToString();
            }
            else if (unit == 0.0001)
            {
                value = (long.Parse(value) / 10000).ToString();
            }

            return value;
        }

        #region 翻译所用得函数
        private void GetControls(Control c)
        {
            if (c is GroupBox || c is TabControl)
            {
                c.Text = LanguageHelper.GetLanguage(c.Name.Remove(0, 2));

                foreach (Control item in c.Controls)
                {
                    this.GetControls(item);
                }
            }
            else
            {
                string name = c.Name;

                if (c is CheckBox)
                {
                    c.Text = LanguageHelper.GetLanguage(name.Remove(0, 2));

                    LTooltip(c as CheckBox, c.Text);
                }
                else if (c is RadioButton)
                {
                    c.Text = LanguageHelper.GetLanguage(name.Remove(0, 2));

                    LTooltip(c as RadioButton, c.Text);
                }
                else if (c is Label)
                {
                    c.Text = LanguageHelper.GetLanguage(name.Remove(0, 3));

                    LTooltip(c as Label, c.Text);
                }
                else if (c is Button)
                {
                    if (name.Contains("Set"))
                    {
                        c.Text = LanguageHelper.GetLanguage("Settings");
                    }
                    else if (name.Contains("_Close"))
                    {
                        c.Text = LanguageHelper.GetLanguage("Systemset_43");
                    }
                    else if (name.Contains("_Open"))
                    {
                        c.Text = LanguageHelper.GetLanguage("Systemset_44");
                    }
                    else if (name.Contains("_Lifted"))
                    {
                        c.Text = LanguageHelper.GetLanguage("Systemset_45");
                    }
                    else
                    {
                        c.Text = LanguageHelper.GetLanguage(name.Remove(0, 3));

                    }
                }
                else if (c is TabPage | c is Panel)
                {
                    foreach (Control item in c.Controls)
                    {
                        this.GetControls(item);
                    }
                }
            }
        }

        public static void LTooltip(System.Windows.Forms.Control control, string value)
        {
            if (value.Length == 0) return;
            control.Text = Abbreviation(control, value);
            var tip = new ToolTip();
            tip.IsBalloon = false;
            tip.ShowAlways = true;
            tip.SetToolTip(control, value);
        }

        public static string Abbreviation(Control control, string str)
        {
            if (str == null)
            {
                return null;
            }
            int strWidth = FontWidth(control.Font, control, str);

            //获取label最长可以显示多少字符
            int len = control.Width * str.Length / strWidth;

            if (len > 20 && len < str.Length)

            {
                return str.Substring(0, 20) + "...";
            }
            else
            {
                return str;
            }
        }

        private static int FontWidth(Font font, Control control, string str)
        {
            using (Graphics g = control.CreateGraphics())
            {
                SizeF siF = g.MeasureString(str, font);
                return (int)siF.Width;
            }
        }
        #endregion

        #region BMS发送内部电池故障信息-模块处理
        /// <summary>
        /// 解析日志内容
        /// </summary>
        /// <param name="_bytes"></param>
        /// <returns></returns>
        private void analysisLog(byte[] data, int faultNum)
        {
            string[] msg = new string[2];

            for (int i = 0; i < data.Length; i++)
            {
                for (short j = 0; j < 8; j++)
                {
                    if (GetBit(data[i], j) == 1)
                    {
                        string type = "";
                        getLog(out msg, i, j, faultNum);
                        if (faultNum == 1)
                        {
                            switch (msg[1])
                            {
                                case "1":
                                    type = "告警";
                                    richTextBox6.AppendText(msg[0] + "\r");
                                    model.Warning2 = richTextBox3.Text.Replace("\n", "，").Replace("\r", "，");

                                    break;
                                case "2":
                                    type = "保护";
                                    richTextBox5.AppendText(msg[0] + "\r");
                                    model.Protection2 = richTextBox2.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "3":
                                    type = "故障";
                                    richTextBox4.AppendText(msg[0] + "\r");
                                    model.Fault2 = richTextBox1.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                            }
                        }
                        else
                        {
                            switch (msg[1])
                            {
                                case "1":
                                    type = "告警";
                                    richTextBox3.AppendText(msg[0] + "\r");
                                    model.Warning = richTextBox3.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "2":
                                    type = "保护";
                                    richTextBox2.AppendText(msg[0] + "\r");
                                    model.Protection = richTextBox2.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "3":
                                    type = "故障";
                                    richTextBox1.AppendText(msg[0] + "\r");
                                    model.Fault = richTextBox1.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                            }
                        }

                        var query = FrmMain.AlarmList.FirstOrDefault(t => t.Id == FrmMain.BMS_ID && t.Content == "BMU:" + msg[0]);
                        if (query == null)
                        {
                            FrmMain.AlarmList.Add(new AlarmInfo()
                            {
                                DataTime = DateTime.Now.ToString("yy-MM-dd HH:mm:ss"),
                                Id = FrmMain.BMS_ID,
                                Type = type,
                                Content = $"BMU:{msg[0]}"
                            });
                        }
                    }
                    else
                    {
                        getLog(out msg, i, j, faultNum);
                        string type = "";
                        switch (msg[1])
                        {
                            case "1":
                                type = "告警";
                                break;
                            case "2":
                                type = "保护";
                                break;
                            case "3":
                                type = "故障";
                                break;
                        }

                        var query = FrmMain.AlarmList.FirstOrDefault(t => t.Id == FrmMain.BMS_ID && t.Type == type && t.Content == "BMU:" + msg[0] && t.State == 0);
                        if (query != null)
                        {
                            query.State = 1;
                            FrmMain.AlarmList.Add(new AlarmInfo()
                            {
                                DataTime = DateTime.Now.ToString("yy-MM-dd HH:mm:ss"),
                                State = 1,
                                Id = FrmMain.BMS_ID,
                                Type = type,
                                Content = $"[解除]BMU:{msg[0]}"
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取字节中的指定Bit的值
        /// </summary>
        /// <param name="b">字节</param>
        /// <param name="index">Bit的索引值(0-7)</param>
        /// <returns></returns>
        public static int GetBit(byte b, short index)
        {
            byte _byte = 0x01;
            switch (index)
            {
                case 0: { _byte = 0x01; } break;
                case 1: { _byte = 0x02; } break;
                case 2: { _byte = 0x04; } break;
                case 3: { _byte = 0x08; } break;
                case 4: { _byte = 0x10; } break;
                case 5: { _byte = 0x20; } break;
                case 6: { _byte = 0x40; } break;
                case 7: { _byte = 0x80; } break;
                default: { return 0; }
            }
            int x = (b & _byte) == _byte ? 1 : 0;

            return (b & _byte) == _byte ? 1 : 0;
        }

        public static string[] getLog(out string[] msg, int row, int column, int faultNum = 0)
        {
            msg = new string[2];
            List<FaultInfo> faultInfos = FrmMain.FaultInfos;
            if (faultNum != 0)
            {
                faultInfos = FrmMain.FaultInfos2;
            }

            for (int i = 0; i < faultInfos.Count; i++)
            {
                if (faultInfos[i].Byte == row && faultInfos[i].Bit == column)
                {
                    int index = LanguageHelper.LanaguageIndex;
                    msg[0] = faultInfos[i].Content.Split(',')[index - 1];
                    msg[1] = faultInfos[i].Type.ToString();
                    break;
                }
            }
            return msg;
        }
        #endregion
    }
}