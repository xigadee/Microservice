#region using
using System;
using System.Collections.Generic;
#endregion
namespace Xigadee
{
    #region CommandBase
    /// <summary>
    /// This is the standard command base constructor.
    /// </summary>
    public abstract class CommandBase: CommandBase<CommandStatistics>
    {
        public CommandBase(CommandPolicy policy = null) : base(policy)
        {
        }
    }
    #endregion
    #region CommandBase<S>
    /// <summary>
    /// This is the extended command constructor that allows for custom statistics.
    /// </summary>
    /// <typeparam name="S">The statistics class type.</typeparam>
    public abstract class CommandBase<S>: CommandBase<S, CommandPolicy>
        where S : CommandStatistics, new()
    {
        public CommandBase(CommandPolicy policy = null) : base(policy)
        {
        }
    }
    #endregion
    #region CommandBase<S,P>
    /// <summary>
    /// This is the extended command constructor that allows for custom statistics.
    /// </summary>
    /// <typeparam name="S">The statistics class type.</typeparam>
    /// <typeparam name="P">The customer command policy.</typeparam>
    public abstract class CommandBase<S,P>: CommandBase<S, P, CommandHandler<CommandHandlerStatistics>>
        where S : CommandStatistics, new()
        where P : CommandPolicy, new()
    {
        public CommandBase(P policy = null) : base(policy)
        {
        }
    }
    #endregion

    /// <summary>
    /// This is the default custom command class that allows for full customization in policy and statistics.
    /// </summary>
    /// <typeparam name="S">The statistics class type.</typeparam>
    /// <typeparam name="P">The customer command policy.</typeparam>
    /// <typeparam name="H">The command handler type.</typeparam>
    public abstract partial class CommandBase<S,P,H>: ServiceBase<S>, ICommand
        where S : CommandStatistics, new()
        where P : CommandPolicy, new()
        where H : class, ICommandHandler, new()
    {
        #region Declarations
        /// <summary>
        /// This is the command policy.
        /// </summary>
        protected readonly P mPolicy;
        /// <summary>
        /// This is the concurrent dictionary that contains the supported commands.
        /// </summary>
        protected Dictionary<MessageFilterWrapper, H> mSupported;
        /// <summary>
        /// This event is used by the component container to discover when a command is registered or deregistered.
        /// Implement IMessageHandlerDynamic to enable this feature.
        /// </summary>
        public event EventHandler<CommandChange> OnCommandChange;
        /// <summary>
        /// This is the job timer
        /// </summary>
        protected List<Schedule> mSchedules;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor that calls the CommandsRegister function.
        /// </summary>
        protected CommandBase(P policy = null)
        {
            mPolicy = PolicyCreateOrValidate(policy);

            mCurrentMasterPollAttempts = 0;
            mSupported = new Dictionary<MessageFilterWrapper, H>();
            mSchedules = new List<Schedule>();

            StartupPriority = mPolicy.StartupPriority ?? 0;

            if (mPolicy.MasterJobEnabled)
                MasterJobInitialise();

            if (mPolicy.JobPollEnabled)
                TimerPollSchedulesRegister();   
        }
        #endregion

        #region PolicyCreateOrValidate(P incomingPolicy)
        /// <summary>
        /// This method ensures that a policy object exists for the command. You should override this method to set any
        /// default configuration properties.
        /// </summary>
        /// <returns>Returns the incoming policy or creates a default policy if this is not set..</returns>
        protected virtual P PolicyCreateOrValidate(P incomingPolicy)
        {
            return incomingPolicy ?? new P();
        } 
        #endregion

        #region StartInternal/StopInternal
        protected override void StartInternal()
        {
            try
            {
                if (mPolicy.OutgoingRequestsEnabled)
                    OutgoingRequestsTimeoutStart();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            mSupported.Keys
                .ForEach((k) =>
                {

                    try
                    {
                        OnCommandChange?.Invoke(this, new CommandChange(false, k));
                    }
                    catch (Exception)
                    {
                    }
                });
        }

        protected override void StopInternal()
        {
            if (mPolicy.OutgoingRequestsEnabled)
                OutgoingRequestsTimeoutStop();

            mSupported.Keys
                .ForEach((k) =>
                {

                    try
                    {
                        OnCommandChange?.Invoke(this, new CommandChange(true, k));
                    }
                    catch (Exception)
                    {
                    }
                });
        }
        #endregion

        #region PayloadSerializer
        /// <summary>
        /// This is the requestPayload serializer used across the system.
        /// </summary>
        public IPayloadSerializationContainer PayloadSerializer
        {
            get;
            set;
        }
        #endregion

        #region EventSource
        /// <summary>
        /// This is the event source writer.
        /// </summary>
        public IEventSource EventSource
        {
            get;
            set;
        }
        #endregion
        #region OriginatorId
        /// <summary>
        /// This is the service originator Id.
        /// </summary>
        public string OriginatorId
        {
            get;
            set;
        }
        #endregion
        #region Logger
        /// <summary>
        /// This is the logger for the message handler.
        /// </summary>
        public ILoggerExtended Logger
        {
            get;
            set;
        }
        #endregion
        #region Scheduler
        /// <summary>
        /// This is the scheduler. It is needed to process request timeouts.
        /// </summary>
        public virtual IScheduler Scheduler
        {
            get; set;
        }
        #endregion

        #region StartupPriority
        /// <summary>
        /// This is the message handler priority used when starting up.
        /// </summary>
        public int StartupPriority
        {
            get; set;
        }
        #endregion
    }
}


