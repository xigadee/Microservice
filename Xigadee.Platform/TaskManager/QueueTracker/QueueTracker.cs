#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class tracks any jobs that are enqueued and records how long they are held in the queue.
    /// </summary>
    [DebuggerDisplay("Queue: {mQueue.Count}")]
    public class QueueTracker: StatisticsBase<MessagingStatistics>, IQueueTracker
    {
        ConcurrentQueue<KeyValuePair<TaskTracker, int>> mQueue;

        public QueueTracker()
        {
            mQueue = new ConcurrentQueue<KeyValuePair<TaskTracker, int>>();
        }

        public void Enqueue(TaskTracker item)
        {
            var wrapper = new KeyValuePair<TaskTracker, int>(item, StatisticsInternal.ActiveIncrement());
            mQueue.Enqueue(wrapper);
        }

        public bool IsEmpty
        {
            get { return mQueue.IsEmpty; }
        }

        public int Count
        {
            get
            {
                return mQueue.Count;
            }
        }

        public bool TryDequeue(out TaskTracker item)
        {
            item = null;
            KeyValuePair<TaskTracker, int> wrapper;
            if (mQueue.TryDequeue(out wrapper))
            {
                item = wrapper.Key;
                StatisticsInternal.ActiveDecrement(wrapper.Value);
                return true;
            }
            return false;
        }

        protected override void StatisticsRecalculate(MessagingStatistics stats)
        {
        }
    }
}
