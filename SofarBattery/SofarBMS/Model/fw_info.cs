using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Model
{
    public class fw_info
    {
        public ushort cmd_version;                  //协议版本号
        public byte[] check_Sum = new byte[4];      //校验和
        public byte[] crc32 = new byte[4];          //CRC校验值
        public byte[] mcu_type = new byte[30];      //芯片型号,字符串
        public byte[] fw_version = new byte[20];    //软件版本,字符串
        public byte[] hw_version = new byte[20];    //硬件版本,字符串
        public byte[] proj_name = new byte[30];     //工程名称,字符串
        public byte[] build_time = new byte[12];    //生成日期,字符串
        public byte[] app_start_addr = new byte[4]; //程序起始地址
        public int fw_obj;                          //升级对象
        public byte[] chip_code = new byte[2];      //芯片代号
        public int bms_obj;                         //BMS对象
        public byte[] resvd = new byte[126];        //保留
    }
}
