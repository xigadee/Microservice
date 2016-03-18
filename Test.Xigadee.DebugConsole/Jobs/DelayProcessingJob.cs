using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class DelayedProcessingJob : JobBase<JobStatistics>
    {
        private Logger mLogger = LogManager.GetCurrentClassLogger();
        private int mCurrentExecutionId;

        public DelayedProcessingJob() : base(JobConfiguration.ToJob(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1), null, isLongRunningJob: true))
        {
        }

        protected override void TimerPollSchedulesRegister(JobConfiguration config)
        {
            var job = new Schedule(ExecuteJob, string.Format("ExecuteJob: {0}", GetType().Name));

            job.Frequency = config.Interval;
            job.InitialWait = config.InitialWait;
            job.InitialTime = config.InitialTime;
            job.IsLongRunning = false;

            mSchedules.Add(job);
        }

        protected async Task ExecuteJob(Schedule state, CancellationToken token)
        {
            int currentJob = mCurrentExecutionId++;
            mLogger.Log(LogLevel.Info, "DelayedProcessingJob - Executing schedule " + currentJob);
            state.State = currentJob;
            
            Dispatcher(this, new TransmissionPayload("interserv", "do", "something", options:ProcessOptions.RouteExternal));

            //await Task.Delay(TimeSpan.FromSeconds(40), token);
            mLogger.Log(LogLevel.Info, "DelayedProcessingJob - Finished executing schedule " + currentJob);
        }
    }
 
}
