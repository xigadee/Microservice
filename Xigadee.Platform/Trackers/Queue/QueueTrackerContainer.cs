using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace Xigadee
{

    [DebuggerDisplay("QueueTracker - Levels: {mLevels} Capacity: {DebugCapacity}")]
    public class QueueTrackerContainer<Q>: ServiceBase<QueueTrackerStatistics>
        where Q : IQueueTracker, new()
    {
        private readonly Dictionary<int, Q> mTasksQueue;

        private readonly int mLevels;

        public QueueTrackerContainer(int levels = 3)
        {
            mLevels = levels;
            mTasksQueue = new Dictionary<int, Q>();
            Enumerable.Range(0, levels).ForEach((i) =>
            {
                Q queue = new Q();
                queue.Statistics.Name = string.Format("Queue {0}", i);
                mTasksQueue.Add(i, queue);
            });
        }

        protected override void StatisticsRecalculate()
        {
            base.StatisticsRecalculate();

            mStatistics.Queues = mTasksQueue.Values.Select((q) => q.Statistics).ToList();
        }

        public bool IsEmpty
        {
            get
            {
                int priority = mLevels;
                while (priority > 0)
                {
                    priority--;
                    if (!mTasksQueue[priority].IsEmpty)
                        return false;
                }
                return true;
            }
        }

        public int Count
        {
            get
            {
                return mTasksQueue.Values.Sum((q) => q.Count);
            }
        }

        public void Enqueue(TaskTracker tracker)
        {
            int priority = tracker.Priority??mLevels - 1;
            var payload = tracker.Context as TransmissionPayload;
            if (payload != null)
            {
                priority = payload.Message.ChannelPriority;
            }

            if (priority > mLevels - 1)
                priority = mLevels - 1;

            if (!tracker.Priority.HasValue)
                tracker.Priority = priority;

            mTasksQueue[priority].Enqueue(tracker);
        }

        public IEnumerable<TaskTracker> Dequeue(int itemCount = 1)
        {
            int priority = mLevels;
            while (itemCount > 0 && priority > 0)
            {
                priority--;
                if (mTasksQueue[priority].IsEmpty)
                    continue;

                TaskTracker tracker;
                while (itemCount > 0 && mTasksQueue[priority].TryDequeue(out tracker))
                {
                    itemCount --;
                    yield return tracker;
                }
            }
        }

        private string DebugCapacity
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                int priority = mLevels;

                while (priority > 0)
                {
                    priority--;
                    sb.Append(mTasksQueue[priority].Count);

                    if (priority>0)
                        sb.Append("/");
                }

                return sb.ToString();
            }
        }

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }
    }
}
