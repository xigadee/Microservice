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
                await mLogger.Log(statistics);
                OnStatisticsIssued(statistics);
            }
            catch (Exception ex)
            {
                //We're not going to throw any exception here
                mLogger.LogException("LogStatistics unhandled exception", ex);
            }
        }
        #endregion
        #region StatisticsRecalculate()
        /// <summary>
        /// This method sets the updated Microservice statistics.
        /// </summary>
        protected override void StatisticsRecalculate(MicroserviceStatistics stats)
        {
            stats.MachineName = mMachineName;
            stats.ServiceId = mServiceId;
            stats.ExternalServiceId = ExternalServiceId;

            stats.VersionId = mServiceVersionId;
            stats.EngineVersionId = mServiceEngineVersionId;
            stats.Created = mStartTime;
            stats.Status = Status.ToString();
            stats.LogTime = DateTime.UtcNow;
            stats.Configuration = ConfigurationOptions;
            stats.StartTime = mStartTime;

            stats.Tasks = mTaskManager?.Statistics;

            stats.Logger = mLogger?.Statistics;

            stats.EventSource = mEventSource?.Statistics;

            stats.Communication = mCommunication?.Statistics;

            stats.Resources = mResourceTracker?.Statistics;

            stats.Commands = mCommands?.Statistics;

            stats.Scheduler = mScheduler?.Statistics;
        }
        #endregion

        protected override void StatisticsInitialise(MicroserviceStatistics stats)
        {
            base.StatisticsInitialise(stats);

            stats.Name = "";
            stats.Application = "";
            stats.Environment = "";
        }
    }
}
