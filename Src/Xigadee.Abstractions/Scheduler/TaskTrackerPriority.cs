using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This enumeration contains the default set of priorty values for the task tracker.
    /// </summary>
    public enum TaskTrackerPriority : int
    {
        /// <summary>
        /// The job priorty is low.
        /// </summary>
        Low = 0,
        /// <summary>
        /// This is the default value.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// The priority is high.
        /// </summary>
        High = 2,
        /// <summary>
        /// The task is s priority.
        /// </summary>
        Priority = 3
    }
}
