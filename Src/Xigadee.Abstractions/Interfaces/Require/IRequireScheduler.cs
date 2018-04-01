using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is set by components that require the schedule functionality built in to the microservice.
    /// </summary>
    public interface IRequireScheduler
    {
        /// <summary>
        /// The scheduler reference.
        /// </summary>
        IScheduler Scheduler { get; set; }
    }
}
