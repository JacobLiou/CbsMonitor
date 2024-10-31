using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Model
{
    public class FaultInfo
    {
        private string _content;
        private int _byte;
        private int _bit;
        private int _value;
        private int _state;
        private int _type;
        public FaultInfo(string content,int canbyte,int canbit, int value, int state,int type)
        {
            this._content = content;
            this._value = value;
            this._state = state;
            this.Byte = canbyte;
            this.Bit = canbit;
            this._type = type;
        }
        public string Content { get => _content; set => _content = value; }
        public int Type { get => _type; set => _type = value; }
        public int Value { get => _value; set => _value = value; }
        public int State { get => _state; set => _state = value; }
        public int Byte { get => _byte; set => _byte = value; }
        public int Bit { get => _bit; set => _bit = value; }
    }
}
