using SofarBMS.Helper;
using SofarBMS.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SofarBMS.UI
{
    public partial class BMSControl : UserControl
    {
        public BMSControl()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
        }

        string[] packSN = new string[3];

        int initCount = 0;
        RealtimeData_GTX5000S model = null;
        EcanHelper ecanHelper = EcanHelper.Instance;

        public static CancellationTokenSource cts = null;

        private void RTAControl_Load(object sender, EventArgs e)
        {
            foreach (Control item in this.Controls)
            {
                GetControls(item);
            }

            //return;
            Task.Run(async delegate
             {
                 try
                 {
                     while (!cts.IsCancellationRequested)
                     {
                         if (ecanHelper.IsConnection)
                         {
                             if (model != null && initCount >= 13)
                             {
                                 var filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}//Log//GTX5000S_{DateTime.Now.ToString("yyyy-MM-dd")}.csv";

                                 //用于确定指定文件是否存在
                                 if (!File.Exists(filePath))
                                 {
                                     File.AppendAllText(filePath, model.GetHeader() + "\r\n");
                                 }
                                 File.AppendAllText(filePath, model.GetValue() + "\r\n");
                                 initCount = 0;
                                 model = null;
                             }

                             //读取BMS序列号
                             ecanHelper.Send(new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                            , new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 });

                             //定时一秒存储一次数据
                             await Task.Delay(1000);
                         }

                         while (EcanHelper._task.Count > 0
                            && !cts.IsCancellationRequested)
                         {
                             CAN_OBJ ch = (CAN_OBJ)EcanHelper._task.Dequeue();

                             this.Invoke(new Action(() =>
                             {
                                 analysisData(ch.ID, ch.Data);
                             }));
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     throw new Exception(ex.Message);
                 }
             }, cts.Token);
        }

        public void analysisData(uint canID, byte[] data)
        {
            if ((canID & 0xff) != FrmMain.BMS_ID)
                return;

            if (model == null)
                model = new RealtimeData_GTX5000S();

            string[] strs;
            string[] controls;
            string[] strs_1;
            string[] controls_1;

            try
            {
                switch (canID | 0xff)
                {
                    case 0x1003FFFF:
                        initCount++;
                        switch (Convert.ToInt32(data[0].ToString("X2"), 16) & 0x0f)//低四位
                        {
                            case 0: txtBatteryStatus.Text = LanguageHelper.GetLanguage("State_Standby"); break;
                            case 1: txtBatteryStatus.Text = LanguageHelper.GetLanguage("State_Charging"); break;
                            case 2: txtBatteryStatus.Text = LanguageHelper.GetLanguage("State_Discharge"); break;
                            case 3: txtBatteryStatus.Text = LanguageHelper.GetLanguage("State_Hibernate"); break;
                            default: txtBatteryStatus.Text = ""; break;
                        }
                        model.BatteryStatus = txtBatteryStatus.Text;

                        switch (((Convert.ToInt32(data[0].ToString("X2"), 16) & 0xf0) >> 4))//高四位
                        {
                            case 0: txtBmsStatus.Text = LanguageHelper.GetLanguage("BmsStatus_Post"); break;
                            case 1: txtBmsStatus.Text = LanguageHelper.GetLanguage("BmsStatus_Run"); break;
                            case 2: txtBmsStatus.Text = LanguageHelper.GetLanguage("BmsStatus_Fault"); break;
                            case 3: txtBmsStatus.Text = LanguageHelper.GetLanguage("BmsStatus_Upgrade"); break;
                            case 4: txtBmsStatus.Text = LanguageHelper.GetLanguage("BmsStatus_Shutdown"); break;
                        }
                        model.BmsStatus = txtBmsStatus.Text;

                        strs = new string[2] { "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 2], data[i * 2 + 1], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[2] { "txtChargeCurrentLimitation", "txtDischargeCurrentLimitation" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        //BMS测量的P-对B-电压
                        strs_1 = new string[1] { "1" };
                        strs_1[0] = Convert.ToUInt16(data[7].ToString("X2") + data[6].ToString("X2"), 16).ToString();
                        controls_1 = new string[1] { "txtLOAD_VOLT_N" };
                        (this.Controls.Find(controls_1[0], true)[0] as TextBox).Text = strs_1[0];


                        Dictionary<int, string> setContorls = new Dictionary<int, string>() {
                            {0,"pbChargeMosEnable" },
                            {1,"pbDischargeMosEnable" },
                            {2,"pbPrechgMosEnable" },
                            {3,"pbStopChgEnable" },
                            {4,"pbHeatEnable" }
                        };
                        for (short i = 0; i < setContorls.Count; i++)
                        {
                            if (GetBit(data[5], i) == 1 && this.Controls.Find(setContorls[i], true) != null)
                            {
                                (this.Controls.Find(setContorls[i], true)[0] as PictureBox).BackColor = Color.Red;
                            }
                            else
                            {
                                (this.Controls.Find(setContorls[i], true)[0] as PictureBox).BackColor = Color.Green;
                            }
                        }
                        model.ChargeCurrentLimitation = Convert.ToDouble(strs[0]);
                        model.DischargeCurrentLimitation = Convert.ToDouble(strs[1]);
                        model.LOAD_VOLT_N = Convert.ToUInt16(strs_1[0]);
                        model.ChargeMosEnable = (ushort)GetBit(data[5], 0);
                        model.DischargeMosEnable = (ushort)GetBit(data[5], 1);
                        model.PrechgMosEnable = (ushort)GetBit(data[5], 2);
                        model.StopChgEnable = (ushort)GetBit(data[5], 3);
                        model.HeatEnable = (ushort)GetBit(data[5], 4);
                        break;
                    case 0x1004FFFF:
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
                        initCount++;
                        model.TotalChgCap = Convert.ToDouble(((data[3] << 24) + (data[2] << 16) + (data[1] << 8) + (data[0] & 0xff)) * 0.001);
                        model.TotalDsgCap = Convert.ToDouble(((data[7] << 24) + (data[6] << 16) + (data[5] << 8) + (data[4] & 0xff)) * 0.001);
                        txtTotalChgCap.Text = model.TotalChgCap.ToString();
                        txtTotalDsgCap.Text = model.TotalDsgCap.ToString();
                        break;
                    case 0x1008FFFF:
                        initCount++;
                        richTextBox1.Clear(); richTextBox2.Clear(); richTextBox3.Clear();
                        analysisLog(data);
                        break;
                    case 0x1009FFFF:
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
                        initCount++;
                        txtRemainCap.Text = BytesToIntger(data[1], data[0], 0.1);
                        txtFullCap.Text = BytesToIntger(data[3], data[2], 0.1);
                        txtCycleTIme.Text = BytesToIntger(data[5], data[4]);
                        model.RemainingCapacity = txtRemainCap.Text;
                        model.FullCapacity = txtFullCap.Text;
                        model.CycleTIme = Convert.ToUInt16(txtCycleTIme.Text);
                        break;
                    case 0x1040FFFF:
                        txtCumulative_discharge_capacity.Text = (((data[3] << 24) + (data[2] << 16) + (data[1] << 8) + (data[0] & 0xff))).ToString();
                        model.CumulativeDischargeCapacity = txtCumulative_discharge_capacity.Text;
                        break;
                    case 0x1041FFFF:
                        txtBalance_temperature1.Text = BytesToIntger(data[1], data[0], 0.1);
                        txtBalance_temperature2.Text = BytesToIntger(data[3], data[2], 0.1);

                        model.BalanceTemperature1 = txtBalance_temperature1.Text;
                        model.BalanceTemperature2 = txtBalance_temperature2.Text;
                        break;
                    case 0x1042FFFF:
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
                        //加热异常，加热继电器粘连，加热继电器断路Byte[0],0/1/2
                        richTextBox1_Fault.Clear(); richTextBox2_Pro.Clear(); richTextBox3_W.Clear();
                        analysisLog(data, 1);
                        break;
                    case 0x104AFFFF:

                        strs = new string[4] { "0.1", "0.1", "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtPowerTemperture1", "txtPowerTemperture2", "txtHeatCur", "txtHeatRelayVol" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        break;
                    case 0x1027E0FF:
                        string strSn = GetPackSN(data);

                        if (!string.IsNullOrEmpty(strSn))
                            txtSN.Text = strSn;
                        break;
                    case 0x102EE0FF:
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
        private void analysisLog(byte[] data, int faultNum = 0)
        {
            string[] msg = new string[2];

            for (int i = 0; i < data.Length; i++)
            {
                for (short j = 0; j < 8; j++)
                {
                    if (GetBit(data[i], j) == 1)
                    {
                        getLog(out msg, i, j, faultNum);

                        if (faultNum == 1)
                        {
                            switch (msg[1])
                            {
                                case "1":
                                    richTextBox3_W.AppendText(msg[0] + "\r");
                                    //model.Warning = richTextBox3_W.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "2":
                                    richTextBox2_Pro.AppendText(msg[0] + "\r");
                                    //model.Protection = richTextBox2_Pro.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "3":
                                    richTextBox1_Fault.AppendText(msg[0] + "\r");
                                    //model.Fault = richTextBox1_Fault.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                            }
                        }
                        else
                        {
                            switch (msg[1])
                            {
                                case "1":
                                    richTextBox3.AppendText(msg[0] + "\r");
                                    model.Warning = richTextBox3.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "2":
                                    richTextBox2.AppendText(msg[0] + "\r");
                                    model.Protection = richTextBox2.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "3":
                                    richTextBox1.AppendText(msg[0] + "\r");
                                    model.Fault = richTextBox1.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                            }
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

        public static string[] getLog(out string[] msg, int row, int column, int faultNum)
        {
            msg = new string[2];
            List<FaultInfo> faultInfos = FrmMain.FaultInfos;
            if (faultNum == 1)
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

        private void lblRealtimeData_45_Click(object sender, EventArgs e)
        {

        }

        private void txtCelltemperature1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblRealtimeData_48_Click(object sender, EventArgs e)
        {

        }

        private void lblRealtimeData_47_Click(object sender, EventArgs e)
        {

        }

        private void lblRealtimeData_46_Click(object sender, EventArgs e)
        {

        }

        private void txtCelltemperature3_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCelltemperature4_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCelltemperature2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}