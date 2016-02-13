using System;

namespace Xigadee
{
    public class CommandPolicy:PolicyBase
    {
        public TimeSpan? Interval { get; set; }

        public TimeSpan? InitialWait { get; set; }

        public DateTime? InitialTime { get; set; }

        public bool IsMasterJob { get; set; }

        public bool HasTimerPoll { get; set; }

        public bool IsLongRunningJob { get; set; }

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
