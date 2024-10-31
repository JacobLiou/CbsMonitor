using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using SofarBMS.Helper;
using SofarBMS.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SofarBMS.UI
{
    public partial class BMSSystemSetControl : UserControl
    {
        public BMSSystemSetControl()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
        }

        EcanHelper ecanHelper = EcanHelper.Instance;
        public static CancellationTokenSource cts = null;

        string[] boardCode = new string[3];

        string[] bmsCode = new string[3];

        string[] bcuCode = new string[3];

        string[] pcuCode = new string[3];

        short[] bitResult { get; set; }
        short[] bitResult1 { get; set; }
        string strResult { get; set; }

        byte[] canid = new byte[] { 0xE0, FrmMain.BMS_ID, 0x00, 0x10 };

        byte[] bytes = new byte[8];

        bool flag = true;

        private void SystemSetControl_Load(object sender, EventArgs e)
        {
            foreach (Control item in this.Controls)
            {
                GetControls(item);
            }

            foreach (Control item in this.gbControl020.Controls)
            {
                if (item is ComboBox)
                {
                    ComboBox cbb = item as ComboBox;
                    cbb.SelectedIndex = 0;
                }
            }
            txtSlaveAddr.Text = FrmMain.BMS_ID.ToString();

            foreach (Control item in this.gbControl0F0.Controls)
            {
                if (item is ComboBox)
                {
                    ComboBox cbb = item as ComboBox;
                    cbb.SelectedIndex = 0;
                }
            }
            txtSlaveAddr_BCU.Text = FrmMain.BCU_ID.ToString();

            string[] setComms = new string[2] {
                LanguageHelper.GetLanguage("PCUCmd_Stop"),
                LanguageHelper.GetLanguage("PCUCmd_Normal")
            };

            cbbSetComm.Items.Clear();
            cbbSetComm.Items.AddRange(setComms);
            btnSystemset_47.Text = LanguageHelper.GetLanguage("BmsDebug_Start");
            Task.Run(async delegate
            {
                while (!cts.IsCancellationRequested)
                {
                    if (ecanHelper.IsConnection)
                    {
                        if (flag)
                        {
                            List<uint> DataLists = new List<uint>() { 0xAA11, 0xAA22, 0xAA33, 0xAA44, 0xAA55, 0xAA66, 0xAA77, 0xAA88 };

                            for (int i = 0; i < DataLists.Count; i++)
                            {
                                DataSelected(DataLists[i]);

                                Thread.Sleep(100);
                            }

                            //首次进入读取一遍
                            byte[] bytes = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 });


                            ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BMS_ID, 0x2C, 0x10 });

                            //BCU                          
                            ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BCU_ID, 0xF9, 0x10 });

                            flag = false;
                            await Task.Delay(1000);
                        }
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

        public void analysisData(uint canID, byte[] data)
        {
            byte[] canid = BitConverter.GetBytes(canID);

            if (!(((canID & 0xff) == FrmMain.BMS_ID) || ((canID & 0xff) == FrmMain.PCU_ID) || ((canID & 0xff) == FrmMain.BCU_ID)))
                return;

            string[] strs;
            string[] controls;

            int[] numbers = BytesToUint16(data);
            int[] numbers_bit = BytesToBit(data);

            switch (BitConverter.ToUInt32(canid, 0) | 0xff)
            {
                case 0x1020E0FF:
                    // cbbSetComm2.SelectedIndex = data[3] == 0xAA ? 0 : 1;

                    foreach (Control item in this.gbControl020.Controls)
                    {
                        if (item is ComboBox)
                        {
                            ComboBox cbb = item as ComboBox;
                            int index = 0;
                            int.TryParse(cbb.Name.Replace("cbbRequest", ""), out index);

                            int value = -1;
                            switch (data[index])
                            {
                                case 0x00:
                                    value = 0;
                                    break;
                                case 0xAA:
                                    value = 1;
                                    break;
                                case 0x55:
                                    value = 2;
                                    break;
                                default:
                                    break;
                            }
                            cbb.SelectedIndex = value;
                        }
                    }
                    break;
                case 0x10F0E0FF:
                    foreach (Control item in this.gbControl0F0.Controls)
                    {
                        if (item is ComboBox)
                        {
                            ComboBox cbb = item as ComboBox;
                            int index = 0;
                            int.TryParse(cbb.Name.Replace("cbbRequestF0_", ""), out index);

                            int value = -1;
                            switch (data[index])
                            {
                                case 0x00:
                                    value = 0;
                                    break;
                                case 0xAA:
                                    value = 1;
                                    break;
                                case 0x55:
                                    value = 2;
                                    break;
                                default:
                                    break;
                            }
                            cbb.SelectedIndex = value;
                        }
                    }
                    break;
                case 0x0B70E0FF:
                    pcuCode[0] = Encoding.Default.GetString(data).Substring(1);
                    break;
                case 0x0B71E0FF:
                    pcuCode[1] = Encoding.Default.GetString(data).Substring(1);
                    break;
                case 0x0B72E0FF:
                    pcuCode[2] = Encoding.Default.GetString(data).Substring(1);

                    txtPCUSN.Text = String.Join("", pcuCode).Trim();
                    pcuCode = new string[3];
                    break;
                case 0x0B73E0FF:
                    strs = new string[4] { "0.001", "0.001", "0.001", "0.001" };
                    strs[0] = BytesToIntger(data[1], data[0], 0.001);
                    strs[1] = BytesToIntger(data[3], data[2], 0.001);
                    strs[2] = BytesToIntger(data[5], data[4], 0.001);
                    strs[3] = BytesToIntger(data[7], data[6], 0.001);

                    controls = new string[4] { "txtVhvbus_Calibration_Coefficient", "txtVpbus_Calibration_Coefficient", "txtHV_Charge_Current_Calibration_Coefficient", "txtHV_Discharge_Current_Calibration_Coefficient" };
                    for (int i = 0; i < controls.Length; i++)
                    {
                        (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                    }
                    break;
                case 0x0B74E0FF:
                    strs = new string[3] { "0.001", "0.001", "0.001" };
                    strs[0] = BytesToIntger(data[1], data[0], 0.001);
                    strs[1] = BytesToIntger(data[3], data[2], 0.001);
                    strs[2] = BytesToIntger(data[5], data[4], 0.001);

                    controls = new string[3] { "txtLV_Calibration_Coefficient", "txtLV_Charge_Current_Calibration_Coefficient", "txtLV_Discharge_Current_Calibration_Coefficient" };
                    for (int i = 0; i < controls.Length; i++)
                    {
                        (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                    }
                    break;
                case 0x0B76E0FF:
                    string testFlag = BytesToIntger(data[1], data[0]);
                    if ((ushort)GetBit(Convert.ToByte(testFlag), 0) == 1)
                    {
                        ckSystemset_30.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        ckSystemset_30.CheckState = CheckState.Unchecked;
                    }

                    if ((ushort)GetBit(Convert.ToByte(testFlag), 1) == 1)
                    {
                        ckSystemset_31.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        ckSystemset_31.CheckState = CheckState.Unchecked;
                    }


                    if ((ushort)GetBit(Convert.ToByte(testFlag), 2) == 1)
                    {
                        ckSystemset_32.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        ckSystemset_32.CheckState = CheckState.Unchecked;
                    }


                    if ((ushort)GetBit(Convert.ToByte(testFlag), 3) == 1)
                    {
                        ckSystemset_33.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        ckSystemset_33.CheckState = CheckState.Unchecked;
                    }
                    break;
                case 0x0B77E0FF:
                case 0x0B6A5FFF:
                    if (data[1] == 0xAA && (data[0] == 0x33 || data[0] == 0x44 || data[0] == 0x55 || data[0] == 0x66))
                        MessageBox.Show(LanguageHelper.GetLanguage("Tip_Write"));
                    break;
            }

            switch (canid[2])
            {
                case 0x23://额度容量-单体电压/温度
                    txt_60.Text = (numbers[0] * 0.1).ToString();
                    txt_61.Text = numbers[1].ToString();
                    txt_62.Text = numbers[2].ToString();
                    break;
                case 0x24://充电/放电容量
                    txt_63.Text = ((data[3] << 24) + (data[2] << 16) + (data[1] << 8) + (data[0] & 0xff)).ToString();
                    txt_64.Text = ((data[7] << 24) + (data[6] << 16) + (data[5] << 8) + (data[4] & 0xff)).ToString();
                    break;
                case 0x25://SOC
                    txt_65.Text = (Convert.ToInt32(data[1].ToString("X2") + data[0].ToString("X2"), 16) * 0.1).ToString();
                    txt_100.Text = (Convert.ToInt32(data[3].ToString("X2") + data[2].ToString("X2"), 16) * 0.1).ToString();
                    txt_101.Text = (Convert.ToInt32(data[5].ToString("X2") + data[4].ToString("X2"), 16) * 0.1).ToString();
                    break;
                case 0x26:
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
                    dateTimePicker1.Text = date.ToString();
                    break;
                case 0x27:
                    switch (data[0])
                    {
                        case 0:
                            bmsCode[0] = Encoding.Default.GetString(data).Substring(1);
                            break;
                        case 1:
                            bmsCode[1] = Encoding.Default.GetString(data).Substring(1);
                            break;
                        case 2:
                            bmsCode[2] = Encoding.Default.GetString(data).Substring(1);
                            txt_67.Text = String.Join("", bmsCode).Trim();
                            bmsCode = new string[3];
                            break;
                        default:
                            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/data.log", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + data[0].ToString() + Environment.NewLine);
                            break;
                    }
                    break;
                case 0x28:
                    switch (data[0])
                    {
                        case 0:
                            boardCode[0] = Encoding.Default.GetString(data).Substring(1);
                            break;
                        case 1:
                            boardCode[1] = Encoding.Default.GetString(data).Substring(1);
                            break;
                        case 2:
                            boardCode[2] = Encoding.Default.GetString(data).Substring(1);
                            txtBoardSN.Text = String.Join("", boardCode).Trim();
                            boardCode = new string[3];
                            break;
                        default:
                            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/data.log", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + data[0].ToString() + Environment.NewLine);
                            break;
                    }
                    break;
                case 0x29:
                    int selVal = numbers_bit[0] - 1;
                    cbb_103.SelectedIndex = selVal;

                    txt_104.Text = numbers_bit[1].ToString();
                    txtFlag.Text = (Convert.ToInt32(data[3].ToString("X2") + data[2].ToString("X2"))).ToString();
                    break;
                case 0x2F:
                    Dictionary<short, Button[]> buttons = new Dictionary<short, Button[]>();
                    buttons.Add(0, new Button[] { btnSystemset_45_Lifted1, btnSystemset_43_Close1, btnSystemset_44_Open1 });
                    buttons.Add(1, new Button[] { btnSystemset_45_Lifted2, btnSystemset_43_Close2, btnSystemset_44_Open2 });
                    buttons.Add(2, new Button[] { btnSystemset_45_Lifted3, btnSystemset_43_Close3, btnSystemset_44_Open3 });
                    buttons.Add(3, new Button[] { btnSystemset_45_Lifted4, btnSystemset_43_Close4, btnSystemset_44_Open4 });
                    buttons.Add(4, new Button[] { btnSystemset_45_Lifted5, btnSystemset_43_Close5, btnSystemset_44_Open5 });
                    buttons.Add(5, new Button[] { btnSystemset_45_Lifted6, btnSystemset_43_Close6, btnSystemset_44_Open6 });
                    buttons.Add(6, new Button[] { btnSystemset_45_Lifted7, btnSystemset_43_Close7, btnSystemset_44_Open7 });
                    buttons.Add(8, new Button[] { btnSystemset_45_Lifted8, btnSystemset_43_Close8, btnSystemset_44_Open8 });
                    buttons.Add(10, new Button[] { btnSystemset_45_Lifted9, btnSystemset_43_Close9, btnSystemset_44_Open9 });
                    buttons.Add(7, new Button[] { btnSystemset_45_Lifted10, btnSystemset_43_Close10, btnSystemset_44_Open10 });

                    //byte[]转为二进制字符串
                    strResult = string.Empty;
                    for (int i = 0; i <= 2; i++)
                    {
                        string strTemp = Convert.ToString(data[i], 2);
                        strTemp = strTemp.Insert(0, new string('0', 8 - strTemp.Length));
                        strResult = strTemp + strResult;
                    }

                    //二进制字符串转化为short[]
                    bitResult = new short[strResult.Length / 2];
                    int index = bitResult.Length - 1;
                    for (int i = 0; i < bitResult.Length; i++)
                    {
                        bitResult[index--] = (short)Convert.ToByte(Convert.ToInt32(strResult.Substring(i * 2, 2)) & 0x03);
                    }

                    //根据short得值来找到对应得button
                    foreach (var item in buttons.Keys)
                    {
                        Button btn = buttons[item][bitResult[item]];
                        string name = btn.Name;
                        btn.Enabled = false;
                    }
                    break;
                case 0x1E:
                    Dictionary<short, Button[]> buttons_unique = new Dictionary<short, Button[]>();
                    buttons_unique.Add(0, new Button[] { btnSystemset_45_Unique_Lifted1, btnSystemset_43_Unique_Close1, btnSystemset_44_Unique_Open1 });
                    buttons_unique.Add(1, new Button[] { btnSystemset_45_Unique_Lifted2, btnSystemset_43_Unique_Close2, btnSystemset_44_Unique_Open2 });
                    buttons_unique.Add(2, new Button[] { btnSystemset_45_Unique_Lifted3, btnSystemset_43_Unique_Close3, btnSystemset_44_Unique_Open3 });

                    //byte[]转为二进制字符串
                    strResult = string.Empty;
                    for (int i = 0; i <= 2; i++)
                    {
                        string strTemp = Convert.ToString(data[i], 2);
                        strTemp = strTemp.Insert(0, new string('0', 8 - strTemp.Length));
                        strResult = strTemp + strResult;
                    }

                    //二进制字符串转化为short[]
                    bitResult = new short[strResult.Length / 2];
                    int index2 = bitResult.Length - 1;
                    for (int i = 0; i < bitResult.Length; i++)
                    {
                        bitResult[index2--] = (short)Convert.ToByte(Convert.ToInt32(strResult.Substring(i * 2, 2)) & 0x03);
                    }

                    //根据short得值来找到对应得button
                    foreach (var item in buttons_unique.Keys)
                    {
                        Button btn = buttons_unique[item][bitResult[item]];
                        string name = btn.Name;
                        btn.Enabled = false;
                    }
                    break;


                //BCU Flash数据
                case 0xEF:
                    if (data[0] == 0x00)
                    {
                        txtFlashData.Text = Encoding.Default.GetString(data).Substring(1);
                    }
                    else
                    {
                        txtFlashData.Text = "";
                    }
                    break;

                //BCU时间
                case 0xF2:
                    StringBuilder date1 = new StringBuilder();
                    date1.Append(numbers_bit[0] + 2000);
                    date1.Append("-");
                    date1.Append(numbers_bit[1]);
                    date1.Append("-");
                    date1.Append(numbers_bit[2]);
                    date1.Append(" ");
                    date1.Append(numbers_bit[3]);
                    date1.Append(":");
                    date1.Append(numbers_bit[4]);
                    date1.Append(":");
                    date1.Append(numbers_bit[5]);
                    dateTimePicker2.Text = date1.ToString();
                    break;

                // BCU PACK_SN
                case 0xF3:
                    switch (data[0])
                    {
                        case 0:
                            bcuCode[0] = Encoding.Default.GetString(data).Substring(1);
                            break;
                        case 1:
                            bcuCode[1] = Encoding.Default.GetString(data).Substring(1);
                            break;
                        case 2:
                            bcuCode[2] = Encoding.Default.GetString(data).Substring(1);
                            txtbcuCode.Text = String.Join("", bcuCode).Trim();
                            bcuCode = new string[3];
                            break;
                        default:
                            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/data.log", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + data[0].ToString() + Environment.NewLine);
                            break;
                    }
                    break;

                //BCU Board_SN
                case 0xF4:
                    switch (data[0])
                    {
                        case 0:
                            boardCode[0] = Encoding.Default.GetString(data).Substring(1);
                            break;
                        case 1:
                            boardCode[1] = Encoding.Default.GetString(data).Substring(1);
                            break;
                        case 2:
                            boardCode[2] = Encoding.Default.GetString(data).Substring(1);
                            txtBCUBoardSN.Text = String.Join("", boardCode).Trim();
                            boardCode = new string[3];
                            break;
                        default:
                            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/data.log", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + data[0].ToString() + Environment.NewLine);
                            break;
                    }
                    break;
                //case 0xF5:
                //    switch (data[0])
                //    {
                //        case 0x01:                                                                                          
                //            txtBCU_Calibration01.Text = Convert.ToUInt16(BytesToIntger(data[1], data[2],0.01)).ToString();                                               
                //            break;
                //        case 0x02:
                //            txtBCU_Calibration02.Text = Convert.ToUInt16(BytesToIntger(data[1], data[2], 0.01)).ToString();
                //            break;
                //        case 0x03:
                //            txtBCU_Calibration03.Text = Convert.ToUInt16(BytesToIntger(data[1], data[2], 0.01)).ToString();
                //            break;
                //        case 0x04:
                //            txtBCU_Calibration04.Text = Convert.ToUInt16(BytesToIntger(data[1], data[2], 0.01)).ToString();
                //            break;
                //        case 0x05:
                //            txtBCU_Calibration05.Text = Convert.ToUInt16(BytesToIntger(data[1], data[2], 0.01)).ToString();
                //            break;
                //        case 0x06:
                //            txtBCU_Calibration06.Text = Convert.ToUInt16(BytesToIntger(data[1], data[2], 0.01)).ToString();
                //            break;
                //        case 0x07:
                //            txtBCU_Calibration07.Text = Convert.ToUInt16(BytesToIntger(data[1], data[2], 0.01)).ToString();
                //            break;
                //        case 0x08:
                //            txtBCU_Calibration08.Text = Convert.ToUInt16(BytesToIntger(data[1], data[2], 0.01)).ToString();
                //            break;
                //    }
                //    break;
                case 0xF6:
                    Dictionary<short, Button[]> buttons_bcu1 = new Dictionary<short, Button[]>();
                    buttons_bcu1.Add(0, new Button[] { btnSystemset_45_BCU1_Lifted1, btnSystemset_43_BCU1_Close1, btnSystemset_44_BCU1_Open1 });
                    buttons_bcu1.Add(1, new Button[] { btnSystemset_45_BCU1_Lifted2, btnSystemset_43_BCU1_Close2, btnSystemset_44_BCU1_Open2 });
                    buttons_bcu1.Add(2, new Button[] { btnSystemset_45_BCU1_Lifted3, btnSystemset_43_BCU1_Close3, btnSystemset_44_BCU1_Open3 });
                    buttons_bcu1.Add(3, new Button[] { btnSystemset_45_BCU1_Lifted4, btnSystemset_43_BCU1_Close4, btnSystemset_44_BCU1_Open4 });

                    //byte[]转为二进制字符串
                    strResult = string.Empty;
                    for (int i = 0; i <= 2; i++)
                    {
                        string strTemp = Convert.ToString(data[i], 2);
                        strTemp = strTemp.Insert(0, new string('0', 8 - strTemp.Length));
                        strResult = strTemp + strResult;
                    }

                    //二进制字符串转化为short[]
                    bitResult1 = new short[strResult.Length / 2];
                    int index4 = bitResult1.Length - 1;
                    for (int i = 0; i < bitResult1.Length; i++)
                    {
                        bitResult1[index4--] = (short)Convert.ToByte(Convert.ToInt32(strResult.Substring(i * 2, 2)) & 0x03);
                    }

                    //根据short得值来找到对应得button
                    foreach (var item in buttons_bcu1.Keys)
                    {
                        Button btn = buttons_bcu1[item][bitResult1[item]];
                        string name = btn.Name;                      
                        btn.Enabled = false;
                        
                    }
                    break;

                case 0xFA:
                    Dictionary<short, Button[]> buttons_bcu = new Dictionary<short, Button[]>();
                    buttons_bcu.Add(0, new Button[] { btnSystemset_45_BCU_Lifted1, btnSystemset_43_BCU_Close1, btnSystemset_44_BCU_Open1 });
                    buttons_bcu.Add(1, new Button[] { btnSystemset_45_BCU_Lifted2, btnSystemset_43_BCU_Close2, btnSystemset_44_BCU_Open2 });
                    buttons_bcu.Add(2, new Button[] { btnSystemset_45_BCU_Lifted3, btnSystemset_43_BCU_Close3, btnSystemset_44_BCU_Open3 });
                    buttons_bcu.Add(3, new Button[] { btnSystemset_45_BCU_Lifted4, btnSystemset_43_BCU_Close4, btnSystemset_44_BCU_Open4 });
                    buttons_bcu.Add(4, new Button[] { btnSystemset_45_BCU_Lifted5, btnSystemset_43_BCU_Close5, btnSystemset_44_BCU_Open5 });
                    buttons_bcu.Add(5, new Button[] { btnSystemset_45_BCU_Lifted6, btnSystemset_43_BCU_Close6, btnSystemset_44_BCU_Open6 });
                    buttons_bcu.Add(6, new Button[] { btnSystemset_45_BCU_Lifted7, btnSystemset_43_BCU_Close7, btnSystemset_44_BCU_Open7 });
                    buttons_bcu.Add(7, new Button[] { btnSystemset_45_BCU_Lifted8, btnSystemset_43_BCU_Close8, btnSystemset_44_BCU_Open8 });
                    buttons_bcu.Add(8, new Button[] { btnSystemset_45_BCU_Lifted9, btnSystemset_43_BCU_Close9, btnSystemset_44_BCU_Open9 });
                    buttons_bcu.Add(9, new Button[] { btnSystemset_45_BCU_Lifted10, btnSystemset_43_BCU_Close10, btnSystemset_44_BCU_Open10 });
                    buttons_bcu.Add(10, new Button[] { btnSystemset_45_BCU_Lifted11, btnSystemset_43_BCU_Close11, btnSystemset_44_BCU_Open11 });
                    buttons_bcu.Add(11, new Button[] { btnSystemset_45_BCU_Lifted12, btnSystemset_43_BCU_Close12, btnSystemset_44_BCU_Open12 });
                    buttons_bcu.Add(12, new Button[] { btnSystemset_45_BCU_Lifted13, btnSystemset_43_BCU_Close13, btnSystemset_44_BCU_Open13 });
                    buttons_bcu.Add(13, new Button[] { btnSystemset_45_BCU_Lifted14, btnSystemset_43_BCU_Close14, btnSystemset_44_BCU_Open14 });
                    buttons_bcu.Add(14, new Button[] { btnSystemset_45_BCU_Lifted15, btnSystemset_43_BCU_Close15, btnSystemset_44_BCU_Open15 });
                    buttons_bcu.Add(15, new Button[] { btnSystemset_45_BCU_Lifted16, btnSystemset_43_BCU_Close16, btnSystemset_44_BCU_Open16 });
                    buttons_bcu.Add(16, new Button[] { btnSystemset_45_BCU_Lifted17, btnSystemset_43_BCU_Close17, btnSystemset_44_BCU_Open17 });
                    buttons_bcu.Add(17, new Button[] { btnSystemset_45_BCU_Lifted18, btnSystemset_43_BCU_Close18, btnSystemset_44_BCU_Open18 });
                    buttons_bcu.Add(18, new Button[] { btnSystemset_45_BCU_Lifted19, btnSystemset_43_BCU_Close19, btnSystemset_44_BCU_Open19 });
                    buttons_bcu.Add(19, new Button[] { btnSystemset_45_BCU_Lifted20, btnSystemset_43_BCU_Close20, btnSystemset_44_BCU_Open20 });
                    buttons_bcu.Add(20, new Button[] { btnSystemset_45_BCU_Lifted21, btnSystemset_43_BCU_Close21, btnSystemset_44_BCU_Open21 });

                    //byte[]转为二进制字符串
                    strResult = string.Empty;
                    //选取byte0-5数据
                    for (int i = 0; i <= 5; i++)
                    {
                        string strTemp = Convert.ToString(data[i], 2);
                        strTemp = strTemp.Insert(0, new string('0', 8 - strTemp.Length));
                        strResult = strTemp + strResult;
                    }

                    //二进制字符串转化为short[]
                    bitResult = new short[strResult.Length / 2];
                    int index5 = bitResult.Length - 1;
                    for (int i = 0; i < bitResult.Length; i++)
                    {
                        bitResult[index5--] = (short)Convert.ToByte(Convert.ToInt32(strResult.Substring(i * 2, 2)) & 0x03);
                    }

                    //根据short得值来找到对应得button
                    foreach (var item in buttons_bcu.Keys)
                    {
                        Button btn = buttons_bcu[item][bitResult[item]];
                        string name = btn.Name;                       
                        btn.Enabled = false;
                       
                    }
                    break;

            }
        }

        #region 读取数据
        private void button1_Click(object sender, EventArgs e)
        {
            List<uint> DataLists = new List<uint>() { 0xAA11, 0xAA22, 0xAA33, 0xAA44, 0xAA55, 0xAA66, 0xAA77, 0xAA88 };//

            for (int i = 0; i < DataLists.Count; i++)
            {
                DataSelected(DataLists[i]);

                Thread.Sleep(100);
            }

            byte[] bytes = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            if (ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 }))
            {
                MessageBox.Show(FrmMain.GetString("keyReadSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyReadFail"));
            }

            //byte[] bytes1 = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            //if (EcanHelper.Send(bytes1, new byte[] { 0xE0, FrmMain.BCU_ID, 0xF9, 0x10 }))
            //{
            //    MessageBox.Show(FrmMain.GetString("keyReadSuccess"));
            //}
            //else
            //{
            //    MessageBox.Show(FrmMain.GetString("keyReadFail"));
            //}

        }

        private void DataSelected(uint type)
        {
            byte[] id = new byte[4] { 0xE0, FrmMain.PCU_ID, 0x77, 0x0B };

            byte[] data = new byte[8];
            data[0] = (byte)(type & 0xff);
            data[1] = (byte)(type >> 8);

            ecanHelper.Send(data, id);
        }

        #endregion

        #region 进入调试AND退出调试
        private void btnDebugStart_Click(object sender, System.EventArgs e)
        {
            if (btnSystemset_47.Text == "结束调试" || btnSystemset_47.Text == "End debugging")
            {
                DialogResult result = MessageBox.Show(LanguageHelper.GetLanguage("BmsDebug_Exit"), LanguageHelper.GetLanguage("BmsDebug_Tip"), MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {
                    byte[] can_id = new byte[] { 0xE0, FrmMain.BMS_ID, 0x20, 0x10 };

                    int num = 0x10 + 0x20 + FrmMain.BMS_ID + 0xe0 + 0x00 + 0x00 + 0x55 + 0x00 + 0x00 + 0x00 + 0x00;

                    byte crc8 = (byte)(num & 0xff);

                    byte[] data = new byte[8] { 0x00, 0x00, 0x55, 0x00, 0x00, 0x00, 0x00, crc8 };

                    ecanHelper.Send(data, can_id);

                    foreach (Control c in gbSystemset_04.Controls)
                    {
                        if (c is Button)
                        {
                            if (c.Name != "btnSystemset_47")
                                c.Enabled = false;
                        }
                    }
                    btnSystemset_47.Enabled = true;
                    btnSystemset_47.Text = LanguageHelper.GetLanguage("BmsDebug_Start");
                    btnSystemset_47.BackColor = Color.Green;
                }
            }
            else
            {
                DialogResult result = MessageBox.Show(LanguageHelper.GetLanguage("BmsDebug_Enter"), LanguageHelper.GetLanguage("BmsDebug_Tip"), MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {
                    ReadCurrentState();

                    byte[] can_id = new byte[] { 0xE0, FrmMain.BMS_ID, 0x20, 0x10 };

                    int num = 0x10 + 0x20 + FrmMain.BMS_ID + 0xe0 + 0x00 + 0x00 + 0xAA + 0x00 + 0x00 + 0x00 + 0x00;

                    byte crc8 = (byte)(num & 0xff);

                    byte[] data = new byte[8] { 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, crc8 };

                    ecanHelper.Send(data, can_id);

                    foreach (Control c in gbSystemset_04.Controls)
                    {
                        if (c is Button)
                        {
                            c.Enabled = true;
                        }
                    }

                    btnSystemset_47.Text = LanguageHelper.GetLanguage("BmsDebug_End");
                    btnSystemset_47.BackColor = Color.Red;
                }
            }
        }

        private void btnDebugStart_BCU_Click(object sender, System.EventArgs e)
        {

            if (btnSystemset_debug.Text == "结束调试" || btnSystemset_debug.Text == "End debugging")
            {
                DialogResult result = MessageBox.Show(LanguageHelper.GetLanguage("BmsDebug_Exit"), LanguageHelper.GetLanguage("BmsDebug_Tip"), MessageBoxButtons.OKCancel);


                if (result == DialogResult.OK)
                {
                    byte[] can_id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xFA, 0x10 };

                    int num = 0x10 + 0x20 + FrmMain.BCU_ID + 0xFA + 0x00 + 0x00 + 0x55 + 0x00 + 0x00 + 0x00 + 0x00;

                    byte crc8 = (byte)(num & 0xff);

                    byte[] data = new byte[8] { 0x00, 0x00, 0x55, 0x00, 0x00, 0x00, 0x00, crc8 };

                    ecanHelper.Send(data, can_id);

                    foreach (Control c in gb0FA.Controls)
                    {
                        if (c is Button)
                        {
                            c.Enabled = false;
                        }
                    }
                    btnSystemset_debug.Enabled = true;
                    btnSystemset_debug.Text = LanguageHelper.GetLanguage("BmsDebug_Start");
                    btnSystemset_debug.BackColor = Color.Green;
                }
            }
            else
            {
                DialogResult result = MessageBox.Show(LanguageHelper.GetLanguage("BmsDebug_Enter"), LanguageHelper.GetLanguage("BmsDebug_Tip"), MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {
                    ReadCurrentState_BCU();

                    byte[] can_id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xFA, 0x10 };

                    int num = 0x10 + 0x20 + FrmMain.BCU_ID + 0xFA + 0x00 + 0x00 + 0xAA + 0x00 + 0x00 + 0x00 + 0x00;

                    byte crc8 = (byte)(num & 0xff);

                    byte[] data = new byte[8] { 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, crc8 };

                    ecanHelper.Send(data, can_id);

                    foreach (Control c in gb0FA.Controls)
                    {
                        if (c is Button)
                        {
                            c.Enabled = true;
                        }
                    }

                    btnSystemset_debug.Text = LanguageHelper.GetLanguage("BmsDebug_End");
                    btnSystemset_debug.BackColor = Color.Red;
                }
            }
        }
        private void btnDebugStart_BCU1_Click(object sender, System.EventArgs e)
        {
            if (btnSystemset_debug1.Text == "结束调试" || btnSystemset_debug1.Text == "End debugging")
            {
                DialogResult result = MessageBox.Show(LanguageHelper.GetLanguage("BmsDebug_Exit"), LanguageHelper.GetLanguage("BmsDebug_Tip"), MessageBoxButtons.OKCancel);


                if (result == DialogResult.OK)
                {
                    byte[] can_id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xF6, 0x10 };

                    int num = 0x10 + 0x20 + FrmMain.BCU_ID + 0xF6 + 0x00 + 0x00 + 0x55 + 0x00 + 0x00 + 0x00 + 0x00;

                    byte crc8 = (byte)(num & 0xff);

                    byte[] data = new byte[8] { 0x00, 0x00, 0x55, 0x00, 0x00, 0x00, 0x00, crc8 };

                    ecanHelper.Send(data, can_id);

                    foreach (Control c in gbControl0F6.Controls)
                    {
                        if (c is Button)
                        {
                            c.Enabled = false;
                        }
                    }
                    btnSystemset_debug1.Enabled = true;
                    btnSystemset_debug1.Text = LanguageHelper.GetLanguage("BmsDebug_Start");
                    btnSystemset_debug1.BackColor = Color.Green;
                }
            }
            else
            {
                DialogResult result = MessageBox.Show(LanguageHelper.GetLanguage("BmsDebug_Enter"), LanguageHelper.GetLanguage("BmsDebug_Tip"), MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {
                    ReadCurrentState_BCU1();

                    byte[] can_id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xF6, 0x10 };

                    int num = 0x10 + 0x20 + FrmMain.BCU_ID + 0xF6 + 0x00 + 0x00 + 0xAA + 0x00 + 0x00 + 0x00 + 0x00;

                    byte crc8 = (byte)(num & 0xff);

                    byte[] data = new byte[8] { 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, crc8 };

                    ecanHelper.Send(data, can_id);

                    foreach (Control c in gbControl0F6.Controls)
                    {
                        if (c is Button)
                        {
                            c.Enabled = true;
                        }
                    }

                    btnSystemset_debug1.Text = LanguageHelper.GetLanguage("BmsDebug_End");
                    btnSystemset_debug1.BackColor = Color.Red;
                }
            }
        }
        #endregion

        #region 读取当前开关机状态
        public void ReadCurrentState()
        {
            byte[] id = new byte[] { 0xE0, FrmMain.BMS_ID, 0x2F, 0x10 };

            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BMS_ID, 0x2F, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            data[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(data, id);
        }

        public void ReadCurrentState(byte[] data)
        {
            byte[] id = new byte[] { 0xE0, FrmMain.BMS_ID, 0x2F, 0x10 };

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BMS_ID, 0x2F, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Array.Copy(data, 0, crcData, 4, data.Length);

            byte[] dataNew = new byte[8];
            Array.Copy(data, 0, dataNew, 0, data.Length);

            dataNew[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(dataNew, id);
        }

        public void ReadCurrentState_BCU()
        {
            byte[] id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xFA, 0x10 };

            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BCU_ID, 0xFA, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            data[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(data, id);
        }
        public void ReadCurrentState_BCU1()
        {
            byte[] id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xF6, 0x10 };

            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BCU_ID, 0xF6, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            data[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(data, id);
        }

        public void TestAte()
        {
            byte[] id = new byte[] { 0xE0, FrmMain.BMS_ID, 0x1E, 0x10 };

            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BMS_ID, 0x1E, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            data[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(data, id);
        }

        public void TestAte(byte[] data)
        {
            byte[] id = new byte[] { 0xE0, FrmMain.BMS_ID, 0x1E, 0x10 };

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BMS_ID, 0x1E, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Array.Copy(data, 0, crcData, 4, data.Length);

            byte[] dataNew = new byte[8];
            Array.Copy(data, 0, dataNew, 0, data.Length);
            dataNew[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(dataNew, id);
        }

        public void TestAte_BCU()
        {
            byte[] id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xFA, 0x10 };

            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BCU_ID, 0xFA, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            data[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(data, id);
        }

        public void TestAte_BCU1()
        {
            byte[] id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xF6, 0x10 };

            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BCU_ID, 0xF6, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            data[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(data, id);
        }

        public void TestAte_BCU(byte[] data)
        {
            byte[] id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xFA, 0x10 };

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BCU_ID, 0xFA, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Array.Copy(data, 0, crcData, 4, data.Length);

            byte[] dataNew = new byte[8];
            Array.Copy(data, 0, dataNew, 0, data.Length);
            dataNew[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(dataNew, id);
        }

        public void TestAte_BCU1(byte[] data)
        {
            byte[] id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xF6, 0x10 };

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BCU_ID, 0xF6, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Array.Copy(data, 0, crcData, 4, data.Length);

            byte[] dataNew = new byte[8];
            Array.Copy(data, 0, dataNew, 0, data.Length);
            dataNew[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(dataNew, id);
        }
        #endregion

        #region 写入当前操作状态值
        private void btnDebugCommand_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                switch (btn.Name)
                {
                    case "btnSystemset_45_Lifted1":
                        bitResult[0] = 0;
                        break;
                    case "btnSystemset_43_Close1":
                        bitResult[0] = 1;
                        break;
                    case "btnSystemset_44_Open1":
                        bitResult[0] = 2;
                        break;
                    case "btnSystemset_45_Lifted2":
                        bitResult[1] = 0;
                        break;
                    case "btnSystemset_43_Close2":
                        bitResult[1] = 1;
                        break;
                    case "btnSystemset_44_Open2":
                        bitResult[1] = 2;
                        break;
                    case "btnSystemset_45_Lifted3":
                        bitResult[2] = 0;
                        break;
                    case "btnSystemset_43_Close3":
                        bitResult[2] = 1;
                        break;
                    case "btnSystemset_44_Open3":
                        bitResult[2] = 2;
                        break;
                    case "btnSystemset_45_Lifted4":
                        bitResult[3] = 0;
                        break;
                    case "btnSystemset_43_Close4":
                        bitResult[3] = 1;
                        break;
                    case "btnSystemset_44_Open4":
                        bitResult[3] = 2;
                        break;
                    case "btnSystemset_45_Lifted5":
                        bitResult[4] = 0;
                        break;
                    case "btnSystemset_43_Close5":
                        bitResult[4] = 1;
                        break;
                    case "btnSystemset_44_Open5":
                        bitResult[4] = 2;
                        break;
                    case "btnSystemset_45_Lifted6":
                        bitResult[5] = 0;
                        break;
                    case "btnSystemset_43_Close6":
                        bitResult[5] = 1;
                        break;
                    case "btnSystemset_44_Open6":
                        bitResult[5] = 2;
                        break;
                    case "btnSystemset_45_Lifted7":
                        bitResult[6] = 0;
                        break;
                    case "btnSystemset_43_Close7":
                        bitResult[6] = 1;
                        break;
                    case "btnSystemset_44_Open7":
                        bitResult[6] = 2;
                        break;
                    case "btnSystemset_45_Lifted10":
                        bitResult[7] = 0;
                        break;
                    case "btnSystemset_43_Close10":
                        bitResult[7] = 1;
                        break;
                    case "btnSystemset_44_Open10":
                        bitResult[7] = 2;
                        break;
                    case "btnSystemset_45_Lifted8":
                        bitResult[8] = 0;
                        break;
                    case "btnSystemset_43_Close8":
                        bitResult[8] = 1;
                        break;
                    case "btnSystemset_44_Open8":
                        bitResult[8] = 2;
                        break;
                    case "btnSystemset_45_Lifted9":
                        bitResult[10] = 0;
                        break;
                    case "btnSystemset_43_Close9":
                        bitResult[10] = 1;
                        break;
                    case "btnSystemset_44_Open9":
                        bitResult[10] = 2;
                        break;

                }

                //short[]转为二级制数组
                strResult = string.Empty;
                for (int i = 0; i < bitResult.Length; i++)
                {
                    string strTemp = Convert.ToString(bitResult[i], 2);
                    strTemp = strTemp.Insert(0, new string('0', 2 - strTemp.Length));
                    strResult = strTemp + strResult;
                }


                //数组转byte[]:0x.... 0xAA CRC8
                byte[] data2 = new byte[strResult.Length / 8];
                for (int i = 0; i < data2.Length; i++)
                {
                    data2[i] = (byte)Convert.ToInt32(strResult.Substring(i * 8, 8), 2);
                }

                //byte数组组装
                int index = 0;
                byte[] data = new byte[7];
                data[index++] = data2[2];
                data[index++] = data2[1];
                data[index++] = data2[0];
                data[index++] = 0x00;
                data[index++] = 0x00;
                data[index++] = 0x00;
                data[index++] = 0xAA;

                ReadCurrentState(data);

                Task.Run(new Action(() =>
                {
                    setUI(true);

                    Thread.Sleep(500);

                    ReadCurrentState();
                }));
            }
        }

        private bool testF = false;
        private void btnDebugCommandUnique_Click(object sender, EventArgs e)
        {
            /*if (!testF)
            {
                DialogResult result = MessageBox.Show(LanguageHelper.GetLanguage("BmsDebug_Enter"), LanguageHelper.GetLanguage("BmsDebug_Tip"), MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {
                    TestAte();

                    byte[] can_id = new byte[] { 0xE0, FrmMain.BMS_ID, 0x20, 0x10 };

                    int num = 0x10 + 0x20 + FrmMain.BMS_ID + 0xe0 + 0x00 + 0x00 + 0xAA + 0xAA + 0x00 + 0x00 + FrmMain.BMS_ID;

                    byte crc8 = (byte)(num & 0xff);

                    byte[] data = new byte[8] { 0x00, 0x00, 0xAA, 0xAA, 0x00, 0x00, FrmMain.BMS_ID, crc8 };

                    ecanHelper.Send(data, can_id);

                    foreach (Control c in gbSystemset_60.Controls)
                    {
                        if (c is Button)
                        {
                            c.Enabled = true;
                        }
                    }
                }

                testF = true;
                return;
            }
            */

            if (sender is Button btn)
            {
                switch (btn.Name)
                {
                    case "btnSystemset_45_Unique_Lifted1":
                        bitResult[0] = 0;
                        break;
                    case "btnSystemset_43_Unique_Close1":
                        bitResult[0] = 1;
                        break;
                    case "btnSystemset_44_Unique_Open1":
                        bitResult[0] = 2;
                        break;
                    case "btnSystemset_45_Unique_Lifted2":
                        bitResult[1] = 0;
                        break;
                    case "btnSystemset_43_Unique_Close2":
                        bitResult[1] = 1;
                        break;
                    case "btnSystemset_44_Unique_Open2":
                        bitResult[1] = 2;
                        break;
                    case "btnSystemset_45_Unique_Lifted3":
                        bitResult[2] = 0;
                        break;
                    case "btnSystemset_43_Unique_Close3":
                        bitResult[2] = 1;
                        break;
                    case "btnSystemset_44_Unique_Open3":
                        bitResult[2] = 2;
                        break;
                }

                //short[]转为二级制数组
                strResult = string.Empty;
                for (int i = 0; i < bitResult.Length; i++)
                {
                    string strTemp = Convert.ToString(bitResult[i], 2);
                    strTemp = strTemp.Insert(0, new string('0', 2 - strTemp.Length));
                    strResult = strTemp + strResult;
                }


                //数组转byte[]:0x.... 0xAA CRC8
                byte[] data2 = new byte[strResult.Length / 8];
                for (int i = 0; i < data2.Length; i++)
                {
                    data2[i] = (byte)Convert.ToInt32(strResult.Substring(i * 8, 8), 2);
                }

                //byte数组组装
                int index = 0;
                byte[] data = new byte[7];
                data[index++] = data2[2];
                data[index++] = data2[1];
                data[index++] = data2[0];
                data[index++] = 0x00;
                data[index++] = 0x00;
                data[index++] = 0x00;
                data[index++] = 0xAA;

                TestAte(data);

                Task.Run(new Action(() =>
                {
                    setUI(true);

                    Thread.Sleep(500);

                    TestAte();
                }));
            }
        }


        private void btnDebugCommandBCU1_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                switch (btn.Name)
                {

                    case "btnSystemset_45_BCU1_Lifted1":
                        bitResult1[0] = 0;
                        break;
                    case "btnSystemset_43_BCU1_Close1":
                        bitResult1[0] = 1;
                        break;
                    case "btnSystemset_44_BCU1_Open1":
                        bitResult1[0] = 2;
                        break;
                    case "btnSystemset_45_BCU1_Lifted2":
                        bitResult1[1] = 0;
                        break;
                    case "btnSystemset_43_BCU1_Close2":
                        bitResult1[1] = 1;
                        break;
                    case "btnSystemset_44_BCU1_Open2":
                        bitResult1[1] = 2;
                        break;
                    case "btnSystemset_45_BCU1_Lifted3":
                        bitResult1[2] = 0;
                        break;
                    case "btnSystemset_43_BCU1_Close3":
                        bitResult1[2] = 1;
                        break;
                    case "btnSystemset_44_BCU1_Open3":
                        bitResult1[2] = 2;
                        break;
                    case "btnSystemset_45_BCU1_Lifted4":
                        bitResult1[3] = 0;
                        break;
                    case "btnSystemset_43_BCU1_Close4":
                        bitResult1[3] = 1;
                        break;
                    case "btnSystemset_44_BCU1_Open4":
                        bitResult1[3] = 2;
                        break;
                }
                //short[]转为二级制数组
                strResult = string.Empty;
                for (int i = 0; i < bitResult1.Length; i++)
                {
                    string strTemp = Convert.ToString(bitResult1[i], 2);
                    strTemp = strTemp.Insert(0, new string('0', 2 - strTemp.Length));
                    strResult = strTemp + strResult;
                }


                //数组转byte[]:0x.... 0xAA CRC8
                byte[] data2 = new byte[strResult.Length / 8];
                for (int i = 0; i < data2.Length; i++)
                {
                    data2[i] = (byte)Convert.ToInt32(strResult.Substring(i * 8, 8), 2);
                }

                //byte数组组装
                int num = 0;
                byte[] data = new byte[7];
                data[num++] = data2[2];
                data[num++] = data2[1];
                data[num++] = data2[0];
                data[num++] = 0x00;
                data[num++] = 0x00;
                data[num++] = 0x00;
                data[num++] = 0xAA;
                //ReadCurrentState_BCU1(data);
                TestAte_BCU1(data);

                Task.Run(new Action(() =>
                {
                    setUI_BCU1(true);

                    Thread.Sleep(500);
                    //ReadCurrentState_BCU1();
                    TestAte_BCU1();
                }));
            }
        }

        private void btnDebugCommandBCU_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                switch (btn.Name)
                {
                    case "btnSystemset_45_BCU_Lifted1":
                        bitResult[0] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close1":
                        bitResult[0] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open1":
                        bitResult[0] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted2":
                        bitResult[1] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close2":
                        bitResult[1] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open2":
                        bitResult[1] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted3":
                        bitResult[2] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close3":
                        bitResult[2] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open3":
                        bitResult[2] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted4":
                        bitResult[3] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close4":
                        bitResult[3] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open4":
                        bitResult[3] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted5":
                        bitResult[4] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close5":
                        bitResult[4] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open5":
                        bitResult[4] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted6":
                        bitResult[5] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close6":
                        bitResult[5] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open6":
                        bitResult[5] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted7":
                        bitResult[6] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close7":
                        bitResult[6] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open7":
                        bitResult[6] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted8":
                        bitResult[7] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close8":
                        bitResult[7] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open8":
                        bitResult[7] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted9":
                        bitResult[8] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close9":
                        bitResult[8] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open9":
                        bitResult[8] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted10":
                        bitResult[9] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close10":
                        bitResult[9] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open10":
                        bitResult[9] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted11":
                        bitResult[10] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close11":
                        bitResult[10] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open11":
                        bitResult[10] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted12":
                        bitResult[11] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close12":
                        bitResult[11] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open12":
                        bitResult[11] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted13":
                        bitResult[12] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close13":
                        bitResult[12] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open13":
                        bitResult[12] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted14":
                        bitResult[13] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close14":
                        bitResult[13] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open14":
                        bitResult[13] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted15":
                        bitResult[14] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close15":
                        bitResult[14] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open15":
                        bitResult[14] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted16":
                        bitResult[15] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close16":
                        bitResult[15] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open16":
                        bitResult[15] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted17":
                        bitResult[16] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close17":
                        bitResult[16] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open17":
                        bitResult[16] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted18":
                        bitResult[17] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close18":
                        bitResult[17] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open18":
                        bitResult[17] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted19":
                        bitResult[18] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close19":
                        bitResult[18] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open19":
                        bitResult[18] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted20":
                        bitResult[19] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close20":
                        bitResult[19] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open20":
                        bitResult[19] = 2;
                        break;
                    case "btnSystemset_45_BCU_Lifted21":
                        bitResult[20] = 0;
                        break;
                    case "btnSystemset_43_BCU_Close21":
                        bitResult[20] = 1;
                        break;
                    case "btnSystemset_44_BCU_Open21":
                        bitResult[20] = 2;
                        break;
                }
                //short[] 转为二级制数组
                strResult = string.Empty;
                for (int i = 0; i < bitResult.Length; i++)
                {
                    string strTemp = Convert.ToString(bitResult[i], 2);
                    strTemp = strTemp.Insert(0, new string('0', 2 - strTemp.Length));
                    strResult = strTemp + strResult;
                }

                //数组转byte[]:0x.... 0xAA CRC8
                byte[] data2 = new byte[strResult.Length / 8];
                for (int i = 0; i < data2.Length; i++)
                {
                    data2[i] = (byte)Convert.ToInt32(strResult.Substring(i * 8, 8), 2);
                }

                //byte数组组装
                int num = 0;
                byte[] data = new byte[7];
                data[num++] = data2[5];
                data[num++] = data2[4];
                data[num++] = data2[3];
                data[num++] = data2[2];
                data[num++] = data2[1];
                data[num++] = data2[0];
                data[num++] = 0xAA;

                //0xAA：强制控制
                TestAte_BCU(data);

                Task.Run(new Action(() =>
                {
                    setUI_BCU(true);

                    Thread.Sleep(500);

                    //0x00：查询控制状态
                    TestAte_BCU();
                }));
            }
        }

        #endregion

        #region UI启用和禁用设置
        private void setUI(bool state)
        {
            this.Invoke(new Action(() =>
            {
                btnSystemset_44_Open1.Enabled = state;
                btnSystemset_43_Close1.Enabled = state;
                btnSystemset_45_Lifted1.Enabled = state;

                btnSystemset_44_Open2.Enabled = state;
                btnSystemset_43_Close2.Enabled = state;
                btnSystemset_45_Lifted2.Enabled = state;

                btnSystemset_44_Open3.Enabled = state;
                btnSystemset_43_Close3.Enabled = state;
                btnSystemset_45_Lifted3.Enabled = state;

                btnSystemset_44_Open6.Enabled = state;
                btnSystemset_43_Close6.Enabled = state;
                btnSystemset_45_Lifted6.Enabled = state;

                btnSystemset_44_Open5.Enabled = state;
                btnSystemset_43_Close5.Enabled = state;
                btnSystemset_45_Lifted5.Enabled = state;

                btnSystemset_44_Open4.Enabled = state;
                btnSystemset_43_Close4.Enabled = state;
                btnSystemset_45_Lifted4.Enabled = state;

                btnSystemset_45_Lifted7.Enabled = state;
                btnSystemset_43_Close7.Enabled = state;
                btnSystemset_44_Open7.Enabled = state;

                btnSystemset_45_Lifted8.Enabled = state;
                btnSystemset_43_Close8.Enabled = state;
                btnSystemset_44_Open8.Enabled = state;

                btnSystemset_45_Lifted9.Enabled = state;
                btnSystemset_43_Close9.Enabled = state;
                btnSystemset_44_Open9.Enabled = state;

                btnSystemset_45_Lifted10.Enabled = state;
                btnSystemset_43_Close10.Enabled = state;
                btnSystemset_44_Open10.Enabled = state;

                btnSystemset_44_Unique_Open1.Enabled = state;
                btnSystemset_43_Unique_Close1.Enabled = state;
                btnSystemset_45_Unique_Lifted1.Enabled = state;

                btnSystemset_44_Unique_Open2.Enabled = state;
                btnSystemset_43_Unique_Close2.Enabled = state;
                btnSystemset_45_Unique_Lifted2.Enabled = state;

                btnSystemset_44_Unique_Open3.Enabled = state;
                btnSystemset_43_Unique_Close3.Enabled = state;
                btnSystemset_45_Unique_Lifted3.Enabled = state;

            }));
        }

        private void setUI_BCU(bool state)
        {
            this.Invoke(new Action(() =>
            {
                btnSystemset_44_BCU_Open1.Enabled = state;
                btnSystemset_43_BCU_Close1.Enabled = state;
                btnSystemset_45_BCU_Lifted1.Enabled = state;

                btnSystemset_44_BCU_Open2.Enabled = state;
                btnSystemset_43_BCU_Close2.Enabled = state;
                btnSystemset_45_BCU_Lifted2.Enabled = state;

                btnSystemset_44_BCU_Open3.Enabled = state;
                btnSystemset_43_BCU_Close3.Enabled = state;
                btnSystemset_45_BCU_Lifted3.Enabled = state;

                btnSystemset_44_BCU_Open4.Enabled = state;
                btnSystemset_43_BCU_Close4.Enabled = state;
                btnSystemset_45_BCU_Lifted4.Enabled = state;

                btnSystemset_44_BCU_Open5.Enabled = state;
                btnSystemset_43_BCU_Close5.Enabled = state;
                btnSystemset_45_BCU_Lifted5.Enabled = state;

                btnSystemset_44_BCU_Open6.Enabled = state;
                btnSystemset_43_BCU_Close6.Enabled = state;
                btnSystemset_45_BCU_Lifted6.Enabled = state;

                btnSystemset_44_BCU_Open7.Enabled = state;
                btnSystemset_43_BCU_Close7.Enabled = state;
                btnSystemset_45_BCU_Lifted7.Enabled = state;

                btnSystemset_44_BCU_Open8.Enabled = state;
                btnSystemset_43_BCU_Close8.Enabled = state;
                btnSystemset_45_BCU_Lifted8.Enabled = state;

                btnSystemset_44_BCU_Open9.Enabled = state;
                btnSystemset_43_BCU_Close9.Enabled = state;
                btnSystemset_45_BCU_Lifted9.Enabled = state;

                btnSystemset_44_BCU_Open10.Enabled = state;
                btnSystemset_43_BCU_Close10.Enabled = state;
                btnSystemset_45_BCU_Lifted10.Enabled = state;

                btnSystemset_44_BCU_Open11.Enabled = state;
                btnSystemset_43_BCU_Close11.Enabled = state;
                btnSystemset_45_BCU_Lifted11.Enabled = state;

                btnSystemset_44_BCU_Open12.Enabled = state;
                btnSystemset_43_BCU_Close12.Enabled = state;
                btnSystemset_45_BCU_Lifted12.Enabled = state;

                btnSystemset_44_BCU_Open13.Enabled = state;
                btnSystemset_43_BCU_Close13.Enabled = state;
                btnSystemset_45_BCU_Lifted13.Enabled = state;

                btnSystemset_44_BCU_Open14.Enabled = state;
                btnSystemset_43_BCU_Close15.Enabled = state;
                btnSystemset_45_BCU_Lifted15.Enabled = state;

                btnSystemset_44_BCU_Open16.Enabled = state;
                btnSystemset_43_BCU_Close16.Enabled = state;
                btnSystemset_45_BCU_Lifted16.Enabled = state;

                btnSystemset_44_BCU_Open17.Enabled = state;
                btnSystemset_43_BCU_Close17.Enabled = state;
                btnSystemset_45_BCU_Lifted17.Enabled = state;

                btnSystemset_44_BCU_Open18.Enabled = state;
                btnSystemset_43_BCU_Close18.Enabled = state;
                btnSystemset_45_BCU_Lifted18.Enabled = state;

                btnSystemset_44_BCU_Open19.Enabled = state;
                btnSystemset_43_BCU_Close19.Enabled = state;
                btnSystemset_45_BCU_Lifted19.Enabled = state;

                btnSystemset_44_BCU_Open20.Enabled = state;
                btnSystemset_43_BCU_Close20.Enabled = state;
                btnSystemset_45_BCU_Lifted20.Enabled = state;

                btnSystemset_44_BCU_Open21.Enabled = state;
                btnSystemset_43_BCU_Close21.Enabled = state;
                btnSystemset_45_BCU_Lifted21.Enabled = state;

            }));
        }

        private void setUI_BCU1(bool state)
        {
            this.Invoke(new Action(() =>
            {
                btnSystemset_44_BCU1_Open1.Enabled = state;
                btnSystemset_43_BCU1_Close1.Enabled = state;
                btnSystemset_45_BCU1_Lifted1.Enabled = state;

                btnSystemset_44_BCU1_Open2.Enabled = state;
                btnSystemset_43_BCU1_Close2.Enabled = state;
                btnSystemset_45_BCU1_Lifted2.Enabled = state;

                btnSystemset_44_BCU1_Open3.Enabled = state;
                btnSystemset_43_BCU1_Close3.Enabled = state;
                btnSystemset_45_BCU1_Lifted3.Enabled = state;

                btnSystemset_44_BCU1_Open4.Enabled = state;
                btnSystemset_43_BCU1_Close4.Enabled = state;
                btnSystemset_45_BCU1_Lifted4.Enabled = state;

            }));
        }
        #endregion

        #region 时间校准
        private void btn_16_Click(object sender, EventArgs e)
        {
            canid[2] = 0x26;
            string[] date = dateTimePicker1.Text.Split(new char[] { ' ', '-', ':' });
            bytes = new byte[] {
                                    Convert.ToByte(Convert.ToInt32(date[0]) - 2000),
                                    Convert.ToByte(Convert.ToInt32(date[1])),
                                    Convert.ToByte(Convert.ToInt32(date[2])),
                                    Convert.ToByte(Convert.ToInt32(date[3])),
                                    Convert.ToByte(Convert.ToInt32(date[4])),
                                    Convert.ToByte(Convert.ToInt32(date[5])),
                                    0x00, 0x00 };
            //发送指令
            if (ecanHelper.Send(bytes, canid))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        #endregion

        #region 条形码AND单板序列号
        private void btn_17_Click(object sender, EventArgs e)
        {
            SetSN(txt_67.Text.Trim());
        }

        private void btnSetBoardSN_Click(object sender, EventArgs e)
        {
            SetSN(txtBoardSN.Text.Trim(), true);
        }

        /// <summary>
        /// 设置SN序列号
        /// </summary>
        /// <param name="bmsId"></param>
        /// <param name="packSn"></param>
        /// <param name="identity">false=SN号设置，true=单板SN设置</param>
        public void SetSN(string packSn, bool identity = false)
        {
            bool flag_WriteSuccess = false;
            if (packSn.Length != 20)
            {
                MessageBox.Show("SN序列号长度不等于20！输入的序列号长度为" + packSn.Length.ToString() + "位");
                return;
            }

            byte[] can_id = new byte[] { 0xE0, FrmMain.BMS_ID, 0x27, 0x10 };
            if (identity)
            {
                can_id = new byte[] { 0xE0, FrmMain.BMS_ID, 0x28, 0x10 };
            }

            string[] strs = new string[packSn.Length];
            for (int j = 0; j < packSn.Length; j++)
            {
                strs[j] = ((int)packSn[j]).ToString("X2");
            }

            for (int j = 0; j < strs.Length; j++)
            {
                int i = 0;
                byte[] data = new byte[8];

                data[i++] = Convert.ToByte((j / 7).ToString(), 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = j == strs.Length ? (byte)0x00 : Convert.ToByte(strs[j], 16);
                if (ecanHelper.Send(data, can_id))
                {
                    flag_WriteSuccess = true;
                }
            }
            if (flag_WriteSuccess)
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        #endregion

        #region BCU序列号

        //写入 BCU PACK_SN
        private void btnbcuCode_Click(object sender, EventArgs e)
        {
            SetBCU_SN(txtbcuCode.Text.Trim());
        }

        //写入 BCU BOARD_SN
        private void btnBCUBoardSN_Click(object sender, EventArgs e)
        {
            SetBCU_SN(txtBCUBoardSN.Text.Trim(), true);
        }

        /// <summary>
        /// 设置BCU SN序列号
        /// </summary>
        /// <param name="bmsId"></param>
        /// <param name="packSn"></param>
        /// <param name="identity">false=SN号设置，true=单板SN设置</param>
        public void SetBCU_SN(string packSn, bool identity = false)
        {
            bool flag_WriteSuccess = false;
            if (packSn.Length != 20)
            {
                MessageBox.Show("SN序列号长度不等于20！输入的序列号长度为" + packSn.Length.ToString() + "位");
                return;
            }

            byte[] can_id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xF3, 0x10 };
            if (identity)
            {
                can_id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xF4, 0x10 };
            }

            string[] strs = new string[packSn.Length];
            for (int j = 0; j < packSn.Length; j++)
            {
                strs[j] = ((int)packSn[j]).ToString("X2");
            }

            for (int j = 0; j < strs.Length; j++)
            {
                int i = 0;
                byte[] data = new byte[8];

                data[i++] = Convert.ToByte((j / 7).ToString(), 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = Convert.ToByte(strs[j++], 16);
                data[i++] = j == strs.Length ? (byte)0x00 : Convert.ToByte(strs[j], 16);
                if (ecanHelper.Send(data, can_id))
                {
                    flag_WriteSuccess = true;
                }
            }
            if (flag_WriteSuccess)
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }
        #endregion

        #region BCU时间校准
        private void btnBCU_Time_Click(object sender, EventArgs e)
        {
            byte[] canid_BCU = new byte[] { 0xE0, FrmMain.BCU_ID, 0xF2, 0x10 };         
            string[] date = dateTimePicker2.Text.Split(new char[] { ' ', '-', ':' });
            bytes = new byte[] {
                                    Convert.ToByte(Convert.ToInt32(date[0]) - 2000),
                                    Convert.ToByte(Convert.ToInt32(date[1])),
                                    Convert.ToByte(Convert.ToInt32(date[2])),
                                    Convert.ToByte(Convert.ToInt32(date[3])),
                                    Convert.ToByte(Convert.ToInt32(date[4])),
                                    Convert.ToByte(Convert.ToInt32(date[5])),
                                    0x00, 0x00 };
            //发送指令
            if (ecanHelper.Send(bytes, canid_BCU))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        #endregion

        #region 额定容量和单体个数
        private void btn_13_Click(object sender, EventArgs e)
        {
            try
            {
                int voltage = Convert.ToInt32(txt_61.Text);
                if (voltage > 20 || voltage < 1)
                {
                    MessageBox.Show("电压输入值为非法数据！", "错误提示");
                    return;
                }

                int temp = Convert.ToInt32(txt_62.Text);
                if (temp > 8 || temp < 1)
                {
                    MessageBox.Show("温度输入值为非法数据！", "错误提示");
                    return;
                }

                canid[2] = 0x23;
                bytes = Uint16ToBytes(txt_60, txt_61, txt_62, txt_0, 0.1, 1, 1, 1);

                if (ecanHelper.Send(bytes, canid))
                {
                    MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
                }
                else
                {
                    MessageBox.Show(FrmMain.GetString("keyWriteFail"));
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region 累计充电/放电容量
        private void btn_14_Click(object sender, EventArgs e)
        {
            try
            {
                canid[2] = 0x24;

                byte[] buf = new byte[4];
                ConvertIntToByteArray(Convert.ToInt32(txt_63.Text), ref buf);

                byte[] buf2 = new byte[4];
                ConvertIntToByteArray(Convert.ToInt32(txt_64.Text), ref buf2);

                bytes = new byte[] { buf[0], buf[1], buf[2], buf[3], buf2[0], buf2[1], buf2[2], buf2[3] };

                if (ecanHelper.Send(bytes, canid))
                {
                    MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
                }
                else
                {
                    MessageBox.Show(FrmMain.GetString("keyWriteFail"));
                }

            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region SOC
        private void btn_15_Click(object sender, EventArgs e)
        {
            canid[2] = 0x25;
            bytes = Uint16ToBytes(txt_65, txt_100, txt_101, txt_102, 0.1, 0.1, 0.1, 0.1);

            if (ecanHelper.Send(bytes, canid))
            {
                //读取数据
                List<uint> DataLists = new List<uint>() { 0xAA11, 0xAA22, 0xAA33, 0xAA44, 0xAA55, 0xAA66, 0xAA77, 0xAA88 };

                for (int i = 0; i < DataLists.Count; i++)
                {
                    DataSelected(DataLists[i]);

                    Thread.Sleep(100);
                }

                byte[] bytes = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 });
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }


        #endregion

        #region PCU老化模式（代码待移除）
        private void btnReadpcu_Click(object sender, EventArgs e)
        {
            PCUAgingMode(0x00);
        }

        /// <summary>
        /// PCU老化模式指令
        /// </summary>
        /// <param name="type">开启：0xAA，关闭：0x55</param>
        private void PCUAgingMode(int type)
        {
            byte[] can_id = new byte[4] { 0xE0, FrmMain.BMS_ID, 0x20, 0x10 };

            byte[] data = new byte[8];
            data[3] = (byte)(type & 0xff);

            byte crc8 = (byte)(0x10 + 0x20 + FrmMain.BMS_ID + 0xE0 + 0x00 + 0x00 + 0x00 + data[3] + 0x00 + 0x00 + 0x00);
            data[7] = crc8;
            if (ecanHelper.Send(data, can_id))
            {
                MessageBox.Show((type == 0x00) ? FrmMain.GetString("keyReadSuccess") : FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show((type == 0x00) ? FrmMain.GetString("keyReadFail") : FrmMain.GetString("keyWriteFail"));
            }
        }

        private void TestMode(int type)
        {
            byte[] can_id = new byte[4] { 0xE0, FrmMain.BMS_ID, 0x20, 0x10 };

            byte[] data = new byte[8];
            data[1] = (byte)(type & 0xff);

            byte crc8 = (byte)(0x10 + 0x20 + FrmMain.BMS_ID + 0xE0 + 0x00 + data[1] + 0x00 + 0x00 + 0x00 + 0x00 + FrmMain.BMS_ID);
            data[7] = crc8;
            if (ecanHelper.Send(data, can_id))
            {
                MessageBox.Show((type == 0x00) ? FrmMain.GetString("keyReadSuccess") : FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show((type == 0x00) ? FrmMain.GetString("keyReadFail") : FrmMain.GetString("keyWriteFail"));
            }
        }
        #endregion

        #region BMS校准系数
        private void btn_CalibrationBMS_Click(object sender, EventArgs e)
        {
            int val;

            if (sender is Button btn)
            {
                switch (btn.Name)
                {
                    case "btnSetCalibration_01":
                        val = Convert.ToInt32(Convert.ToDouble(txtCalibration01.Text.Trim()) / 0.1);

                        CalibrationBMS(0x01, val);
                        break;
                    case "btnSetCalibration_02":
                        val = Convert.ToInt32(Convert.ToDouble(txtCalibration02.Text.Trim()) / 0.1);

                        CalibrationBMS(0x02, val);
                        break;
                    case "btnSetCalibration_03":
                        val = Convert.ToInt32(Convert.ToDouble(txtCalibration03.Text.Trim()) / 0.01);

                        CalibrationBMS(0x03, val);
                        break;
                    case "btnSetCalibration_04":
                        val = Convert.ToInt32(Convert.ToDouble(txtCalibration04.Text.Trim()) / 0.01);

                        CalibrationBMS(0x04, val);
                        break;
                    case "btnSetCalibration_05":
                        val = Convert.ToInt32(Convert.ToDouble(txtCalibration05.Text.Trim()) / 0.01);

                        CalibrationBMS(0x05, val);
                        break;
                    case "btnSetCalibration_06":
                        val = Convert.ToInt32(Convert.ToDouble(txtCalibration06.Text.Trim()) / 0.01);

                        CalibrationBMS(0x06, val);
                        break;
                    case "btnSetCalibration_07":
                        val = Convert.ToInt32(Convert.ToDouble(txtCalibration07.Text.Trim()) / 0.1);

                        CalibrationBMS(0x07, val);
                        break;
                }
            }
        }

        private void CalibrationBMS(byte firstData, int CalibrationVal)
        {
            byte[] can_id = new byte[] { 0xE0, FrmMain.BMS_ID, 0x2A, 0x10 };

            byte[] data = new byte[8] { firstData, Convert.ToByte(CalibrationVal & 0xff), Convert.ToByte(CalibrationVal >> 8), 0x00, 0x00, 0x00, 0x00, 0x00 };

            if (ecanHelper.Send(data, can_id))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        #endregion

        #region BCU校准系数
        private void btn_CalibrationBCU_Click(object sender, EventArgs e)
        {
            int val;

            if (sender is Button btn)
            {
                switch (btn.Name)
                {
                    case "btnSetCalibration_BCU_01":
                        val = Convert.ToInt32(Convert.ToDouble(txtBCU_Calibration01.Text.Trim()) / 0.1);

                        CalibrationBCU(0x01, val);
                        break;
                    case "btnSetCalibration_BCU_02":
                        val = Convert.ToInt32(Convert.ToDouble(txtBCU_Calibration02.Text.Trim()) / 0.1);

                        CalibrationBCU(0x02, val);
                        break;
                    case "btnSetCalibration_BCU_03":
                        val = Convert.ToInt32(Convert.ToDouble(txtBCU_Calibration03.Text.Trim()) / 0.01);

                        CalibrationBCU(0x03, val);
                        break;
                    case "btnSetCalibration_BCU_04":
                        val = Convert.ToInt32(Convert.ToDouble(txtBCU_Calibration04.Text.Trim()) / 0.01);

                        CalibrationBCU(0x04, val);
                        break;
                    case "btnSetCalibration_BCU_05":
                        val = Convert.ToInt32(Convert.ToDouble(txtBCU_Calibration05.Text.Trim()) / 0.01);

                        CalibrationBCU(0x05, val);
                        break;
                    case "btnSetCalibration_BCU_06":
                        val = Convert.ToInt32(Convert.ToDouble(txtBCU_Calibration06.Text.Trim()) / 0.01);

                        CalibrationBCU(0x06, val);
                        break;
                    case "btnSetCalibration_BCU_07":
                        val = Convert.ToInt32(Convert.ToDouble(txtBCU_Calibration07.Text.Trim()) / 0.1);

                        CalibrationBCU(0x07, val);
                        break;
                    case "btnSetCalibration_BCU_08":
                        val = Convert.ToInt32(Convert.ToDouble(txtBCU_Calibration08.Text.Trim()) / 0.1);

                        CalibrationBCU(0x08, val);
                        break;
                }
            }
        }

        private void CalibrationBCU(byte firstData, int CalibrationVal)
        {
            byte[] can_id = new byte[] { 0xE0, FrmMain.BCU_ID, 0xF5, 0x10 };

            byte[] data = new byte[8] { firstData, Convert.ToByte(CalibrationVal & 0xff), Convert.ToByte(CalibrationVal >> 8), 0x00, 0x00, 0x00, 0x00, 0x00 };

            if (ecanHelper.Send(data, can_id))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        #endregion

        #region 翻译函数
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

        #region PCU序列号、BDU序列号
        private void btnSetSN_Click(object sender, EventArgs e)
        {
            string sn = txtPCUSN.Text.Trim();
            bool flag_WriteSuccess = false;
            if (sn.Length == 20)
            {
                byte[] can_id = new byte[4];
                sn = sn + "0";

                for (int i = 0; i < sn.Length; i += 7)
                {
                    if (i == 0)
                        can_id = new byte[4] { 0xE0, FrmMain.PCU_ID, 0x70, 0x0B };
                    else if (i == 7)
                        can_id = new byte[4] { 0xE0, FrmMain.PCU_ID, 0x71, 0x0B };
                    else if (i == 14)
                        can_id = new byte[4] { 0xE0, FrmMain.PCU_ID, 0x72, 0x0B };

                    byte[] bufferSN = Encoding.ASCII.GetBytes(sn.Substring(i, 7));

                    byte[] data = new byte[bufferSN.Length + 1];

                    Array.Copy(bufferSN, 0, data, 1, bufferSN.Length);
                    data[0] = 0x01;
                    if (ecanHelper.Send(data, can_id))
                    {
                        // 设置写入成功标志
                        flag_WriteSuccess = true;
                    }
                    Thread.Sleep(100);
                }
                if (flag_WriteSuccess)
                {
                    MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
                }
                else
                {
                    MessageBox.Show(FrmMain.GetString("keyWriteFail"));
                }
            }
            else
            {
                MessageBox.Show("序列号异常,长度不等于20位,输入的序列号为" + sn.Length.ToString() + "位。");
            }
        }

        private void btnSetBDUSN_Click(object sender, EventArgs e)
        {
            string sn = txtBDUSN.Text.Trim();
            bool flag_WriteSuccess = false;
            if (sn.Length == 20)
            {
                byte[] can_id = new byte[4];
                sn = sn + "0";

                for (int i = 0; i < sn.Length; i += 7)
                {
                    if (i == 0)
                        can_id = new byte[4] { 0xE0, FrmMain.BDU_ID, 0x00, 0x14 };
                    else if (i == 7)
                        can_id = new byte[4] { 0xE0, FrmMain.BDU_ID, 0x00, 0x14 };
                    else if (i == 14)
                        can_id = new byte[4] { 0xE0, FrmMain.BDU_ID, 0x00, 0x14 };

                    byte[] bufferSN = Encoding.ASCII.GetBytes(sn.Substring(i, 7));

                    byte[] data = new byte[bufferSN.Length + 1];

                    Array.Copy(bufferSN, 0, data, 1, bufferSN.Length);
                    data[0] = Convert.ToByte(i / 7);
                    if (ecanHelper.Send(data, can_id))
                    {
                        // 设置写入成功标志
                        flag_WriteSuccess = true;
                    }
                    Thread.Sleep(100);
                }
                if (flag_WriteSuccess)
                {
                    MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
                }
                else
                {
                    MessageBox.Show(FrmMain.GetString("keyWriteFail"));
                }
            }
            else
            {
                MessageBox.Show("序列号异常,长度不等于20位,输入的序列号为" + sn.Length.ToString() + "位。");
            }
        }
        #endregion

        #region PCU控制指令
        private void btnSetComm_Click(object sender, EventArgs e)
        {
            if (cbbSetComm.SelectedIndex == 0)
            {
                StopDischarge(0xAAAA);
            }
            else if (cbbSetComm.SelectedIndex == 1)
            {
                StopDischarge(0x5555);
            }
        }

        /// <summary>
        /// PCU停机指令
        /// </summary>
        /// <param name="type">停机：0xAAAA，正常工作：0x5555</param>
        private void StopDischarge(int type)
        {
            byte[] can_id = new byte[4] { 0x41, FrmMain.PCU_ID, 0x6A, 0x0B };

            byte[] data = new byte[8];
            data[0] = (byte)(type & 0xff);
            data[1] = (byte)(type >> 8);

            if (ecanHelper.Send(data, can_id))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        #endregion

        #region PCU测试标识
        private void btnSetTestFlag_Click(object sender, EventArgs e)
        {
            int flag = 0;
            if (ckSystemset_30.Checked) flag += 1;
            if (ckSystemset_31.Checked) flag += 2;
            if (ckSystemset_32.Checked) flag += 4;
            if (ckSystemset_33.Checked) flag += 8;

            byte[] can_id = new byte[4] { 0xE0, FrmMain.PCU_ID, 0x76, 0x0B };
            byte[] data = new byte[8];
            data[0] = (byte)(flag & 0xff);
            data[1] = (byte)(flag >> 8);

            if (ecanHelper.Send(data, can_id))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        #endregion

        #region PCU参数校准
        private void btn_CalibrationPCU_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                switch (btn.Name)
                {
                    case "btnSetCalibration_11":
                        CalibrationPCU(0x1111, decimal.Parse(txtLV_Calibration_Coefficient.Text));
                        break;
                    case "btnSetCalibration_12":
                        CalibrationPCU(0x2222, decimal.Parse(txtLV_Charge_Current_Calibration_Coefficient.Text));
                        break;
                    case "btnSetCalibration_13":
                        CalibrationPCU(0x3333, decimal.Parse(txtLV_Discharge_Current_Calibration_Coefficient.Text));
                        break;
                    case "btnSetCalibration_14":
                        CalibrationPCU(0x5555, decimal.Parse(txtVhvbus_Calibration_Coefficient.Text));
                        break;
                    case "btnSetCalibration_15":
                        CalibrationPCU(0x6666, decimal.Parse(txtVpbus_Calibration_Coefficient.Text));
                        break;
                    case "btnSetCalibration_16":
                        CalibrationPCU(0x7777, decimal.Parse(txtHV_Charge_Current_Calibration_Coefficient.Text));
                        break;
                    case "btnSetCalibration_17":
                        CalibrationPCU(0x8888, decimal.Parse(txtHV_Discharge_Current_Calibration_Coefficient.Text));
                        break;
                }
            }
        }

        /// <summary>
        /// 校准变量选择（写入校准数据）
        /// </summary>
        /// <param name="type">0x1111：低压端电压；0x2222：低压端充电电流；0x3333：低压端放电电流；0x4444：清除发电量
        /// 0x5555：高压侧电压Vhvbus；0x6666：高压侧电压Vpbus；0x7777：高压侧充电电流；0x8888：高压侧放电电流；</param>
        private void CalibrationPCU(uint type, decimal val)
        {
            byte[] can_id = new byte[4] { 0xE0, FrmMain.PCU_ID, 0x73, 0x0B };
            int value = Convert.ToInt32(val * 1000);

            byte[] data = new byte[8];
            int i = 0;
            data[i++] = (byte)(type & 0xff);
            data[i++] = (byte)(type >> 8);
            data[i++] = (byte)(value & 0xff);
            data[i++] = (byte)(value >> 8);

            if (ecanHelper.Send(data, can_id))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        #endregion

        #region 数据类型转换
        private int GetBit(byte b, short index)
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

            return (b & _byte) == _byte ? 1 : 0;
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

        private byte[] Uint16ToBytes(TextBox t1, TextBox t2, TextBox t3, TextBox t4,
           double scaling1, double scaling2, double scaling3, double scaling4)
        {
            byte[] b1 = Uint16ToBytes(Convert.ToUInt32(float.Parse(t1.Text) / scaling1));
            byte[] b2 = Uint16ToBytes(Convert.ToUInt32(float.Parse(t2.Text) / scaling2));
            byte[] b3 = Uint16ToBytes(Convert.ToUInt32(float.Parse(t3.Text) / scaling3));
            byte[] b4 = Uint16ToBytes(Convert.ToUInt32(float.Parse(t4.Text) / scaling4));

            return new byte[] { b1[0], b1[1], b2[0], b2[1], b3[0], b3[1], b4[0], b4[1] };
        }

        private byte[] Uint16ToBytes(uint ivalue)
        {
            byte[] data = new byte[2];
            data[1] = (byte)(ivalue >> 8);
            data[0] = (byte)(ivalue & 0xff);

            return data;
        }

        private bool ConvertIntToByteArray(Int32 m, ref byte[] arry)
        {
            if (arry == null) return false;
            if (arry.Length < 4) return false;

            arry[0] = (byte)(m & 0xFF);
            arry[1] = (byte)((m & 0xFF00) >> 8);
            arry[2] = (byte)((m & 0xFF0000) >> 16);
            arry[3] = (byte)((m >> 24) & 0xFF);

            return true;
        }

        private int[] BytesToUint16(byte[] data)
        {
            int[] numbers = new int[4];
            for (int i = 0; i < data.Length; i += 2)
            {
                numbers[i / 2] = Convert.ToInt32(data[i + 1].ToString("X2") + data[i].ToString("X2"), 16);
            }
            return numbers;
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

        private byte[] Uint8ToBits(List<int> vals)
        {
            if (vals.Count > 8 || vals.Count <= 0)
                return null;

            byte[] bytes = new byte[8];
            for (int i = 0; i < vals.Count; i++)
            {
                bytes[i] = (byte)Convert.ToByte(vals[i].ToString("X2"), 16);
            }

            return bytes;
        }
        #endregion

        #region CRC校验
        /*******************************************************************************
        * Function Name  : Crc8_8210_nBytesCalculate
        * Description    : CRC校验,多项式为0x2F
        *******************************************************************************/
        public static uint Crc8_8210_nBytesCalculate(byte[] pBuff, uint bLen, uint bCrsMask)
        {
            uint i;
            for (int k = 0; k < bLen; k++)
            {
                for (i = 0x80; i > 0; i >>= 1)
                {
                    if (0 != (bCrsMask & 0x80))
                    {
                        bCrsMask <<= 1;
                        bCrsMask ^= 0x2F;
                    }
                    else
                    {
                        bCrsMask <<= 1;
                    }
                    if (0 != (pBuff[k] & i))
                    {
                        bCrsMask ^= 0x2F;
                    }
                }
            }
            return bCrsMask;
        }

        #endregion

        private void Send(byte[] id, List<int> lists)
        {
            byte[] data = Uint8ToBits(lists);
            if (ecanHelper.Send(data, id))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        private void btnSetBatteryinfo_Click(object sender, EventArgs e)
        {
            byte[] can_id = new byte[4] { 0xE0, FrmMain.BMS_ID, 0x29, 0x10 };

            List<int> lists = new List<int>();
            int batteryInfo1 = cbb_103.SelectedIndex + 1;
            lists.Add(batteryInfo1);
            lists.Add(Convert.ToInt32(txt_104.Text));
            lists.Add(cbb_105.SelectedIndex);
            lists.Add(cbb_106.SelectedIndex);

            Send(can_id, lists);
        }

        #region CBS5000 特有属性

        private void btnSet020_1_Click(object sender, EventArgs e)
        {
            byte[] can_id = new byte[4] { 0xE0, FrmMain.BMS_ID, 0x20, 0x10 };

            byte[] data = new byte[8];
            foreach (Control item in this.gbControl020.Controls)
            {
                if (item is ComboBox)
                {
                    ComboBox cbb = item as ComboBox;
                    int index = 0;
                    int.TryParse(cbb.Name.Replace("cbbRequest", ""), out index);
                    int value = 0;
                    switch (cbb.SelectedIndex)
                    {
                        case 0: value = 0x00; break;
                        case 1: value = 0xAA; break;
                        case 2: value = 0x55; break;
                        default:
                            break;
                    }
                    data[index] = (byte)(value & 0xff);
                }
            }
            data[6] = Convert.ToByte(txtSlaveAddr.Text.Trim());
            byte crc8 = (byte)(0x10 + 0x20 + data[6] + 0xE0);
            for (int i = 0; i < data.Length - 1; i++)
            {
                crc8 += data[i];
            }

            data[7] = crc8;
            if (ecanHelper.Send(data, can_id))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        private void btnSet0F0_Click(object sender, EventArgs e)
        {
            byte[] can_id = new byte[4] { 0xE0, 0x9F, 0xF0, 0x10 };

            byte[] data = new byte[8];
            foreach (Control item in this.gbControl0F0.Controls)
            {
                if (item is ComboBox)
                {
                    ComboBox cbb = item as ComboBox;
                    int index = 0;
                    int.TryParse(cbb.Name.Replace("cbbRequestF0_", ""), out index);
                    int value = 0;
                    switch (cbb.SelectedIndex)
                    {
                        case 0: value = 0x00; break;
                        case 1: value = 0xAA; break;
                        case 2: value = 0x55; break;
                        default:
                            break;
                    }
                    data[index] = (byte)(value & 0xff);
                }
            }
            data[6] = Convert.ToByte(txtSlaveAddr_BCU.Text.Trim());
            byte crc8 = (byte)((0xE0 + 0x9F + 0xF0 + 0x10) & 0xFF);
            for (int i = 0; i < data.Length - 1; i++)
            {
                crc8 += data[i];
            }

            data[7] = (byte)(crc8 & 0xFF);
            if (ecanHelper.Send(data, can_id))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }

        private void btnSetStateParam_Click(object sender, EventArgs e)
        {
            //List<byte> bytes = new List<byte>();

            byte[] can_id = new byte[4] { 0xE0, FrmMain.BMS_ID, 0x60, 0x10 };
            //bytes.AddRange(can_id);

            byte[] data = new byte[8];

            int value = cbbState.SelectedIndex & 0x03;

            if (ckSystemset_67.Checked)
            {
                value = value | 0x4;
            }
            if (ckSystemset_68.Checked)
            {
                value = value | 0x8;
            }
            if (ckSystemset_69.Checked)
            {
                value = value | 0x10;
            }

            int current = Convert.ToInt32(txtPackCurrent.Text.Trim());
            data[0] = (byte)(current & 0xff);
            data[1] = (byte)(current >> 8);
            data[2] = Convert.ToByte(value);
            data[3] = Convert.ToByte(Convert.ToInt32(txtSyncFallSoc.Text, 16));
            //bytes.AddRange(data);

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BMS_ID, 0x60, 0x10, data[0], data[1], data[2], data[3], data[4], data[5], data[6] };

            data[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);
            ecanHelper.Send(data, can_id);
        }

        private void btnSetControlInfo_Click(object sender, EventArgs e)
        {
            //List<byte> bytes = new List<byte>();

            byte[] can_id = new byte[4] { 0xE0, FrmMain.BMS_ID, 0x61, 0x10 };
            //bytes.AddRange(canid);

            byte[] data = new byte[8];
            //主动均衡充电使能
            data[0] = (byte)Convert.ToByte(cbbActiveBalanceCtrl.SelectedIndex);
            //主动均衡电流
            int current = Convert.ToInt32(txtPackActiveBalanceCur.Text.Trim());
            data[1] = (byte)(current & 0xff);
            data[2] = (byte)(current >> 8);
            //主动均衡容量
            int capacity = Convert.ToInt32(txtPackActiveBalanceCap.Text.Trim());
            data[3] = (byte)(capacity & 0xff);
            data[4] = (byte)(capacity >> 8);
            //bytes.AddRange(data);

            byte[] crcData = new byte[11] { 0xE0, FrmMain.BMS_ID, 0x61, 0x10, data[0], data[1], data[2], data[3], data[4], data[5], data[6] };
            data[7] = (byte)(Crc8_8210_nBytesCalculate(crcData, 11, 0) & 0xff);

            ecanHelper.Send(data, can_id);
        }
        #endregion

        private void btnSetComm3_Click(object sender, EventArgs e)
        {
            TestMode(0xAA);
        }

        private void btnFlashData_Click(object sender, EventArgs e)
        {
            byte[] can_id = new byte[4] { 0xE0, FrmMain.BCU_ID, 0xEF, 0x10 };
            byte[] data = new byte[8];

            data[0] = 0x01;
            byte[] data1 = Encoding.Default.GetBytes(txtFlashData.Text);
            int lengthToCopy = Math.Min(data1.Length, data.Length - 1);
            Array.Copy(data1, 0, data, 1, lengthToCopy);

            if (ecanHelper.Send(data, can_id))
            {
                MessageBox.Show(FrmMain.GetString("keyWriteSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyWriteFail"));
            }
        }


        private void btnRead_Click(object sender, EventArgs e)
        {
            List<uint> DataLists = new List<uint>() { 0xAA11, 0xAA22, 0xAA33, 0xAA44, 0xAA55, 0xAA66, 0xAA77, 0xAA88 };//

            for (int i = 0; i < DataLists.Count; i++)
            {
                DataSelected(DataLists[i]);

                Thread.Sleep(100);
            }

            byte[] bytes1 = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            if (ecanHelper.Send(bytes1, new byte[] { 0xE0, FrmMain.BCU_ID, 0xF9, 0x10 }))
            {
                MessageBox.Show(FrmMain.GetString("keyReadSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyReadFail"));
            }
        }
    }
}
