using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{

    public interface IScheduler
    {
        bool Unregister(Schedule schedule);

        Schedule Register(Schedule schedule);

        Schedule Register(Func<Schedule, CancellationToken, Task> action, TimeSpan? frequency, string name = null, TimeSpan? initialWait = null, DateTime? initialTime = null, bool shouldPoll = true, bool isInternal = false);
    }

    /// <summary>
    /// This interface is set by components that require the schedule functionality built in to the microservice.
    /// </summary>
    public interface IRequireScheduler
    {
        IScheduler Scheduler { get; set; }
    }
}
