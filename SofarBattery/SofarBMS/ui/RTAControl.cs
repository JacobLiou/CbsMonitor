using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SofarBMS.Helper;
using SofarBMS.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SofarBMS.UI
{
    public partial class RTAControl : UserControl
    {
        public RTAControl()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
        }

        int initCount = 0;
        RealtimeData_BTS5K model = null;
        EcanHelper ecanHelper = EcanHelper.Instance;

        string[] eventList
        {
            get
            {
                string eventStr = @"预留,NULL
充电软启动失败,IschgSoftStartFail
放电软启失败,IsdchgSoftStartFail
预留,NULL
预留,NULL
预留,NULL
预留,NULL
预留,NULL
散热器过温告警,HSHighTempWarning
环境温度过温告警,EnvHighTempWarning
预留,NULL
预留,NULL
预留,NULL
禁止充电告警,StopChgWarning
禁止放电告警,StopDchgWarning
电池不均衡告警,BatUnbalanced
电池状态故障,BatteryStateErr
PWM模式故障,PWMModeErr
BMS版本错误,BMSVerionErr
BMS过压过流故障,BMSOVOCP
电池平均值过流保护,SwBatAvgOCP
平均值过载保护,SwAvgOverloadP
母线软件过流,SwBusInstantOCP
软件CBC过流,SwCBCOCP
PackID错误,PackIDErr
启机短路保护,BusSCP
母线平均值欠压,SwBusAvgUVP
时钟故障,ClockFault
PCS通信故障,PCSCommFault
预留,NULL
预留,NULL
预留,NULL
散热器过温保护,HSOverTempFault
环境温度过温保护,OverTempFault_Env
Sci通信故障,SciCommFault
Can通信故障,Can1CommFault
继电器1故障,Relay1Fail
预留,NULL
预留,NULL
预留,NULL
母线软件过压,SwBusInstantOVP
母线软件欠压,SwBusInstantUVP
电池软件过压,SwBatInstantOVP
电池软件欠压,SwBatInstantUVP
电池软件过流,SwBatInstantOCP
预留,NULL
预留,NULL
硬件过流,HwOCP
永久母线过压,unrecoverBusAvgOV
永久电池欠压,unrecoverBatAvgUV
永久瞬时过流,unrecoverOCPInstant
永久硬件过流,unrecoverHwOCP
预留,NULL
预留,NULL
预留,NULL
预留,NULL
永久继电器故障,unrecoverRelayFail
预留,NULL
预留,NULL
预留,NULL
预留,NULL
永久性母线短路保护,unrecoverBusSCP
永久电池激活失败故障,unrecoverBatActFail
永久性反接故障,unrecoverBusRPP";

                return eventStr.Split("\n".ToCharArray());
            }
        }
        string[] packSN = new string[3];
        string[] packSN_BDU = new string[3];

        public static CancellationTokenSource cts = null;

        private void RTAControl_Load(object sender, EventArgs e)
        {
            //翻译：轮询界面控件
            foreach (Control item in this.Controls)
            {
                GetControls(item);
            }

            //启动接收线程
            Task.Run(async delegate
            {
                while (!cts.IsCancellationRequested)
                {
                    if (ecanHelper.IsConnection)
                    {
                        if (model != null && initCount >= 13)
                        {
                            var filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}//Log//BTS5K_{DateTime.Now.ToString("yyyy-MM-dd")}.csv";

                            //用于确定指定文件是否存在
                            if (!File.Exists(filePath))
                            {
                                File.AppendAllText(filePath, model.GetHeader() + "\r\n");
                            }
                            File.AppendAllText(filePath, model.GetValue() + "\r\n");
                            initCount = 0;
                            model = null;
                        }

                        //获取实时数据指令
                        ecanHelper.Send(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                       , new byte[] { 0xE0, FrmMain.BMS_ID, 0x2C, 0x10 });

                        //读取BMS序列号
                        ecanHelper.Send(new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                       , new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 });

                        //获取高压放电电流、充电电流；获取低压放电电流、充电电流
                        ecanHelper.Send(new byte[] { 0x77, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                   , new byte[4] { 0xE0, Convert.ToByte(FrmMain.BMS_ID + 0x20), 0x77, 0x0B });

                        ecanHelper.Send(new byte[] { 0x66, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                   , new byte[4] { 0xE0, Convert.ToByte(FrmMain.BMS_ID + 0x20), 0x77, 0x0B });

                        //读取BDU序列号
                        GetBduSn();
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
            }, cts.Token);
        }

        private void GetBduSn()
        {
            //读取SN从4开始-6结束
            for (int i = 4; i < 7; i++)
            {
                byte[] data = new byte[8];
                data[0] = (byte)i;
                ecanHelper.Send(data, new byte[] { 0xE0, FrmMain.BDU_ID, 0x00, 0x14 });
            }
        }

        public void analysisData(uint canID, byte[] data)
        {
            if (!(((canID & 0xff) == FrmMain.BMS_ID)
                || ((canID & 0xff) == FrmMain.PCU_ID)
                || ((canID & 0xff) == FrmMain.BDU_ID)
                || ((canID & 0xff) == FrmMain.BCU_ID)))
                return;

            if (model == null)
                model = new RealtimeData_BTS5K();

            string[] strs;
            string[] controls;
            try
            {
                switch (canID | 0xff)
                {
                    case 0x1003FFFF:
                        initCount++;
                        switch (Convert.ToInt32(data[0].ToString("X2"), 16) & 0x0f)
                        {
                            case 0: txtBatteryStatus.Text = LanguageHelper.GetLanguage("State_Standby"); break;
                            case 1: txtBatteryStatus.Text = LanguageHelper.GetLanguage("State_Charging"); break;
                            case 2: txtBatteryStatus.Text = LanguageHelper.GetLanguage("State_Discharge"); break;
                            case 3: txtBatteryStatus.Text = LanguageHelper.GetLanguage("State_Hibernate"); break;
                            default: txtBatteryStatus.Text = ""; break;
                        }
                        model.BatteryStatus = txtBatteryStatus.Text;

                        strs = new string[2] { "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 2], data[i * 2 + 1], Convert.ToDouble(strs[i]));
                            controls = new string[2] { "txtChargeCurrentLimitation", "txtDischargeCurrentLimitation" };
                        }
                        controls = new string[2] { "txtChargeCurrentLimitation", "txtDischargeCurrentLimitation" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }
                        model.ChargeCurrentLimitation = Convert.ToDouble(strs[0]);
                        model.DischargeCurrentLimitation = Convert.ToDouble(strs[1]);

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
                        txtTotalChgCap.Text = (((data[3] << 24) + (data[2] << 16) + (data[1] << 8) + (data[0] & 0xff)) * 0.001).ToString();
                        txtTotalDsgCap.Text = (((data[7] << 24) + (data[6] << 16) + (data[5] << 8) + (data[4] & 0xff)) * 0.001).ToString();

                        model.TotalChgCap = Convert.ToDouble(txtTotalChgCap.Text.Trim());
                        model.TotalDsgCap = Convert.ToDouble(txtTotalDsgCap.Text.Trim());
                        break;
                    case 0x1008FFFF:
                        initCount++;
                        richTextBox1.Clear(); richTextBox2.Clear(); richTextBox3.Clear();
                        analysisLog(data, 0);
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

                        model.MosTemperature = Convert.ToDouble(strs[0]);
                        model.EnvTemperature = Convert.ToDouble(strs[1]);
                        model.SOH = Convert.ToDouble(strs[2]);

                        for (int i = 6; i < 8; i++)
                        {
                            for (short j = 0; j < 8; j++)
                            {
                                if (GetBit(data[i], j) == 1)
                                {
                                    (this.Controls.Find(string.Format("txtCellvoltage{0}", j + 1 + ((i - 6) * 8)), true)[0] as TextBox).BackColor = Color.Aquamarine;
                                }
                                else
                                {
                                    (this.Controls.Find(string.Format("txtCellvoltage{0}", j + 1 + ((i - 6) * 8)), true)[0] as TextBox).BackColor = Color.White;
                                }
                            }
                        }
                        break;
                    case 0x100FFFFF:
                        initCount++;
                        strs = new string[3] { "0.1", "0.1", "1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }
                        controls = new string[3] { "txtRemaining_capacity", "txtFull_capacity", "txtCycleTime" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.RemainingCapacity = txtRemaining_capacity.Text;
                        model.FullCapacity = txtFull_capacity.Text;
                        model.CycleTIme = txtCycleTime.Text;
                        break;
                    case 0x1040FFFF:
                        txtCumulative_discharge_capacity.Text = (((data[3] << 24) + (data[2] << 16) + (data[1] << 8) + (data[0] & 0xff)) * 0.001).ToString();//BytesToIntger(data[1], data[0]);
                        break;
                    case 0x1041FFFF:
                        strs = new string[2] { "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }
                        controls = new string[2] { "txtBalanceTemperature1", "txtBalanceTemperature2" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.BalanceTemperature1 = Convert.ToDouble(strs[0]);
                        model.BalanceTemperature2 = Convert.ToDouble(strs[1]);
                        break;
                    case 0x1042FFFF:
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
                    case 0x0B605FFF:
                        strs = new string[4] { "0.1", "0.1", "0.1", "1" };
                        strs[0] = BytesToIntger(data[1], data[0], 0.1);
                        strs[1] = BytesToIntger(data[3], data[2], 0.1);
                        strs[2] = BytesToIntger(data[5], data[4], 0.1);
                        strs[3] = BytesToIntger(data[7], data[6]);
                        controls = new string[4] { "txtDC_high_voltage", "txtDC_Low_voltage", "txtDC_Low_Current", "txtDC_Low_Power" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.HighvoltageSideVoltage = txtDC_high_voltage.Text;
                        model.LowvoltageSideVoltage = txtDC_Low_voltage.Text;
                        model.LowvoltageSideCurrent = txtDC_Low_Current.Text;
                        model.LowvoltageSidePower = txtDC_Low_Power.Text;
                        break;
                    case 0x0B615FFF:
                        strs = new string[4];
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2]);
                        }
                        controls = new string[4] { "txtWork_State", "txtBattery_State", "txtInternal_temperature", "txtRadiator_temperature" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            if (i == 0)
                            {
                                (this.Controls.Find(controls[i], true)[0] as TextBox).Text = Enum.Parse(typeof(WorkState), strs[i]).ToString();
                            }
                            else if (i == 1)
                            {
                                (this.Controls.Find(controls[i], true)[0] as TextBox).Text = Enum.Parse(typeof(BatteryState), strs[i]).ToString();

                                switch (Convert.ToInt32(strs[i]))
                                {
                                    case 1:
                                        txtHV_Charge_Current.Text = LanguageHelper.GetLanguage("Invalid_value");
                                        break;
                                    case 0:
                                        txtHV_Discharge_Current.Text = LanguageHelper.GetLanguage("Invalid_value");
                                        break;
                                }
                            }
                            else
                            {
                                (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                            }
                        }

                        model.WorkState = txtWork_State.Text;
                        model.BatteryState = txtBattery_State.Text;
                        model.CaseTemperature = txtInternal_temperature.Text;
                        model.RadiatorTemperature = txtRadiator_temperature.Text;
                        break;
                    case 0x0B625FFF:
                        List<int> pcuError = GetEventList(data);
                        DataTable dtEvent = new DataTable();
                        dtEvent.Columns.Add("item");
                        dtEvent.Columns.Add("value");

                        string strError = string.Empty;
                        foreach (int value in pcuError)
                        {
                            DataRow dr = dtEvent.NewRow();

                            dr["item"] = value;

                            if (value <= eventList.Length && value >= 1)
                            {
                                int index = LanguageHelper.LanaguageIndex;
                                string errInfo = eventList[value - 1].Split(',')[index - 1];

                                dr["value"] = errInfo;
                                strError += errInfo;
                            }

                            dtEvent.Rows.Add(dr);
                        }
                        dataGridView1.DataSource = dtEvent;

                        model.PcuEvents = strError;
                        break;
                    case 0x0B635FFF:
                        //软件版本
                        string[] soft = new string[3];
                        for (int i = 0; i < 3; i++)
                        {
                            soft[i] = data[i + 1].ToString().PadLeft(2, '0');
                        }
                        txtSoftware_Version.Text = Encoding.ASCII.GetString(new byte[] { data[0] }) + string.Join("", soft);

                        //硬件版本
                        string[] hard = new string[2];
                        for (int i = 0; i < 2; i++)
                        {
                            hard[i] = data[i + 4].ToString("X2");
                        }
                        txtHardware_Version.Text = string.Join("", hard);

                        //CAN协议版本
                        txtCAN_Version.Text = data[7].ToString("X2") + data[6].ToString("X2");
                        break;
                    case 0x0B665FFF:
                        //PCBA硬件版本
                        string[] pcbaV = new string[3];
                        for (int i = 0; i < 3; i++)
                        {
                            pcbaV[i] = data[i + 1].ToString();
                        }
                        txtPcbaHardware_Version.Text = Encoding.ASCII.GetString(new byte[] { data[0] }) + string.Join("", pcbaV);

                        if (data[4] == 1 && this.Controls.Find("pbHfilmState", true) != null)
                        {
                            (this.Controls.Find("pbHfilmState", true)[0] as PictureBox).BackColor = Color.Red;
                        }
                        else
                        {
                            (this.Controls.Find("pbHfilmState", true)[0] as PictureBox).BackColor = Color.Green;
                        }

                        if (data[5] == 1 && this.Controls.Find("pbHfimForbiddenCmd", true) != null)
                        {
                            (this.Controls.Find("pbHfimForbiddenCmd", true)[0] as PictureBox).BackColor = Color.Red;
                        }
                        else
                        {
                            (this.Controls.Find("pbHfimForbiddenCmd", true)[0] as PictureBox).BackColor = Color.Green;
                        }

                        model.HfilmState = Convert.ToInt32(data[4]).ToString();
                        model.HfimForbiddenCmd = Convert.ToInt32(data[5]).ToString();
                        break;
                    case 0x0B78E0FF:
                        txtHV_Charge_Current.Text = BytesToIntger(data[5], data[4], 0.01);
                        txtHV_Discharge_Current.Text = BytesToIntger(data[7], data[6], 0.01);

                        model.HighvoltageChargingCurrent = txtHV_Charge_Current.Text;
                        model.HighvoltageDischargeCurrent = txtHV_Discharge_Current.Text;
                        break;
                    case 0x1026E0FF:
                        int[] numbers_bit = BytesToBit(data);

                        StringBuilder date = new StringBuilder();
                        date.Append(numbers_bit[0] + 2000);
                        date.Append("-");
                        date.Append(numbers_bit[1]);
                        date.Append("-");
                        date.Append(numbers_bit[2]);
                        date.Append(" ");
                        date.Append(numbers_bit[3]);
                        date.Append(":");
                        date.Append(numbers_bit[4]);
                        date.Append(":");
                        date.Append(numbers_bit[5]);
                        txtDatetime.Text = date.ToString();
                        break;
                    case 0x1027E0FF:
                        string strSn = GetPackSN(data);
                        if (!string.IsNullOrEmpty(strSn))
                            txtSN.Text = strSn;
                        break;
                    case 0x102EE0FF:
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
                    case 0x1403FFFF:
                        //软件版本
                        string[] bdu_soft = new string[3];
                        for (int i = 0; i < 3; i++)
                        {
                            bdu_soft[i] = data[i + 1].ToString().PadLeft(2, '0');
                        }
                        txtSoftware_Version_BDU.Text = Encoding.ASCII.GetString(new byte[] { data[0] }) + string.Join("", bdu_soft);

                        //硬件版本
                        string[] bdu_hard = new string[2];
                        for (int i = 0; i < 2; i++)
                        {
                            bdu_hard[i] = data[i + 4].ToString("X2");
                        }
                        txtHardware_Version_BDU.Text = string.Join("", bdu_hard);
                        break;
                    case 0x1400E0FF:
                        string strSn_BDU = GetPackSN_BDU(data);
                        if (!string.IsNullOrEmpty(strSn_BDU))
                            txtSN_BDU.Text = strSn_BDU;
                        break;
                    case 0x1045E0FF:
                    case 0x1045FFFF:
                        initCount++;
                        richTextBox1_45.Clear(); richTextBox2_45.Clear(); richTextBox3_45.Clear();
                        analysisLog(data, 1);
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
        private string GetPackSN_BDU(byte[] data)
        {
            //条形码解析
            switch (data[0])
            {
                case 4:
                    packSN_BDU[0] = Encoding.Default.GetString(data).Substring(1);
                    break;
                case 5:
                    packSN_BDU[1] = Encoding.Default.GetString(data).Substring(1);
                    break;
                case 6:
                    packSN_BDU[2] = Encoding.Default.GetString(data).Substring(1);
                    break;
            }

            //判断sn是否接收完成
            if (packSN_BDU[0] != null && packSN_BDU[1] != null && packSN_BDU[2] != null)
            {
                string strSN = String.Join("", packSN_BDU);
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

        #region 加载实时数据表
        public static DataTable List2DataTable(IList list)
        {
            var result = new DataTable();

            if (list.Count > 0)
            {
                var properties = list[0].GetType().GetProperties();

                foreach (var pi in properties) result.Columns.Add(pi.Name, pi.PropertyType);
                foreach (var t in list)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in properties)
                    {
                        var obj = pi.GetValue(t, null);
                        tempList.Add(obj);
                    }

                    var array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }

            return result;
        }

        public static DataTable Dictionary2DataTable(Dictionary<string, RealtimeData_BTS5K> dictionary)
        {
            var result = new DataTable();
            if (dictionary.Count > 0)
            {
                RealtimeData_BTS5K realtimeData = new RealtimeData_BTS5K();
                var properties = realtimeData.GetType().GetProperties();
                foreach (var pi in properties)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                foreach (var item in dictionary)
                {

                    var tempList = new ArrayList();
                    foreach (var pi in properties)
                    {
                        var obj = pi.GetValue(item.Value, null);
                        tempList.Add(obj);
                    }

                    var array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }

            return result;
        }
        #endregion

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

        public static void LTooltip(Control control, string value)
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

            int len = control.Width * str.Length / strWidth;

            if (len > 15 && len < str.Length)
            {
                return str.Substring(0, 15) + "...";
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
                        getLog(out msg, i, j, faultNum);
                        if (faultNum == 1)
                        {
                            switch (msg[1])
                            {
                                case "1":
                                    richTextBox3_45.AppendText(msg[0] + "\r");
                                    model.Warning2 = richTextBox3_45.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "2":
                                    richTextBox2_45.AppendText(msg[0] + "\r");
                                    model.Protection2 = richTextBox2_45.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "3":
                                    richTextBox1_45.AppendText(msg[0] + "\r");
                                    model.Fault2 = richTextBox1_45.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                            }
                        }
                        else
                        {
                            switch (msg[1])
                            {
                                case "1":
                                    richTextBox3.AppendText(msg[0] + "\r\n");
                                    model.Warning = richTextBox3.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "2":
                                    richTextBox2.AppendText(msg[0] + "\r\n");
                                    model.Protection = richTextBox2.Text.Replace("\n", "，").Replace("\r", "，");
                                    break;
                                case "3":
                                    richTextBox1.AppendText(msg[0] + "\r\n");
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

        private void tsmiClearData_Click(object sender, EventArgs e)
        {
            SQLiteHelper.Update("delete from RealtimeData");
        }

        private void tsmiExportData_Click(object sender, EventArgs e)
        {
            DataSet ds = SQLiteHelper.GetDataSet("select * from RealtimeData");
            DataTable dt = ds.Tables[0];

            //List集合导出为Excel
            //创建工作簿对象
            IWorkbook workbook = new HSSFWorkbook();
            //创建工作表
            ISheet sheet = workbook.CreateSheet("onesheet");
            IRow row0 = sheet.CreateRow(0);
            string[] cells = new string[] { "运行时间", "电池状态", "故障信息", "告警信息", "保护信息", "充电电流上限", "放电电流上限", "充电MOS", "放电MOS", "预充MOS", "充电急停", "加热MOS", "电池电压", "负载电压", "电池电流", "电池剩余容量", "最高单体电压", "最高单体电压编号", "最低单体电压", "最低单体电压编号", "单体电压差值", "最高单体温度", "最高单体温度编号", "最低单体温度", "最低单体温度编号", "累计充电容量", "累计放电容量", "电芯电压1", "电芯电压2", "电芯电压3", "电芯电压4", "电芯电压5", "电芯电压6", "电芯电压7", "电芯电压8", "电芯电压9", "电芯电压10", "电芯电压11", "电芯电压12", "电芯电压13", "电芯电压14", "电芯电压15", "电芯电压16", "电芯温度1", "电芯温度2", "电芯温度3", "电芯温度4", "Mos温度", "环境温度", "电池健康程度" };
            for (int i = 0; i < cells.Length; i++)
            {
                row0.CreateCell(i).SetCellValue(cells[i]);
            }

            for (int r = 1; r < dt.Rows.Count; r++)
            {
                //创建行row
                IRow row = sheet.CreateRow(r);
                int c = 0;
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["CreateDate"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatteryStatus"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Fault"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Warning"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Protection"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Charge_current_limitation"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Discharge_current_limitation"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["ChargeMosEnable"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["DischargeMosEnable"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["PrechgMosEnable"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["StopChgEnable"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["HeatEnable"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Battery_Volt"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Load_Volt"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Battery_current"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["SOC"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMaxCellVolt"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMaxCellVoltNum"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMinCellVolt"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMinCellVoltNum"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatDiffCellVolt"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMaxCellTemp"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMaxCellTempNum"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMinCellTemp"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMinCellTempNum"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["TotalChgCap"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["TotalDsgCap"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage1"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage2"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage3"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage4"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage5"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage6"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage7"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage8"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage9"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage10"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage11"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage12"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage13"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage14"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage15"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage16"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_temperature1"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_temperature2"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_temperature3"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_temperature4"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["MOS_temperature"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Env_Temperature"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["SOH"].ToString());
            }

            //创建流对象并设置存储Excel文件的路径
            using (FileStream url = File.OpenWrite(@"C:\Users\admin\Desktop\1.xls"))
            {
                //导出Excel文件
                workbook.Write(url);
            };
        }

        private void ExportData(string path)
        {
            DataSet ds = SQLiteHelper.GetDataSet("select * from RealtimeData");
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count == 0)
                return;

            //List集合导出为Excel
            //创建工作簿对象
            IWorkbook workbook = new HSSFWorkbook();
            //创建工作表
            ISheet sheet = workbook.CreateSheet("onesheet");
            IRow row0 = sheet.CreateRow(0);
            string[] cells = new string[] { "运行时间", "电池状态", "故障信息", "告警信息", "保护信息", "充电电流上限", "放电电流上限", "充电MOS", "放电MOS", "预充MOS", "充电急停", "加热MOS", "电池电压", "负载电压", "电池电流", "电池剩余容量", "最高单体电压", "最高单体电压编号", "最低单体电压", "最低单体电压编号", "单体电压差值", "最高单体温度", "最高单体温度编号", "最低单体温度", "最低单体温度编号", "累计充电容量", "累计放电容量", "电芯电压1", "电芯电压2", "电芯电压3", "电芯电压4", "电芯电压5", "电芯电压6", "电芯电压7", "电芯电压8", "电芯电压9", "电芯电压10", "电芯电压11", "电芯电压12", "电芯电压13", "电芯电压14", "电芯电压15", "电芯电压16", "电芯温度1", "电芯温度2", "电芯温度3", "电芯温度4", "Mos温度", "环境温度", "电池健康程度" };
            for (int i = 0; i < cells.Length; i++)
            {
                row0.CreateCell(i).SetCellValue(cells[i]);
            }

            for (int r = 1; r < dt.Rows.Count; r++)
            {
                //创建行row
                IRow row = sheet.CreateRow(r);
                int c = 0;
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["CreateDate"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatteryStatus"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Fault"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Warning"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Protection"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Charge_current_limitation"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Discharge_current_limitation"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["ChargeMosEnable"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["DischargeMosEnable"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["PrechgMosEnable"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["StopChgEnable"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["HeatEnable"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Battery_Volt"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Load_Volt"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Battery_current"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["SOC"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMaxCellVolt"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMaxCellVoltNum"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMinCellVolt"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMinCellVoltNum"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatDiffCellVolt"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMaxCellTemp"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMaxCellTempNum"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMinCellTemp"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["BatMinCellTempNum"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["TotalChgCap"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["TotalDsgCap"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage1"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage2"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage3"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage4"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage5"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage6"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage7"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage8"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage9"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage10"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage11"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage12"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage13"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage14"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage15"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_voltage16"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_temperature1"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_temperature2"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_temperature3"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Cell_temperature4"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["MOS_temperature"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["Env_Temperature"].ToString());
                row.CreateCell(c++).SetCellValue(dt.Rows[r]["SOH"].ToString());
            }

            //创建流对象并设置存储Excel文件的路径
            using (FileStream url = File.OpenWrite(path))//OpenWrite  @"C:\Users\admin\Desktop\1.xls"
            {
                //导出Excel文件
                workbook.Write(url);
            };
        }

        private int[] BytesToBit(byte[] data)
        {
            int[] numbers = new int[8];
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = Convert.ToInt32(data[i].ToString("X2"), 16);
            }
            return numbers;
        }
    }
}