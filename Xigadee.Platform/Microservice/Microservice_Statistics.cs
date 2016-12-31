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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
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
        protected async Task LogStatistics()
        {
            try
            {
                var statistics = Statistics;
                mDataCollection?.Write(statistics, DataCollectionSupport.Statistics);
                mEventsWrapper.OnStatisticsIssued(statistics);
            }
            catch (Exception ex)
            {
                //We're not going to throw any exception here
                mDataCollection?.LogException("LogStatistics unhandled exception", ex);
            }
        }
        #endregion
        #region StatisticsRecalculate()
        /// <summary>
        /// This method sets the updated Microservice statistics.
        /// </summary>
        protected override void StatisticsRecalculate(MicroserviceStatistics stats)
        {
            stats.Id = Id;

            stats.Name = Id.Name;
            stats.Created = Id.StartTime;

            stats.Status = Status.ToString();
            stats.LogTime = DateTime.UtcNow;

            stats.Tasks = mTaskManager?.Statistics;

            stats.DataCollection = mDataCollection?.Statistics;

            stats.Communication = mCommunication?.Statistics;

            stats.Resources = mResourceTracker?.Statistics;

            stats.Commands = mCommands?.Statistics;

            stats.Scheduler = mScheduler?.Statistics;

            stats.Security = mSecurity?.Statistics;

            stats.Serialization = mSerializer?.Statistics;
        }
        #endregion
    }
}
