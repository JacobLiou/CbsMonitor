using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Model
{
    public class Protocols
    {
        //XX是代表BMS的编号，XX在前面说明BMS是接收方，XX在最后的字节说明BMS是发送方 LU：定义index为XX的坐标位置
        private int _index;
        private int _id;
        public int Index { get => _index; set => _index = value; }
        public int Id { get => _id; set => _id = value; }

        public Protocols(int index, int id)
        {
            this.Index = index;
            this.Id = id;
        }
    }
}
