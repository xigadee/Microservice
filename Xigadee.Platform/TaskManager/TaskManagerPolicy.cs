#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the policy object for the TaskManager, that determines how it operates.
    /// </summary>
    public class TaskManagerPolicy:PolicyBase
    {
        public TaskManagerPolicy()
        {
        }
        /// <summary>
        /// This specifies whether autotune should be supported.
        /// </summary>
        public bool AutotuneEnabled { get; set; }
        /// <summary>
        /// This is the time that a process is marked as killed after it has been marked as cancelled.
        /// </summary>
        public TimeSpan? ProcessKillOverrunGracePeriod { get; set; } = TimeSpan.FromSeconds(15);
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
        public int ConcurrentRequestsMax { get; set; } = Environment.ProcessorCount * 16;
        /// <summary>
        /// This is the minimum number of concurrent requests.
        /// </summary>
        public int ConcurrentRequestsMin { get; set; } = Environment.ProcessorCount * 2;

        /// <summary>
        /// This is the default time that the process loop should pause before another cycle if it is not triggered
        /// by a task submission or completion. The default is 200 ms.
        /// </summary>
        public int LoopPauseTimeInMs { get; set; } = 50;

        public bool ExecuteInternalDirect { get; set; } = true;

    }
}
