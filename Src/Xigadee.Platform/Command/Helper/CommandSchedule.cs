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
    /// This schedule class is used to record the timeout count statistics.
    /// </summary>
    public class CommandTimeoutSchedule: CommandSchedule
    {
        private long mTimeouts;

        public CommandTimeoutSchedule(Func<Schedule, CancellationToken, Task> execute, CommandTimerPoll timerConfig, string name = null)
            : base(execute, timerConfig, name)
        {
        }

        public long Timeouts
        {
            get { return mTimeouts; }
        }

        public void TimeoutIncrement(long timeouts)
        {
            Interlocked.Add(ref mTimeouts, timeouts);
        }
    }

    public class CommandSchedule: Schedule
    {
        public CommandSchedule(Func<Schedule, CancellationToken, Task> execute, CommandTimerPoll timerConfig, string name = null, bool isLongRunning = false)
            : base(execute, name)
        {
            base.Frequency = timerConfig.Interval;
            base.InitialTime = timerConfig.InitialWaitUTCTime;
            base.InitialWait = timerConfig.InitialWait;
            base.IsLongRunning = IsLongRunning;
        }
    }
}
