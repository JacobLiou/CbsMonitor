using Org.BouncyCastle.Asn1.Ocsp;
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
using SofarBMS.Helper;
using SofarBMS.Model;

namespace SofarBMS.UI
{
    public partial class BMSMMultipleControl : UserControl
    {
        EcanHelper ecanHelper = EcanHelper.Instance;
        public static CancellationTokenSource cts = null;
        List<FaultInfo> faultInfos = new List<FaultInfo>() {
                    new FaultInfo("单体过压保护",0,0,0,0,2),
                    new FaultInfo("单体过压告警",0,1,0,0,1),
                    new FaultInfo("单体欠压保护",0,2,0,0,2),
                    new FaultInfo("单体欠压告警",0,3,0,0,1),
                    new FaultInfo("总压过压保护",0,4,0,0,2),
                    new FaultInfo("总压过压告警",0,5,0,0,1),
                    new FaultInfo("总压欠压保护",0,6,0,0,2),
                    new FaultInfo("总压欠压告警",0,7,0,0,1),
                    new FaultInfo("充电温度过高保护",1,0,0,0,2),
                    new FaultInfo("充电温度过高告警",1,1,0,0,1),
                    new FaultInfo("充电温度过低保护",1,2,0,0,2),
                    new FaultInfo("充电温度过低告警",1,3,0,0,1),
                    new FaultInfo("放电温度过高保护",1,4,0,0,2),
                    new FaultInfo("放电温度过高告警",1,5,0,0,1),
                    new FaultInfo("放电温度过低保护",1,6,0,0,2),
                    new FaultInfo("放电温度过低告警",1,7,0,0,1),
                    new FaultInfo("充电过流保护",2,0,0,0,2),
                    new FaultInfo("充电过流告警",2,1,0,0,1),
                    new FaultInfo("放电过流保护",2,2,0,0,2),
                    new FaultInfo("放电过流告警",2,3,0,0,1),
                    new FaultInfo("环境温度过高保护",2,4,0,0,2),
                    new FaultInfo("环境温度过高告警",2,5,0,0,1),
                    new FaultInfo("环境温度过低保护",2,6,0,0,2),
                    new FaultInfo("环境温度过低告警",2,7,0,0,1),
                    new FaultInfo("MOS温度过高保护",3,0,0,0,2),
                    new FaultInfo("MOS温度过高告警",3,1,0,0,1),
                    new FaultInfo("SOC过低保护",3,2,0,0,2),
                    new FaultInfo("SOC过低告警",3,3,0,0,1),
                    new FaultInfo("总压采样异常",3,4,0,0,0),
                    new FaultInfo("总压过大硬件保护",3,5,0,0,2),
                    new FaultInfo("充电过流硬件保护",3,6,0,0,2),
                    new FaultInfo("放电过流硬件保护",3,7,0,0,2),
                    new FaultInfo("电池满充",4,0,0,0,1),
                    new FaultInfo("短路保护",4,1,0,0,2),
                    new FaultInfo("EEPROM异常",4,2,0,0,3),
                    new FaultInfo("电芯失效",4,3,0,0,3),
                    new FaultInfo("NTC异常",4,4,0,0,3),
                    new FaultInfo("充电MOS异常",4,5,0,0,3),
                    new FaultInfo("放电MOS异常",4,6,0,0,3),
                    new FaultInfo("采集异常",4,7,0,0,3),
                    new FaultInfo("限流异常",5,0,0,0,3),
                    new FaultInfo("充电器反接",5,1,0,0,3),
                    new FaultInfo("CAN通信异常",5,2,0,0,2),
                    new FaultInfo("CAN_ID冲突保护",5,3,0,0,2),
                    new FaultInfo("电池放空",5,4,0,0,1),
                    new FaultInfo("PCU永久故障",5,5,0,0,3),
                    new FaultInfo("预充失败",5,6,0,0,0),
                    new FaultInfo("软件异常",5,7,0,0,3),
                    new FaultInfo("充电电流大环零点不良",6,0,0,0,3),
                    new FaultInfo("充电电流小环零点不良",6,1,0,0,3),
                    new FaultInfo("零点电流异常",6,2,0,0,3),
                    new FaultInfo("主回路保险丝熔断",6,3,0,0,3),
                    new FaultInfo("锁存器异常",6,4,0,0,3),
                    new FaultInfo("12V电压异常",6,5,0,0,3),
                    new FaultInfo("电池过压严重故障",6,6,0,0,3),
                    new FaultInfo("电池欠压严重故障",6,7,0,0,3),
                    new FaultInfo("放电电流大环零点不良",7,0,0,0,3),
                    new FaultInfo("放电电流小环零点不良",7,1,0,0,3),
                    new FaultInfo("电芯温度过大",7,2,0,0,2),
        };
        Dictionary<uint, int> devList = new Dictionary<uint, int>()
        {
            {0x1, 0},
            {0x2, 0},
            {0x3, 0},
            {0x4, 0},
            {0x5, 0},
            {0x6, 0},
            {0x7, 0},
            {0x8, 0},
            {0x11, 0},
            {0x12, 0},
            {0x13, 0},
            {0x14, 0},
            {0x15, 0},
            {0x16, 0},
            {0x17, 0},
            {0x18, 0},
        };

        public BMSMMultipleControl()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
        }

        private void BMSMMultipleControl_Load(object sender, EventArgs e)
        {
            foreach (Control item in this.Controls)
            {
                GetControls(item);
            }

            Task.Run(async delegate
            {
                while (!cts.IsCancellationRequested)
                {
                    string controlState = "lblDevState_";
                    for (int i = 1; i <= 8; i++)
                    {
                        if (this.Controls.Find(controlState + i, true).Length > 0)
                        {
                            (this.Controls.Find(controlState + i, true)[0] as Label).BackColor = Color.FromArgb(238, 238, 238);
                        }
                    }

                    if (ecanHelper.IsConnection)
                    {
                        //获取实时数据指令
                        ecanHelper.Send(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                       , new byte[] { 0xE0, 0xFF, 0x2C, 0x10 });

                        await Task.Delay(100);

                        //发送条形码读取
                        ecanHelper.Send(new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
                                        , new byte[] { 0xE0, FrmMain.BMS_ID, 0x2E, 0x10 });
                    }


                    //增加信号量状态的检查
                    while (!cts.IsCancellationRequested && EcanHelper._task.Count > 0)
                    {
                        CAN_OBJ ch = (CAN_OBJ)EcanHelper._task.Dequeue();
                        analysisData(ch.ID, ch.Data);
                    }
                    await Task.Delay(1000);
                }
            }, cts.Token);
        }

        public void analysisData(uint canID, byte[] data)
        {
            string[] strs;
            string[] controls;

            this.Invoke(new Action(() =>
            {
                uint id = canID & 0xff;

                //在线状态
                string controlName = "lblDevState_" + id;

                if (this.Controls.Find(controlName, true).Length > 0)
                {
                    devList[id] = 1;

                    (this.Controls.Find(controlName, true)[0] as Label).BackColor = Color.FromArgb(0, 153, 204);
                }

                switch (canID | 0xff)
                {
                    case 0x1003FFFF:
                        strs = new string[2] { "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i += 2)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 2], data[i * 2 + 1], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[2] { "txtChargeCurrentLimitation", "txtDischargeCurrentLimitation" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }

                        string ChargeMosEnable = "lblChargeMosEnable_" + id;

                        if (this.Controls.Find(ChargeMosEnable, true).Length > 0)
                        {
                            if (GetBit(data[5], 0) == 1)
                            {
                                (this.Controls.Find(ChargeMosEnable, true)[0] as Label).BackColor = Color.FromArgb(255, 0, 51);
                            }
                            else
                            {
                                (this.Controls.Find(ChargeMosEnable, true)[0] as Label).BackColor = Color.FromArgb(238, 238, 238);
                            }
                        }

                        string DischargeMosEnable = "lblDischargeMosEnable_" + id;

                        if (this.Controls.Find(DischargeMosEnable, true).Length > 0)
                        {
                            if (GetBit(data[5], 1) == 1)
                            {
                                (this.Controls.Find(DischargeMosEnable, true)[0] as Label).BackColor = Color.FromArgb(255, 0, 51);
                            }
                            else
                            {
                                (this.Controls.Find(DischargeMosEnable, true)[0] as Label).BackColor = Color.FromArgb(238, 238, 238);
                            }
                        }

                        string PrechgMosEnable = "lblPrechgMosEnable_" + id;

                        if (this.Controls.Find(PrechgMosEnable, true).Length > 0)
                        {
                            if (GetBit(data[5], 2) == 1)
                            {
                                (this.Controls.Find(PrechgMosEnable, true)[0] as Label).BackColor = Color.FromArgb(255, 0, 51);
                            }
                            else
                            {
                                (this.Controls.Find(PrechgMosEnable, true)[0] as Label).BackColor = Color.FromArgb(238, 238, 238);
                            }
                        }
                        break;
                    case 0x1004FFFF:
                        strs = new string[4] { "0.1", "0.1", "0.01", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }

                        controls = new string[4] { "txtBatteryVolt_" + id, "txtLoadVolt", "txtBatteryCurrent_" + id, "txtSOC_" + id };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            if (this.Controls.Find(controls[i], true).Length >= 1)
                                (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }
                        break;
                    case 0x1005FFFF:
                        strs = new string[2];
                        strs[0] = BytesToIntger(data[1], data[0]);
                        strs[1] = BytesToIntger(data[4], data[3]);

                        controls = new string[2] { "txtBatMaxCellVolt_" + id, "txtBatMinCellVolt_" + id };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }
                        break;
                    case 0x1006FFFF:
                        strs = new string[2] { "0.1", "0.1" };
                        strs[0] = BytesToIntger(data[1], data[0], 0.1);
                        strs[1] = BytesToIntger(data[4], data[3], 0.1);

                        controls = new string[2] { "txtBatMaxCellTemp_" + id, "txtBatMinCellTemp_" + id };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }
                        break;

                    case 0x100FFFFF:
                        strs = new string[2] { "0.1", "0.1" };
                        for (int i = 0; i < strs.Length; i++)
                        {
                            strs[i] = BytesToIntger(data[i * 2 + 1], data[i * 2], Convert.ToDouble(strs[i]));
                        }
                        controls = new string[2] { "txtRemainingcapacity_" + id, "txtFullcapacity_" + id };
                        for (int i = 0; i < controls.Length; i++)
                        {
                            (this.Controls.Find(controls[i], true)[0] as TextBox).Text = strs[i];
                        }
                        break;
                    case 0x1008FFFF:
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
                                            string AlarmEnable = "lblFaultEnable" + id;

                                            if (this.Controls.Find(AlarmEnable, true).Length > 0)
                                            {
                                                if (GetBit(data[5], 0) == 1)
                                                {
                                                    (this.Controls.Find(AlarmEnable, true)[0] as Label).BackColor = Color.FromArgb(255, 0, 51);
                                                }
                                                else
                                                {
                                                    (this.Controls.Find(AlarmEnable, true)[0] as Label).BackColor = Color.FromArgb(238, 238, 238);
                                                }
                                            }

                                            break;
                                        case "2":
                                            string ProtectionEnable = "lblProtectionEnable_" + id;

                                            if (this.Controls.Find(ProtectionEnable, true).Length > 0)
                                            {
                                                if (GetBit(data[5], 0) == 1)
                                                {
                                                    (this.Controls.Find(ProtectionEnable, true)[0] as Label).BackColor = Color.FromArgb(255, 0, 51);
                                                }
                                                else
                                                {
                                                    (this.Controls.Find(ProtectionEnable, true)[0] as Label).BackColor = Color.FromArgb(238, 238, 238);
                                                }
                                            }

                                            break;
                                        case "3":
                                            string FaultEnable = "lblFaultEnable_" + id;

                                            if (this.Controls.Find(FaultEnable, true).Length > 0)
                                            {
                                                if (GetBit(data[5], 0) == 1)
                                                {
                                                    (this.Controls.Find(FaultEnable, true)[0] as Label).BackColor = Color.FromArgb(255, 0, 51);
                                                }
                                                else
                                                {
                                                    (this.Controls.Find(FaultEnable, true)[0] as Label).BackColor = Color.FromArgb(238, 238, 238);
                                                }
                                            }

                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    case 0x104EFFFF:
                        string BmsStartState = "lblBmsStartState_" + id;

                        if (this.Controls.Find(BmsStartState, true).Length > 0)
                        {
                            if (GetBit(data[6], 0) == 1)
                            {
                                (this.Controls.Find(BmsStartState, true)[0] as Label).BackColor = Color.FromArgb(255, 0, 51);
                            }
                            else
                            {
                                (this.Controls.Find(BmsStartState, true)[0] as Label).BackColor = Color.FromArgb(238, 238, 238);
                            }
                        }

                        string BmsDsgReady = "lblBmsDsgReady_" + id;

                        if (this.Controls.Find(BmsDsgReady, true).Length > 0)
                        {
                            if (GetBit(data[6], 4) == 1)
                            {
                                (this.Controls.Find(BmsDsgReady, true)[0] as Label).BackColor = Color.FromArgb(255, 0, 51);
                            }
                            else
                            {
                                (this.Controls.Find(BmsDsgReady, true)[0] as Label).BackColor = Color.FromArgb(238, 238, 238);
                            }
                        }

                        string BmsChgReady = "lblBmsChgReady_" + id;

                        if (this.Controls.Find(BmsChgReady, true).Length > 0)
                        {
                            if (GetBit(data[6], 3) == 1)
                            {
                                (this.Controls.Find(BmsChgReady, true)[0] as Label).BackColor = Color.FromArgb(255, 0, 51);
                            }
                            else
                            {
                                (this.Controls.Find(BmsChgReady, true)[0] as Label).BackColor = Color.FromArgb(238, 238, 238);
                            }
                        }
                        break;
                    case 0x104FFFFF:
                        byte[] table = { 0x01, 0x02, 0x04, 0x8, 0x10, 0x20, 0x40, 0x80 };

                        byte[] responsedata = data;//new byte[8] { data[3], data[2], data[1], data[0], data[7], data[6], data[5], data[4] };
                        List<int> lists = new List<int>();
                        int cnt = 0;
                        for (int i = 0; i < responsedata.Length; i++)
                        {
                            if (responsedata[i] != 0x00)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    if ((table[j] & responsedata[i]) == table[j])
                                    {
                                        lists.Add(j + cnt + 1 + (responsedata[4] * 24));
                                    }
                                }
                            }
                            cnt += 8;
                        }

                        for (int i = 0; i < lists.Count; i++)
                        {
                            string ParallelCtrl = "ckParallelCtrl_" + lists[i];

                            if (this.Controls.Find(ParallelCtrl, true).Length > 0)
                            {
                                (this.Controls.Find(ParallelCtrl, true)[0] as CheckBox).Checked = true;
                            }
                        }
                        break;
                }
            }));
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

                string[] labelContrl = new string[] { "lblRealtimeData_19", "lblRealtimeData_21", "lblRealtimeData_24", "lblRealtimeData_26", "lblRealtimeData_14", "lblRealtimeData_16",
                "ChargeMosEnable","DischargeMosEnable","PrechgMosEnable","FaultEnable","ProtectionEnable","AlarmEnable",
                "lblBmsStartState","lblBmsDsgReady","lblBmsChgReady",
                "lblRealtimeData_17","lblRealtimeData_52","lblRealtimeData_53"};

                foreach (var item in labelContrl)
                {
                    if (name.Contains(item))
                    {
                        name = name.Remove(name.LastIndexOf('_'));
                        break;
                    }
                }

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

            if (len > 10 && len < str.Length)

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

        public string[] getLog(out string[] msg, int row, int column)
        {
            msg = new string[2];

            for (int i = 0; i < faultInfos.Count; i++)
            {
                if (faultInfos[i].Byte == row && faultInfos[i].Bit == column)
                {
                    msg[0] = faultInfos[i].Content;
                    msg[1] = faultInfos[i].Type.ToString();
                    break;
                }
            }
            return msg;
        }

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

        private void label19_Click(object sender, EventArgs e)
        {

        }
    }
}
