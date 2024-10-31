using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Queue
{
    public class QueueItem
    {
        public QueueItem(int priority, object data)
        {
            Priority = priority;
            Data = data;
        }

        public int Priority { get; set; }//key优先级，设备ID
        public object Data { get; set; }
    }
}
