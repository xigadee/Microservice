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
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class contains the statistics for the scheduler collection.
    /// </summary>
    /// <seealso cref="Xigadee.CollectionStatistics" />
    public class SchedulerStatistics: CollectionStatistics
    {
        #region Name
        /// <summary>
        /// This is the service name.
        /// </summary>
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }
        #endregion
        #region ItemCount
        /// <summary>
        /// The item count.
        /// </summary>
        public override int ItemCount
        {
            get
            {
                return base.ItemCount;
            }

            set
            {
                base.ItemCount = value;
            }
        }
        #endregion
        #region DefaultPollInMs
        /// <summary>
        /// Displays the default poll time in milliseconds.
        /// </summary>
        public int DefaultPollInMs { get; set; } 
        #endregion

        /// <summary>
        /// Gets or sets the schedule statistics.
        /// </summary>
        public List<Schedule> Schedules { get; set; }
    }
}
