using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Threading;
using SofarBMS.Helper;
using SofarBMS.Model;
using NPOI.POIFS.Crypt.Dsig;

namespace SofarBMS.UI
{
    public partial class CBS_BCU_ParamControl : UserControl
    {
        public static CancellationTokenSource cts = null;
        private EcanHelper ecanHelper = EcanHelper.Instance;
        XmlDocument mDocument;

        public bool flag = true;
        public static int index = 1;
        public static Dictionary<string, string> keys = new Dictionary<string, string>();
        public CBS_BCU_ParamControl()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
        }

        private void ParamControl_Load(object sender, EventArgs e)
        {
            foreach (Control item in this.Controls)
            {
                GetControls(item);
            }

            #region 指令集合
            if (keys.Count == 0)
            {            
                keys.Add("txt_1", "总体过充保护(V)");
                keys.Add("txt_2", "总体过充保护解除(V)");
                keys.Add("txt_3", "总体过充告警(V)");
                keys.Add("txt_4", "总体过充告警解除(V)");           
                keys.Add("txt_5", "总体过放保护(V)");
                keys.Add("txt_6", "总体过放保护解除(V)");
                keys.Add("txt_7", "总体过放告警(V)");
                keys.Add("txt_8", "总体过放告警解除(V)");
                keys.Add("txt_9", "充电过流保护(A)");
                keys.Add("txt_10", "充电过流保护解除(A)");
                keys.Add("txt_11", "充电过流告警(A)");
                keys.Add("txt_12", "充电过流告警解除(A)");
                keys.Add("txt_13", "放电过流保护1(A)");
                keys.Add("txt_14", "放电过流保护解除1(A)");
                keys.Add("txt_15", "放电过流告警(A)");
                keys.Add("txt_16", "放电过流告警解除(A)");
                keys.Add("txt_17", "放电过流保护2(A)");
                keys.Add("txt_18", "放电过流保护解除2(A)");
                keys.Add("txt_19", "充电过流保护2(A)");
                keys.Add("txt_20", "充电过流保护解除2(A)");              
                keys.Add("txt_21", "低电量告警(%)");
                keys.Add("txt_22", "低电量告警解除(%)");
                keys.Add("txt_23", "风扇开启温度(℃)");
                keys.Add("txt_24", "风扇关闭温度(℃)");
                keys.Add("txt_25", "满充电压(mV)");
                keys.Add("txt_26", "单簇PACK个数");              
            }
            #endregion

            Task.Run(async delegate
            {
                while (!cts.IsCancellationRequested)
                {
                    if (ecanHelper.IsConnection)
                    {
                        if (flag)
                        {
                            byte[] bytes = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                            ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BCU_ID, 0xF9, 0x10 });

                            flag = false;
                            await Task.Delay(1000);
                        }
                    }

                    lock (EcanHelper._locker)
                    {
                        while (EcanHelper._task.Count > 0 
                            && !cts.IsCancellationRequested)
                        {
                            CAN_OBJ ch = (CAN_OBJ)EcanHelper._task.Dequeue();

                            this.Invoke(new Action(() => { analysisData(ch.ID, ch.Data); }));
                        }
                    }
                }
            }, cts.Token);
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (!ecanHelper.IsConnection)
            {
                MessageBox.Show(FrmMain.GetString("keyOpenPrompt"));
                return;
            }

            flag = true;
            byte[] bytes = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            if (ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BCU_ID, 0xF9, 0x10 }))
            {
                MessageBox.Show(FrmMain.GetString("keyReadSuccess"));
            }
            else
            {
                MessageBox.Show(FrmMain.GetString("keyReadFail"));
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (!ecanHelper.IsConnection)
            {
                MessageBox.Show(FrmMain.GetString("keyOpenPrompt"));
                return;
            }

            Dictionary<string, bool> sendResult = new Dictionary<string, bool>();

            int errorCount = 0;
            byte[] canid = new byte[] { 0xE0, FrmMain.BCU_ID, 0xF9, 0x10 };
            byte[] bytes = new byte[8];
            int num = 7;
           
            if (false)
            {

            }
            else
            {
                for (int i = 1; i <= num; i++)
                {
                    if (this.Controls.Find("ckb_" + i, true).Length <= 0)
                        continue;

                    Control c = this.Controls.Find("ckb_" + i, true)[0];
                    if (((CheckBox)c).Checked)
                    {
                        switch (c.Name)
                        {
                            case "ckb_1":
                                canid[2] = 0xE0;
                                bytes = Uint16ToBytes(txt_1, txt_2, txt_3, txt_4, 0.1, 0.1, 0.1, 0.1);
                                break;
                            case "ckb_2":
                                canid[2] = 0xE1;
                                bytes = Uint16ToBytes(txt_5, txt_6, txt_7, txt_8, 0.1, 0.1, 0.1, 0.1);
                                break;
                            case "ckb_3":
                                canid[2] = 0xE2;
                                bytes = Uint16ToBytes(txt_9, txt_10, txt_11, txt_12, 0.01, 0.01, 0.01, 0.01);
                                break;
                            case "ckb_4":
                                canid[2] = 0xE3;
                                bytes = Uint16ToBytes(txt_13, txt_14, txt_15, txt_16, 0.01, 0.01, 0.01, 0.01);
                                break;
                            case "ckb_5":
                                canid[2] = 0xE4;
                                bytes = Uint16ToBytes(txt_17, txt_18, txt_19, txt_20, 0.01, 0.01, 0.01, 0.01);
                                break;
                            case "ckb_6":
                                canid[2] = 0xE5;
                                bytes = Uint16ToBytes(txt_21, txt_22, txt_21, txt_22, 1, 1, 1, 1);
                                break;
                            case "ckb_7":
                                canid[2] = 0xF1;
                                bytes = Uint16ToBytes(txt_23, txt_24, txt_25, txt_26, 0.1, 0.1, 1, 1);
                                break;

                        }

                        //发送指令
                        bool result = ecanHelper.Send(bytes, canid);

                        //记录信息
                        sendResult.Add(c.Name, result);
                    }
                }
            }

            if (sendResult.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in sendResult)
                {
                    if (!item.Value)
                    {
                        sb.AppendLine(item.Key + FrmMain.GetString("keyWriteFail"));
                    }
                }

                string msgInfo = string.IsNullOrEmpty(sb.ToString()) ? FrmMain.GetString("keyWriteSuccess") : sb.ToString();
                MessageBox.Show(msgInfo);
            }
        }

        private bool CheckTextBoxRange(TextBox textBox, double minValue, double maxValue, string fieldName)
        {
            double value;
            if (!double.TryParse(textBox.Text, out value))
            {
                MessageBox.Show(fieldName + "应为有效数字！");
                return false;
            }

            if (value < minValue || value > maxValue)
            {
                MessageBox.Show(fieldName + "的值应在 " + minValue + " 和 " + maxValue + " 之间！");
                return false;
            }

            return true;
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            if (!ecanHelper.IsConnection)
            {
                MessageBox.Show(FrmMain.GetString("keyOpenPrompt"));
                return;
            }

            byte[] bytes = new byte[8] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BCU_ID, 0xF9, 0x10 });

            Thread.Sleep(1000);
            ecanHelper.Send(new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new byte[] { 0xE0, FrmMain.BCU_ID, 0xF9, 0x10 });
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

        #region 2021/10/27 导入导出功能（建议提取到Excel帮助类库中处理）
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Multiselect = true;
                fileDialog.Title = "请选择文件";
                fileDialog.Filter = "所有文件(*xml*)|*.xml";//设置要选择的文件的类型
                string file = "";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    file = fileDialog.FileName;//返回文件的完整路径
                }

                TextBox textBox = new TextBox();
                mDocument = new XmlDocument();
                mDocument.Load(file);

                XmlNodeList nodeList = mDocument.DocumentElement.SelectNodes("//param");
                string[] str = new string[1];
                foreach (XmlNode xmlNo in nodeList)
                {
                    XmlElement xe = (XmlElement)xmlNo;
                    {
                        Control txt = this.Controls.Find(xe.GetAttribute("id"), true)[0];

                        if (xe.Name == "param")
                            txt.Text = xe.InnerText;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            try
            {
                StreamWriter sw = File.CreateText(ShowSaveFileDialog());
                sw.WriteLine("<documentParams>");
                for (int i = 0; i < 26; i++)
                {
                    string controlName1 = string.Format("txt_{0}", i + 1);
                    string val = (Controls.Find(controlName1, true)[0]).Text.Trim().Length != 0 ? (Controls.Find(controlName1, true)[0]).Text : null;

                    sw.WriteLine(" <param id=\"" + controlName1 + "\" name=\"" + keys[controlName1] + "\">" + val + "</param>");
                }
                sw.WriteLine("</documentParams>");
                sw.Close();
            }
            catch (Exception ex)
            {
            }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        //选择保存路径
        private string ShowSaveFileDialog()
        {
            string localFilePath = "";
            //string localFilePath, fileNameExt, newFileName, FilePath; 
            SaveFileDialog sfd = new SaveFileDialog();
            //设置文件类型 
            sfd.Filter = "XML文档（*.xml）|*.xml";

            //设置默认文件类型显示顺序 
            sfd.FilterIndex = 1;

            //保存对话框是否记忆上次打开的目录 
            sfd.RestoreDirectory = true;

            //点了保存按钮进入 
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                localFilePath = sfd.FileName.ToString(); //获得文件路径 
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
            }

            return localFilePath;
        }
        #endregion

        public void analysisData(uint canID, byte[] data)
        {
            byte[] canid = BitConverter.GetBytes(canID);
            if (canid[0] != FrmMain.BCU_ID || !(canid[0] == FrmMain.BCU_ID && (canid[1] == 0xE0|| canid[1] == 0xFF) && canid[3] == 0x10)) return;

            int[] numbers = BytesToUint16(data);       

            switch (canid[2])
            {
                case 0xE0://总体过充
                    txt_1.Text = (numbers[0] * 0.1).ToString();
                    txt_2.Text = (numbers[1] * 0.1).ToString();
                    txt_3.Text = (numbers[2] * 0.1).ToString();
                    txt_4.Text = (numbers[3] * 0.1).ToString();
                    break;
                case 0xE1://总体过放
                    txt_5.Text = (numbers[0] * 0.1).ToString();
                    txt_6.Text = (numbers[1] * 0.1).ToString();
                    txt_7.Text = (numbers[2] * 0.1).ToString();
                    txt_8.Text = (numbers[3] * 0.1).ToString();
                    break;
                case 0xE2://充电过流
                    txt_9.Text  = (numbers[0] * 0.01).ToString();
                    txt_10.Text = (numbers[1] * 0.01).ToString();
                    txt_11.Text = (numbers[2] * 0.01).ToString();
                    txt_12.Text = (numbers[3] * 0.01).ToString();
                    break;
                case 0xE3://放电过流1
                    txt_13.Text = (numbers[0] * 0.01).ToString();
                    txt_14.Text = (numbers[1] * 0.01).ToString();
                    txt_15.Text = (numbers[2] * 0.01).ToString();
                    txt_16.Text = (numbers[3] * 0.01).ToString();
                    break;
                case 0xE4://放电过流2
                    txt_17.Text = (numbers[0] * 0.01).ToString();
                    txt_18.Text = (numbers[1] * 0.01).ToString();
                    txt_19.Text = (numbers[2] * 0.01).ToString();
                    txt_20.Text = (numbers[3] * 0.01).ToString();
                    break;
                case 0xE5://低电量
                    txt_21.Text = (numbers[2] * 1).ToString();
                    txt_22.Text = (numbers[3] * 1).ToString();                 
                    break;
                case 0xF1://风扇、满充、Pack
                    txt_23.Text = (numbers[0] * 0.1).ToString();
                    txt_24.Text = (numbers[1] * 0.1).ToString();
                    txt_25.Text = (numbers[2]).ToString();
                    txt_26.Text = (numbers[3]).ToString();                  
                    break;            
            }
        }

        int[] BytesToBit(byte[] data)
        {
            int[] numbers = new int[8];
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = Convert.ToInt32(data[i].ToString("X2"), 16);
            }
            return numbers;
        }
        int[] BytesToUint16(byte[] data)
        {
            int[] numbers = new int[4];
            for (int i = 0; i < data.Length; i += 2)
            {
                numbers[i / 2] = Convert.ToInt32(data[i + 1].ToString("X2") + data[i].ToString("X2"), 16);
            }
            return numbers;
        }
        string ASCIIToHEX(string data)
        {
            StringBuilder result = new StringBuilder(data.Length * 2);
            for (int i = 0; i < data.Length; i++)
            {
                result.Append(((int)data[i]).ToString("X2") + " ");
            }
            return Convert.ToString(result);
        }

        byte[] Uint16ToBytes(TextBox t1, TextBox t2, TextBox t3, TextBox t4,
           double scaling1, double scaling2, double scaling3, double scaling4)
        {
            byte[] b1 = Uint16ToBytes(Convert.ToUInt32(float.Parse(t1.Text) / scaling1));
            byte[] b2 = Uint16ToBytes(Convert.ToUInt32(float.Parse(t2.Text) / scaling2));
            byte[] b3 = Uint16ToBytes(Convert.ToUInt32(float.Parse(t3.Text) / scaling3));
            byte[] b4 = Uint16ToBytes(Convert.ToUInt32(float.Parse(t4.Text) / scaling4));

            return new byte[] { b1[0], b1[1], b2[0], b2[1], b3[0], b3[1], b4[0], b4[1] };
        }
        byte[] Uint16ToBytes(uint ivalue)
        {
            byte[] data = new byte[2];
            data[1] = (byte)(ivalue >> 8);
            data[0] = (byte)(ivalue & 0xff);

            return data;
        }

        byte[] Uint8ToBits(int t1, int t2, int t3, int t4, int t5, int t6, int t7, int t8)
        {
            byte b1 = Convert.ToByte(t1.ToString("X2"), 16);
            byte b2 = Convert.ToByte(t2.ToString("X2"), 16);
            byte b3 = Convert.ToByte(t3.ToString("X2"), 16);
            byte b4 = Convert.ToByte(t4.ToString("X2"), 16);
            byte b5 = Convert.ToByte(t5.ToString("X2"), 16);
            byte b6 = Convert.ToByte(t6.ToString("X2"), 16);
            byte b7 = Convert.ToByte(t7.ToString("X2"), 16);
            byte b8 = Convert.ToByte(t8.ToString("X2"), 16);

            return new byte[] { b1, b2, b3, b4, b5, b6, b7, b8 };
        }
    }
}