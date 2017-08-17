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
    /// This class is used to signal a change to a job or a masterjob schedule.
    /// </summary>
    /// <seealso cref="Xigadee.CommandEventArgsBase" />
    public class ScheduleChangeEventArgs: CommandEventArgsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleChangeEventArgs"/> class.
        /// </summary>
        /// <param name="isRemoval">True if the schedule is being removed.</param>
        /// <param name="schedule">The schedule.</param>
        public ScheduleChangeEventArgs(bool isRemoval, CommandJobSchedule schedule)
        {
            IsRemoval = isRemoval;
            Schedule = schedule;
        }
        /// <summary>
        /// Gets a value indicating whether this instance is a removal.
        /// </summary>
        public bool IsRemoval { get; }
        /// <summary>
        /// Gets the schedule.
        /// </summary>
        public CommandJobSchedule Schedule { get; }
    }
}
