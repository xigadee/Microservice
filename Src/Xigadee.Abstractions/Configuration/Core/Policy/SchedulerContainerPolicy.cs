using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    #region SchedulerContainerPolicy
    /// <summary>
    /// This is the scheduler policy.
    /// </summary>
    /// <seealso cref="Xigadee.PolicyBase" />
    public class SchedulerContainerPolicy : PolicyBase
    {
        /// <summary>
        /// Gets or sets the default poll in milliseconds.
        /// </summary>
        public virtual int DefaultPollInMs { get; set; } = 100;

        /// <summary>
        /// Gets or sets the default task priority that schedules are set for the Task Manager,.
        /// </summary>
        public int DefaultTaskPriority { get; set; } = 2;
    }
    #endregion
}
