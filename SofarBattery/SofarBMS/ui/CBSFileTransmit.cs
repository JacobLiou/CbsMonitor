using log4net.Util;
using SofarBMS.Helper;
using SofarBMS.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace SofarBMS.UI
{
    public partial class CBSFileTransmit : UserControl
    {
        public static CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationTokenSource _token = new CancellationTokenSource();
        EcanHelper ecanHelper = EcanHelper.Instance;

        //变量定义
        private readonly string FiveMinHeadStr = "时间年,月,日,时,分,秒,电池采集电压(mV),电池累计电压(mV),SOC显示值(%),SOH显示值(%),SOC计算值,SOH计算值,电池电流(A),最高单体电压(mV),最低单体电压(mV),最高单体电压序号,最低单体电压序号,最高单体温度(℃),最低单体温度(℃),最高单体温度序号,最低单体温度序号,BMU编号,系统状态,充放电使能,切断请求,关机请求,充电电流上限(A),放电电流上限(A),保护1,保护2,告警1,告警2,故障1,故障2,故障1,故障2,故障3,故障4,主动均衡状态,均衡母线电压(mV),均衡母线电流(mA),辅助供电电压(mV),满充容量(Ah),循环次数,累计放电安时(Ah),累计充电安时(Ah),累计放电瓦时(Wh),累计充电瓦时(Wh),环境温度(℃),DCDC温度1(℃),均衡温度1(℃),均衡温度2(℃),功率端子温度1(℃),功率端子温度2(℃),其他温度1(℃),其他温度2(℃),其他温度3(℃),其他温度4(℃),1-16串均衡状态,单体电压1(mV),单体电压2(mV),单体电压3(mV),单体电压4(mV),单体电压5(mV),单体电压6(mV),单体电压7(mV),单体电压8(mV),单体电压9(mV),单体电压10(mV),单体电压11(mV),单体电压12(mV),单体电压13(mV),单体电压14(mV),单体电压15(mV),单体电压16(mV),单体温度1(℃),单体温度2(℃),单体温度3(℃),单体温度4(℃),单体温度5(℃),单体温度6(℃),单体温度7(℃),单体温度8(℃),单体温度9(℃),单体温度10(℃),单体温度11(℃),单体温度12(℃),单体温度13(℃),单体温度14(℃),单体温度15(℃),单体温度16(℃),RSV1,RSV2,RSV3,RSV4,RSV5,RSV6\r\n";
        private readonly string FaultRecordStr = "电流(A),最大电压(mV),最小电压(mV),最大温度(℃),最小温度(℃)\r\n";
        private readonly string HistoryEventStr = "时间年,月,日,时,分,秒,事件类型\r\n";

        //代号,数据类型,数据长度,精度,单位
        private readonly string FaultRecordText = @"电流,I16,1,1,A
最大电压,U16,1,1,V
最小电压,U16,1,1,V
最大温度,I8,1,1,℃
最小温度,I8,1,1,℃";
        private readonly string FiveMinText = @"时间-年,U8,1,1,1,
时间-月,U8,1,1,1,
时间-日,U8,1,1,1,
时间-时,U8,1,1,1,
时间-分,U8,1,1,1,
时间-秒,U8,1,1,1,
电池采样电压,U16,1,1,mV
电池累计电压,U16,1,1,mV
SOC显示值,U8,1,1,%
SOH显示值,U8,1,1,%
SOC计算值,U32,1,0.001,%
SOH计算值,U32,1,0.001,%
电池电流,I16,1,0.01,A
最高单体电压,U16,1,1,mV
最低单体电压,U16,1,1,mV
最高单体电压序号,U8,1,,
最低单体电压序号,U8,1,,
最高单体温度,I16,1,0.1,℃
最低单体温度,I16,1,0.1,℃
最高单体温度序号,U8,1,,
最低单体温度序号,U8,1,,
BMU编号,U8,1,,
系统状态,U8,1,,
充放电使能,U16,1,,
切断请求,U8,1,,
关机请求,U8,1,,
充电电流上限,U16,1,1,A
放电电流上限,U16,1,1,A
保护1,U16,1,,
保护2,U16,1,,
告警1,U16,1,,
告警2,U16,1,,
故障1,U16,1,,
故障2,U16,1,,
告警3,U16,1,,
告警4,U16,1,,
故障3,U16,1,,
故障4,U16,1,,
主动均衡状态,U16,1,,
均衡母线电压,U16,1,,mV
均衡母线电流,I16,1,,mA
辅助供电电压,U16,1,,mV
满充容量,U16,1,,Ah
循环次数,U16,1,,
累计放电安时,U32,1,1,Ah
累计充电安时,U32,1,1,Ah
累计放电瓦时,U32,1,1,Wh
累计充电瓦时,U32,1,1,Wh
环境温度,I16,1,0.1,℃
dcdc温度1,I16,1,0.1,℃
均衡温度1,I16,1,0.1,℃
均衡温度2,I16,1,0.1,℃
功率端子温度1,I16,1,0.1,℃
功率端子温度2,I16,1,0.1,℃
其他温度1,I16,1,0.1,℃
其他温度2,I16,1,0.1,℃
其他温度3,I16,1,0.1,℃
其他温度4,I16,1,0.1,℃
1-16串均衡状态,U16,1,1,
单体电压1,U16,1,1,mV
单体电压2,U16,1,1,mV
单体电压3,U16,1,1,mV
单体电压4,U16,1,1,mV
单体电压5,U16,1,1,mV
单体电压6,U16,1,1,mV
单体电压7,U16,1,1,mV
单体电压8,U16,1,1,mV
单体电压9,U16,1,1,mV
单体电压10,U16,1,1,mV
单体电压11,U16,1,1,mV
单体电压12,U16,1,1,mV
单体电压13,U16,1,1,mV
单体电压14,U16,1,1,mV
单体电压15,U16,1,1,mV
单体电压16,U16,1,1,mV
单体温度1,I16,1,0.1,℃
单体温度2,I16,1,0.1,℃
单体温度3,I16,1,0.1,℃
单体温度4,I16,1,0.1,℃
单体温度5,I16,1,0.1,℃
单体温度6,I16,1,0.1,℃
单体温度7,I16,1,0.1,℃
单体温度8,I16,1,0.1,℃
单体温度9,I16,1,0.1,℃
单体温度10,I16,1,0.1,℃
单体温度11,I16,1,0.1,℃
单体温度12,I16,1,0.1,℃
单体温度13,I16,1,0.1,℃
单体温度14,I16,1,0.1,℃
单体温度15,I16,1,0.1,℃
单体温度16,I16,1,0.1,℃
RSV1,U16,1,,
RSV2,U32,1,,
RSV3,U32,1,,
RSV4,U32,1,,
RSV5,U32,1,,
RSV6,U32,1,,";
        private readonly string HistoryEventText = @"时间-年,U8,1,1,1,
时间-月,U8,1,1,1,
时间-日,U8,1,1,1,
时间-时,U8,1,1,1,
时间-分,U8,1,1,1,
时间-秒,U8,1,1,1,
事件类型,U16,1,1,";

        private bool _state;
        public bool state
        {
            get { return _state; }
            set
            {
                _state = value;
                this.Invoke(new Action(() =>
                {
                    if (_state)
                    {
                        btnFileTransmit.Text = "终止";
                    }
                    else
                    {
                        btnFileTransmit.Text = "启动";
                    }
                }));
            }
        }

        StepRemark stepFlag;
        bool isResponse;

        int slaveAddress = -1;
        int fileNumber = -1;
        int readType = -1;

        int questCycle = 0;
        int fileOffset = 200;
        int dataLength = 200;

        int fileSize = 0;
        string textStr = "";
        string headStr = "";
        string filePath = "";
        string fileName = "";
        List<byte> dataBuffer = new List<byte>();


        public CBSFileTransmit()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
        }

        private void CBSFileTransmit_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    lock (EcanHelper._locker)
                    {
                        while (EcanHelper._task.Count > 0
                        && !_token.IsCancellationRequested)
                        {
                            //出队
                            CAN_OBJ ch = (CAN_OBJ)EcanHelper._task.Dequeue();

                            //解析
                            this.Invoke(new Action(() => { AnalysisData(ch.ID, ch.Data); }));
                        }
                    }
                }
            }, cts.Token);

            //初始化值
            this.txtSlaveAddress.Text = FrmMain.BMS_ID.ToString();
            this.cbbFileNumber.SelectedIndex = 3;
            this.cbbModeName.SelectedIndex = 1;
            this.cbbModeName.Enabled = false;
            this.ckReadAll.Checked = true;
        }

        private void btnFileTransmit_Click(object sender, EventArgs e)
        {
            fileNumber = cbbFileNumber.SelectedIndex;//确认解析点表
            readType = ckReadAll.Checked ? 0 : 1;

            StartTransmit();
        }


        private void AnalysisData(uint id, byte[] data)
        {
            id = id | 0xff;

            switch (stepFlag)
            {
                case StepRemark.None:
                    break;
                case StepRemark.读文件指令帧:
                    if (id == 0x7F0E0FF)
                    {
                        AddLog(System.DateTime.Now.ToString("HH:mm:ss:fff"), id.ToString("X8"), byteToHexString(data));

                        if (data[3] == 0x0)
                        {
                            isResponse = true;
                            string headName = "";

                            if (fileNumber == 0 || fileNumber == 1 || fileNumber == 2)
                            {
                                headStr = FaultRecordStr;
                                headName = "故障录波";
                            }
                            else if (fileNumber == 3)
                            {
                                headStr = FiveMinHeadStr;
                                headName = "五分钟特性数据";
                            }
                            else if (fileNumber == 4)
                            {
                                headStr = "none";
                                headName = "运行日志";
                            }
                            else if (fileNumber == 5)
                            {
                                headStr = HistoryEventStr;
                                headName = "历史事件";
                            }

                            fileSize = Convert.ToInt32(data[6].ToString("X2") + data[5].ToString("X2") + data[4].ToString("X2"), 16);

                            if (!string.IsNullOrEmpty(headStr))
                            {
                                fileName = string.Format("{0}_{1}_{2}_{3}", headName, cbbModeName.Text.Trim(), slaveAddress, System.DateTime.Now.ToString("yy-MM-dd-HH-mm-ss"));
                                filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}//Log//{fileName}.csv";

                                if ((fileNumber < 3 && fileNumber >= 0) || fileNumber == 5)
                                {
                                    fileOffset = 8;
                                    dataLength = 8;
                                }
                                else if (fileNumber == 3 || fileNumber == 4)
                                {
                                    if (fileNumber == 4)
                                    {
                                        filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}//Log//{fileName}.txt";
                                    }
                                    fileOffset = 200;
                                    dataLength = 200;
                                }
                                questCycle = fileSize % fileOffset == 0 ? (fileSize / fileOffset - 1) : fileSize / fileOffset;//一般结果为true
                            }

                            if (!File.Exists(filePath))
                            {
                                File.AppendAllText(filePath, headStr);
                            }

                            stepFlag = StepRemark.读文件数据内容帧;
                        }
                    }
                    break;
                case StepRemark.读文件数据内容帧:
                    if (id == 0x7F2E0FF)
                    {
                        dataBuffer.AddRange(data);

                        if (fileNumber == 0 || fileNumber == 1 || fileNumber == 2)
                        {
                            if (dataBuffer.Count == 8)
                            {
                                textStr = FaultRecordText;
                            }
                        }
                        else if (fileNumber == 3)
                        {
                            if (dataBuffer.Count == 200)
                            {
                                textStr = FiveMinText;

                                //测试打印
                                Debug.WriteLine(byteToHexString(dataBuffer.ToArray()));
                            }
                        }
                        else if (fileNumber == 5)
                        {
                            textStr = HistoryEventText;
                        }
                    }
                    else if (id == 0x7F3E0FF)
                    {
                        isResponse = true;
                        AddLog(System.DateTime.Now.ToString("HH:mm:ss:fff"), id.ToString("X8"), byteToHexString(data));

                        if (!string.IsNullOrEmpty(textStr))
                        {
                            string getValue = ToAnalysis(textStr, dataBuffer.ToArray());
                            dataBuffer.Clear();

                            File.AppendAllText(filePath, getValue + "\r\n");
                        }
                        else
                        {
                            if (fileNumber == 4)
                            {
                                string sbContent = "";
                                for (int i = 0; i < dataBuffer.Count; i++)
                                {
                                    String asciiStr = ((char)dataBuffer[i]).ToString();//十六进制转ASCII码
                                    sbContent += asciiStr;
                                }
                                TxtHleper.FileWrite(filePath, sbContent);
                                Debug.WriteLine(sbContent);
                            }
                        }
                    }
                    break;
                case StepRemark.查询完成状态帧:
                    if (id == 0x7F4E0FF)
                    {
                        AddLog(System.DateTime.Now.ToString("HH:mm:ss:fff"), id.ToString("X8"), byteToHexString(data));

                        if (data[3] == 0x0)
                        {
                            AddLog(System.DateTime.Now.ToString("HH:mm:ss:fff"), id.ToString("X8"), "已完成本次读取...");
                            isResponse = true;
                            EndTransmit();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private string ToAnalysis(string textStr, byte[] dataBuffer)
        {
            StringBuilder valList = new StringBuilder();
            int index = 0;
            bool toHex = false;

            List<ProtocolModel> _protocols = ToProtocol(textStr);
            for (int i = 0; i < _protocols.Count; i++)
            {
                ProtocolModel model = _protocols[i];
                object val = null;

                if (toHex)
                {
                    switch (model.DataType)
                    {
                        case "U8":
                            val = $"0x{dataBuffer[index++].ToString("X2")}";
                            break;
                        case "U16":
                            byte[] bytes = new byte[2];

                            int byte1 = bytes[0] = dataBuffer[index++];
                            int byte2 = bytes[1] = dataBuffer[index++];
                            val = $"0x{(byte1 + (byte2 << 8)).ToString("X4")}";
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (model.DataType)
                    {
                        case "U8":
                            val = Convert.ToByte(dataBuffer[index++]);
                            break;
                        case "U16":
                            byte[] bytes = new byte[2];

                            int byte1 = bytes[0] = dataBuffer[index++];
                            int byte2 = bytes[1] = dataBuffer[index++];
                            val = byte1 + (byte2 << 8);
                            break;
                        case "I16":
                            string byteStr1 = dataBuffer[index++].ToString("X2");
                            string byteStr2 = dataBuffer[index++].ToString("X2");
                            val = Convert.ToInt16(byteStr2 + byteStr1, 16);
                            break;
                        case "U32":
                            val = (dataBuffer[index++] & 0xff) + (dataBuffer[index++] << 8) + (dataBuffer[index++] << 16) + (dataBuffer[index++] << 24);
                            break;
                        default:
                            break;
                    }

                    model.Value = (model.Accuracy * Convert.ToDouble(val)).ToString();
                }

                valList.Append(model.Value + ",");

                // toHex=true 直接按16进制显示
                if (model.Name == "BMU编号")
                {
                    toHex = true;
                }
                else if (model.Name == "关机请求")
                {
                    toHex = false;
                }
                else if (model.Name== "放电电流上限")
                {
                    toHex = true;
                }
                else if (model.Name== "主动均衡状态")
                {
                    toHex = false;
                }
                else if (model.Name == "其他温度4")
                {
                    toHex = true;
                }
                else if (model.Name== "1-16串均衡状态")
                {
                    toHex = false;
                }
            }

            return valList.ToString();
        }

        private List<ProtocolModel> ToProtocol(string text)
        {
            List<ProtocolModel> protocols = new List<ProtocolModel>();

            string[] textStr = text.Split('\n');

            for (int i = 0; i < textStr.Length; i++)
            {
                string[] datas = textStr[i].Split(',');

                ProtocolModel model = new ProtocolModel();
                model.Name = datas[0];
                model.DataType = datas[1];
                model.DataLength = Convert.ToInt32(datas[2]);
                model.Accuracy = datas[3] == "" ? 1 : Convert.ToDouble(datas[3]);
                model.Unit = datas[4].ToString();

                protocols.Add(model);
            }

            return protocols;
        }

        private void StartTransmit()
        {
            int recount = 0;
            if (!state)
            {
                state = true;
                stepFlag = StepRemark.读文件指令帧;
                _token = new CancellationTokenSource();

                Task.Factory.StartNew(() =>
                {
                    while (!_token.IsCancellationRequested)
                    {
                        switch (stepFlag)
                        {
                            case StepRemark.None:
                                break;
                            case StepRemark.读文件指令帧:
                                if (!ReadFileCommand())
                                {
                                    recount++;
                                    Debug.WriteLine("Read file command:fail!");
                                }
                                break;
                            case StepRemark.读文件数据内容帧:
                                if (questCycle > 1)
                                {
                                    Thread.Sleep(100);
                                    if (!ReadFileDataContent())
                                    {
                                        recount++;
                                        Debug.WriteLine("Read file data content:fail!");
                                    }
                                    else
                                    {
                                        questCycle--;
                                        fileOffset += dataLength;
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("End read file data content...");
                                    stepFlag = StepRemark.查询完成状态帧;
                                }
                                break;
                            case StepRemark.查询完成状态帧:
                                if (QueryCompletionStatus())
                                {
                                    Debug.WriteLine("query status:success!");
                                    EndTransmit();
                                }
                                else
                                {
                                    recount++;
                                }
                                break;
                            default:
                                Debug.WriteLine("error:not normal!");
                                break;
                        }

                        if (recount == 5)
                        {
                            string tag = "";
                            if (stepFlag == StepRemark.读文件指令帧)
                            {
                                tag = "F0";
                            }
                            else if (stepFlag == StepRemark.读文件数据内容帧)
                            {
                                tag = "F1";
                            }
                            else if (stepFlag == StepRemark.查询完成状态帧)
                            {
                                tag = "F4";
                            }

                            AddLog(System.DateTime.Now.ToString("HH:mm:ss:fff"), tag, "终止传输，重试执行5次失败！");
                            recount = 0;
                            EndTransmit();
                            break;
                        }

                        Thread.Sleep(100);
                    }
                });
            }
            else
            {
                stepFlag = StepRemark.读文件指令帧;

                EndTransmit();
            }
        }

        private void EndTransmit()
        {
            //按钮禁用15s
            Stopwatch stopwatch = Stopwatch.StartNew();
            Task.Run(async delegate
            {
                this.Invoke(new Action(() => { btnFileTransmit.Enabled = false; }));
                await Task.Delay(1000 * 15);
                this.Invoke(new Action(() => { btnFileTransmit.Enabled = true; }));
            });

            //关闭线程，清除缓存
            _token.Cancel();
            dataBuffer.Clear();

            state = false;
            isResponse = false;
            stepFlag = StepRemark.None;

            fileNumber = -1;
            readType = -1;

            questCycle = 0;
            fileOffset = 200;
            dataLength = 200;

            fileSize = 0;
            textStr = "";
            headStr = "";
            filePath = "";
            fileName = "";
        }

        private bool ReadFileCommand()
        {
            byte[] canid = { 0xE0, Convert.ToByte(slaveAddress), 0xF0, 0x07 };

            byte[] data = new byte[8];
            data[0] = 0x0;
            data[1] = (byte)slaveAddress;
            data[2] = (byte)fileNumber;
            data[3] = (byte)readType;

            ecanHelper.Send(data, canid);
            AddLog(System.DateTime.Now.ToString("HH:mm:ss:fff"), "F0", byteToHexString(data));
            return CheckResponse();
        }

        private bool ReadFileDataContent()
        {
            byte[] canid = { 0xE0, Convert.ToByte(slaveAddress), 0xF1, 0x07 };

            byte[] data = new byte[8];
            data[0] = 0x0;
            data[1] = (byte)slaveAddress;
            data[2] = (byte)fileNumber;
            data[3] = (byte)(fileOffset & 0xff);
            data[4] = (byte)(fileOffset >> 8);
            data[5] = (byte)(fileOffset >> 16);
            data[6] = (byte)(dataLength & 0xff);
            data[7] = (byte)(dataLength >> 8);

            ecanHelper.Send(data, canid);
            AddLog(System.DateTime.Now.ToString("HH:mm:ss:fff"), "F1", byteToHexString(data));
            return CheckResponse();
        }

        private bool QueryCompletionStatus()
        {
            byte[] canid = { 0xE0, Convert.ToByte(slaveAddress), 0xF4, 0x07 };

            byte[] data = new byte[8];
            data[0] = 0x0;
            data[1] = (byte)slaveAddress;
            data[2] = (byte)fileNumber;
            data[3] = (byte)0x01;

            ecanHelper.Send(data, canid);
            AddLog(System.DateTime.Now.ToString("HH:mm:ss:fff"), "F4", byteToHexString(data));
            return CheckResponse();
        }

        private bool CheckResponse()
        {
            bool isOk = isResponse = false;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {
                if (isResponse)
                {
                    isOk = true;
                    break;
                }

                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    stopwatch.Stop();
                    break;
                }
            }

            return isOk;
        }

        private void AddLog(string date, string id, string context)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = date;
            lvi.SubItems.Add(id);
            lvi.SubItems.Add(context);
            this.Invoke(new Action(() =>
            {
                this.lvPrintBlock.Items.Insert(0, lvi);
            }));
        }

        private string byteToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in data)
            {
                sb.Append(b.ToString("X2") + " ");
            }

            return sb.ToString();
        }

        private void txtSlaveAddress_TextChanged(object sender, EventArgs e)
        {
            slaveAddress = Convert.ToInt32(txtSlaveAddress.Text);
        }

        private void ckReadAll_CheckedChanged(object sender, EventArgs e)
        {
            if (ckReadAll.Checked)
            {
                txtStartLocal.Enabled = false;
                txtReadCount.Enabled = false;
            }
            else
            {
                txtStartLocal.Enabled = true;
                txtReadCount.Enabled = true;
            }
        }
    }

    class ProtocolModel
    {
        public string Name;
        public string DataType;
        public int DataLength;
        public double Accuracy;
        public string Unit;
        public object Value;
    }

    enum StepRemark
    {
        None,
        读文件指令帧,
        读文件数据内容帧,
        查询完成状态帧
    }
}
