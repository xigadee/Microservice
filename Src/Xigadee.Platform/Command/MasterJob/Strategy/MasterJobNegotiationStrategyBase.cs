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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using Xigadee;
#endregion
namespace Xigadee
{
    /// <summary>
    /// The base functionality for the master job negotiation strategy.
    /// </summary>
    public abstract class MasterJobNegotiationStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterJobNegotiationStrategyBase"/> class.
        /// </summary>
        /// <param name="name">The strategy name.</param>
        protected MasterJobNegotiationStrategyBase(string name)
        {
            Name = name;
            Generator = new Random(Environment.TickCount);
        }

        /// <summary>
        /// Gets the random number generator.
        /// </summary>
        protected Random Generator { get; }  

        /// <summary>
        /// Gets the strategy name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// This is the frequency that jobs wait between polling for status. The default is 20s.
        /// </summary>
        public TimeSpan? InitialPollFrequency { get; set; } = TimeSpan.FromSeconds(20);
        /// <summary>
        /// This is the initial wait after a master job starts that it waits to begin polling. The default is 5s.
        /// </summary>
        public TimeSpan? InitialPollWait { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Returns true if the maximum poll attempts have been exceeded. The default is 3.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="currentMasterPollAttempts">The current master poll attempts.</param>
        /// <returns>True if exceeded.</returns>
        public virtual bool PollAttemptsExceeded(MasterJobState state, int currentMasterPollAttempts)
        {
            switch (state)
            {
                case MasterJobState.Active:
                case MasterJobState.TakingControl:
                case MasterJobState.Requesting2:
                    return currentMasterPollAttempts >= 1;
                case MasterJobState.Requesting1:
                    return currentMasterPollAttempts >= 2;
                default:
                    return currentMasterPollAttempts >= 3;
            }
        }

        /// <summary>
        /// Sets the next poll time for the poll schedule.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="state">The current state</param>
        /// <param name="pollAttempts">The current poll attempts.</param>
        public abstract void SetNextPollTime(Schedule schedule, MasterJobState state, int pollAttempts);
    }
}
