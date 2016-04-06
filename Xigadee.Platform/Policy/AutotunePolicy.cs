using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class TaskManagerPolicy:PolicyBase
    {
        public TaskManagerPolicy()
        {

        }
        /// <summary>
        /// This specifies whether autotune should be supported.
        /// </summary>
        public bool Supported { get; set; }
        /// <summary>
        /// This is the time that a process is marked as killed after it has been marked as cancelled.
        /// </summary>
        public TimeSpan ProcessKillOverrunGracePeriod { get; set; }
        /// <summary>
        /// This is maximum target percentage usuage limit.
        /// </summary>
        public int ProcessorTargetLevelPercentage { get; set; }
        /// <summary>
        /// This is the maximum number overload processes permitted.
        /// </summary>
        public int OverloadProcessLimitMax { get; set; }
        /// <summary>
        /// This is the minimum number of overload processors available.
        /// </summary>
        public int OverloadProcessLimitMin { get; set; }
        /// <summary>
        /// This is the maximum time that an overload process task can run.
        /// </summary>
        public int OverloadProcessTimeInMs { get; set; }
        /// <summary>
        /// This is the maximum number of concurrent requests.
        /// </summary>
        public int ConcurrentRequestsMax { get; set; }
        /// <summary>
        /// This is the minimum number of concurrent requests.
        /// </summary>
        public int ConcurrentRequestsMin { get; set; }
    }
}
