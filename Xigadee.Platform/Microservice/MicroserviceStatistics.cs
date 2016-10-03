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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the current status of the Microservice container.
    /// </summary>
    [DebuggerDisplay("{Name}-{Status} @ {LogTime}")]
    public class MicroserviceStatistics: MessagingStatistics, ILogStoreName
    {
        #region Constructor
        /// <summary>
        /// This is the statistics default constructor.
        /// </summary>
        public MicroserviceStatistics() : base()
        {

        } 
        #endregion

        #region Name
        /// <summary>
        /// This override places the name at the top of the JSON
        /// </summary>
        public override string Name
        {
            get
            {
                return Id?.Name;
            }

            set
            {
            }
        }
        #endregion

        /// <summary>
        /// This is the Microservice identifier collection.
        /// </summary>
        public MicroserviceId Id { get; set; }

        /// <summary>
        /// This is the current status of the service.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// This is the last time that the statistics were updated.
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// This is the service uptime.
        /// </summary>
        public string Uptime
        {
            get
            {
                var span = LogTime - Id.StartTime;
                return StatsCounter.LargeTime(span);
            }
        }

        /// <summary>
        /// This is the task manager statistics.
        /// </summary>
        public TaskManagerStatistics Tasks { get; set; }

        /// <summary>
        /// This is a list of the handlers active on the system and their status.
        /// </summary>
        public CommunicationStatistics Communication { get; set; }

        /// <summary>
        /// This is the command container statistics/
        /// </summary>
        public CommandContainerStatistics Commands { get; set; }
        /// <summary>
        /// The resource statistics.
        /// </summary>
        public ResourceTrackerStatistics Resources { get; set; }

        /// <summary>
        /// The security statictics.
        /// </summary>
        public SecurityStatistics Security { get; set; }
        /// <summary>
        /// The scheduler statistics.
        /// </summary>
        public SchedulerStatistics Scheduler { get; set; }

        /// <summary>
        /// The data collection statistics. These include the logger, event source and telemetry statistics.
        /// </summary>
        public DataCollectionStatistics DataCollection { get; set; }

        #region StorageId
        /// <summary>
        /// This is the Id used in the undelying storage.
        /// </summary>
        public string StorageId
        {
            get
            {
                return string.Format("{0}_{3:yyyyMMddHHmmssFFF}_{1}_{2}", Id.Name, Id.MachineName, Id.ServiceId, LogTime);
            }
        }
        #endregion
    }
}
