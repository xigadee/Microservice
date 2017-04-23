using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The command policy sets or enables various settings for the command.
    /// </summary>
    public class CommandPolicy:PolicyBase
    {
        /// <summary>
        /// This is the default listening channel for incoming messages.
        /// </summary>
        public virtual string ChannelId { get; set; }
        /// <summary>
        /// This property specifies that the listening channel id can be automatically set during configuration.
        /// </summary>
        public bool ChannelIdAutoSet { get; set; } = true;
        /// <summary>
        /// This is the response channel set in outgoing request, and is used to route response messages back to the command.
        /// </summary>
        public virtual string ResponseChannelId { get; set; }
        /// <summary>
        /// This property specifies whether the response channel can be automatically set by the pipeline.
        /// </summary>
        public bool ResponseChannelIdAutoSet { get; set; } = true;

        /// <summary>
        /// This is the channel for incoming masterjob negotiation messages.
        /// </summary>
        public virtual string MasterJobNegotiationChannelIdIncoming { get; set; }
        /// <summary>
        /// This is the channel for outgoing masterjob negotiation messages, if this is null then the incoming channel will be used.
        /// </summary>
        public virtual string MasterJobNegotiationChannelIdOutgoing { get; set; }
        /// <summary>
        /// This property specifies whether these channels can be set automatically from the configuration pipeline.
        /// </summary>
        public bool MasterJobNegotiationChannelIdAutoSet { get; set; } = true;

        /// <summary>
        /// This is the command startup prioroty.
        /// </summary>
        public int? StartupPriority { get; set; }=0;

        public CommandNotificationBehaviour CommandNotify { get; set; } = CommandNotificationBehaviour.OnRegistration;
        /// <summary>
        /// This is the default timeout for outgoing requests from the Command to other commands when not set in the settings.
        /// The default is 30s.
        /// </summary>
        public virtual TimeSpan OutgoingRequestMaxProcessingTimeDefault { get; set; } = TimeSpan.FromSeconds(30);
        /// <summary>
        /// This property specifies that outgoing settings are enabled. By default this is not set.
        /// </summary>
        public virtual bool OutgoingRequestsEnabled { get; set; } = false;

        public virtual CommandTimerPoll OutgoingRequestsTimeoutPoll { get; set; } = new CommandTimerPoll();

        //Job Poll
        public virtual bool JobPollEnabled { get; set; } = false;

        public virtual CommandTimerPoll JobPollSchedule { get; set; } = new CommandTimerPoll();

        public virtual bool JobPollIsLongRunning { get; set; } = false;

        //Master Job
        public virtual bool MasterJobEnabled { get; set; }

        public virtual int MasterJobNegotiationChannelPriority { get; set; }

        public virtual string MasterJobNegotiationChannelType { get; set; }

        public virtual string MasterJobName { get; set; }

        public static CommandPolicy ToJob(TimeSpan? interval, TimeSpan? initialWait, DateTime? initialTime, bool isLongRunningJob = false)
        {
            return ToJob<CommandPolicy>(interval, initialWait, initialTime, isLongRunningJob);
        }

        public static P ToJob<P>(TimeSpan? interval, TimeSpan? initialWait, DateTime? initialTime, bool isLongRunningJob = false)
            where P : CommandPolicy, new()
        {
            return new P
            {
                  JobPollEnabled = true
                , JobPollIsLongRunning = isLongRunningJob
                , MasterJobEnabled = false
                , OutgoingRequestsEnabled = false
                , JobPollSchedule = new CommandTimerPoll(interval, initialWait, initialTime)
            };
        }

        public static CommandPolicy ToMasterJob(string negotiationChannelId, string negotiationChannelType = null, int negotiationChannelPriority = 1, string name = null)
        {
            return ToMasterJob<CommandPolicy>(negotiationChannelId, negotiationChannelType, negotiationChannelPriority, name);
        }

        public static P ToMasterJob<P>(string negotiationChannelId, string negotiationChannelType = null, int negotiationChannelPriority = 1, string name = null)
            where P: CommandPolicy, new()
        {
            return new P()
            {
                  JobPollEnabled = true
                , MasterJobEnabled = true
                , MasterJobNegotiationChannelIdOutgoing = negotiationChannelId
                , MasterJobNegotiationChannelType = negotiationChannelType
                , MasterJobNegotiationChannelPriority = negotiationChannelPriority
                , MasterJobName = name
                , OutgoingRequestsEnabled = false
            };
        }
    }


}
