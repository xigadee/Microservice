using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This schedule class is used to record the timeout count statistics.
    /// </summary>
    public class CommandTimeoutSchedule: CommandSchedule
    {
        private long mTimeouts;

        public CommandTimeoutSchedule(Func<Schedule, CancellationToken, Task> execute, CommandTimerPoll timerConfig, string name = null)
            : base(execute, timerConfig, name)
        {
        }

        public long Timeouts
        {
            get { return mTimeouts; }
        }

        public void TimeoutIncrement(long timeouts)
        {
            Interlocked.Add(ref mTimeouts, timeouts);
        }
    }

    public class CommandSchedule: Schedule
    {
        public CommandSchedule(Func<Schedule, CancellationToken, Task> execute, CommandTimerPoll timerConfig, string name = null)
            : base(execute, name)
        {
            base.Frequency = timerConfig.Interval;
            base.InitialTime = timerConfig.InitialWaitUTCTime;
            base.InitialWait = timerConfig.InitialWait;
        }
    }
}
