using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Model
{
    /// <summary>
    /// 协议模型
    /// </summary>
    public class ProtocolModel
    {
        public string Id;
        public string Name;
        public string DataType;
        public string Scaling;
        public string Unit;
        public string Min;
        public string Max;
        public string Default;
        public string Description;
        public string CAN_Byte;
        public string CAN_Byte_Bit;
        public string Remake;
        public string value;//+
    }
}
