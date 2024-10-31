using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SofarBMS.Queue
{
    public class MultiLevelQueueManager
    {
        public ConcurrentDictionary<int, BlockingCollection<QueueItem>> _queues;

        public MultiLevelQueueManager()
        {
            _queues = new ConcurrentDictionary<int, BlockingCollection<QueueItem>>();
        }

        public void Enqueue(QueueItem item)
        {
            //确保队列存在
            if (!_queues.ContainsKey(item.Priority))
            {
                _queues[item.Priority] = new BlockingCollection<QueueItem>();
            }

            //添加项到队列
            _queues[item.Priority].Add(item);
        }

        public BlockingCollection<QueueItem> GetQueue(int priority)
        {
            if (_queues.TryGetValue(priority, out BlockingCollection<QueueItem> queue))
            {
                return queue;
            }

            return null;
        }

        public void Dequeue()
        {
            _queues.Clear();

            _queues = new ConcurrentDictionary<int, BlockingCollection<QueueItem>>();
        }
    }
}
