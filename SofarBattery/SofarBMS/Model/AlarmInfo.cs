using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Model
{
    /// <summary>
    /// 报警记录
    /// </summary>
    public class AlarmInfo
    {
        public int State { get; set; } = 0; //0.异常 1.解除

        public int Id { get; set; }

        public string Type { get; set; }

        public string Content { get; set; }

        public string DataTime { get; set; }
    }
}
