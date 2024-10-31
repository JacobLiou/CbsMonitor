using NPOI.SS.Formula.Functions;
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
    public partial class CBS_BCU_Control : UserControl
    {
        public CBS_BCU_Control()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
        }

        string[] packSN = new string[3];

        int initCount = 0;
        RealtimeData_CBS5000S_BCU model = null;

        public static CancellationTokenSource cts = null;
        private EcanHelper ecanHelper = EcanHelper.Instance;

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
                        if (model != null && initCount >= 15)
                        {
                            var filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}//Log//CBS5000_BCU{DateTime.Now.ToString("yyyy-MM-dd")}.csv";

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

                        //一键操作标定参数
                        ecanHelper.Send(new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                       , new byte[] { 0xE0, FrmMain.BCU_ID, 0xF9, 0x10 });

                        await Task.Delay(500);

                        //上位机监控读取
                        ecanHelper.Send(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                       , new byte[] { 0xE0, FrmMain.BCU_ID, 0xF7, 0x10 });

                        //定时一秒存储一次数据
                        await Task.Delay(500);
                    }
                }
            }, cts.Token);
        }


        public void AnalysisData(uint canID, byte[] data)
        {
            if ((canID & 0xff) != FrmMain.BCU_ID)
                return;

            if (model == null)
                model = new RealtimeData_CBS5000S_BCU();

            string[] strs;
            string[] controls;

            try
            {
                switch (canID | 0xff)
                {
                    //BCU

                    //0x0B6:BCU遥信数据上报1
                    case 0x10B6E0FF:
                        initCount++;
                        strs = new string[5];
                        //for (int i = 0; i < strs.Length; i++)
                        //{
                        //    strs[i] = BytesToIntger(0x00, data[i]);
                        //}
                        strs[0] = BytesToIntger(0x00, data[0]);
                        strs[1] = BytesToIntger(data[2], data[1]);
                        strs[2] = BytesToIntger(data[4], data[3]);
                        strs[3] = BytesToIntger(0x00, data[5]);
                        strs[4] = BytesToIntger(0x00, data[6]);
                        controls = new string[5] { "txtDi_Status_Get", "txtBalance_Chg_Status", "txtBalance_Dchg_Status", "txtRelay_Status", "txtOther_Dev_Staus" };

                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        var statusBytesArray = new List<Dictionary<short, string>>[]
                        {
                             new List<Dictionary<short, string>>
                             {
                                 new Dictionary<short, string> {{0, "pbExternalCANAddressingInputIOStatus" }},
                                 new Dictionary<short, string> {{1, "pbContactorPositiveSwitchDetectionHighLevelClosure" }},
                                 new Dictionary<short, string> {{2, "pbContactorNegativeSwitchDetectionHighLevelClosure" }},
                                 new Dictionary<short, string> {{3, "pbDryContactInput1Channel" }},
                                 new Dictionary<short, string> {{4, "pbADC1115_1Feedback1" }},
                                 new Dictionary<short, string> {{5, "pbADC1115_1Feedback2" }},
                                 new Dictionary<short, string> {{6, "pbChargingWakeUp" }}
                             },
                             new List<Dictionary<short, string>>
                             {
                                 new Dictionary<short, string> {{0, "pbBatteryPack_1_BalancedState" }},
                                 new Dictionary<short, string> {{1, "pbBatteryPack_2_BalancedState" }},
                                 new Dictionary<short, string> {{2, "pbBatteryPack_3_BalancedState" }},
                                 new Dictionary<short, string> {{3, "pbBatteryPack_4_BalancedState" }},
                                 new Dictionary<short, string> {{4, "pbBatteryPack_5_BalancedState" }},
                                 new Dictionary<short, string> {{5, "pbBatteryPack_6_BalancedState" }},
                                 new Dictionary<short, string> {{6, "pbBatteryPack_7_BalancedState" }},
                                 new Dictionary<short, string> {{7, "pbBatteryPack_8_BalancedState" }},
                                 new Dictionary<short, string> {{8, "pbBatteryPack_9_BalancedState" }},
                                 new Dictionary<short, string> {{9, "pbBatteryPack_10_BalancedState" }}
                             },
                             new List<Dictionary<short, string>>
                             {
                                 new Dictionary<short, string> {{0, "pbBatteryPackBalancedState_1" }},
                                 new Dictionary<short, string> {{1, "pbBatteryPackBalancedState_2" }},
                                 new Dictionary<short, string> {{2, "pbBatteryPackBalancedState_3" }},
                                 new Dictionary<short, string> {{3, "pbBatteryPackBalancedState_4" }},
                                 new Dictionary<short, string> {{4, "pbBatteryPackBalancedState_5" }},
                                 new Dictionary<short, string> {{5, "pbBatteryPackBalancedState_6" }},
                                 new Dictionary<short, string> {{6, "pbBatteryPackBalancedState_7" }},
                                 new Dictionary<short, string> {{7, "pbBatteryPackBalancedState_8" }},
                                 new Dictionary<short, string> {{8, "pbBatteryPackBalancedState_9" }},
                                 new Dictionary<short, string> {{9, "pbBatteryPackBalancedState_10" }}
                             },
                             new List<Dictionary<short, string>>
                             {
                                 new Dictionary<short, string> {{0, "pbPositiveRelay" }},
                                 new Dictionary<short, string> {{1, "pbNegativeRelay" }},
                                 new Dictionary<short, string> {{2, "pbPreChargeRelay" }},
                                 new Dictionary<short, string> {{3, "pbInsulationDetectionRelay1" }},
                                 new Dictionary<short, string> {{4, "pbInsulationDetectionRelay2" }},
                                 new Dictionary<short, string> {{5, "pbHeatingFilmRelay" }}
                             },
                             new List<Dictionary<short, string>>
                             {
                                 new Dictionary<short, string> {{0, "pbFanStatus"}},
                                 new Dictionary<short, string> {{1, "pbChagreStatus"}},
                                 new Dictionary<short, string> {{2, "pbDischargeStatus"}},
                                 new Dictionary<short, string> {{3, "pbForceChargingEnable"}},
                                 new Dictionary<short, string> {{4, "pbFullCharge"}},
                                 new Dictionary<short, string> {{5, "pbEmpty"}}
                             }
                        };

                        for (int i = 0; i < statusBytesArray.Length; i++)
                        {
                            UpdateControlStatus(statusBytesArray[i], Convert.ToUInt16(strs[i]));
                        }

                        model.Di_Status_Get = Convert.ToUInt16(strs[0]);
                        model.Balance_Chg_Status = Convert.ToUInt16(strs[1]);
                        model.Balance_Dchg_Status = Convert.ToUInt16(strs[2]);
                        model.Relay_Status = Convert.ToUInt16(strs[3]);
                        model.Other_Dev_Staus = Convert.ToUInt16(strs[4]);
                        break;

                    //0x0B7:BCU遥信数据上报2
                    case 0x10B7E0FF:
                        initCount++;
                        strs = new string[4] { "0.1", "0.1", "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtPower_Terminal_Temperature1", "txtPower_Terminal_Temperature2", "txtPower_Terminal_Temperature3", "txtPower_Terminal_Temperature4" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.Power_Terminal_Temperature1 = Convert.ToDouble(strs[0]);
                        model.Power_Terminal_Temperature2 = Convert.ToDouble(strs[1]);
                        model.Power_Terminal_Temperature3 = Convert.ToDouble(strs[2]);
                        model.Power_Terminal_Temperature4 = Convert.ToDouble(strs[3]);
                        break;

                    //0x0B8:BCU遥信数据上报3
                    case 0x10B8E0FF:
                        initCount++;
                        strs = new string[4] { "1", "0.01", "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtInsulation_Resistance", "txtAuxiliary_Power_Supply_Voltage", "txtFuse_Voltage", "txtPower_Voltage" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.Insulation_Resistance = Convert.ToDouble(strs[0]);
                        model.Auxiliary_Power_Supply_Voltage = Convert.ToDouble(strs[1]);
                        model.Fuse_Voltage = Convert.ToDouble(strs[2]);
                        model.Power_Voltage = Convert.ToDouble(strs[3]);
                        break;

                    //0x0B9:BCU遥信数据上报4
                    case 0x10B9E0FF:
                        initCount++;
                        strs = new string[1] { "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[1] { "txtLoad_voltage" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.Load_Voltage = Convert.ToDouble(strs[0]);
                        break;

                    //0x0BA:BCU遥信数据上报5
                    case 0x10BAE0FF:
                        initCount++;
                        strs = new string[6];
                        strs[0] = BytesToIntger(data[1], data[0], 0.001);
                        strs[1] = BytesToIntger(0x00, data[2]);
                        strs[2] = BytesToIntger(0x00, data[3]);
                        strs[3] = BytesToIntger(data[5], data[4], 0.001);
                        strs[4] = BytesToIntger(0x00, data[6]);
                        strs[5] = BytesToIntger(0x00, data[7]);
                        controls = new string[6] { "txtBat_Max_Cell_Volt", "txtBat_Max_Cell_VoltPack", "txtBat_Max_Cell_VoltNum", "txtBat_Min_Cell_Volt", "txtBat_Min_Cell_Volt_Pack", "txtBat_Min_Cell_Volt_Num" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.Bat_Max_Cell_Volt = Convert.ToUInt16(strs[0]);
                        model.Bat_Max_Cell_VoltPack = Convert.ToUInt16(strs[1]);
                        model.Bat_Max_Cell_VoltNum = Convert.ToUInt16(strs[2]);
                        model.Bat_Min_Cell_Volt = Convert.ToUInt16(strs[3]);
                        model.Bat_Min_Cell_Volt_Pack = Convert.ToUInt16(strs[4]);
                        model.Bat_Min_Cell_Volt_Num = Convert.ToUInt16(strs[5]);
                        break;

                    //0x0BA:BCU遥信数据上报6
                    case 0x10BBE0FF:
                        initCount++;
                        strs = new string[6];
                        strs[0] = BytesToIntger(data[1], data[0], 0.1);
                        strs[1] = BytesToIntger(0x00, data[2]);
                        strs[2] = BytesToIntger(0x00, data[3]);
                        strs[3] = BytesToIntger(data[5], data[4], 0.1);
                        strs[4] = BytesToIntger(0x00, data[6]);
                        strs[5] = BytesToIntger(0x00, data[7]);
                        controls = new string[6] { "txtBat_Max_Cell_Temp", "txtBat_Max_Cell_Temp_Pack", "txtBat_Max_Cell_Temp_Num", "txtBat_Min_Cell_Temp", "txtBat_Min_Cell_Temp_Pack", "txtBat_Min_Cell_Temp_Num" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.Bat_Max_Cell_Temp = Convert.ToUInt16(strs[0]);
                        model.Bat_Max_Cell_Temp_Pack = Convert.ToUInt16(strs[1]);
                        model.Bat_Max_Cell_Temp_Num = Convert.ToUInt16(strs[2]);
                        model.Bat_Min_Cell_Temp = Convert.ToUInt16(strs[3]);
                        model.Bat_Min_Cell_Temp_Pack = Convert.ToUInt16(strs[4]);
                        model.Bat_Min_Cell_Temp_Num = Convert.ToUInt16(strs[5]);
                        break;

                    //0x0BC:BCU遥测数据1
                    case 0x10BCE0FF:
                        initCount++;
                        strs = new string[4] { "0.1", "0.1", "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtBattery_Charge_Voltage", "txtCharge_Current_Limitation", "txtDischarge_Current_Limitation", "txtBattery_Discharge_Voltage" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.Battery_Charge_Voltage = Convert.ToDouble(strs[0]);
                        model.Charge_Current_Limitation = Convert.ToDouble(strs[1]);
                        model.Discharge_Current_Limitation = Convert.ToDouble(strs[2]);
                        model.Battery_Discharge_Voltage = Convert.ToDouble(strs[3]);
                        break;

                    //0x0BD:BCU遥测数据2
                    case 0x10BDE0FF:
                        initCount++;
                        strs = new string[4] { "0.1", "0.1", "0.1", "1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtCluster_Voltage", "txtCluster_Current", "txtMax_Power_Terminal_Temperature", "txtCycles" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.Cluster_Voltage = Convert.ToDouble(strs[0]);
                        model.Cluster_Current = Convert.ToDouble(strs[1]);
                        model.Max_Power_Terminal_Temperature = Convert.ToDouble(strs[2]);
                        model.Cycles = Convert.ToDouble(strs[3]);
                        break;

                    //0x0BE:BCU遥测数据3
                    case 0x10BEE0FF:
                        initCount++;
                        strs = new string[4] { "0.1", "0.1", "1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtRemaining_Total_Capacity", "txtBat_Temp", "txtCluster_Rate_Power", "txtBat_Bus_Volt" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.Remaining_Total_Capacity = Convert.ToDouble(strs[0]);
                        model.Bat_Temp = Convert.ToDouble(strs[1]);
                        model.Cluster_Rate_Power = Convert.ToDouble(strs[2]);
                        model.Bat_Bus_Volt = Convert.ToDouble(strs[3]);
                        break;

                    //0x0BF:BCU遥测数据4
                    case 0x10BFE0FF:
                        initCount++;
                        strs = new string[5];
                        //strs[0] = BytesToIntger(0x00,data[0]);
                        strs[1] = BytesToIntger(0x00, data[1]);
                        strs[2] = BytesToIntger(0x00, data[2]);
                        strs[3] = BytesToIntger(0x00, data[3]);
                        strs[4] = BytesToIntger(0x00, data[4]);

                        controls = new string[5] { "txtBms_State", "txtCluster_SOC", "txtCluster_SOH", "txtPack_Num", "txtHW_Version" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        txtBms_State.Text = Enum.Parse(typeof(BMSState), (Convert.ToInt32(data[0].ToString("X2"), 16) & 0x0f).ToString()).ToString();
                        model.Bms_State = txtBms_State.Text;
                        model.Cluster_SOC = Convert.ToUInt16(strs[1]);
                        model.Cluster_SOH = Convert.ToUInt16(strs[2]);
                        model.Pack_Num = Convert.ToUInt16(strs[3]);
                        model.HW_Version = Convert.ToUInt16(strs[4]);
                        break;

                    //0x0C0:BCU系统时间
                    case 0x10C0E0FF:
                        initCount++;
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
                        txtBCU_System_Time.Text = date.ToString();
                        break;

                    //0x0C1:模拟量与测试结果1(一般用于ate测试)
                    case 0x10C1E0FF:
                        initCount++;
                        strs = new string[4] { "1", "1", "1", "1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtMax_Ring_Charge_Zero_Volt", "txtMin_Ring_Charge_Zero_Volt", "txtMax_Ring_Discharge_Zero_Volt", "txtMin_Ring_Discharge_Zero_Volt" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        model.Max_Ring_Charge_Zero_Volt = Convert.ToDouble(strs[0]);
                        model.Min_Ring_Charge_Zero_Volt = Convert.ToDouble(strs[1]);
                        model.Max_Ring_Discharge_Zero_Volt = Convert.ToDouble(strs[2]);
                        model.Min_Ring_Discharge_Zero_Volt = Convert.ToDouble(strs[3]);
                        break;

                    //0x0C2:模拟量与测试结果2(一般用于ate测试)
                    case 0x10C2E0FF:
                        initCount++;
                        strs = new string[5];

                        strs[0] = BytesToIntger(data[3], data[2], 0.1);
                        strs[1] = BytesToIntger(0x00, data[4]);
                        //strs[2] = BytesToIntger(0x00, data[5]);
                        strs[3] = BytesToIntger(0x00, data[6]);
                        //strs[4] = BytesToIntger(0x00, data[7]);
                        controls = new string[5] { "txtRT1_Tempture", "txtTestResult", "txtReset_Mode", "txtDry2_In_Status", "txtWake_Source" };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }
                        txtReset_Mode.Text = Enum.Parse(typeof(ResetMode), (Convert.ToInt32(data[5].ToString("X2"), 16) & 0x0f).ToString()).ToString();
                        txtWake_Source.Text = Enum.Parse(typeof(WakeSource), (Convert.ToInt32(data[7].ToString("X2"), 16) & 0x0f).ToString()).ToString();
                        model.RT1_Tempture = Convert.ToUInt16(strs[0]);


                        byte TestResult = Convert.ToByte(strs[1]);

                        txtEeprom_Test_Result.Text = GetBit(TestResult, 0).ToString();
                        txtEeprom_Test_Result.Text = GetBit(TestResult, 1).ToString();
                        txtTest_Result_485.Text = GetBit(TestResult, 2).ToString();
                        txtCAN1_Test_Result.Text = GetBit(TestResult, 3).ToString();
                        txtCAN2_Test_Result.Text = GetBit(TestResult, 4).ToString();
                        txtCAN3_Test_Result.Text = GetBit(TestResult, 5).ToString();
                        model.Eeprom_Test_Result = txtEeprom_Test_Result.Text;
                        model.Test_Result_485 = txtTest_Result_485.Text;
                        model.CAN1_Test_Result = txtCAN1_Test_Result.Text;
                        model.CAN2_Test_Result = txtCAN2_Test_Result.Text;
                        model.CAN3_Test_Result = txtCAN3_Test_Result.Text;
                        model.Reset_Mode = txtReset_Mode.Text;
                        model.Dry2_In_Status = Convert.ToUInt16(strs[3]);
                        model.Wake_Source = txtWake_Source.Text;
                        break;

                    //故障显示
                    case 0x10C3E0FF:
                        initCount++;
                        richTextBox4.Clear(); richTextBox5.Clear(); richTextBox6.Clear();
                        analysisLog(data, 2);
                        break;
                    case 0x10C4E0FF:
                    case 0x10C5E0FF:
                        richTextBox1.Clear(); richTextBox2.Clear(); richTextBox3.Clear();
                        analysisLog(data, 0);
                        break;
                    case 0x10C6E0FF:
                        //richTextBox1.Clear(); richTextBox2.Clear(); richTextBox3.Clear();
                        analysisLog(data, 1);
                        break;

                    //序列号
                    case 0x10F3E0FF:
                        string strSn = GetPackSN(data);

                        if (!string.IsNullOrEmpty(strSn))
                            txtSN.Text = strSn;
                        break;
                    case 0x10F9E0FF:
                        initCount++;
                        //BCU软件版本
                        string[] bcu_soft = new string[3];
                        for (int i = 0; i < 3; i++)
                        {
                            bcu_soft[i] = data[i + 2].ToString().PadLeft(2, '0');
                        }
                        txtSoftware_Version_BCU.Text = Encoding.ASCII.GetString(new byte[] { data[1] }) + string.Join("", bcu_soft);
                        //BCU硬件版本
                        string[] bsm_HW = new string[2];
                        for (int i = 0; i < 2; i++)
                        {
                            bsm_HW[i] = data[i + 5].ToString().PadLeft(2, '0');
                        }
                        txtHardware_Version_BCU.Text = string.Join("", bsm_HW);

                        model.BCUSaftwareVersion = txtSoftware_Version_BCU.Text;
                        model.BCUHardwareVersion = txtHardware_Version_BCU.Text;
                        break;

                }

                model.PackID = FrmMain.BCU_ID.ToString("X2");
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
        private int[] BytesToBit(byte[] data)
        {
            int[] numbers = new int[8];
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = Convert.ToInt32(data[i].ToString("X2"), 16);
            }
            return numbers;
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
                        getLog(out msg, i, j, faultNum);
                        string type = "";
                        if (faultNum == 2)
                        {
                            switch (msg[1])
                            {
                                case "1":
                                    richTextBox6.AppendText(msg[0] + "\r");
                                    model.Warning2 = richTextBox6.Text.Replace("\n", "，").Replace("\r", "，");
                                    type = "告警";
                                    break;
                                case "2":
                                    richTextBox5.AppendText(msg[0] + "\r");
                                    model.Protection2 = richTextBox5.Text.Replace("\n", "，").Replace("\r", "，");
                                    type = "保护";
                                    break;
                                case "3":
                                    richTextBox4.AppendText(msg[0] + "\r");
                                    model.Fault2 = richTextBox4.Text.Replace("\n", "，").Replace("\r", "，");
                                    type = "故障";
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
                                    type = "告警";
                                    break;
                                case "2":
                                    richTextBox2.AppendText(msg[0] + "\r");
                                    model.Protection = richTextBox2.Text.Replace("\n", "，").Replace("\r", "，");
                                    type = "保护";
                                    break;
                                case "3":
                                    richTextBox1.AppendText(msg[0] + "\r");
                                    model.Fault = richTextBox1.Text.Replace("\n", "，").Replace("\r", "，");
                                    type = "故障";
                                    break;
                            }
                        }

                        var query = FrmMain.AlarmList.FirstOrDefault(t => t.Id == FrmMain.BMS_ID && t.Content == "BCU:" + msg[0]);
                        if (query == null)
                        {
                            FrmMain.AlarmList.Add(new AlarmInfo()
                            {
                                DataTime = DateTime.Now.ToString("yy-MM-dd HH:mm:ss"),
                                Id = FrmMain.BMS_ID,
                                Type = type,
                                Content = $"BCU:{msg[0]}"
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
                                Content = $"[解除]BCU:{msg[0]}"
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
            switch (faultNum)
            {
                case 0:
                    faultInfos = FrmMain.FaultInfos;// 0x08:BMS发送内部电池故障信息1     BCU 0x0C5
                    break;
                case 1:
                    faultInfos = FrmMain.FaultInfos2;//0x45:BMS发送内部电池故障信息2     BCU 0x0C6
                    break;
                case 2:
                    faultInfos = FrmMain.FaultInfos3;//0x0C3:BCU故障上报1
                    break;
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

        public void UpdateControlStatus(List<Dictionary<short, string>> statusByteList, ushort value)
        {
            foreach (var statusByte in statusByteList)
            {
                foreach (var kvp in statusByte)
                {
                    short bitIndex = kvp.Key;
                    string controlName = kvp.Value;
                    var control = this.Controls.Find(controlName, true).FirstOrDefault() as PictureBox;
                    if (control != null)
                    {
                        control.BackColor = GetBit((byte)value, bitIndex) == 0 ? Color.Red : Color.Green;
                    }
                }
            }
        }
    }
}