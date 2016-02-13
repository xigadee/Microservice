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

        public TimeSpan? Interval { get; set; }

        public TimeSpan? InitialWait { get; set; }

        public DateTime? InitialTime { get; set; }

        /// <summary>
        /// This is the the initial wait time before the request timeout poll starts. The default value is 10 seconds.
        /// </summary>
        public TimeSpan RequestTimeoutInitialWait { get; set; }
        /// <summary>
        /// This is the frequency that request time outs are calculated. The default value is 5 seconds.
        /// </summary>
        public TimeSpan RequestTimeoutPollFrequency { get; set; }

        public virtual bool IsMasterJob { get; set; }

        public virtual bool HasTimerPoll { get; set; }

        public virtual bool IsLongRunningJob { get; set; }

        public int MasterJobNegotiationChannelPriority { get; set; }

        public string MasterJobNegotiationChannelId { get; set; }

        public string MasterJobNegotiationChannelType { get; set; }

        public string MasterJobName { get; set; }

        public static CommandPolicy ToJob(TimeSpan? interval, TimeSpan? initialWait, DateTime? initialTime, bool isLongRunningJob = false)
        {
            return new CommandPolicy { HasTimerPoll = true, InitialTime = initialTime, InitialWait = initialWait, Interval = interval, IsMasterJob = false, IsLongRunningJob = isLongRunningJob };
        }

        public static CommandPolicy ToMasterJob(string negotiationChannelId, string negotiationChannelType = null, int negotiationChannelPriority = 1, string name = null)
        {
            return new CommandPolicy()
            {
                InitialWait = TimeSpan.FromSeconds(2)
                , HasTimerPoll = true
                , Interval = TimeSpan.FromSeconds(20)
                , IsMasterJob = true
                , MasterJobNegotiationChannelId = negotiationChannelId
                , MasterJobNegotiationChannelType = negotiationChannelType
                , MasterJobNegotiationChannelPriority = negotiationChannelPriority
                , MasterJobName = name
            };
        }
    }
}
