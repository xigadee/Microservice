using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This schedule class is used to record the timeout count statistics.
    /// </summary>
    public class CommandTimeoutSchedule: CommandJobSchedule
    {
        #region Declarations
        private long mTimeouts = 0;
        #endregion        
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="execute">The function to execute.</param>
        /// <param name="timerConfig">The timer configuration.</param>
        /// <param name="name">The name.</param>
        /// <param name="executionPriority">This is the priority that the timeout job should run as.</param>
        public CommandTimeoutSchedule(Func<Schedule, CancellationToken, Task> execute
            , ScheduleTimerConfig timerConfig
            , string name
            , int? executionPriority)
            : base(execute, timerConfig, name, executionPriority: executionPriority)
        {
        }
        #endregion

        #region Timeouts
        /// <summary>
        /// This is the total count of the number of timeouts.
        /// </summary>
        public long Timeouts
        {
            get { return mTimeouts; }
        }
        #endregion
        #region TimeoutIncrement(long timeouts)
        /// <summary>
        /// This method increments the number of timeouts atomically.
        /// </summary>
        /// <param name="timeouts"></param>
        public void TimeoutIncrement(long timeouts)
        {
            Interlocked.Add(ref mTimeouts, timeouts);
        }
        #endregion

        /// <summary>
        /// This is the debug message used for the statistics.
        /// </summary>
        public override string Debug
        {
            get
            {
                return $"CommandTimeout=[{CommandId.ToString("N")}]:'{Name ?? Id.ToString("N").ToUpperInvariant()}' Active={Active} [ShouldExecute={ShouldExecute}] @ {NextExecuteTime} Run={ExecutionCount}";
            }
        }
    }
}
