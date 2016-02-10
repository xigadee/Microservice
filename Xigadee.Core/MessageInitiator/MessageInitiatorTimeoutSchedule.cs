using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This schedule class is used to record the timeout count statistics.
    /// </summary>
    public class TimeoutSchedule: Schedule
    {
        private long mTimeouts;

        public long Timeouts
        {
            get { return mTimeouts; }
        }

        public TimeoutSchedule(Func<Schedule, CancellationToken, Task> execute, string name = null) 
            : base(execute, name)
        {

        }

        public void TimeoutIncrement(long timeouts)
        {
            Interlocked.Add(ref mTimeouts, timeouts);
        }


    }
}
