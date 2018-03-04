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

#region using
using System;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the default negotiation strategy for a master job command.
    /// </summary>
    /// <seealso cref="Xigadee.MasterJobNegotiationStrategyBase" />
    public class MasterJobNegotiationStrategy: MasterJobNegotiationStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobNegotiationStrategy"/> class.
        /// </summary>
        public MasterJobNegotiationStrategy() : base("Default")
        {
        }

        /// <summary>
        /// Sets the next poll time for the poll schedule.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="state">The current state</param>
        /// <param name="pollAttempts">The current poll attempts.</param>
        public override void SetNextPollTime(Schedule schedule, MasterJobState state, int pollAttempts)
        {
            switch (state)
            {
                case MasterJobState.Active:
                    schedule.Interval = TimeSpan.FromSeconds(5 + Generator.Next(10));
                    break;
                case MasterJobState.Inactive:
                case MasterJobState.VerifyingComms:
                    //schedule.Frequency = TimeSpan.FromMilliseconds(300);
                    schedule.UTCPollTime = DateTime.UtcNow.AddSeconds(3);
                    break;
                default:
                    if (this.PollAttemptsExceeded(state, pollAttempts))
                        schedule.Interval = TimeSpan.FromSeconds(5 + Generator.Next(25));
                    else
                        schedule.Interval = TimeSpan.FromSeconds(10 + Generator.Next(20));
                    break;
            }
        }
    }

    /// <summary>
    /// This is the default negotiation strategy for a master job command.
    /// </summary>
    /// <seealso cref="Xigadee.MasterJobNegotiationStrategyBase" />
    public class MasterJobNegotiationStrategyDebug: MasterJobNegotiationStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobNegotiationStrategy"/> class.
        /// </summary>
        public MasterJobNegotiationStrategyDebug() : base("Debug")
        {
            InitialPoll = new ScheduleTimerConfig(TimeSpan.FromMilliseconds(100),TimeSpan.FromMilliseconds(100));
        }

        /// <summary>
        /// Sets the next poll time for the poll schedule.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="state">The current state</param>
        /// <param name="pollAttempts">The current poll attempts.</param>
        public override void SetNextPollTime(Schedule schedule, MasterJobState state, int pollAttempts)
        {
            switch (state)
            {
                case MasterJobState.Active:
                    schedule.Interval = TimeSpan.FromMilliseconds(5 + Generator.Next(10));
                    break;
                case MasterJobState.Inactive:
                case MasterJobState.VerifyingComms:
                case MasterJobState.Starting:
                    schedule.Interval = TimeSpan.FromMilliseconds(300);
                    schedule.UTCPollTime = DateTime.UtcNow.AddMilliseconds(300);
                    break;
                default:
                    if (this.PollAttemptsExceeded(state,pollAttempts))
                        schedule.Interval = TimeSpan.FromMilliseconds(5 + Generator.Next(25));
                    else
                        schedule.Interval = TimeSpan.FromMilliseconds(25 + Generator.Next(50));
                    break;
            }
        }
    }
}
