using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Model
{
    /// <summary>
    /// 升级存储模型
    /// </summary>
    public class UpgradeModel
    {
        public byte[] FileBuffer = new byte[256];
        public int FileLength { get; set; }
        public int OffsetAddress { get; set; }
        public string FileName { get; set; }
        public int FwObj { get; set; }
        public string ChipCode { get; set; }
    }
}
