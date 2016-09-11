using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class CommandPolicyHelper
    {
        public static CommandSchedule ToCommandSchedule(this CommandPolicy policy, Func<Schedule
            , CancellationToken, Task> execute, string name)
        {
            return new CommandSchedule(execute
                , policy.JobPollSchedule
                , name
                , policy.JobPollIsLongRunning);
        }
    }
}
