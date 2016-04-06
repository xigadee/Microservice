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


        public int Active { get; set; }


        public int SlotsAvailable { get; set; }

        public int Killed { get; set; }

        public long KilledDidReturn { get; set; }

        public long KilledTotal { get; set; }

        public string[] Levels { get; set; }

        public string[] Running { get; set; }

        public bool AutotuneActive { get; set; }

        public int TasksMaxConcurrent { get; set; }

        public int OverloadTasksConcurrent { get; set; }

        public QueueTrackerStatistics Queues { get; set; }
    }
}
