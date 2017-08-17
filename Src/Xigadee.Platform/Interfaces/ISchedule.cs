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
    /// This interface allows polling schedules to be registered or unregistered.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// This removes a schedule from the collection.
        /// </summary>
        /// <param name="schedule">The schedule to remove.</param>
        /// <returns>Returns true if the schedule is removed successfully.</returns>
        bool Unregister(Schedule schedule);
        /// <summary>
        /// This method registers a schedule.
        /// </summary>
        /// <param name="schedule">The schedule object.</param>
        /// <returns>Returns the schedule after it has been added to the schedule collection.</returns>
        Schedule Register(Schedule schedule);
        /// <summary>
        /// This method registers a schedule.
        /// </summary>
        /// <param name="action">The schedule action.</param>
        /// <param name="frequency">The poll frequency.</param>
        /// <param name="name">The optional poll name.</param>
        /// <param name="initialWait">The optional initial wait until the poll begins.</param>
        /// <param name="initialTime">The optional initial time to start the poll.</param>
        /// <param name="shouldExecute">A flag that indicates whether the schedule execution is active. The default is true.</param>
        /// <param name="isInternal">Specifies whether the poll is internal. The default is false.</param>
        /// <returns>Returns the new schedule after it has been added to the collection.</returns>
        Schedule Register(Func<Schedule, CancellationToken, Task> action, TimeSpan? frequency
            , string name = null, TimeSpan? initialWait = null, DateTime? initialTime = null, bool shouldExecute = true, bool isInternal = false);
    }

    /// <summary>
    /// This interface is set by components that require the schedule functionality built in to the microservice.
    /// </summary>
    public interface IRequireScheduler
    {
        /// <summary>
        /// The scheduler reference.
        /// </summary>
        IScheduler Scheduler { get; set; }
    }
}
