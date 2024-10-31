using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SofarBMS.Model
{
    public class BaseModel
    {
        protected ushort startaddress = 0x00;
        protected ushort numbeforePoints = 0x00;
        protected string csv = "";

        public ArrayList alldataArray = new ArrayList();
        public ArrayList readdataArray = new ArrayList();
        public ArrayList writedataArray = new ArrayList();
        public List<ProtocolModel> protocol = new List<ProtocolModel>();
        public ArrayList dataArray = new ArrayList();

        public BaseModel()
        {
            csvToProtocol();
        }

        /// <summary>
        /// CSV转协议类信息
        /// </summary>
        public void csvToProtocol()
        {
            if (csv.Length > 0)
            {
                string[] csv_all = csv.Split("\n".ToCharArray());
                foreach (string str in csv_all)
                {
                    string[] strs = str.Split(',');
                    ProtocolModel model = new ProtocolModel();
                    for (int i = 0; i < strs.Length; i++)
                    {
                        model.Id = strs[0].ToString();
                        model.Name = strs[1].ToString();
                        model.DataType = strs[2].ToString();
                        model.Scaling = strs[3].ToString();
                        model.Unit = strs[4].ToString();
                        model.Min = strs[5].ToString();
                        model.Max = strs[6].ToString();
                        model.Default = strs[7].ToString();
                        model.Description = strs[8].ToString();
                        model.CAN_Byte = strs[9].ToString();
                        model.CAN_Byte_Bit = strs[10].ToString();
                        model.Remake = strs[11].ToString();
                        model.value = "0";
                    }
                    this.protocol.Add(model);
                }
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        public virtual bool ReadMsg()
        {
            return EcanHelper.Send(new byte[] { }, new byte[] { });
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <returns></returns>
        public virtual bool WriteMsg()
        {
            return EcanHelper.Send(new byte[] { }, new byte[] { });
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void reloadData(byte[] buffer)
        {
            this.dataArray.Clear();

            for (int i = 0; i < this.protocol.Count; i++)
            {
                ProtocolModel model = this.protocol[i];
                if (model.DataType == "U16")
                {

                }
                else if (model.DataType == "I16")
                {

                }
            }
        }
    }

   

    /// <summary>
    /// 实时数据模块
    /// </summary>
    public class RealtimeModel : BaseModel
    {
        public RealtimeModel()
        {

        }
    }

    public class FaultModel : BaseModel
    {
        public FaultModel()
        {
            this.startaddress = 0x01;
            this.csv = @"0x1010E0FF,单体过充保护,U16,1,mV,0,65535,,,[0:1],,txt_1
0x1010E0FF,单体过充保护解除,U16,1,mV,0,65535,,,[2:3],,txt_2
0x1010E0FF,单体过充告警,U16,1,mV,0,65535,,,[4:5],,txt_3
0x1010E0FF,单体过充告警解除,U16,1,mV,0,65535,,,[6:7],,txt_4
0x1011E0FF,总体过充保护,U16,1,mV,0,65535,,,[0:1],,txt_5
0x1011E0FF,总体过充保护解除,U16,1,mV,0,65535,,,[2:3],,txt_6
0x1011E0FF,总体过充告警,U16,1,mV,0,65535,,,[4:5],,txt_7
0x1011E0FF,总体过充告警解除,U16,1,mV,0,65535,,,[6:7],,txt_8
0x1012E0FF,单体过放保护,U16,1,mV,0,65535,,,[0:1],,txt_5
0x1012E0FF,单体过放保护解除,U16,1,mV,0,65535,,,[2:3],,txt_6
0x1012E0FF,单体过放告警,U16,1,mV,0,65535,,,[4:5],,txt_7
0x1012E0FF,单体过放告警解除,U16,1,mV,0,65535,,,[6:7],,txt_8";
        }
    }
}
