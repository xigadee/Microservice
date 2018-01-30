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
using System.Diagnostics;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    //Statistics
    public partial class Microservice
    {
        #region LogStatistics()
        /// <summary>
        /// This method logs the current status as part of the time poll.
        /// </summary>
        /// <returns></returns>
        protected Task LogStatistics()
        {
            try
            {
                var statistics = StatisticsRecalculated;
                mDataCollection?.Write(statistics, DataCollectionSupport.Statistics);
                mEventsWrapper.OnStatisticsIssued(statistics);
            }
            catch (Exception ex)
            {
                //We're not going to throw any exception here
                mDataCollection?.LogException("LogStatistics unhandled exception", ex);
            }

            return Task.FromResult(0);
        }
        #endregion
        #region StatisticsRecalculate()
        /// <summary>
        /// This method sets the updated Microservice statistics.
        /// </summary>
        protected override void StatisticsRecalculate(Microservice.Statistics stats)
        {
            stats.Id = Id;

            stats.Name = Id.Name;
            stats.Created = Id.StartTime;

            stats.Status = Status.ToString();
            stats.LogTime = DateTime.UtcNow;

            stats.Tasks = mTaskManager?.StatisticsRecalculated;

            stats.DataCollection = mDataCollection?.StatisticsRecalculated;

            stats.Communication = mCommunication?.StatisticsRecalculated;

            stats.Resources = mResourceMonitor?.StatisticsRecalculated;

            stats.Commands = mCommands?.StatisticsRecalculated;

            stats.Scheduler = mScheduler?.StatisticsRecalculated;

            stats.ServiceHandlers = mServiceHandlers?.StatisticsRecalculated;

        }
        #endregion

        #region Class -> Statistics
        /// <summary>
        /// This class holds the current status of the Microservice container.
        /// </summary>
        [DebuggerDisplay("{Name}-{Status} @ {LogTime}")]
        public class Statistics: MessagingStatistics, ILogStoreName
        {
            #region Constructor
            /// <summary>
            /// This is the statistics default constructor.
            /// </summary>
            public Statistics() : base()
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
            public TaskManager.Statistics Tasks { get; set; }

            /// <summary>
            /// This is a list of the handlers active on the system and their status.
            /// </summary>
            public CommunicationContainer.Statistics Communication { get; set; }

            /// <summary>
            /// This is the command container statistics/
            /// </summary>
            public CommandContainer.Statistics Commands { get; set; }
            /// <summary>
            /// The resource statistics.
            /// </summary>
            public ResourceContainer.Statistics Resources { get; set; }

            /// <summary>
            /// The security statistics.
            /// </summary>
            public SecurityContainer.Statistics Security { get; set; }

            /// <summary>
            /// The service handler statistics.
            /// </summary>
            public ServiceHandlerContainer.Statistics ServiceHandlers { get; set; }
            /// <summary>
            /// The scheduler statistics.
            /// </summary>
            public SchedulerContainer.Statistics Scheduler { get; set; }

            /// <summary>
            /// The data collection statistics. These include the logger, event source and telemetry statistics.
            /// </summary>
            public DataCollectionContainer.Statistics DataCollection { get; set; }

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
        #endregion
    }
}
