using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SofarBMS.Model;

namespace SofarBMS.Observer
{
    public abstract class RealtimeDataObserver
    {
        public abstract void SaveData(RealtimeData_BTS5K data);
    }
}
