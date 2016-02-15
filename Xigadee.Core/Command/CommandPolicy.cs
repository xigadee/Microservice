using System;

namespace Xigadee
{
    public class CommandPolicy:PolicyBase
    {
        public CommandPolicy()
        {
            RequestTimeoutInitialWait = TimeSpan.FromSeconds(10);
            RequestTimeoutPollFrequency = TimeSpan.FromSeconds(5);
        }


        public virtual TimeSpan? Interval { get; set; }

        public virtual TimeSpan? InitialWait { get; set; }

        public virtual DateTime? InitialTime { get; set; }

        public virtual bool RequireCommandRequestTracking { get; set; } = false;

        public virtual CommandTimerPoll CommandRequestTimeoutPoll { get; set; }
        /// <summary>
        /// This is the the initial wait time before the request timeout poll starts. The default value is 10 seconds.
        /// </summary>
        public virtual TimeSpan RequestTimeoutInitialWait { get; set; }
        /// <summary>
        /// This is the frequency that request time outs are calculated. The default value is 5 seconds.
        /// </summary>
        public virtual TimeSpan RequestTimeoutPollFrequency { get; set; }

        //JobPoll
        public virtual bool RequireJobPoll { get; set; }

        public virtual CommandTimerPoll JobPollSchedule { get; set; }

        public virtual bool JobPollIsLongRunning { get; set; }


        //MasterJob
        public virtual bool RequireMasterJob { get; set; }

        public virtual int MasterJobNegotiationChannelPriority { get; set; }

        public virtual string MasterJobNegotiationChannelId { get; set; }

        public virtual string MasterJobNegotiationChannelType { get; set; }

        public virtual string MasterJobName { get; set; }

        public static CommandPolicy ToJob(TimeSpan? interval, TimeSpan? initialWait, DateTime? initialTime, bool isLongRunningJob = false)
        {
            return new CommandPolicy { RequireJobPoll = true, InitialTime = initialTime, InitialWait = initialWait, Interval = interval, RequireMasterJob = false, JobPollIsLongRunning = isLongRunningJob };
        }

        public static CommandPolicy ToMasterJob(string negotiationChannelId, string negotiationChannelType = null, int negotiationChannelPriority = 1, string name = null)
        {
            return new CommandPolicy()
            {
                InitialWait = TimeSpan.FromSeconds(2)
                , RequireJobPoll = true
                , Interval = TimeSpan.FromSeconds(20)
                , RequireMasterJob = true
                , MasterJobNegotiationChannelId = negotiationChannelId
                , MasterJobNegotiationChannelType = negotiationChannelType
                , MasterJobNegotiationChannelPriority = negotiationChannelPriority
                , MasterJobName = name
            };
        }
    }
}
