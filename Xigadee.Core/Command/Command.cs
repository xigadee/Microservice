#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public abstract class CommandBase: CommandBase<CommandStatistics, CommandPolicy>
    {
        public CommandBase(CommandPolicy policy = null) : base(policy)
        {
        }
    }

    public abstract class CommandBase<S>: CommandBase<S, CommandPolicy>
        where S : CommandStatistics, new()
    {
        public CommandBase(CommandPolicy policy = null) : base(policy)
        {
        }
    }
    /// <summary>
    /// This command is the base implementation that allows multiple commands to be handled 
    /// within a single container.
    /// </summary>
    public abstract partial class CommandBase<S,P>: ServiceBase<S>, ICommand
        where S : CommandStatistics, new()
        where P : CommandPolicy, new()
    {
        #region Declarations
        /// <summary>
        /// This is the command policy.
        /// </summary>
        protected readonly P mPolicy;
        /// <summary>
        /// This is the concurrent dictionary that contains the supported commands.
        /// </summary>
        protected Dictionary<MessageFilterWrapper, CommandHandler> mSupported;
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
        public CommandBase(P policy = null)
        {
            mPolicy = policy ?? new P();
            mSupported = new Dictionary<MessageFilterWrapper, CommandHandler>();
            mSchedules = new List<Schedule>();

            if (mPolicy.StartupPriority.HasValue)
                StartupPriority = mPolicy.StartupPriority.Value;

            if (mPolicy.MasterJobEnabled)
                MasterJobInitialise();

            if (mPolicy.JobPollEnabled)
                TimerPollSchedulesRegister();   
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
        }

        protected override void StopInternal()
        {
            if (mPolicy.OutgoingRequestsEnabled)
                OutgoingRequestsTimeoutStop();
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
        #region Dispatcher
        /// <summary>
        /// This is the link to the Microservice dispatcher.
        /// </summary>
        public Action<IService, TransmissionPayload> Dispatcher
        {
            get;
            set;
        }
        #endregion

        #region Items
        /// <summary>
        /// This returns the list of handlers for logging purposes.
        /// </summary>
        public IEnumerable<CommandHandler> Items
        {
            get
            {
                return mSupported.Values;
            }
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


