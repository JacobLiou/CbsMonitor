using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Model
{
    public class FirmwareModel
    {
        public FirmwareModel()
        {

        }
        public FirmwareModel(string firmwareName, string firmwareType, int startAddress, string firmwareVersion, int length)
        {
            this.FirmwareName = firmwareName;
            this.FirmwareType = firmwareType;
            this.StartAddress = startAddress;
            this.FirmwareVersion = firmwareVersion;
            this.Length = length;
        }

        public string FirmwareName { get; set; }
        public string FirmwareType { get; set; }
        public int StartAddress { get; set; }
        public string FirmwareVersion { get; set; }
        public int Length { get; set; }
        public bool CheckFlg { get; set; }
    }
}
