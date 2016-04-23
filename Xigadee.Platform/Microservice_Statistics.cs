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

            if (mTaskContainer != null) stats.Tasks = mTaskContainer.Statistics;

            if (mLogger != null) stats.Logger = mLogger.Statistics;

            if (mEventSource != null) stats.EventSource = mEventSource.Statistics;

            if (mCommunication != null) stats.Communication = mCommunication.Statistics;

            if (mResourceTracker != null) stats.Resources = mResourceTracker.Statistics;

            if (mComponents != null) stats.Commands = mComponents.Statistics;

            if (mScheduler != null) stats.Scheduler = mScheduler.Statistics;
        }
        #endregion
    }
}
