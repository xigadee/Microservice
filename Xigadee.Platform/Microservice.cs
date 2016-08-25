#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class enables the communication fabric to be plugged in to existing code with minimal effort.
    /// </summary>
    public partial class Microservice : ServiceBase<MicroserviceStatistics>
    {
        #region Declarations
        /// <summary>
        /// This is the Microservice create time.
        /// </summary>
        private readonly DateTime mStartTime;
        /// <summary>
        /// This class contains the common configuration options for the Microservice.
        /// </summary>
        public MicroserviceConfigurationOptions ConfigurationOptions { get; set; }
        /// <summary>
        /// This collection holds the serializer
        /// </summary>
        protected SerializationContainer mSerializer;
        /// <summary>
        /// This collection holds the event sources for the Microservice.
        /// </summary>
        protected EventSourceContainer mEventSource;
        /// <summary>
        /// This collection holds the telemetry components.
        /// </summary>
        protected TelemetryContainer mTelemetry;
        /// <summary>
        /// This collection holds the channels.
        /// </summary>
        protected ChannelContainer mChannels;
        /// <summary>
        /// This container is used to hold the security infrastructure for the Microservice.
        /// </summary>
        protected SecurityContainer mSecurity;
        /// <summary>
        /// This container holds the components that do work on the system.
        /// </summary>
        protected CommandContainer mCommands;
        /// <summary>
        /// This container holds the communication components.
        /// </summary>
        protected CommunicationContainer mCommunication;
        /// <summary>
        /// This collection holds the loggers for the Microservice.
        /// </summary>
        protected LoggerContainer mLogger;
        /// <summary>
        /// This class is used to track resource starvation and to rate limit incoming requests.
        /// </summary>
        protected ResourceTracker mResourceTracker;
        /// <summary>
        /// These are the message loggers for the Microservice.
        /// </summary>
        protected List<ILogger> mLoggers;
        /// <summary>
        /// These are the telemetry loggers for the Microservice.
        /// </summary>
        protected List<ITelemetry> mTelemetries;
        /// <summary>
        /// These are the event sources used for logging the incoming and outgoing messages.
        /// </summary>
        protected List<IEventSource> mEventSources;
        /// <summary>
        /// This contains the supported serializers.
        /// </summary>
        protected List<IPayloadSerializer> mPayloadSerializers;
        /// <summary>
        /// This is the collection of policy settings for the Microservice.
        /// </summary>
        protected List<PolicyBase> mPolicySettings;

        protected readonly string mServiceVersionId;
        protected readonly string mServiceEngineVersionId;

        protected readonly string mServiceId;
        protected readonly string mMachineName;
        protected readonly string mName;
        protected readonly string mExternalServiceId;
        #endregion
        #region Constructors
        /// <summary>
        /// This is the default Unity ServiceLocator based constructor.
        /// </summary>
        public Microservice(MicroserviceConfigurationOptions options = null
            , string name = null, string serviceId = null, IEnumerable<PolicyBase> policySettings = null)
            : base(name)
        {
            mPolicySettings = policySettings?.ToList()??new List<PolicyBase>();

            mStartTime = DateTime.UtcNow;
            mMachineName = Environment.MachineName;

            mServiceId = string.IsNullOrEmpty(serviceId)?Guid.NewGuid().ToString("N").ToUpperInvariant(): serviceId;

            mName = string.IsNullOrEmpty(name)? GetType().Name:name;

            mExternalServiceId = string.Format("{0}_{1}_{2:yyyyMMddHHmm}_{3}", mName, mMachineName, mStartTime, mServiceId);

            ConfigurationOptions = options ?? new MicroserviceConfigurationOptions();

            mServiceVersionId = Assembly.GetCallingAssembly().GetName().Version.ToString();
            mServiceEngineVersionId = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            mSecurity = InitialiseSecurityContainer();
            mChannels = InitialiseChannelContainer();
            mCommunication = InitialiseCommunicationContainer();
            mCommands = InitialiseCommandContainer();
            mResourceTracker = InitialiseResourceTracker();

            mTelemetries = new List<ITelemetry>();
            mLoggers = new List<ILogger>();
            mPayloadSerializers = new List<IPayloadSerializer>();
            mEventSources = new List<IEventSource>();
        }
        #endregion

        #region ConfigurationInitialise()
            /// <summary>
            /// This method is used to set the dynamic system configuration parameters such and max and min concurrent jobs
            /// before the core system starts.
            /// </summary>
        protected virtual void ConfigurationInitialise()
        {
            //Do some sanity checking on the Max/Min settings. 
            if (ConfigurationOptions.ConcurrentRequestsMin < 0)
                ConfigurationOptions.ConcurrentRequestsMin = 0;

            if (ConfigurationOptions.ConcurrentRequestsMax < ConfigurationOptions.ConcurrentRequestsMin)
                ConfigurationOptions.ConcurrentRequestsMax = ConfigurationOptions.ConcurrentRequestsMin;

            if (ConfigurationOptions.ConcurrentRequestsMax == 0)
                ConfigurationOptions.ConcurrentRequestsMax = 1;

            //mAutotuneTasksMinConcurrent = ConfigurationOptions.ConcurrentRequestsMin;
            //mAutotuneTasksMaxConcurrent = ConfigurationOptions.ConcurrentRequestsMax;

            //mAutotuneOverloadTasksConcurrent = ConfigurationOptions.OverloadProcessLimitMax;
        } 
        #endregion

        #region Dispose(bool disposing)
        /// <summary>
        /// This override desposes of the event sources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                mEventSources = null;

            base.Dispose(disposing);
        }
        #endregion

        #region SchedulesRegister()
        /// <summary>
        /// This method is used to register the housekeeping jobs for the Microservice.
        /// </summary>
        protected virtual void SchedulesRegister()
        {
            //Set the status log frequency.
            mScheduler.Register(async (s, cancel) => await LogStatistics()
                , ConfigurationOptions.StatusLogFrequency, "Status Poll", TimeSpan.FromSeconds(0), isInternal:true);

            //Set the status log frequency.
            mScheduler.Register(async (s, cancel) => await mTaskManager.Autotune()
                , TimeSpan.FromSeconds(1), "Autotune", TimeSpan.FromSeconds(10), isInternal: true);

            // Flush the accumulated telemetry 
            mScheduler.Register(async (s, cancel) => await mTelemetry.Flush()
                , TimeSpan.FromMinutes(15), "Telemetry Flush", TimeSpan.FromSeconds(10), isInternal: true);

            // Kills any overrunning tasks
            mScheduler.Register(async (s, cancel) => await mTaskManager.TaskTimedoutKill()
                , TimeSpan.FromMinutes(1), "Tasks timed out kill", TimeSpan.FromSeconds(1), isInternal: true);
        }
        #endregion

        #region StartInternal()
        /// <summary>
        /// This override sends the service start message.
        /// </summary>
        protected override void StartInternal()
        {

            //This method starts the components in the service in the correct order.
            try
            {
                OnStartRequested();

                //This method initialises the configuration.
                EventStart(() => ConfigurationInitialise(), "Configuration");

                //This initialises the process loop.
                EventStart(() => TaskManagerInitialise(), "Task Manager Initialization");

                //This method populates the components in the service.
                EventStart(() => PopulateComponents(), "Components");

                //Start the logger components.
                EventStart(() => ServiceStart(mLogger), "Logger");

                //OK, register the resource tracker
                EventStart(() => ServiceStart(mResourceTracker), "Resource Tracker");

                //Start the event source components.
                EventStart(() => ServiceStart(mEventSource), "Event Source");

                //This method connects any components that require Shared Service together before they start.
                EventStart(() => mCommands.SharedServicesConnect(), "Shared Services");

                //Start the channel controller.
                EventStart(() => ServiceStart(mChannels), "Channel Container");

                //Ensure that the communication handler is working.
                EventStart(() => ServiceStart(mCommunication), "Communication Container");

                //Start the senders
                EventStart(() => mCommunication.SendersStart(), "Senders");

                //Ensure that any handlers are registered.
                EventStart(() => ServiceStart(mCommands), "Commands Container");

                //OK, start the loop to start processing requests and picking up messages from the listeners.
                EventStart(() => TaskManagerStart(), "Task Manager");

                //Finally register the housekeeping schedules.
                EventStart(() => SchedulesRegister(), "Scheduler");

                //Now start the listeners and deadletter listeners
                EventStart(() => mCommunication.ListenersStart(), "Communication Listeners");

                //Ok start the commands in parallel at the same priority group.
                EventStart(() => mCommands.CommandsStart(ServiceStart), "Commands");

                //Signal that start has completed.
                OnStartCompleted();

                //Do a final log now that everything has started up.
                LogStatistics().Wait(TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                try
                {
                    //Just try and tidy up where possible.
                    StopInternal();
                }
                catch (Exception)
                {
                    // Nothing do be done here
                }

                //Throw the original exception.
                mLogger.LogException("StartInternal unhandled exception thrown - service is stopping", ex);
            }
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This override sends the service stop message.
        /// </summary>
        protected override void StopInternal()
        {
            OnStopRequested();

            LogStatistics().Wait(TimeSpan.FromSeconds(15));

            EventStop(() => mCommands.CommandsStop(ServiceStop), "Commands");

            EventStop(() => mCommunication.ListenersStop(), "Communication Listeners");

            EventStop(() => ServiceStop(mCommands), "Command Container");

            EventStop(() => TaskManagerStop(), "Task Manager");

            //Stop the sender
            EventStop(() => mCommunication.SendersStop(), "Communication Senders");

            EventStop(() => ServiceStop(mCommunication), "Communication Container");

            //Stop the channel controller.
            EventStop(() => ServiceStop(mChannels), "Channel Container");

            if (mPayloadSerializers != null)
                mPayloadSerializers.Clear();

            EventStop(() => ServiceStop(mEventSource), "Event Source");

            EventStop(() => ServiceStop(mResourceTracker), "Resource Tracker");

            EventStop(() => ServiceStop(mLogger), "Logger");

            OnStopCompleted();
        }
        #endregion

        #region Event wrappers...
        /// <summary>
        /// This wrapper is used for starting
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="title">The section title.</param>
        protected virtual void EventStart(Action action, string title)
        {
            EventGeneric(action, title, MicroserviceStatusChangeAction.Starting);
        }
        /// <summary>
        /// This wrapper is used for stopping
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="title">The section title.</param>
        protected virtual void EventStop(Action action, string title)
        {
            EventGeneric(action, title, MicroserviceStatusChangeAction.Stopping);
        }
        /// <summary>
        /// This is the generic exception wrapper.
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="title">The section title.</param>
        /// <param name="type">The action type, i.e. starting or stopping.</param>
        protected virtual void EventGeneric(Action action, string title, MicroserviceStatusChangeAction type)
        {
            var args = new MicroserviceStatusEventArgs(type, title);

            try
            {
                ComponentStatusChange?.Invoke(this, args);
                action();
                args.State = MicroserviceStatusChangeState.Completed;
                ComponentStatusChange?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                args.Ex = new MicroserviceStatusChangeException(title, ex);
                args.State = MicroserviceStatusChangeState.Failed;
                ComponentStatusChange?.Invoke(this, args);
                throw args.Ex;
            }
        } 
        #endregion

        #region ServiceStart(object service)
        /// <summary>
        /// This method populates the service with system components and then starts the service.
        /// </summary>
        /// <param name="service">The service to index.</param>
        protected override void ServiceStart(object service)
        {
            try
            {
                if (service is IServiceLogger)
                    ((IServiceLogger)service).Logger = mLogger;

                if (service is IServiceOriginator)
                    ((IServiceOriginator)service).OriginatorId = ExternalServiceId;

                if (service is IServiceEventSource)
                    ((IServiceEventSource)service).EventSource = mEventSource;

                if (service is IPayloadSerializerConsumer)
                    ((IPayloadSerializerConsumer)service).PayloadSerializer = mSerializer;

                if (service is IRequireScheduler)
                    ((IRequireScheduler)service).Scheduler = mScheduler;

                if (service is IRequireSharedServices)
                    ((IRequireSharedServices)service).SharedServices = SharedServices;

                if (service is IContainerService)
                    ((IContainerService)service).Services.ForEach(s => ServiceStart(s));

                //Set the transmission action.
                if (service is ICommand)
                    ((ICommand)service).TaskManager = mTaskManager.ExecuteOrEnqueue;

                base.ServiceStart(service);
            }
            catch (Exception ex)
            {
                mLogger.LogException(string.Format("Service Start failed for service {0}", service.GetType().Name), ex);
                throw;
            }
        }

        #endregion
        #region ServiceStop(object service)
        /// <summary>
        /// This override clears the dispatcher service.
        /// </summary>
        /// <param name="service">The service to stop.</param>
        protected override void ServiceStop(object service)
        {
            (service as IContainerService)?.Services.ForEach(ServiceStop);
            base.ServiceStop(service);
        }
        #endregion

        #region Name
        /// <summary>
        /// This is the public name of the service.
        /// </summary>
        public string Name
        {
            get
            {
                return mName;
            }
        }
        #endregion
        #region ServiceId
        /// <summary>
        /// This is the public ServiceId of the service.
        /// </summary>
        public string ServiceId
        {
            get
            {
                return mServiceId;
            }
        }
        #endregion
        #region ExternalServiceId
        /// <summary>
        /// This is the service id used for communication.
        /// </summary>
        public virtual string ExternalServiceId
        {
            get
            {
                return mExternalServiceId;
            }
        } 
        #endregion

        #region Logger
        /// <summary>
        /// This is the internal logger. This is exposed to allow external services to log using the Microservice logging services.
        /// </summary>
        public ILoggerExtended Logger { get { return mLogger; } } 
        #endregion
    }
}
