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
    public partial class MicroserviceBase
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
        protected override void StatisticsRecalculate()
        {
            base.StatisticsRecalculate();
            try
            {
                mStatistics.MachineName = mMachineName;
                mStatistics.Name = mName;
                mStatistics.ServiceId = mServiceId;
                mStatistics.ExternalServiceId = ExternalServiceId;

                mStatistics.VersionId = mServiceVersionId;
                mStatistics.EngineVersionId = mServiceEngineVersionId;
                mStatistics.Created = mStartTime;
                mStatistics.Status = Status.ToString();
                mStatistics.LogTime = DateTime.UtcNow;
                mStatistics.Configuration = ConfigurationOptions;
                mStatistics.StartTime = mStartTime;

                if (mTaskRequests != null)
                {
                    mStatistics.Tasks.Active = mTaskRequests.Count;
                    mStatistics.Tasks.SlotsAvailable = TaskSlotsAvailable;
                }

                mStatistics.Tasks.Internal = mTasksInternal;

                mStatistics.Tasks.Killed = mTasksKilled;
                mStatistics.Tasks.KilledTotal = mTasksKilledTotal;
                mStatistics.Tasks.KilledDidReturn = mTasksKilledDidReturn;

                if (mTaskRequests != null) mStatistics.Tasks.Running =
                    mTaskRequests.Values
                    .Where((t) => t.ProcessSlot.HasValue)
                    .OrderByDescending((t) => t.ProcessSlot.Value)
                    .Select((t) => t.Debug)
                    .ToList();

                mStatistics.Cpu = mCpuStats;

                mStatistics.Autotune.Active = ConfigurationOptions.SupportAutotune;
                mStatistics.Autotune.TasksMaxConcurrent = mAutotuneTasksMaxConcurrent;
                mStatistics.Autotune.OverloadTasksConcurrent = mAutotuneOverloadTasksConcurrent;

                if (mLogger != null) mStatistics.Logger = mLogger.Statistics;

                if (mEventSource != null) mStatistics.EventSource = mEventSource.Statistics;

                if (mCommunication != null) mStatistics.Communication = mCommunication.Statistics;

                if (mResourceTracker != null) mStatistics.Resources = mResourceTracker.Statistics;

                if (mComponents != null) mStatistics.Components = mComponents.Statistics;

                if (mTasksQueue != null) mStatistics.Queues = mTasksQueue.Statistics;

                if (mScheduler != null) mStatistics.Scheduler = mScheduler.Statistics;



            }
            catch (Exception ex)
            {
                mStatistics.Ex = ex;
            }
        } 
        #endregion

    }
}
