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
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This attribute can be set against a command method to register it for a schedule job poll call.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class JobScheduleAttributeBase: CommandMethodAttributeBase
    {
        private string mInitialWait, mFrequency;

        /// <summary>
        /// This is the default constructor. 
        /// </summary>
        /// <param name="name">The name for the schedule. This will be displayed in the Microservice statistics.</param>
        /// <param name="isMasterJob">Specifies whether the attribute is connected to a master job.</param>
        /// <param name="initialWait">The initial wait before the job first polls. If this is null the job will be polled immediately and you can set the schedule manually.
        /// Leave this as null if you do not want an initial wait, but wish the job to poll after the frequency.
        /// The string should be in the format of a [ws][-]{ d | [d.]hh:mm[:ss[.ff]] }[ws] as defined in the Timespan.Parse method.
        /// </param>
        /// <param name="frequency">The frequency of the poll after the initial wait.
        /// The string should be in the format of a [ws][-]{ d | [d.]hh:mm[:ss[.ff]] }[ws] as defined in the Timespan.Parse method.
        /// </param>
        /// <param name="isLongRunningProcess">Specifies whether the schedule is a long running process.</param>
        /// <see cref="https://msdn.microsoft.com/en-us/library/se73z7b9(v=vs.110).aspx"/>
        protected JobScheduleAttributeBase(string name, bool isMasterJob, string initialWait = null, string frequency = null, bool isLongRunningProcess = false)
        {
            Name = name ?? throw new ArgumentNullException("name", $"{GetType().Name}");
            mInitialWait = initialWait;
            mFrequency = frequency;
            IsLongRunningProcess = isLongRunningProcess;
            IsMasterJob = isMasterJob;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected to master job.
        /// </summary>
        public bool IsMasterJob { get; }

        /// <summary>
        /// This is the Initial Wait as a timespan.
        /// </summary>
        /// <returns>Returns the timespan or null.</returns>
        public TimeSpan? InitialWait
        {
            get
            {
                if (mInitialWait == null)
                    return null;

                return TimeSpan.Parse(mInitialWait);
            }
        }
        /// <summary>
        /// This is the frequency as a timespan.
        /// </summary>
        /// <returns>Returns the timespan or null.</returns>
        public TimeSpan? Frequency
        {
            get
            {
                if (mFrequency == null)
                    return null;

                return TimeSpan.Parse(mFrequency);
            }
        }

        /// <summary>
        /// This is the schedule name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Indicates whether this schedule is long running process.
        /// </summary>
        public bool IsLongRunningProcess { get; }
        /// <summary>
        /// Gets the reference for the attribute.
        /// </summary>
        public override string Reference => Name;

        /// <summary>
        /// To the timer config.
        /// </summary>
        /// <returns>A scheduler timer configuration</returns>
        public ScheduleTimerConfig ToTimerConfig()
        {
            return new ScheduleTimerConfig(initialWait: InitialWait, interval: Frequency, enforceSetting:false);
        }
    }
}
