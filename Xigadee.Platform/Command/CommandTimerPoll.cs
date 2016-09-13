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

namespace Xigadee
{
    /// <summary>
    /// This is the timer poll configuration used by several components in the command policy.
    /// </summary>
    public class CommandTimerPoll
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor with an initial wait time of 10 seconds and a poll frequency of 5 seconds.
        /// </summary>
        public CommandTimerPoll()
        {
            Interval = TimeSpan.FromSeconds(5);
            InitialWait = TimeSpan.FromSeconds(10);
            InitialWaitUTCTime = null;
        }
        /// <summary>
        /// This is the extended constructor.
        /// </summary>
        /// <param name="interval">The poll interval.</param>
        /// <param name="initialWait">The initial wait.</param>
        /// <param name="initialWaitUTCTime">The optional initial UTC wait time that the polling will begin if the initialWait is set to null.</param>
        public CommandTimerPoll(TimeSpan? interval, TimeSpan? initialWait, DateTime? initialWaitUTCTime = null)
        {
            Interval = interval;
            InitialWait = initialWait;
            InitialWaitUTCTime = initialWaitUTCTime;

            if (! interval.HasValue && !InitialWait.HasValue && !initialWaitUTCTime.HasValue)
                throw new ArgumentNullException("At least one parameter must be set when using this constructor.");
        }
        #endregion

        /// <summary>
        /// This is the poll interval. The default is 5 seconds.
        /// </summary>
        public virtual TimeSpan? Interval { get; set; }
        /// <summary>
        /// This is the initial wait time before polling begins, the default is 10 seconds.
        /// </summary>
        public virtual TimeSpan? InitialWait { get; set; }
        /// <summary>
        /// This is the initial UTC time that polling should begin if the initial wait is not set. 
        /// </summary>
        public virtual DateTime? InitialWaitUTCTime { get; set; }
    }
}
