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
    public abstract class CommandBase<S, P>: CommandBase<S, P, CommandHandler<CommandHandlerStatistics>>
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
    public abstract partial class CommandBase<S, P, H>: ServiceBase<S>, ICommand
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
        private List<Schedule> mSchedules;
        /// <summary>
        /// This is the shared service collection.
        /// </summary>
        protected ISharedService mSharedServices;
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
                if (mPolicy == null)
                    throw new CommandStartupException("Command policy cannot be null");

                CommandsRegister();

                if (mPolicy.OutgoingRequestsEnabled)
                    OutgoingRequestsInitialise();

                if (mPolicy.MasterJobEnabled)
                    MasterJobInitialise();

                if (mPolicy.JobPollEnabled)
                    TimerPollSchedulesRegister();

                if (mPolicy.CommandNotify == CommandNotificationBehaviour.OnStartUp)
                    CommandsNotify();
            }
            catch (Exception ex)
            {
                Logger?.LogException($"Command '{GetType().Name}' start exception", ex);
                throw ex;
            }
        }

        protected override void StopInternal()
        {
            try
            {
                mSchedules.ForEach((s) => Scheduler.Unregister(s));
                mSchedules.Clear();

                if (mPolicy.OutgoingRequestsEnabled)
                    OutgoingRequestsTimeoutStop();

                CommandsNotify(true);
            }
            catch (Exception ex)
            {
                Logger?.LogException($"Command '{GetType().Name}' stop exception", ex);
                throw;
            }
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

        #region TimerPollSchedulesRegister()
        /// <summary>
        /// This method can be overriden to enable additional schedules to be registered for the job.
        /// </summary>
        protected virtual void TimerPollSchedulesRegister()
        {

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
        #region SchedulerRegister(Schedule schedule)
        /// <summary>
        /// This method registers a schedule and adds it to the collection so that it can be 
        /// deregistered later when the command stops.
        /// </summary>
        /// <param name="schedule">The schedule to register.</param>
        protected virtual void SchedulerRegister(Schedule schedule)
        {
            mSchedules.Add(Scheduler.Register(schedule));
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

        #region SharedServices
        /// <summary>
        /// This is the shared service collection for commands that wish to share direct access to internal data.
        /// </summary>
        public virtual ISharedService SharedServices
        {
            get
            {
                return mSharedServices;
            }

            set
            {
                SharedServicesChange(value);
            }
        }
        /// <summary>
        /// This method is called to set or remove the shared service reference.
        /// You can override your logic to safely set the shared service collection here.
        /// </summary>
        /// <param name="sharedServices">The shared service reference or null if this is not set.</param>
        protected virtual void SharedServicesChange(ISharedService sharedServices)
        {
            mSharedServices = sharedServices;
        }
        #endregion
    }
}


