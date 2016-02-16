using System;

namespace Xigadee
{
    public class CommandPolicy:PolicyBase
    {
        public CommandPolicy()
        {
        }

        /// <summary>
        /// This is the command startup prioroty.
        /// </summary>
        public int? StartupPriority { get; set; }

        //Outgoing Request
        public virtual bool OutgoingRequestsEnabled { get; set; } = false;

        public virtual CommandTimerPoll OutgoingRequestsTimeoutPoll { get; set; } = new CommandTimerPoll();

        //Job Poll
        public virtual bool JobPollEnabled { get; set; } = false;

        public virtual CommandTimerPoll JobPollSchedule { get; set; } = new CommandTimerPoll();

        public virtual bool JobPollIsLongRunning { get; set; } = false;

        //Master Job
        public virtual bool MasterJobEnabled { get; set; }

        public virtual int MasterJobNegotiationChannelPriority { get; set; }

        public virtual string MasterJobNegotiationChannelId { get; set; }

        public virtual string MasterJobNegotiationChannelType { get; set; }

        public virtual string MasterJobName { get; set; }



        public static CommandPolicy ToJob(TimeSpan? interval, TimeSpan? initialWait, DateTime? initialTime, bool isLongRunningJob = false)
        {
            return new CommandPolicy { JobPollEnabled = true, JobPollIsLongRunning = isLongRunningJob, MasterJobEnabled = false };
        }

        public static CommandPolicy ToMasterJob(string negotiationChannelId, string negotiationChannelType = null, int negotiationChannelPriority = 1, string name = null)
        {
            return new CommandPolicy()
            {
                  JobPollEnabled = true
                , MasterJobEnabled = true
                , MasterJobNegotiationChannelId = negotiationChannelId
                , MasterJobNegotiationChannelType = negotiationChannelType
                , MasterJobNegotiationChannelPriority = negotiationChannelPriority
                , MasterJobName = name
            };
        }
    }
}
