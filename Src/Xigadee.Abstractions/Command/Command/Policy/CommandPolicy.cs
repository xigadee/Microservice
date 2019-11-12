using System;

namespace Xigadee
{
    /// <summary>
    /// The command policy sets or enables various settings for the command.
    /// </summary>
    public class CommandPolicy:PolicyBase
    {
        /// <summary>
        /// Specifies that the commands are supported by reflection, i.e. those commands that use the CommandContractAttribute.
        /// </summary>
        public bool CommandReflectionSupported { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the command contract attribute should include inherited methods.
        /// </summary>
        public bool CommandContractAttributeInherit { get; set; } = true;

        //Job Poll        
        /// <summary>
        /// Gets or sets a value indicating whether job schedules are enabled on the command. The default setting is true.
        /// </summary>
        public virtual bool JobSchedulesEnabled { get; set; } = true;
        /// <summary>
        /// Specifies that schedule reflection commands are supported, i.e. those that use the CommandScheduleAttribute.
        /// </summary>
        public bool JobScheduleReflectionSupported { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the job schedule attribute should include inherited methods.
        /// </summary>
        public bool JobScheduleAttributeInherit { get; set; } = true;

        /// <summary>
        /// The behaviour to follow for an uncaught exception.
        /// </summary>
        public ProcessRequestExceptionBehaviour OnProcessRequestException { get; set; } = ProcessRequestExceptionBehaviour.ThrowException;
        /// <summary>
        /// Gets or sets a value indicating whether unhandled exceptions should be logged. The default value is true.
        /// </summary>
        public bool OnProcessRequestExceptionLog { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the TransmissionPayload trace flag should be set to true.
        /// </summary>
        public bool TransmissionPayloadTraceEnabled { get; set; }

        /// <summary>
        /// This is the default job schedule execution priorty if not set explicitly.
        /// </summary>
        public int JobScheduleDefaultExecutionPriority { get; set; } = (int)TaskTrackerPriority.Low;

        /// <summary>
        /// This is the default listening channel for incoming messages.
        /// </summary>
        public virtual string ChannelId { get; set; }
        /// <summary>
        /// This property specifies that the listening channel id can be automatically set during configuration by the pipeline. The default is true.
        /// </summary>
        public bool ChannelIdAutoSet { get; set; } = true;

        /// <summary>
        /// This is the response channel set in outgoing request, and is used to route response messages back to the command.
        /// </summary>
        public virtual string ResponseChannelId { get; set; }
        /// <summary>
        /// This property specifies whether the response channel can be automatically set by the pipeline. The default is true.
        /// </summary>
        public bool ResponseChannelIdAutoSet { get; set; } = true;

        /// <summary>
        /// This is the command start-up priority. The default is 0.
        /// </summary>
        public int? StartupPriority { get; set; } = 0;
        /// <summary>
        /// This property specifies how the command notifies the communication container when a new command is registered and becomes active.
        /// </summary>
        public CommandNotificationBehaviour CommandNotify { get; set; } = CommandNotificationBehaviour.OnRegistration;

        /// <summary>
        /// This boolean property specifies whether outgoing requests are enables.
        /// </summary>
        public virtual bool OutgoingRequestsEnabled { get; set; } = false;
        /// <summary>
        /// This is the default timeout for outgoing requests from the Command to other commands when not set in the settings.
        /// The default is 30s.
        /// </summary>
        public virtual TimeSpan OutgoingRequestMaxProcessingTimeDefault { get; set; } = TimeSpan.FromSeconds(30);
        /// <summary>
        /// Gets or sets the outgoing request default timespan, which by default is 5 seconds less than OutgoingRequestMaxProcessingTimeDefault.
        /// </summary>
        public virtual TimeSpan? OutgoingRequestDefaultTimespan { get; set; } = TimeSpan.FromSeconds(25);
        /// <summary>
        /// This is the default time out poll, which is set at an initial 10 second wait and then a repeated 5 seconds poll by default.
        /// </summary>
        public virtual ScheduleTimerConfig OutgoingRequestsTimeoutPollInterval { get; set; } = new ScheduleTimerConfig(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));

        //Master Job
        /// <summary>
        /// Gets or sets the master job negotiation strategy. The default is MasterJobNegotiationStrategy. You can set this to the MasterJobNegotiationStrategyDebug class 
        /// for a more aggressive approach to negotiating, which can be useful when debugging.
        /// </summary>
        public MasterJobNegotiationStrategyBase MasterJobNegotiationStrategy { get; set; } = new MasterJobNegotiationStrategy();
        /// <summary>
        /// Gets or sets a value indicating whether the masterjob service is enabled.
        /// </summary>
        public virtual bool MasterJobEnabled { get; set; }
        /// <summary>
        /// Gets or sets the masterjob negotiation channel priority.
        /// </summary>
        public virtual int MasterJobNegotiationChannelPriority { get; set; } = 2;
        /// <summary>
        /// Gets or sets the type of the masterjob negotiation channel message type..
        /// </summary>
        public virtual string MasterJobNegotiationChannelMessageType { get; set; }
        /// <summary>
        /// Gets or sets the name of the master job.
        /// </summary>
        public virtual string MasterJobName { get; set; }

        /// <summary>
        /// Specifies that the master job commands are supported by reflection, i.e. those that use the MasterJobCommandContractAttribute.
        /// </summary>
        public bool MasterJobCommandReflectionSupported { get; set; } = true;
        /// <summary>
        /// Specifies that schedule reflection commands are supported for master jobs, i.e. those that use the MasterJobScheduleAttribute.
        /// </summary>
        public bool MasterJobScheduleReflectionSupported { get; set; } = true;
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
        /// Gets or sets a value indicating whether the masterjob command contract attribute should include inherited methods.
        /// </summary>
        public bool MasterJobCommandContractAttributeInherit { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the masterjob schedule attribute should include inherited methods.
        /// </summary>
        public bool MasterJobScheduleAttributeInherit { get; set; } = true;
    }
}
