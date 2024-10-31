using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SofarBMS.Model;

namespace SofarBMS.Observer
{
    public static class RealtimeDataManager
    {
        private static List<RealtimeDataObserver> observers = new List<RealtimeDataObserver>();
        private static Queue<RealtimeData_BTS5K> QueueDatas = new Queue<RealtimeData_BTS5K>();

        static RealtimeDataManager()
        {
            //定义发送定时器
            var timer1 = new System.Timers.Timer();
            timer1.Interval = 1000;
            timer1.AutoReset = true;
            timer1.Elapsed += Timer1_Elapsed;
            timer1.Start();
        }
        private static void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SaveData();
        }
        public static void Attach(RealtimeDataObserver observer)
        {
            observers.Add(observer);
        }

        public static void WriteData(RealtimeData_BTS5K data)
        {
            QueueDatas.Enqueue(data);
        }

        private static void SaveData()
        {
            try
            {
                RealtimeData_BTS5K data = QueueDatas.Dequeue();

                if (data != null)
                {
                    NotifyAllObservers(data);
                }
            }
            catch (Exception)
            {
            }
        }

        private static void NotifyAllObservers(RealtimeData_BTS5K data)
        {
            foreach (var observer in observers)
            {
                observer.SaveData(data);
            }
        }
    }
}
