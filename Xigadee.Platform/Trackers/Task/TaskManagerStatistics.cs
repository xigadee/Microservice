using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public class TaskManagerStatistics: MessagingStatistics
    {
        public TaskManagerStatistics()
        {
        }

        private long mTimeouts;

        public void TimeoutRegister(long count)
        {
            Interlocked.Add(ref mTimeouts, count);
        }

        public ICpuStats Cpu { get; set; }

        public TaskAvailabilityStatistics Availability { get; set; }

        public string[] Running { get; set; }

        public bool AutotuneActive { get; set; }

        public int TaskCount { get; set; }

        public QueueTrackerStatistics Queues { get; set; }
    }
}
