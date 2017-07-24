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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the master job command.
    /// </summary>
    public class MasterJobHolder
    {
        public MasterJobHolder(string name
            , Schedule Schedule
            , Func<Schedule, Task> Action
            , Action<Schedule> Initialise = null
            , Action<Schedule> Cleanup = null
            )
        {
            Name = name;
            this.Schedule = Schedule;
            this.Action = Action;
            this.Initialise = Initialise;
            this.Cleanup = Cleanup;
        }
        /// <summary>
        /// This is the name for the master job schedule.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// This is the poll schedule for the job.
        /// </summary>
        public Schedule Schedule { get; set; }
        /// <summary>
        /// This is the schedule action.
        /// </summary>
        public Func<Schedule, Task> Action { get; set; }
        /// <summary>
        /// Gets or sets the initialise.
        /// </summary>
        public Action<Schedule> Initialise { get; set; }
        /// <summary>
        /// Gets or sets the cleanup.
        /// </summary>
        public Action<Schedule> Cleanup { get; set; }
    }
}
