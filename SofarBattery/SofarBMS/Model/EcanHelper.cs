using SofarBMS.Helper;
using SofarBMS.Queue;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SofarBMS.Model
{
    public class EcanHelper
    {
        private static EcanHelper instance = null;
        private static readonly object obj = new object();

        private EcanHelper() { }
        public static EcanHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new EcanHelper();
                        }
                    }
                }
                return instance;
            }
        }

        public const int REC_MSG_BUF_MAX = 0x2710;

        public CAN_OBJ[] gRecMsgBuf = new CAN_OBJ[REC_MSG_BUF_MAX];
        public uint gRecMsgBufHead = 0;
        public uint gRecMsgBufTail = 0;

        public const int SEND_MSG_BUF_MAX = 0x2710;

        public CAN_OBJ[] gSendMsgBuf;
        public uint gSendMsgBufHead;
        public uint gSendMsgBufTail;

        /*创建一个更新收发数据显示的线程*/
        public readonly static object _locker = new object();
        public static Queue<CAN_OBJ> _task = new Queue<CAN_OBJ>();
        public ConcurrentDictionary<int, Byte[]> Devices = new ConcurrentDictionary<int, Byte[]>();

        public static EventWaitHandle _wh = new AutoResetEvent(false);
        public static List<Protocols> protocols = new List<Protocols>() {
            new Protocols(3,0x1020FFFF),new Protocols(3,0x1020E0FF)
            ,new Protocols(3,0x07F0E0FF),new Protocols(3,0x07F1E0FF),new Protocols(3,0x07F2E0FF),new Protocols(3,0x07F3E0FF),new Protocols(3,0x07F4E0FF)

            ,new Protocols(3,0x1003E0FF), new Protocols(3,0x1004E0FF),new Protocols(3,0x1005E0FF),new Protocols(3,0x1006E0FF),new Protocols(3,0x1007E0FF),new Protocols(3,0x1008E0FF),new Protocols(3,0x1009E0FF),new Protocols(3,0x100AE0FF),new Protocols(3,0x100BE0FF),new Protocols(3,0x100CE0FF),new Protocols(3,0x100DE0FF),new Protocols(3,0x100EE0FF),new Protocols(3,0x100FE0FF)
            ,new Protocols(3,0x1040E0FF),new Protocols(3,0x1041E0FF),new Protocols(3,0x1042E0FF), new Protocols(3,0x1043E0FF),new Protocols(3,0x1044E0FF),new Protocols(3,0x104AE0FF),new Protocols(3,0x104EE0FF),new Protocols(3,0x104E0FFF),new Protocols(3,0x1045E0FF),new Protocols(3,0x1046E0FF),new Protocols(3,0x1047E0FF),new Protocols(3,0x1048E0FF),new Protocols(3,0x1049E0FF)

            ,new Protocols(3,0x1003FFFF), new Protocols(3,0x1004FFFF),new Protocols(3,0x1005FFFF),new Protocols(3,0x1006FFFF),new Protocols(3,0x1007FFFF),new Protocols(3,0x1008FFFF),new Protocols(3,0x1009FFFF),new Protocols(3,0x100AFFFF),new Protocols(3,0x100BFFFF),new Protocols(3,0x100CFFFF),new Protocols(3,0x100DFFFF),new Protocols(3,0x100EFFFF),new Protocols(3,0x100FFFFF)
            ,new Protocols(3,0x1040FFFF),new Protocols(3,0x1041FFFF),new Protocols(3,0x1042FFFF), new Protocols(3,0x1043FFFF),new Protocols(3,0x1044FFFF),new Protocols(3,0x104AFFFF),new Protocols(3,0x104EFFFF),new Protocols(3,0x104FFFFF),new Protocols(3,0x1045FFFF),new Protocols(3,0x1046FFFF),new Protocols(3,0x1047FFFF),new Protocols(3,0x1048FFFF),new Protocols(3,0x1049FFFF)

            ,new Protocols(3,0x106AE0FF),new Protocols(3,0x106BE0FF),new Protocols(3,0x106CE0FF),new Protocols(3,0x106DE0FF)
            ,new Protocols(3,0x106AFFFF),new Protocols(3,0x106BFFFF),new Protocols(3,0x106CFFFF),new Protocols(3,0x106DFFFF)
            ,new Protocols(2,0x102FFFE0)
            ,new Protocols(3,0x1030FFFF),new Protocols(3,0x1031FFFF),new Protocols(3,0x1033FFFF),new Protocols(3,0x1034FFFF),new Protocols(3,0x1035FFFF),new Protocols(3,0x1036FFFF),new Protocols(3,0x1037FFFF),new Protocols(3,0x1038FFFF),new Protocols(3,0x1039FFFF),new Protocols(3,0x103AFFFF),new Protocols(3,0x103BFFFF),new Protocols(3,0x103CFFFF),new Protocols(3,0x103DFFFF),new Protocols(3,0x103EFFFF),new Protocols(3,0x103FFFFF),new Protocols(3,0x1050FFFF),new Protocols(3,0x1051FFFF),new Protocols(3,0x102DFFFF)
            ,new Protocols(3,0x1030E0FF),new Protocols(3,0x1031E0FF),new Protocols(3,0x1033E0FF),new Protocols(3,0x1034E0FF),new Protocols(3,0x1035E0FF),new Protocols(3,0x1036E0FF),new Protocols(3,0x1037E0FF),new Protocols(3,0x1038E0FF),new Protocols(3,0x1039E0FF),new Protocols(3,0x103AE0FF),new Protocols(3,0x103BE0FF),new Protocols(3,0x103CE0FF),new Protocols(3,0x103DE0FF),new Protocols(3,0x103EE0FF),new Protocols(3,0x103FE0FF),new Protocols(3,0x1050E0FF),new Protocols(3,0x1051E0FF),new Protocols(3,0x102DE0FF)
            
            ,new Protocols(3,0x1010E0FF),new Protocols(3,0x1011E0FF),new Protocols(3,0x1012E0FF),new Protocols(3,0x1013E0FF),new Protocols(3,0x1014E0FF),new Protocols(3,0x1015E0FF),new Protocols(3,0x1016E0FF),new Protocols(3,0x1017E0FF),new Protocols(3,0x1018E0FF),new Protocols(3,0x1019E0FF),new Protocols(3,0x101AE0FF),new Protocols(3,0x101BE0FF),new Protocols(3,0x101CE0FF)
            ,new Protocols(3,0x1021E0FF),new Protocols(3,0x1022E0FF),new Protocols(3,0x1023E0FF),new Protocols(3,0x1024E0FF),new Protocols(3,0x1025E0FF),new Protocols(3,0x1026E0FF),new Protocols(3,0x1027E0FF),new Protocols(3,0x1028E0FF),new Protocols(3,0x1029E0FF),new Protocols(3,0x102AE0FF),new Protocols(3,0x102EE0FF),new Protocols(3,0x102FE0FF),new Protocols(3,0x101EE0FF)

            ,new Protocols(3,0x1010FFFF),new Protocols(3,0x1011FFFF),new Protocols(3,0x1012FFFF),new Protocols(3,0x1013FFFF),new Protocols(3,0x1014FFFF),new Protocols(3,0x1015FFFF),new Protocols(3,0x1016FFFF),new Protocols(3,0x1017FFFF),new Protocols(3,0x1018FFFF),new Protocols(3,0x1019FFFF),new Protocols(3,0x101AFFFF),new Protocols(3,0x101BFFFF),new Protocols(3,0x101CFFFF)
            ,new Protocols(3,0x1021FFFF),new Protocols(3,0x1022FFFF),new Protocols(3,0x1023FFFF),new Protocols(3,0x1024FFFF),new Protocols(3,0x1025FFFF),new Protocols(3,0x1026FFFF),new Protocols(3,0x1027FFFF),new Protocols(3,0x1028FFFF),new Protocols(3,0x1029FFFF),new Protocols(3,0x102AFFFF),new Protocols(3,0x102EFFFF),new Protocols(3,0x102FFFFF),new Protocols(3,0x101EFFFF)

            ,new Protocols(3,0x07FAE0FF),new Protocols(3,0x07FBE0FF),new Protocols(3,0x07FCE0FF),new Protocols(3,0x07FDE0FF),new Protocols(3,0x07FEE0FF),new Protocols(3,0x07FFE0FF)
            ,new Protocols(3,0x07FB41FF),new Protocols(3,0x07FC41FF),new Protocols(3,0x07FD41FF),new Protocols(3,0x07FE41FF),new Protocols(3,0x07FF41FF),new Protocols(3,0x07FF5FFF)
            ,new Protocols(3,0xB605FFF),new Protocols(3,0xB615FFF),new Protocols(3,0xB625FFF),new Protocols(3,0xB635FFF),new Protocols(3,0xB665FFF),new Protocols(3,0x0B70E0FF),new Protocols(3,0x0B71E0FF),new Protocols(3,0x0B72E0FF),new Protocols(3,0x0B73E0FF),new Protocols(3,0x0B74E0FF),new Protocols(3,0x0B75E0FF),new Protocols(3,0x0B76E0FF),new Protocols(3,0x0B77E0FF),new Protocols(3,0x0B78E0FF),new Protocols(3,0x0B6A5FFF)
            ,new Protocols(3,0xB70E0FF),new Protocols(3,0xB71E0FF),new Protocols(3,0xB72E0FF),new Protocols(3,0xB73E0FF),new Protocols(3,0xB74E0FF),new Protocols(3,0xB76E0FF),new Protocols(3,0xB77E0FF),new Protocols(3,0x0B78E0FF),new Protocols(3,0x0B6A5FFF)
            ,new Protocols(3,0x1403FFFF),new Protocols(3,0x1400E0FF)
            ,new Protocols(3,0x1060FFFF),new Protocols(3,0x1060FF1F),new Protocols(3, 0x1061FFFF),new Protocols(3,0x1061FF1F)
            ,new Protocols(3,0x10B6E0FF),new Protocols(3,0x10B7E0FF),new Protocols(3,0x10B8E0FF),new Protocols(3,0x10B9E0FF),new Protocols(3,0x10BAE0FF),new Protocols(3,0x10BBE0FF),new Protocols(3,0x10BCE0FF),new Protocols(3,0x10BDE0FF),new Protocols(3,0x10BEE0FF),new Protocols(3,0x10BFE0FF),new Protocols(3,0x10C0E0FF),new Protocols(3,0x10C1E0FF),new Protocols(3,0x10C2E0FF),new Protocols(3,0x10C3E0FF),new Protocols(3,0x10C4E0FF),new Protocols(3,0x10C5E0FF),new Protocols(3,0x10C6E0FF)
            ,new Protocols(3,0x10E0E0FF),new Protocols(3,0x10E1E0FF),new Protocols(3,0x10E2E0FF),new Protocols(3,0x10E3E0FF),new Protocols(3,0x10E4E0FF),new Protocols(3,0x10E5E0FF),new Protocols(3,0x10EFE0FF),new Protocols(3,0x10F0E0FF),new Protocols(3,0x10F1E0FF),new Protocols(3,0x10F2E0FF),new Protocols(3,0x10F3E0FF),new Protocols(3,0x10F4E0FF),new Protocols(3,0x10F5E0FF),new Protocols(3,0x10F6E0FF),new Protocols(3,0x10F7E0FF),new Protocols(3,0x10F8E0FF),new Protocols(3,0x10F9E0FF),new Protocols(3,0x10FAE0FF)
        };
        public bool IsConnection { get; set; }
        private int Can_error_count = 0;

        public delegate void ReceiveInfo(CAN_OBJ _obj);
        public event ReceiveInfo ReceivecEventHandler;
        public bool StartListen { get; set; }

        public bool Send(Byte[] data, byte[] canid)
        {
            gSendMsgBuf = new CAN_OBJ[SEND_MSG_BUF_MAX];
            gSendMsgBufHead = 0;
            gSendMsgBufTail = 0;

            CAN_OBJ co = new CAN_OBJ();
            co.SendType = 0;
            co.DataLen = 8;
            co.Data = data;
            co.ID = BitConverter.ToUInt32(canid, 0);

            gSendMsgBuf[gSendMsgBufHead].ID = co.ID;
            gSendMsgBuf[gSendMsgBufHead].Data = co.Data;
            gSendMsgBuf[gSendMsgBufHead].DataLen = co.DataLen;
            gSendMsgBuf[gSendMsgBufHead].ExternFlag = 1;
            gSendMsgBuf[gSendMsgBufHead].RemoteFlag = 0;
            gSendMsgBufHead++;
            if (gSendMsgBufHead >= SEND_MSG_BUF_MAX)
            {
                gSendMsgBufHead = 0;
            }

            CAN_OBJ[] coMsg = new CAN_OBJ[2];

            if (gSendMsgBufHead != gSendMsgBufTail)
            {
                coMsg[0] = gSendMsgBuf[gSendMsgBufTail];
                coMsg[1] = gSendMsgBuf[gSendMsgBufTail];
                gSendMsgBufTail++;

                if (gSendMsgBufTail >= SEND_MSG_BUF_MAX)
                {
                    gSendMsgBufTail = 0;
                }

                if (ECANHelper.Transmit(1, 0, 0, coMsg, 1) == ECANStatus.STATUS_OK)
                {
                    CAN_ERR_INFO err_info = new CAN_ERR_INFO();
                    var v = ECANHelper.ReadErrInfo(1, 0, 0, out err_info) == ECANStatus.STATUS_OK;
                    if (err_info.ErrCode == 0x00)//成功
                    {
                        Debug.WriteLine($"{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")} 发送数据   帧ID:{co.ID.ToString("X8")}");
                        return true;
                    }
                    else if (err_info.ErrCode == 0x400)
                    {
                        Debug.WriteLine($"{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}-->发送失败");
                    }
                }
            }

            return false;
        }

        public void Receive()
        {
            ReceivecEventHandler += EnqueueTask;
            ReceivecEventHandler += EnqueueTask_MLQ;

            Task.Run(() =>
            {
                while (true)
                {
                    CAN_OBJ coMsg = new CAN_OBJ();

                    if (ECANHelper.Receive(1, 0, 0, out coMsg, 1, 1) == ECANStatus.STATUS_OK)
                    {
                        gRecMsgBuf[gRecMsgBufHead].ID = coMsg.ID;
                        gRecMsgBuf[gRecMsgBufHead].Data = coMsg.Data;
                        gRecMsgBuf[gRecMsgBufHead].DataLen = coMsg.DataLen;
                        gRecMsgBuf[gRecMsgBufHead].ExternFlag = coMsg.ExternFlag;
                        gRecMsgBuf[gRecMsgBufHead].RemoteFlag = coMsg.RemoteFlag;
                        gRecMsgBuf[gRecMsgBufHead].TimeStamp = coMsg.TimeStamp;
                        gRecMsgBuf[gRecMsgBufHead].Reserved = coMsg.Reserved;
                        gRecMsgBuf[gRecMsgBufHead].TimeFlag = coMsg.TimeFlag;
                        gRecMsgBufHead += 1;
                        if (gRecMsgBufHead >= REC_MSG_BUF_MAX)
                        {
                            gRecMsgBufHead = 0;
                        }

                        //进入队列前，先进行筛选（集合内的ID可加入至队列，否则过滤掉）
                        foreach (Protocols item in protocols)
                        {
                            //uint index = 0x00;
                            //switch (item.Index)
                            //{
                            //    case 0: index = 0xff000000; break;
                            //    case 1: index = 0xff0000; break;
                            //    case 2: index = 0xff00; break;
                            //    case 3: index = 0xff; break;
                            //}

                            uint revId = coMsg.ID | 0xff;
                            uint devId = AnalysisID(coMsg.ID);
                            if (revId == item.Id)
                            {

                                //EnqueueTask(coMsg);
                                ReceivecEventHandler(coMsg);
                                break;
                            }
                        }
                    }
                }
            });
        }

        public uint AnalysisID(uint id)
        {
            // SA源地址（bit0~bit7）
            uint sa = (id & 0xFF);

            // TA目标地址（bit8~bit15）
            uint ta = ((id >> 8) & 0xFF);

            // PF功能码(bit16~bit26)
            uint pf = ((id >> 16) & 0x7FF);

            // PR优先级(bit27~bit28)
            uint pr = ((id >> 27) & 0x3);

            // R预留(bit29~31)
            uint r = ((id >> 29) & 0x7);

            // 打印每行数据
            //Debug.WriteLine($"0x{id:X},{sa:X},{ta:X},{pf:X},{pr:X},{r:X}");
            return sa;
        }

        public string ReadError()
        {
            string error = string.Empty;

            CAN_ERR_INFO err_info = new CAN_ERR_INFO();

            if (ECANHelper.ReadErrInfo(1, 0, 0, out err_info) == ECANStatus.STATUS_OK)
            {
                error = "当前错误码：" + err_info.ErrCode.ToString("X2");

                if (err_info.ErrCode >= 0x01 && err_info.ErrCode < 0x100)
                {
                    if (Can_error_count < 10)
                    {
                        Can_error_count++;
                    }
                    else
                    {
                        if (ECANHelper.ResetCAN(1, 0, 0) == ECANStatus.STATUS_OK)
                        {
                            Can_error_count = 0;
                            ECANHelper.StartCAN(1, 0, 0);
                            Debug.WriteLine($"当前错误码：{0}，执行了复位CAN操作");
                        }
                    }
                }
                else { Can_error_count = 0; }
            }
            else
            {
                error = "Read_Error Fault";
            }

            return error;
        }

        private void EnqueueTask(CAN_OBJ CANOBJ)
        {
            lock (_locker)
            {
                //测试打印接收报文
                Debug.WriteLine($"{System.DateTime.Now.ToString("hh:mm:ss:fff")} 入队数据   帧ID:{CANOBJ.ID.ToString("X8")}");

                _task.Enqueue(CANOBJ);
                _wh.Set();
            }
        }

        private void EnqueueTask_MLQ(CAN_OBJ coMsg)
        {
            //未启动监听，终止入队操作
            if (!StartListen)
                return;

            int devId = (int)coMsg.ID;
            if (!Devices.TryAdd(devId, coMsg.Data))
            {
                Devices[devId] = coMsg.Data;
            }

            string ss = "";
            for (int i = 0; i < coMsg.Data.Length; i++)
            {
                ss += " " + coMsg.Data[i].ToString("X2");
            }
            
            //Debug.WriteLine($"{System.DateTime.Now.ToString("hh:mm:ss:fff")} Dev:{devId} CAN_ID:{coMsg.ID.ToString("X8")},Data：{ss.ToString()}");

        }
    }
}
