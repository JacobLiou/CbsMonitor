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
    public partial class ParamControl : UserControl
    {
        public static CancellationTokenSource cts = null;

        EcanHelper ecanHelper = EcanHelper.Instance;
        XmlDocument mDocument;

        public bool flag = true;
        public static int index = 1;
        public static Dictionary<string, string> keys = new Dictionary<string, string>();
        public ParamControl()
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
                keys.Add("txt_1", "单体过充保护(mV)");
                keys.Add("txt_2", "单体过充保护解除(mV)");
                keys.Add("txt_3", "单体过充告警(mV)");
                keys.Add("txt_4", "单体过充告警解除(mV)");
                keys.Add("txt_5", "总体过充保护(V)");
                keys.Add("txt_6", "总体过充保护解除(V)");
                keys.Add("txt_7", "总体过充告警(V)");
                keys.Add("txt_8", "总体过充告警解除(V)");
                keys.Add("txt_9", "单体过放保护(mV)");
                keys.Add("txt_10", "单体过放保护解除(mV)");
                keys.Add("txt_11", "单体过放告警(mV)");
                keys.Add("txt_12", "单体过放告警解除(mV)");
                keys.Add("txt_13", "总体过放保护(V)");
                keys.Add("txt_14", "总体过放保护解除(V)");
                keys.Add("txt_15", "总体过放告警(V)");
                keys.Add("txt_16", "总体过放告警解除(V)");
                keys.Add("txt_17", "充电过流保护(A)");
                keys.Add("txt_18", "充电过流保护解除(A)");
                keys.Add("txt_19", "充电过流告警(A)");
                keys.Add("txt_20", "充电过流告警解除(A)");
                keys.Add("txt_21", "放电过流保护(A)");
                keys.Add("txt_22", "放电过流保护解除(A)");
                keys.Add("txt_23", "放电过流告警(A)");
                keys.Add("txt_24", "放电过流告警解除(A)");
                keys.Add("txt_25", "充电高温保护(℃)");
                keys.Add("txt_26", "充电高温保护解除(℃)");
                keys.Add("txt_27", "充电高温告警(℃)");
                keys.Add("txt_28", "充电高温告警解除(℃)");
                keys.Add("txt_29", "放电高温保护(℃)");
                keys.Add("txt_30", "放电高温保护解除(℃)");
                keys.Add("txt_31", "放电高温告警(℃)");
                keys.Add("txt_32", "放电高温告警解除(℃)");
                keys.Add("txt_33", "充电低温保护(℃)");
                keys.Add("txt_34", "充电低温保护解除(℃)");
                keys.Add("txt_35", "充电低温告警(℃)");
                keys.Add("txt_36", "充电低温告警解除(℃)");
                keys.Add("txt_37", "放电低温保护(℃)");
                keys.Add("txt_38", "放电低温保护解除(℃)");
                keys.Add("txt_39", "放电低温告警(℃)");
                keys.Add("txt_40", "放电低温告警解除(℃)");
                keys.Add("txt_41", "环境高温保护(℃)");
                keys.Add("txt_42", "环境高温保护解除(℃)");
                keys.Add("txt_43", "环境高温告警(℃)");
                keys.Add("txt_44", "环境高温告警解除(℃)");
                keys.Add("txt_45", "环境低温保护(℃)");
                keys.Add("txt_46", "环境低温保护解除(℃)");
                keys.Add("txt_47", "环境低温告警(℃)");
                keys.Add("txt_48", "环境低温告警解除(℃)");
                keys.Add("txt_49", "低电量保护(%)");
                keys.Add("txt_50", "低电量保护解除(%)");
                keys.Add("txt_51", "低电量告警(%)");
                keys.Add("txt_52", "低电量告警解除(%)");
                keys.Add("txt_53", "均衡开启电压(mV)");
                keys.Add("txt_54", "均衡开启压差(mV)");
                keys.Add("txt_55", "满充电压(mV)");
                keys.Add("txt_56", "加热模开启温度(℃)");
                keys.Add("txt_57", "加热膜关闭温度(℃)");
                keys.Add("txt_58", "电池包截止电压(V)");
                keys.Add("txt_59", "电池包截止电流(A)");
                keys.Add("txt_60", "额定容量(AH)");
                keys.Add("txt_61", "单体电压个数(NA)");
                keys.Add("txt_62", "单体温度个数(NA)");
                keys.Add("txt_63", "累计充电容量(mAH)");
                keys.Add("txt_64", "累计放电容量(mAH)");
                keys.Add("txt_65", "SOC(%)");
                keys.Add("txt_66", "时间");
                keys.Add("txt_67", "条形码");
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
                            ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 });

                            flag = false;
                            await Task.Delay(1000);
                        }

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
            if (ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 }))
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
            byte[] canid = new byte[] { 0xE0, FrmMain.BMS_ID, 0x00, 0x10 };
            byte[] bytes = new byte[8];
            int num = 13;
            //检查输入数据的合法性
            if (!CheckTextBoxRange(txt_1, 0, 4000, "单体过充保护") || !CheckTextBoxRange(txt_2, 0, 3800, "单体过充解除") || !CheckTextBoxRange(txt_3, 0, 3800, "单体过充告警") ||
                !CheckTextBoxRange(txt_4, 0, 3600, "单体过充解除") || !CheckTextBoxRange(txt_5, 0, 64.0, "总体过充保护") || !CheckTextBoxRange(txt_6, 0, 60.8, "总体过充解除") ||
                !CheckTextBoxRange(txt_7, 0, 60.8, "总体过充告警") || !CheckTextBoxRange(txt_8, 0, 57.7, "总体过充解除") || !CheckTextBoxRange(txt_9, 2300, 65535, "单体过放保护") ||
                !CheckTextBoxRange(txt_10, 2500, 65535, "单体过放解除") || !CheckTextBoxRange(txt_11, 2500, 65535, "单体过放告警") || !CheckTextBoxRange(txt_12, 2700, 65535, "单体过放解除") ||
                !CheckTextBoxRange(txt_13, 36.8, 6553.5, "总体过放保护") || !CheckTextBoxRange(txt_14, 40, 6553.5, "总体过放解除") || !CheckTextBoxRange(txt_15, 40, 6553.5, "总体过放告警") ||
                !CheckTextBoxRange(txt_16, 44, 6553.5, "总体过放解除") || !CheckTextBoxRange(txt_17, 0, 200, "充电过流保护") || !CheckTextBoxRange(txt_18, 0, 655.35, "充电过流解除") ||
                !CheckTextBoxRange(txt_19, 0, 655.35, "充电过流告警") || !CheckTextBoxRange(txt_20, 0, 655.35, "充电过流解除") || !CheckTextBoxRange(txt_21, 0, 200, "放电过流保护") ||
                !CheckTextBoxRange(txt_22, 0, 655.35, "放电过流解除") || !CheckTextBoxRange(txt_23, 0, 655.35, "放电过流告警") || !CheckTextBoxRange(txt_24, 0, 655.35, "放电过流解除") ||
                !CheckTextBoxRange(txt_25, -40, 60, "充电高温保护") || !CheckTextBoxRange(txt_26, -40, 50, "充电高温解除") || !CheckTextBoxRange(txt_27, -40, 125, "充电高温告警") ||
                !CheckTextBoxRange(txt_28, -40, 125, "充电高温解除") || !CheckTextBoxRange(txt_29, -40, 65, "放电高温保护") || !CheckTextBoxRange(txt_30, -40, 56, "放电高温解除") ||
                !CheckTextBoxRange(txt_31, -40, 125, "放电高温告警") || !CheckTextBoxRange(txt_32, -40, 125, "放电高温解除") || !CheckTextBoxRange(txt_33, 0, 125, "充电低温保护") ||
                !CheckTextBoxRange(txt_34, 2, 125, "充电低温解除") || !CheckTextBoxRange(txt_35, 0, 125, "充电低温告警") || !CheckTextBoxRange(txt_36, 2, 125, "充电低温告警") ||
                !CheckTextBoxRange(txt_37, -22, 125, "放电低温保护") || !CheckTextBoxRange(txt_38, -20, 125, "放电低温解除") || !CheckTextBoxRange(txt_39, -40, 125, "放电低温告警") ||
                !CheckTextBoxRange(txt_40, -40, 125, "放电低温解除") || !CheckTextBoxRange(txt_41, -40, 125, "环境高温保护") || !CheckTextBoxRange(txt_42, -40, 125, "环境高温解除") ||
                !CheckTextBoxRange(txt_43, -40, 125, "环境高温告警") || !CheckTextBoxRange(txt_44, -40, 125, "环境高温解除") || !CheckTextBoxRange(txt_45, -40, 125, "环境低温保护") ||
                !CheckTextBoxRange(txt_46, -40, 125, "环境低温解除") || !CheckTextBoxRange(txt_47, -40, 125, "环境低温告警") || !CheckTextBoxRange(txt_48, -40, 125, "环境低温解除") ||
                !CheckTextBoxRange(txt_49, 0, 100, "低电量保护") || !CheckTextBoxRange(txt_50, 0, 100, "低电量解除") || !CheckTextBoxRange(txt_51, 0, 100, "低电量告警") ||
                !CheckTextBoxRange(txt_52, 0, 100, "低电量解除") || !CheckTextBoxRange(txt_53, 0, 4000, "均衡开启电压") || !CheckTextBoxRange(txt_54, 0, 1000, "均衡开启压差") ||
                !CheckTextBoxRange(txt_55, 3000, 4000, "满充电压") || !CheckTextBoxRange(txt_56, -40, 125, "加热膜开启温度") || !CheckTextBoxRange(txt_57, -40, 125, "加热膜关闭温度") ||
                !CheckTextBoxRange(txt_58, 0, 60.8, "电池包截止电压") || !CheckTextBoxRange(txt_59, 0, 655.35, "电池包截止电流") || !CheckTextBoxRange(txt_60, -40, 125, "MOS高温保护") ||
                !CheckTextBoxRange(txt_61, -40, 125, "MOS高温解除") || !CheckTextBoxRange(txt_62, -40, 125, "MOS高温告警") || !CheckTextBoxRange(txt_63, -40, 125, "MOS高温解除"))
            {
                return;// 范围检查失败
            }
            else
            {
                for (int i = 1; i <= num; i++)
                {
                    Control c = this.Controls.Find("ckb_" + i, true)[0];
                    if (((CheckBox)c).Checked)
                    {
                        switch (c.Name)
                        {
                            case "ckb_1":
                                canid[2] = 0x10;
                                bytes = Uint16ToBytes(txt_1, txt_2, txt_3, txt_4, 1, 1, 1, 1);
                                break;
                            case "ckb_2":
                                canid[2] = 0x11;
                                bytes = Uint16ToBytes(txt_5, txt_6, txt_7, txt_8, 0.1, 0.1, 0.1, 0.1);
                                break;
                            case "ckb_3":
                                canid[2] = 0x12;
                                bytes = Uint16ToBytes(txt_9, txt_10, txt_11, txt_12, 1, 1, 1, 1);
                                break;
                            case "ckb_4":
                                canid[2] = 0x13;
                                bytes = Uint16ToBytes(txt_13, txt_14, txt_15, txt_16, 0.1, 0.1, 0.1, 0.1);
                                break;
                            case "ckb_5":
                                canid[2] = 0x14;
                                bytes = Uint16ToBytes(txt_17, txt_18, txt_19, txt_20, 0.01, 0.01, 0.01, 0.01);
                                break;
                            case "ckb_6":
                                canid[2] = 0x15;
                                bytes = Uint16ToBytes(txt_21, txt_22, txt_23, txt_24, 0.01, 0.01, 0.01, 0.01);
                                break;
                            case "ckb_7":
                                canid[2] = 0x16;
                                bytes = Uint8ToBits(Convert.ToInt32(txt_25.Text) + 40, Convert.ToInt32(txt_26.Text) + 40,
                                                Convert.ToInt32(txt_27.Text) + 40, Convert.ToInt32(txt_28.Text) + 40,
                                                Convert.ToInt32(txt_29.Text) + 40, Convert.ToInt32(txt_30.Text) + 40,
                                                Convert.ToInt32(txt_31.Text) + 40, Convert.ToInt32(txt_32.Text) + 40);
                                break;
                            case "ckb_8":
                                canid[2] = 0x17;
                                bytes = Uint8ToBits(Convert.ToInt32(txt_33.Text) + 40, Convert.ToInt32(txt_34.Text) + 40,
                                                Convert.ToInt32(txt_35.Text) + 40, Convert.ToInt32(txt_36.Text) + 40,
                                                Convert.ToInt32(txt_37.Text) + 40, Convert.ToInt32(txt_38.Text) + 40,
                                                Convert.ToInt32(txt_39.Text) + 40, Convert.ToInt32(txt_40.Text) + 40);
                                break;
                            case "ckb_9":
                                canid[2] = 0x18;
                                bytes = Uint8ToBits(Convert.ToInt32(txt_41.Text) + 40, Convert.ToInt32(txt_42.Text) + 40,
                                                Convert.ToInt32(txt_43.Text) + 40, Convert.ToInt32(txt_44.Text) + 40,
                                                Convert.ToInt32(txt_45.Text) + 40, Convert.ToInt32(txt_46.Text) + 40,
                                                Convert.ToInt32(txt_47.Text) + 40, Convert.ToInt32(txt_48.Text) + 40);
                                break;
                            case "ckb_10":
                                canid[2] = 0x19;
                                bytes = Uint16ToBytes(txt_49, txt_50, txt_51, txt_52, 1, 1, 1, 1);
                                break;
                            case "ckb_11":
                                canid[2] = 0x21;
                                bytes = Uint16ToBytes(txt_53, txt_54, txt_55, txt_0, 1, 1, 1, 1);
                                bytes[6] = Convert.ToByte(Convert.ToInt32(txt_56.Text) + 40);
                                bytes[7] = Convert.ToByte(Convert.ToInt32(txt_57.Text) + 40);
                                break;
                            case "ckb_12":
                                canid[2] = 0x22;
                                bytes = Uint16ToBytes(txt_58, txt_59, txt_0, txt_0, 0.1, 0.01, 1, 1);
                                break;
                            case "ckb_13":
                                canid[2] = 0x1A;
                                bytes = Uint8ToBits(Convert.ToInt32(txt_60.Text) + 40
                                    , Convert.ToInt32(txt_61.Text) + 40
                                    , Convert.ToInt32(txt_62.Text) + 40
                                    , Convert.ToInt32(txt_63.Text) + 40
                                    , 0, 0, 0, 0);
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
            ecanHelper.Send(bytes, new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 });

            Thread.Sleep(1000);
            ecanHelper.Send(new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 });
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
                for (int i = 0; i < 59; i++)
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
            if (canid[0] != FrmMain.BMS_ID || !(canid[0] == FrmMain.BMS_ID && canid[1] == 0xE0 && canid[3] == 0x10)) return;

            int[] numbers = BytesToUint16(data);
            int[] numbers_bit = BytesToBit(data);

            switch (canid[2])
            {
                case 0x10://单体过充
                    txt_1.Text = numbers[0].ToString();
                    txt_2.Text = numbers[1].ToString();
                    txt_3.Text = numbers[2].ToString();
                    txt_4.Text = numbers[3].ToString();
                    break;
                case 0x11://总体过充
                    txt_5.Text = (numbers[0] * 0.1).ToString();
                    txt_6.Text = (numbers[1] * 0.1).ToString();
                    txt_7.Text = (numbers[2] * 0.1).ToString();
                    txt_8.Text = (numbers[3] * 0.1).ToString();
                    break;
                case 0x12://单体过放
                    txt_9.Text = numbers[0].ToString();
                    txt_10.Text = numbers[1].ToString();
                    txt_11.Text = numbers[2].ToString();
                    txt_12.Text = numbers[3].ToString();
                    break;
                case 0x13://总体过放
                    txt_13.Text = (numbers[0] * 0.1).ToString();
                    txt_14.Text = (numbers[1] * 0.1).ToString();
                    txt_15.Text = (numbers[2] * 0.1).ToString();
                    txt_16.Text = (numbers[3] * 0.1).ToString();
                    break;
                case 0x14://充电过流
                    txt_17.Text = (numbers[0] * 0.01).ToString();
                    txt_18.Text = (numbers[1] * 0.01).ToString();
                    txt_19.Text = (numbers[2] * 0.01).ToString();
                    txt_20.Text = (numbers[3] * 0.01).ToString();
                    break;
                case 0x15://放电过流
                    txt_21.Text = (numbers[0] * 0.01).ToString();
                    txt_22.Text = (numbers[1] * 0.01).ToString();
                    txt_23.Text = (numbers[2] * 0.01).ToString();
                    txt_24.Text = (numbers[3] * 0.01).ToString();
                    break;
                case 0x16://充电/放电高温
                    txt_25.Text = (numbers_bit[0] - 40).ToString();
                    txt_26.Text = (numbers_bit[1] - 40).ToString();
                    txt_27.Text = (numbers_bit[2] - 40).ToString();
                    txt_28.Text = (numbers_bit[3] - 40).ToString();
                    txt_29.Text = (numbers_bit[4] - 40).ToString();
                    txt_30.Text = (numbers_bit[5] - 40).ToString();
                    txt_31.Text = (numbers_bit[6] - 40).ToString();
                    txt_32.Text = (numbers_bit[7] - 40).ToString();
                    break;
                case 0x17://充电/放电低温
                    txt_33.Text = (numbers_bit[0] - 40).ToString();
                    txt_34.Text = (numbers_bit[1] - 40).ToString();
                    txt_35.Text = (numbers_bit[2] - 40).ToString();
                    txt_36.Text = (numbers_bit[3] - 40).ToString();
                    txt_37.Text = (numbers_bit[4] - 40).ToString();
                    txt_38.Text = (numbers_bit[5] - 40).ToString();
                    txt_39.Text = (numbers_bit[6] - 40).ToString();
                    txt_40.Text = (numbers_bit[7] - 40).ToString();
                    break;
                case 0x18://环境高温/低温
                    txt_41.Text = (numbers_bit[0] - 40).ToString();
                    txt_42.Text = (numbers_bit[1] - 40).ToString();
                    txt_43.Text = (numbers_bit[2] - 40).ToString();
                    txt_44.Text = (numbers_bit[3] - 40).ToString();
                    txt_45.Text = (numbers_bit[4] - 40).ToString();
                    txt_46.Text = (numbers_bit[5] - 40).ToString();
                    txt_47.Text = (numbers_bit[6] - 40).ToString();
                    txt_48.Text = (numbers_bit[7] - 40).ToString();
                    break;
                case 0x19://低电量
                    txt_49.Text = numbers[0].ToString();
                    txt_50.Text = numbers[1].ToString();
                    txt_51.Text = numbers[2].ToString();
                    txt_52.Text = numbers[3].ToString();
                    break;
                case 0x21://混合（均衡-满充电压-加热膜）
                    txt_53.Text = numbers[0].ToString();
                    txt_54.Text = numbers[1].ToString();
                    txt_55.Text = numbers[2].ToString();
                    txt_56.Text = (numbers_bit[6] - 40).ToString();
                    txt_57.Text = (numbers_bit[7] - 40).ToString();
                    break;
                case 0x22://电池包
                    txt_58.Text = (numbers[0] * 0.1).ToString();
                    txt_59.Text = (numbers[1] * 0.01).ToString();
                    break;
                case 0x1A://MOS
                    txt_60.Text = (numbers_bit[0] - 40).ToString();
                    txt_61.Text = (numbers_bit[1] - 40).ToString();
                    txt_62.Text = (numbers_bit[2] - 40).ToString();
                    txt_63.Text = (numbers_bit[3] - 40).ToString();
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