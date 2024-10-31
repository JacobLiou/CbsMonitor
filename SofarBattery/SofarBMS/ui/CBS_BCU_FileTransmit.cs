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
    public partial class CBS_BCU_FileTransmit : UserControl
    {
        public static CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationTokenSource _token = new CancellationTokenSource();
        private EcanHelper ecanHelper = EcanHelper.Instance;

        //变量定义
        private readonly string FiveMinHeadStr = "时间年,月,日,时,分,秒,电池簇累加电压(V),SOC(%),SOH(%),电池簇电流(A),继电器状态,风扇状态,继电器切断请求,BCU状态,充放电使能,保护信息1,保护信息2,保护信息3,保护信息4,告警信息1,告警信息2,最高pack电压(V),最低pack电压(V),最高pack电压序号,最低pack电压序号,簇号,簇内电池包个数,高压绝缘阻抗(V),保险丝后电压(V),母线侧电压(V),负载端电压(V),辅助电源电压(V),电池簇的充电电压(V),电池簇充电电流上限(A),电池簇放电电流上限(A),电池簇的放电截止电压(V),电池包均衡状态,最高功率端子温度(℃),环境温度(℃),累计充电安时(Ah),累计放电安时(Ah),累计充电瓦时(Wh),累计放电瓦时(Wh),BMU编号,单体电压1(mV),单体电压2(mV),单体电压3(mV),单体电压4(mV),单体电压5(mV),单体电压6(mV),单体电压7(mV),单体电压8(mV),单体电压9(mV),单体电压10(mV),单体电压11(mV),单体电压12(mV),单体电压13(mV),单体电压14(mV),单体电压15(mV),单体电压16(mV),PACK1最大单体电压(mV),PACK2最大单体电压(mV),PACK3最大单体电压(mV),PACK4最大单体电压(mV),PACK5最大单体电压(mV),PACK6最大单体电压(mV),PACK7最大单体电压(mV),PACK8最大单体电压(mV),PACK9最大单体电压(mV),PACK10最大单体电压(mV),PACK1最小单体电压(mV),PACK2最小单体电压(mV),PACK3最小单体电压(mV),PACK4最小单体电压(mV),PACK5最小单体电压(mV),PACK6最小单体电压(mV),PACK7最小单体电压(mV),PACK8最小单体电压(mV),PACK9最小单体电压(mV),PACK10最小单体电压(mV),PACK1平均单体电压(mV),PACK2平均单体电压(mV),PACK3平均单体电压(mV),PACK4平均单体电压(mV),PACK5平均单体电压(mV),PACK6平均单体电压(mV),PACK7平均单体电压(mV),PACK8平均单体电压(mV),PACK9平均单体电压(mV),PACK10平均单体电压(mV),PACK1最大单体温度(℃),PACK2最大单体温度(℃),PACK3最大单体温度(℃),PACK4最大单体温度(℃),PACK5最大单体温度(℃),PACK6最大单体温度(℃),PACK7最大单体温度(℃),PACK8最大单体温度(℃),PACK9最大单体温度(℃),PACK10最大单体温度(℃),PACK1最小单体温度(℃),PACK2最小单体温度(℃),PACK3最小单体温度(℃),PACK4最小单体温度(℃),PACK5最小单体温度(℃),PACK6最小单体温度(℃),PACK7最小单体温度(℃),PACK8最小单体温度(℃),PACK9最小单体温度(℃),PACK10最小单体温度(℃),告警信息3,告警信息4,RSV2\r\n";
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
电池簇累加电压,U16,1,0.1,V
电池簇SOC,U8,1,1,%
电池簇SOH,U8,1,1,%
电池簇电流,I16,1,0.01,A
继电器状态,U16,1,1,
风扇状态,U8,1,1,
继电器切断请求,U8,1,1,
BCU状态,U8,1,1,
充放电使能,U16,1,1,
保护信息1,U16,1,,
保护信息2,U16,1,,
保护信息3,U16,1,,
保护信息4,U16,1,,
告警信息1,U16,1,,
告警信息2,U16,1,,
最高pack电压,U16,1,0.1,V
最低pack电压,U16,1,0.1,V
最高pack电压序号,U8,1,1,号
最低pack电压序号,U8,1,1,号
簇号,U8,1,,号
簇内电池包个数,U8,1,1,个
高压绝缘阻抗,U16,1,0.1,V
保险丝后电压,U16,1,0,1,V
母线侧电压,U16,1,0.1,V
负载端电压,U16,1,0.1,V
辅助电源电压,U16,1,0.1,V
电池簇的充电电压,U16,1,0.1,V
电池簇充电电流上限,U16,1,0.1,A
电池簇放电电流上限,U16,1,0.1,A
电池簇的放电截止电压,U16,1,0.1,V
电池包均衡状态,U16,1,,
最高功率端子温度,I16,1,0.1,℃
环境温度,I16,1,0.1,℃
累计放电安时,U32,1,1,Ah
累计充电安时,U32,1,1,Ah
累计放电瓦时,U32,1,1,Wh
累计充电瓦时,U32,1,1,Wh
BMU编号,U8,1,,号
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
PACK1最大单体电压,U16,1,1,mV
PACK2最大单体电压,U16,1,1,mV
PACK3最大单体电压,U16,1,1,mV
PACK4最大单体电压,U16,1,1,mV
PACK5最大单体电压,U16,1,1,mV
PACK6最大单体电压,U16,1,1,mV
PACK7最大单体电压,U16,1,1,mV
PACK8最大单体电压,U16,1,1,mV
PACK9最大单体电压,U16,1,1,mV
PACK10最大单体电压,U16,1,1,mV
PACK1最小单体电压,U16,1,1,mV
PACK2最小单体电压,U16,1,1,mV
PACK3最小单体电压,U16,1,1,mV
PACK4最小单体电压,U16,1,1,mV
PACK5最小单体电压,U16,1,1,mV
PACK6最小单体电压,U16,1,1,mV
PACK7最小单体电压,U16,1,1,mV
PACK8最小单体电压,U16,1,1,mV
PACK9最小单体电压,U16,1,1,mV
PACK10最小单体电压,U16,1,1,mV
PACK1平均单体电压,U16,1,1,mV
PACK2平均单体电压,U16,1,1,mV
PACK3平均单体电压,U16,1,1,mV
PACK4平均单体电压,U16,1,1,mV
PACK5平均单体电压,U16,1,1,mV
PACK6平均单体电压,U16,1,1,mV
PACK7平均单体电压,U16,1,1,mV
PACK8平均单体电压,U16,1,1,mV
PACK9平均单体电压,U16,1,1,mV
PACK10平均单体电压,U16,1,1,mV
PACK1最大单体温度,I8,1,1,℃
PACK2最大单体温度,I8,1,1,℃
PACK3最大单体温度,I8,1,1,℃
PACK4最大单体温度,I8,1,1,℃
PACK5最大单体温度,I8,1,1,℃
PACK6最大单体温度,I8,1,1,℃
PACK7最大单体温度,I8,1,1,℃
PACK8最大单体温度,I8,1,1,℃
PACK9最大单体温度,I8,1,1,℃
PACK10最大单体温度,I8,1,1,℃
PACK1最小单体温度,I8,1,1,℃
PACK2最小单体温度,I8,1,1,℃
PACK3最小单体温度,I8,1,1,℃
PACK4最小单体温度,I8,1,1,℃
PACK5最小单体温度,I8,1,1,℃
PACK6最小单体温度,I8,1,1,℃
PACK7最小单体温度,I8,1,1,℃
PACK8最小单体温度,I8,1,1,℃
PACK9最小单体温度,I8,1,1,℃
PACK10最小单体温度,I8,1,1,℃
告警信息3,U16,1,,
告警信息4,U16,1,,
RSV2,U32,1,,";
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
        int subDeviceAddress = -1;
        int fileNumber = -1;
        int readType = -1;

        int questCycle = 0;
        int fileOffset = 200;
        int dataLength = 200;
        int fileOffset_BCU = 200;
        int readCount = 0;
        int readIndex = 0;

        int fileSize = 0;
        string textStr = "";
        string headStr = "";
        string filePath = "";
        string fileName = "";
        List<byte> dataBuffer = new List<byte>();


        public CBS_BCU_FileTransmit()
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
            this.txtSlaveAddress.Text = FrmMain.BCU_ID.ToString();
            this.txtSubDeviceAddress.Text = "0";
            this.cbbFileNumber.SelectedIndex = 3;
            this.cbbModeName.SelectedIndex = 0;
            this.cbbModeName.Enabled = false;
            this.ckReadAll.Checked = true;

            //slaveAddress = Convert.ToInt32(txtSlaveAddress.Text);
            //subDeviceAddress= Convert.ToInt32(txtSubDeviceAddress.Text);
            //readCount = Convert.ToInt32(txtReadCount.Text);
        }

        private void btnFileTransmit_Click(object sender, EventArgs e)
        {
            fileNumber = cbbFileNumber.SelectedIndex;//确认解析点表
            readType = ckReadAll.Checked ? 0 : 1;

            //if (readType == 1)
            //{
            //    if (!int.TryParse(txtStartLocal.Text, out readIndex))
            //    {
            //        MessageBox.Show("起始地址错误，终止操作！");
            //        return;
            //    }

            //    fileOffset *= readIndex;//变更起始偏移地址
            //}

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
                                fileName = string.Format("{0}_{1}_{2}_{3}", headName, cbbModeName.Text.Trim(), subDeviceAddress, System.DateTime.Now.ToString("yy-MM-dd-HH-mm-ss"));
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



                                if (readType == 0)
                                {
                                    fileOffset_BCU = fileOffset;
                                    questCycle = fileSize % fileOffset == 0 ? fileSize / fileOffset + 1 : (fileSize / fileOffset);//一般结果为true
                                    //questCycle = fileSize % fileOffset == 0 ? (fileSize / fileOffset - 1) : fileSize / fileOffset;//一般结果为true

                                }
                                else
                                {
                                    fileOffset_BCU = ((fileSize - readCount * fileOffset) < 0) ? fileOffset : (fileSize - readCount * fileOffset);
                                    questCycle = (fileSize - fileOffset_BCU) % fileOffset == 0 ? ((fileSize - fileOffset_BCU) / fileOffset) + 1 : ((fileSize - fileOffset_BCU) / fileOffset);

                                }


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
                if (model.Name == "电池簇电流")
                {
                    toHex = true;
                }
                else if (model.Name == "告警信息2")
                {
                    toHex = false;
                }
                else if (model.Name == "PACK10最小单体温度")
                {
                    toHex = true;
                }
                else if (model.Name == "告警信息4")
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
                                        fileOffset_BCU += dataLength;
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

                        Thread.Sleep(20);
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

            //fileNumber = -1;
            //readType = -1;

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
            data[1] = (byte)subDeviceAddress;
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
            data[1] = (byte)subDeviceAddress;
            data[2] = (byte)fileNumber;
            data[3] = (byte)(fileOffset_BCU & 0xff);
            data[4] = (byte)(fileOffset_BCU >> 8);
            data[5] = (byte)(fileOffset_BCU >> 16);
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
            data[1] = (byte)subDeviceAddress;
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

        private void txtSubDeviceAddress_TextChanged(object sender, EventArgs e)
        {
            subDeviceAddress = Convert.ToInt32(txtSubDeviceAddress.Text);
        }

        private void txtReadCount_TextChanged(object sender, EventArgs e)
        {
            readCount = Convert.ToInt32(txtReadCount.Text);
        }
    }

    //class ProtocolModel
    //{
    //    public string Name;
    //    public string DataType;
    //    public int DataLength;
    //    public double Accuracy;
    //    public string Unit;
    //    public object Value;
    //}

    //enum StepRemark
    //{
    //    None,
    //    读文件指令帧,
    //    读文件数据内容帧,
    //    查询完成状态帧
    //}
}
