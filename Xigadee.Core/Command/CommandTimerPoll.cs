using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the timer poll configuration used by several components in the command policy.
    /// </summary>
    public class CommandTimerPoll
    {
        public CommandTimerPoll()
        {
            Interval = TimeSpan.FromSeconds(5);
            InitialWait = TimeSpan.FromSeconds(10);
            InitialWaitUTCTime = null;
        }

        public CommandTimerPoll(TimeSpan interval, TimeSpan? initialWait = null, DateTime? initialWaitUTCTime = null)
        {
            Interval = interval;
            InitialWait = initialWait ?? TimeSpan.FromSeconds(10);
            InitialWaitUTCTime = initialWaitUTCTime;
        }

        /// <summary>
        /// This is the poll interval. The default is 5 seconds.
        /// </summary>
        public virtual TimeSpan? Interval { get; set; }
        /// <summary>
        /// This is the initial wait time before polling begins, the default is 10 seconds.
        /// </summary>
        public virtual TimeSpan? InitialWait { get; set; }
        /// <summary>
        /// This is the initial UTC time that polling should begin if the initial wait is not set. 
        /// </summary>
        public virtual DateTime? InitialWaitUTCTime { get; set; }
    }
}
