using Microsoft.Office.Interop.Excel;
using NPOI.OpenXmlFormats.Spreadsheet;
using SofarBMS.Helper;
using SofarBMS.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using DataTable = System.Data.DataTable;

namespace SofarBMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //线程状态
        private bool flag = false;
        private RealtimeData_GTX5000S model = null;
        private Dictionary<uint, RealtimeData_GTX5000S> allQueue = new Dictionary<uint, RealtimeData_GTX5000S>();

        // 启动消费者线程，这里假设我们处理每个优先级的队列  
        CancellationTokenSource cts;
        string[] packSN = new string[3];
        StringBuilder protectionState = new StringBuilder();
        StringBuilder faultState = new StringBuilder();
        StringBuilder warningState = new StringBuilder();

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!EcanHelper.IsConnection)
            {
                if (!Connect())
                    return;

                btnConnect.Text = "Disconnect";
                EcanHelper.IsConnection = true;
                Task.Run(() => { EcanHelper.Receive(); });

                Task.Run(() =>
                {
                    while (true)
                    {
                        if (!EcanHelper.IsConnection)
                            continue;

                        if (cts != null && !cts.IsCancellationRequested)
                        {
                            Consumer();
                        }
                    }
                });

                Task.Run(() =>
                {
                    while (true)
                    {
                        if (!EcanHelper.IsConnection)
                            continue;

                        ////获取实时数据指令
                        //EcanHelper.Send(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                        //               , new byte[] { 0xE0, FrmMain.BMS_ID, 0x2C, 0x10 });

                        // 等待一段时间，保存实时数据信息
                        Task.Delay(1000 * 1).Wait();

                        SaveRealtimeData();
                    }
                });
            }
            else
            {
                ECANHelper.CloseDevice(0, 0);
                btnConnect.Text = "Connect";
                EcanHelper.IsConnection = false;
            }
        }

        private void SaveRealtimeData()
        {
            lock (allQueue)
            {
                //List<RealtimeData> lists = new List<RealtimeData>();
                foreach (var _queue in allQueue)
                {
                    uint id = _queue.Key;
                    RealtimeData_GTX5000S item = _queue.Value;
                    //lists.Add(item);

                    var filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}//Log//GTX5000S_{id}_{DateTime.Now.ToString("yyyy-MM-dd")}.csv";

                    ////用于确定指定文件是否存在
                    //if (!File.Exists(filePath))
                    //{
                    //    File.AppendAllText(filePath, item.GetHeader() + "\r\n");
                    //}
                    File.AppendAllText(filePath, item.GetValue() + "\r\n");
                }


                //DataTable dt = ModelsToDataTable(lists);
                //List<DataTable> dtList = new List<DataTable>();
                //foreach (var item in dt.Rows)
                //{
                //    dtList.Add(item);
                //}

                //EPPlusHelpr.LoadFromDataTables(new List<DataTable>(){dt });
            }
        }
        /// <summary>
        /// List泛型转换DataTable.
        /// </summary>
        public static DataTable ModelsToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                tb.Rows.Add(values);
            }
            return tb;
        }
        /// <summary>
        /// 如果类型可空，则返回基础类型，否则返回类型
        /// </summary>
        private static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
        /// <summary>
        /// 指定类型是否可为空
        /// </summary>
        private static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private bool Connect(string baud = "500Kbps")
        {
            if (ECANHelper.OpenDevice(1, 0, 0) != ECANStatus.STATUS_OK)
            {
                MessageBox.Show(LanguageHelper.GetLanguage("OpenCAN_Error"));
                return false;
            }

            INIT_CONFIG INITCONFIG = new INIT_CONFIG();
            INITCONFIG.AccCode = 0;
            INITCONFIG.AccMask = 0xffffffff;
            INITCONFIG.Filter = 0;
            switch (baud)
            {
                case "125Kbps":
                    INITCONFIG.Timing0 = 0x03;
                    INITCONFIG.Timing1 = 0x1C;
                    break;
                case "250Kbps":
                    INITCONFIG.Timing0 = 0x01;
                    INITCONFIG.Timing1 = 0x1C;
                    break;
                case "500Kbps":
                    INITCONFIG.Timing0 = 0x00;
                    INITCONFIG.Timing1 = 0x1C;
                    break;
                case "1000Kbps":
                    INITCONFIG.Timing0 = 0x00;
                    INITCONFIG.Timing1 = 0x14;
                    break;
            }
            INITCONFIG.Mode = 0;


            if (ECANHelper.InitCAN(1, 0, 0, ref INITCONFIG) != ECANStatus.STATUS_OK)
            {
                MessageBox.Show(LanguageHelper.GetLanguage("InitCAN_Error"));
                ECANHelper.CloseDevice(1, 0);
                return false;
            }

            if (ECANHelper.StartCAN(1, 0, 0) != ECANStatus.STATUS_OK)
            {
                MessageBox.Show(LanguageHelper.GetLanguage("StartCAN_Error"));
                ECANHelper.CloseDevice(1, 0);
                return false;
            }

            return true;
        }

        private void PrintInfo(CAN_OBJ coMsg, int p = -1)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new System.Action(() =>
                {
                    string ss = "";
                    for (int i = 0; i < coMsg.Data.Length; i++)
                    {
                        ss += " " + coMsg.Data[i].ToString("X2");
                    }

                    richTextBox1.AppendText($"{System.DateTime.Now.ToString("HH:mm:ss:fff")} Priority{p} Dequeu   CAN_ID:{coMsg.ID.ToString("X8")},Data：{ss.ToString()}\r\n");
                    richTextBox1.ScrollToCaret();
                }));
            }
            else
            {
                string ss = "";
                for (int i = 0; i < coMsg.Data.Length; i++)
                {
                    ss += " " + coMsg.Data[i].ToString("X2");
                }

                richTextBox1.AppendText($"{System.DateTime.Now.ToString("HH:mm:ss:fff")} Dequeu   CAN_ID:{coMsg.ID.ToString("X8")},Data：{ss.ToString()}\r\n");
            }
        }

        public void analysisData(uint canID, byte[] data)
        {
            // if ((canID & 0xff) != FrmMain.BMS_ID)
            //     return;

            uint devID = EcanHelper.AnalysisID(canID);

            if (model == null)
                model = new RealtimeData_GTX5000S();

            string[] strs;
            string[] strs_1;

            model.PackID = devID.ToString();

            switch (canID | 0xff)
            {
                case 0x1003FFFF:
                    string batteryStatus = "";
                    switch (Convert.ToInt32(data[0].ToString("X2"), 16) & 0x0f)//低四位
                    {
                        case 0: batteryStatus = LanguageHelper.GetLanguage("State_Standby"); break;
                        case 1: batteryStatus = LanguageHelper.GetLanguage("State_Charging"); break;
                        case 2: batteryStatus = LanguageHelper.GetLanguage("State_Discharge"); break;
                        case 3: batteryStatus = LanguageHelper.GetLanguage("State_Hibernate"); break;
                        default: batteryStatus = ""; break;
                    }
                    model.BatteryStatus = allQueue[devID].BatteryStatus = batteryStatus;

                    string bmsStatus = "";
                    switch (((Convert.ToInt32(data[0].ToString("X2"), 16) & 0xf0) >> 4))//高四位
                    {
                        case 0: bmsStatus = LanguageHelper.GetLanguage("BmsStatus_Post"); break;
                        case 1: bmsStatus = LanguageHelper.GetLanguage("BmsStatus_Run"); break;
                        case 2: bmsStatus = LanguageHelper.GetLanguage("BmsStatus_Fault"); break;
                        case 3: bmsStatus = LanguageHelper.GetLanguage("BmsStatus_Upgrade"); break;
                        case 4: bmsStatus = LanguageHelper.GetLanguage("BmsStatus_Shutdown"); break;
                    }
                    model.BmsStatus = allQueue[devID].BmsStatus = bmsStatus;

                    strs = new string[2] { "0.1", "0.1" };
                    for (int i = 0; i < strs.Length; i++)
                    {
                        strs[i] = BytesToIntger(data[i * 2 + 2], data[i * 2 + 1], Convert.ToDouble(strs[i]));
                    }

                    //controls = new string[2] { "txtChargeCurrentLimitation", "txtDischargeCurrentLimitation" };
                    //for (int i = 0; i < controls.Length; i++)
                    //{
                    //    (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                    //}

                    //BMS测量的P-对B-电压
                    strs_1 = new string[1] { "1" };
                    strs_1[0] = Convert.ToUInt16(data[7].ToString("X2") + data[6].ToString("X2"), 16).ToString();
                    //controls_1 = new string[1] { "txtLOAD_VOLT_N" };
                    //(this.Controls.Find(controls_1[0], true)[0] as TextBox).Text = strs_1[0];

                    Dictionary<int, string> setContorls = new Dictionary<int, string>() {
                            {0,"pbChargeMosEnable" },
                            {1,"pbDischargeMosEnable" },
                            {2,"pbPrechgMosEnable" },
                            {3,"pbStopChgEnable" },
                            {4,"pbHeatEnable" }
                        };
                    for (short i = 0; i < setContorls.Count; i++)
                    {
                        //if (GetBit(data[5], i) == 1 && this.Controls.Find(setContorls[i], true) != null)
                        //{
                        //    (this.Controls.Find(setContorls[i], true)[0] as PictureBox).BackColor = Color.Red;
                        //}
                        //else
                        //{
                        //    (this.Controls.Find(setContorls[i], true)[0] as PictureBox).BackColor = Color.Green;
                        //}
                    }
                    model.ChargeCurrentLimitation = allQueue[devID].ChargeCurrentLimitation = Convert.ToDouble(strs[0]);
                    model.DischargeCurrentLimitation = allQueue[devID].DischargeCurrentLimitation = Convert.ToDouble(strs[1]);
                    model.LOAD_VOLT_N = allQueue[devID].LOAD_VOLT_N = Convert.ToUInt16(strs_1[0]);
                    model.ChargeMosEnable = allQueue[devID].ChargeMosEnable = (ushort)GetBit(data[5], 0);
                    model.DischargeMosEnable = allQueue[devID].DischargeMosEnable = (ushort)GetBit(data[5], 1);
                    model.PrechgMosEnable = allQueue[devID].PrechgMosEnable = (ushort)GetBit(data[5], 2);
                    model.StopChgEnable = allQueue[devID].StopChgEnable = (ushort)GetBit(data[5], 3);
                    model.HeatEnable = allQueue[devID].HeatEnable = (ushort)GetBit(data[5], 4);
                    break;
                case 0x1004FFFF:
                    strs = new string[4] { "0.1", "0.1", "0.01", "0.1" };
                    for (int i = 0; i < strs.Length; i++)
                    {
                        strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                    }

                    //controls = new string[4] { "txtBatteryVolt", "txtLoadVolt", "txtBatteryCurrent", "txtSOC" };
                    //for (int i = 0; i < strs.Length; i++)
                    //{
                    //    (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                    //}

                    model.BatteryVolt = allQueue[devID].BatteryVolt = Convert.ToDouble(strs[0]);
                    model.LoadVolt = allQueue[devID].LoadVolt = Convert.ToDouble(strs[1]);
                    model.BatteryCurrent = allQueue[devID].BatteryCurrent = Convert.ToDouble(strs[2]);
                    model.SOC = allQueue[devID].SOC = Convert.ToDouble(strs[3]);
                    break;
                case 0x1005FFFF:
                    strs = new string[5];
                    strs[0] = BytesToIntger(data[1], data[0]);
                    strs[1] = BytesToIntger(0x00, data[2]);
                    strs[2] = BytesToIntger(data[4], data[3]);
                    strs[3] = BytesToIntger(0x00, data[5]);
                    strs[4] = (Convert.ToInt32(strs[0]) - Convert.ToInt32(strs[2])).ToString();

                    //controls = new string[5] { "txtBatMaxCellVolt", "txtBatMaxCellVoltNum", "txtBatMinCellVolt", "txtBatMinCellVoltNum", "txtBatDiffCellVolt" };
                    //for (int i = 0; i < controls.Length; i++)
                    //{
                    //    (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                    //}

                    model.BatMaxCellVolt = allQueue[devID].BatMaxCellVolt = Convert.ToUInt16(strs[0]);
                    model.BatMaxCellVoltNum = allQueue[devID].BatMaxCellVoltNum = Convert.ToUInt16(strs[1]);
                    model.BatMinCellVolt = allQueue[devID].BatMinCellVolt = Convert.ToUInt16(strs[2]);
                    model.BatMinCellVoltNum = allQueue[devID].BatMinCellVoltNum = Convert.ToUInt16(strs[3]);
                    model.BatDiffCellVolt = allQueue[devID].BatDiffCellVolt = Convert.ToUInt16(strs[4]);
                    break;
                case 0x1006FFFF:
                    strs = new string[4] { "0.1", "1", "0.1", "1" };
                    strs[0] = BytesToIntger(data[1], data[0], 0.1);
                    strs[1] = BytesToIntger(0x00, data[2]);
                    strs[2] = BytesToIntger(data[4], data[3], 0.1);
                    strs[3] = BytesToIntger(0x00, data[5]);

                    //controls = new string[4] { "txtBatMaxCellTemp", "txtBatMaxCellTempNum", "txtBatMinCellTemp", "txtBatMinCellTempNum" };
                    //for (int i = 0; i < controls.Length; i++)
                    //{
                    //    (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                    //}

                    model.BatMaxCellTemp = allQueue[devID].BatMaxCellTemp = Convert.ToDouble(strs[0]);
                    model.BatMaxCellTempNum = allQueue[devID].BatMaxCellTempNum = Convert.ToUInt16(strs[1]);
                    model.BatMinCellTemp = allQueue[devID].BatMinCellTemp = Convert.ToDouble(strs[2]);
                    model.BatMinCellTempNum = allQueue[devID].BatMinCellTempNum = Convert.ToUInt16(strs[3]);
                    break;
                case 0x1007FFFF:
                    model.TotalChgCap = allQueue[devID].TotalChgCap = Convert.ToDouble(((data[3] << 24) + (data[2] << 16) + (data[1] << 8) + (data[0] & 0xff)) * 0.001);
                    model.TotalDsgCap = allQueue[devID].TotalDsgCap = Convert.ToDouble(((data[7] << 24) + (data[6] << 16) + (data[5] << 8) + (data[4] & 0xff)) * 0.001);
                    //txtTotalChgCap.Text = model.TotalChgCap.ToString();
                    //txtTotalDsgCap.Text = model.TotalDsgCap.ToString();
                    break;
                case 0x1008FFFF:
                    //richTextBox1.Clear(); richTextBox2.Clear(); richTextBox3.Clear();
                    protectionState.Clear();
                    warningState.Clear();
                    faultState.Clear();
                    analysisLog(data);
                    break;
                case 0x1009FFFF:
                    strs = new string[4];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2]);
                    }

                    model.CellVoltage1 = allQueue[devID].CellVoltage1 = Convert.ToUInt32(strs[0]);
                    model.CellVoltage2 = allQueue[devID].CellVoltage2 = Convert.ToUInt32(strs[1]);
                    model.CellVoltage3 = allQueue[devID].CellVoltage3 = Convert.ToUInt32(strs[2]);
                    model.CellVoltage4 = allQueue[devID].CellVoltage4 = Convert.ToUInt32(strs[3]);
                    break;
                case 0x100AFFFF:
                    strs = new string[4];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2]);
                    }

                    model.CellVoltage5 = allQueue[devID].CellVoltage5 = Convert.ToUInt32(strs[0]);
                    model.CellVoltage6 = allQueue[devID].CellVoltage6 = Convert.ToUInt32(strs[1]);
                    model.CellVoltage7 = allQueue[devID].CellVoltage7 = Convert.ToUInt32(strs[2]);
                    model.CellVoltage8 = allQueue[devID].CellVoltage8 = Convert.ToUInt32(strs[3]);
                    break;
                case 0x100BFFFF:
                    strs = new string[4];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2]);
                    }

                    model.CellVoltage9 = allQueue[devID].CellVoltage9 = Convert.ToUInt32(strs[0]);
                    model.CellVoltage10 = allQueue[devID].CellVoltage10 = Convert.ToUInt32(strs[1]);
                    model.CellVoltage11 = allQueue[devID].CellVoltage11 = Convert.ToUInt32(strs[2]);
                    model.CellVoltage12 = allQueue[devID].CellVoltage12 = Convert.ToUInt32(strs[3]);
                    break;
                case 0x100CFFFF:
                    strs = new string[4];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2]);
                    }

                    model.CellVoltage13 = allQueue[devID].CellVoltage13 = Convert.ToUInt32(strs[0]);
                    model.CellVoltage14 = allQueue[devID].CellVoltage14 = Convert.ToUInt32(strs[1]);
                    model.CellVoltage15 = allQueue[devID].CellVoltage15 = Convert.ToUInt32(strs[2]);
                    model.CellVoltage16 = allQueue[devID].CellVoltage16 = Convert.ToUInt32(strs[3]);
                    break;
                case 0x100DFFFF:
                    strs = new string[4] { "0.1", "0.1", "0.1", "0.1" };
                    for (int i = 0; i < strs.Length; i++)
                    {
                        strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                    }

                    model.CellTemperature1 = allQueue[devID].CellTemperature1 = Convert.ToDouble(strs[0]);
                    model.CellTemperature2 = allQueue[devID].CellTemperature2 = Convert.ToDouble(strs[1]);
                    model.CellTemperature3 = allQueue[devID].CellTemperature3 = Convert.ToDouble(strs[2]);
                    model.CellTemperature4 = allQueue[devID].CellTemperature4 = Convert.ToDouble(strs[3]);
                    break;
                case 0x100EFFFF:
                    strs = new string[3] { "0.1", "0.1", "0.1" };
                    for (int i = 0; i < strs.Length; i++)
                    {
                        strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                    }

                    string strAdd = "[1~16]:";
                    for (int i = 6; i < 8; i++)
                    {
                        for (short j = 0; j < 8; j++)
                        {
                            if (GetBit(data[i], j) == 1)
                            {
                                //(this.Controls.Find(string.Format("txtCellvoltage{0}", j + 1 + ((i - 6) * 8)), true)[0] as TextBox).BackColor = Color.Aquamarine;
                                strAdd += "1";
                            }
                            else
                            {
                                //(this.Controls.Find(string.Format("txtCellvoltage{0}", j + 1 + ((i - 6) * 8)), true)[0] as TextBox).BackColor = Color.White;
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

                    model.MosTemperature = allQueue[devID].MosTemperature = Convert.ToDouble(strs[0]);
                    model.EnvTemperature = allQueue[devID].EnvTemperature = Convert.ToDouble(strs[1]);
                    model.SOH = allQueue[devID].SOH = Convert.ToDouble(strs[2]);
                    model.EquaState = allQueue[devID].EquaState = strAdd;
                    break;
                case 0x100FFFFF:
                    model.RemainingCapacity = allQueue[devID].RemainingCapacity = BytesToIntger(data[1], data[0], 0.1);
                    model.FullCapacity = allQueue[devID].FullCapacity = BytesToIntger(data[3], data[2], 0.1);
                    model.CycleTIme = allQueue[devID].CycleTIme = Convert.ToUInt16(BytesToIntger(data[5], data[4]));
                    break;
                case 0x1040FFFF:
                    model.CumulativeDischargeCapacity = allQueue[devID].CumulativeDischargeCapacity = (((data[3] << 24) + (data[2] << 16) + (data[1] << 8) + (data[0] & 0xff))).ToString();
                    break;
                case 0x1041FFFF:
                    model.BalanceTemperature1 = allQueue[devID].BalanceTemperature1 = BytesToIntger(data[1], data[0], 0.1);
                    model.BalanceTemperature2 = allQueue[devID].BalanceTemperature2 = BytesToIntger(data[3], data[2], 0.1);
                    break;
                case 0x1042FFFF:
                    strs = new string[4] { "0.1", "0.1", "0.1", "0.1" };
                    for (int i = 0; i < strs.Length; i++)
                    {
                        strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                    }

                    model.CellTemperature5 = allQueue[devID].CellTemperature5 = Convert.ToDouble(strs[0]);
                    model.CellTemperature6 = allQueue[devID].CellTemperature6 = Convert.ToDouble(strs[1]);
                    model.CellTemperature7 = allQueue[devID].CellTemperature7 = Convert.ToDouble(strs[2]);
                    model.CellTemperature8 = allQueue[devID].CellTemperature8 = Convert.ToDouble(strs[3]);
                    break;
                    //case 0x1027E0FF:
                    //    string strSn = GetPackSN(data);

                    //    if (!string.IsNullOrEmpty(strSn))
                    //        txtSN.Text = strSn;
                    //    break;
                    //case 0x102EE0FF:
                    //    //BMS软件版本
                    //    string[] bsm_soft = new string[3];
                    //    for (int i = 0; i < 3; i++)
                    //    {
                    //        bsm_soft[i] = data[i + 2].ToString().PadLeft(2, '0');
                    //    }
                    //    txtSoftware_Version_Bms.Text = Encoding.ASCII.GetString(new byte[] { data[1] }) + string.Join("", bsm_soft);
                    //    //BMS硬件版本
                    //    string[] bsm_HW = new string[2];
                    //    for (int i = 0; i < 2; i++)
                    //    {
                    //        bsm_HW[i] = data[i + 5].ToString().PadLeft(2, '0');
                    //    }
                    //    txtHardware_Version_Bms.Text = string.Join("", bsm_HW);
                    //    break;
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


        #region BMS发送内部电池故障信息-模块处理
        /// <summary>
        /// 解析日志内容
        /// </summary>
        /// <param name="_bytes"></param>
        /// <returns></returns>
        private void analysisLog(byte[] data)
        {
            string[] msg = new string[2];

            for (int i = 0; i < data.Length; i++)
            {
                for (short j = 0; j < 8; j++)
                {
                    if (GetBit(data[i], j) == 1)
                    {
                        getLog(out msg, i, j);
                        switch (msg[1])
                        {
                            case "1":
                                warningState.Append(msg[0] + "\r");
                                model.Warning = warningState.ToString().Replace("\r", "，");
                                break;
                            case "2":
                                protectionState.Append(msg[0] + "\r");
                                model.Protection = protectionState.ToString().Replace("\r", "，");
                                break;
                            case "3":
                                faultState.Append(msg[0] + "\r");
                                model.Fault = faultState.ToString().Replace("\r", "，");
                                break;
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

        public static string[] getLog(out string[] msg, int row, int column)
        {
            msg = new string[2];
            List<FaultInfo> faultInfos = FrmMain.FaultInfos;

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

        private void Consumer()
        {
            // 等待一段时间，然后停止消费者线程（仅为示例，实际应用中可能有其他停止条件）  
            if (cts == null)
            {
                cts = new CancellationTokenSource();
            }

            Task.Run(() =>
            {
                try
                {
                    foreach (var queue in EcanHelper._queueManager._queues.Values)
                    {
                        foreach (var item in queue.GetConsumingEnumerable(cts.Token))
                        {
                            if (item.Data == null)
                                continue;

                            CAN_OBJ v = (CAN_OBJ)item.Data;
                            analysisData(v.ID, v.Data);
                            PrintInfo(v, item.Priority);
                        }
                    }
                }
                catch (Exception)
                {
                    cts.Cancel();
                    //throw;
                }

            }, cts.Token);
        }

        private void btnStartThread_Click(object sender, EventArgs e)
        {
            if (!flag)
            {
                cts = new CancellationTokenSource();
                btnStartThread.Text = "终止";
                flag = true;
            }
            else
            {
                cts.Cancel();
                btnStartThread.Text = "启动";
                flag = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SQLiteHelper.ConStr = "Data Source=" + Application.StartupPath + "\\DB\\RealtimeDataBase;Pooling=true;FailIfMissing=false";

            allQueue = new Dictionary<uint, RealtimeData_GTX5000S>()
            {
                { 1, new RealtimeData_GTX5000S() {}},
                { 2, new RealtimeData_GTX5000S() {}},
                { 3, new RealtimeData_GTX5000S() {}},
                { 4, new RealtimeData_GTX5000S() {}},
            };
        }
    }
}