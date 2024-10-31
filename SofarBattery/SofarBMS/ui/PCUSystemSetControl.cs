using SofarBMS.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SofarBMS.ui
{
    public partial class PCUSystemSetControl : UserControl
    {
        public static CancellationTokenSource cts;

        public PCUSystemSetControl()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
        }

        string[] pcuCode = new string[3];

        bool flag = true;

        private void PCUSystemSetControl_Load(object sender, EventArgs e)
        {
            foreach (Control item in this.Controls)
            {
                GetControls(item);
            }

            Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    if (EcanHelper.IsConnection)
                    {
                        if (flag)
                        {
                            Thread.Sleep(1000);
                            List<uint> DataLists = new List<uint>() { 0xAA11, 0xAA22, 0xAA33, 0xAA44, 0xAA55, 0xAA66, 0xAA77, 0xAA88 };//

                            for (int i = 0; i < DataLists.Count; i++)
                            {
                                DataSelected(DataLists[i]);

                                Thread.Sleep(100);
                            }

                            flag = false;
                        }

                        lock (EcanHelper._locker)
                        {
                            while (EcanHelper._task.Count > 0)
                            {
                                CAN_OBJ ch = (CAN_OBJ)EcanHelper._task.Dequeue();

                                this.Invoke(new Action(() =>
                                {
                                    //解析数据
                                    analysisData(ch.ID, ch.Data);
                                }));
                            }
                        }
                    }
                }
            });
        }

        public void analysisData(uint canID, byte[] data)
        {
            byte[] canid = BitConverter.GetBytes(canID);
            if (!(((canID & 0xff) == FrmMain.BMS_ID) || ((canID & 0xff) == FrmMain.PCU_ID))) return;

            string[] strs;
            string[] controls;


            int[] numbers = BytesToUint16(data);
            int[] numbers_bit = BytesToBit(data);

            switch (BitConverter.ToUInt32(canid, 0) | 0xff)
            {
                case 0x0B70E0FF:
                    pcuCode[0] = Encoding.Default.GetString(data).Substring(1);
                    break;
                case 0x0B71E0FF:
                    pcuCode[1] = Encoding.Default.GetString(data).Substring(1);
                    break;
                case 0x0B72E0FF:
                    pcuCode[2] = Encoding.Default.GetString(data).Substring(1);

                    txtPCUSN.Text = String.Join("", pcuCode);
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
                        MessageBox.Show("写入成功");
                    break;
            }
        }

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

            if (len > 5 && len < str.Length)

            {
                return str.Substring(0, len - 3) + "...";
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

        #region 读取数据
        private void btnRead_Click(object sender, EventArgs e)
        {
            List<uint> DataLists = new List<uint>() { 0xAA11, 0xAA22, 0xAA33, 0xAA44, 0xAA55, 0xAA66, 0xAA77, 0xAA88 };//

            for (int i = 0; i < DataLists.Count; i++)
            {
                DataSelected(DataLists[i]);

                Thread.Sleep(100);
            }
        }

        private void DataSelected(uint type)
        {
            byte[] id = new byte[4] { 0xE0, FrmMain.PCU_ID, 0x77, 0x0B };

            byte[] data = new byte[8];
            data[0] = (byte)(type & 0xff);
            data[1] = (byte)(type >> 8);

            EcanHelper.Send(data, id);
        }
        #endregion

        #region PCU序列号
        private void btnSetSN_Click(object sender, EventArgs e)
        {
            string sn = txtPCUSN.Text.Trim();

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

                    EcanHelper.Send(data, can_id);

                    Thread.Sleep(100);
                }
            }
            else
            {
                MessageBox.Show("序列号异常,长度不等于20位");
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

            EcanHelper.Send(data, can_id);
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

            EcanHelper.Send(data, can_id);
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

            EcanHelper.Send(data, can_id);
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

    }
}
