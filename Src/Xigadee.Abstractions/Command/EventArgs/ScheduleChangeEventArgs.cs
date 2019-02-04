using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used to signal a change to a job or a masterjob schedule.
    /// </summary>
    /// <seealso cref="Xigadee.CommandEventArgsBase" />
    public class ScheduleChangeEventArgs: CommandEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleChangeEventArgs"/> class.
        /// </summary>
        /// <param name="isRemoval">True if the schedule is being removed.</param>
        /// <param name="schedule">The schedule.</param>
        public ScheduleChangeEventArgs(bool isRemoval, CommandJobSchedule schedule)
        {
            IsRemoval = isRemoval;
            Schedule = schedule;
        }
        /// <summary>
        /// Gets a value indicating whether this instance is a removal.
        /// </summary>
        public bool IsRemoval { get; }
        /// <summary>
        /// Gets the schedule.
        /// </summary>
        public CommandJobSchedule Schedule { get; }
    }
}
