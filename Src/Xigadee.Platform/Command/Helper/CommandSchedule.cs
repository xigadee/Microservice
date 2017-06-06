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
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the command schedule generated for the specific command requirement.
    /// </summary>
    public class CommandSchedule: Schedule
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="execute">The execution function.</param>
        /// <param name="timerConfig">The timer poll configuration.</param>
        /// <param name="name">The name of the schedule.</param>
        /// <param name="isLongRunning">A boolean flag that specifies whether the process is long running.</param>
        public CommandSchedule(Func<Schedule, CancellationToken, Task> execute, CommandTimerPoll timerConfig, string name = null, bool isLongRunning = false)
            : base(execute, name)
        {
            Frequency = timerConfig.Interval;
            InitialTime = timerConfig.InitialWaitUTCTime;
            InitialWait = timerConfig.InitialWait;
            IsLongRunning = isLongRunning;
        }
    }
}
