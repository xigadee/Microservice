using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This enum defines how command registration is notified to the controlling container.
    /// </summary>
    public enum CommandNotificationBehaviour
    {
        OnRegistration,
        OnRegistrationIfStarted,
        Manual
    }

    /// <summary>
    /// The command policy sets or enables various settings for the command.
    /// </summary>
    public class CommandPolicy:PolicyBase
    {
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

        public virtual string MasterJobNegotiationChannelId { get; set; }

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
                , OutgoingRequestsEnabled = true
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
                , MasterJobNegotiationChannelId = negotiationChannelId
                , MasterJobNegotiationChannelType = negotiationChannelType
                , MasterJobNegotiationChannelPriority = negotiationChannelPriority
                , MasterJobName = name
                , OutgoingRequestsEnabled = true
            };
        }
    }

    public static class CommandPolicyHelper
    {
        public static CommandSchedule ToCommandSchedule(this CommandPolicy policy, Func<Schedule, CancellationToken, Task> execute, string name)
        {
            return new CommandSchedule(execute
                , policy.JobPollSchedule
                , name
                , policy.JobPollIsLongRunning);
        }
    }
}
