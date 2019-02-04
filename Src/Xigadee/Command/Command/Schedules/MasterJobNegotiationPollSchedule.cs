#region using
using System;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This schedule is used by the master job poller.
    /// </summary>
    /// <seealso cref="Xigadee.Schedule" />
    public class MasterJobNegotiationPollSchedule: Schedule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobNegotiationPollSchedule"/> class.
        /// </summary>
        /// <param name="execute">The async schedule function.</param>
        /// <param name="timerConfig">The timer poll configuration.</param>
        /// <param name="name">The masterjob name.</param>
        public MasterJobNegotiationPollSchedule(Func<Schedule, CancellationToken, Task> execute, ScheduleTimerConfig timerConfig, string name = null) 
            : base(execute, timerConfig, name, isLongRunning:false)
        {
        }

    }
}
