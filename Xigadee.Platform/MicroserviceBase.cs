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
    public partial class MicroserviceBase : ServiceBase<MicroserviceStatistics>
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
        /// This container holds the components that do work on the system.
        /// </summary>
        protected CommandContainer mComponents;
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
        public MicroserviceBase(MicroserviceConfigurationOptions options = null
            , string name = null, string serviceId = null)
            : base(name)
        {
            mStartTime = DateTime.UtcNow;
            mMachineName = Environment.MachineName;

            mServiceId = string.IsNullOrEmpty(serviceId)?Guid.NewGuid().ToString("N").ToUpperInvariant(): serviceId;
            mName = string.IsNullOrEmpty(name)? GetType().Name:name;

            mExternalServiceId = string.Format("{0}_{1}_{2:yyyyMMddHHmm}_{3}", mName, mMachineName, mStartTime, mServiceId);

            ConfigurationOptions = options ?? new MicroserviceConfigurationOptions();

            mServiceVersionId = Assembly.GetCallingAssembly().GetName().Version.ToString();
            mServiceEngineVersionId = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            mCommunication = InitialiseCommunicationContainer();
            mComponents = InitialiseCommandsContainer();
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

            mAutotuneTasksMinConcurrent = ConfigurationOptions.ConcurrentRequestsMin;
            mAutotuneTasksMaxConcurrent = ConfigurationOptions.ConcurrentRequestsMax;

            mAutotuneOverloadTasksConcurrent = ConfigurationOptions.OverloadProcessLimitMax;
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
                , ConfigurationOptions.StatusLogFrequency, "Status Poll", TimeSpan.FromSeconds(15), isInternal:true);

            //Set the status log frequency.
            mScheduler.Register(async (s, cancel) => await Autotune()
                , TimeSpan.FromSeconds(1), "Autotune", TimeSpan.FromSeconds(10), isInternal: true);

            // Flush the accumulated telemetry 
            mScheduler.Register(async (s, cancel) => await mTelemetry.Flush()
                , TimeSpan.FromMinutes(15), "Telemetry Flush", TimeSpan.FromSeconds(10), isInternal: true);

            // Flush the accumulated telemetry 
            mScheduler.Register(async (s, cancel) => await TaskTimedoutKill()
                , TimeSpan.FromMinutes(1), "Tasks timed out kill", TimeSpan.FromSeconds(1), isInternal: true);
        }
        #endregion

        #region StartInternal()
        /// <summary>
        /// This override sends the service start message.
        /// </summary>
        /// <param name="args">The incoming arguments.</param>
        protected override void StartInternal()
        {
            OnStartRequested();

            //This method initialises the configuration.
            ConfigurationInitialise();

            //This initialises the process loop.
            ProcessLoopInitialise();

            //This method populates the components in the service.
            ComponentsPopulate();

            //This method starts the components in the service in the correct order.
            try
            {
                //Start the logger components.
                ServiceStart(mLogger);

                //OK, register the resource tracker
                ServiceStart(mResourceTracker);

                //Start the event source components.
                ServiceStart(mEventSource);

                //This method connects any components that require Shared Service together before they start.
                mComponents.SharedServicesConnect();

                //Ensure that the communication handler is working.
                ServiceStart(mCommunication);

                //Start the senders
                mCommunication.SendersStart();

                //Ensure that any handlers are registered.
                ServiceStart(mComponents);

                //Start the handlers in decending priority
                mComponents.Commands
                    .OrderByDescending((h) => h.StartupPriority)
                    .ForEach(h => ServiceStart(h));

                //OK, start the loop to start processing requests and picking up messages from the listeners.
                ProcessLoopStart();

                //Now start the listeners and deadletter listeners
                mCommunication.ListenersStart();

                //Finally register the housekeeping schedules.
                SchedulesRegister();

                LogStatistics().Wait(TimeSpan.FromSeconds(1));

                OnStartCompleted();
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

            mCommunication.ListenersStop();

            //Ok, stop the commands.
            mComponents.Commands
                .OrderBy((h) => h.StartupPriority)
                .ForEach(h => ServiceStop(h));

            ServiceStop(mComponents);

            ProcessLoopStop();

            //Stop the sender
            mCommunication.SendersStop();

            ServiceStop(mCommunication);
            
            if (mPayloadSerializers != null)
                mPayloadSerializers.Clear();

            ServiceStop(mEventSource);

            ServiceStop(mResourceTracker);

            ServiceStop(mLogger);

            OnStopCompleted();
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
                if (service is IRequireScheduler)
                    ((IRequireScheduler)service).Scheduler = mScheduler;

                if (service is IRequireSharedServices)
                    ((IRequireSharedServices)service).SharedServices = SharedServices;

                if (service is IContainerService)
                    ((IContainerService)service).Services.ForEach(s => ServiceStart(s));

                if (service is IServiceLogger)
                    ((IServiceLogger)service).Logger = mLogger;

                if (service is IServiceOriginator)
                    ((IServiceOriginator)service).OriginatorId = ExternalServiceId;

                //Set the transmission action.
                if (service is IServiceInitiator)
                    ((IServiceInitiator)service).Dispatcher = ExecuteOrEnqueue;

                if (service is IServiceEventSource)
                    ((IServiceEventSource)service).EventSource = mEventSource;

                if (service is IPayloadSerializerConsumer)
                    ((IPayloadSerializerConsumer)service).PayloadSerializer = mSerializer;

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
            if (service is IContainerService)
                ((IContainerService)service).Services.ForEach(s => ServiceStop(s));

            base.ServiceStop(service);

            if (service is IServiceInitiator)
                ((IServiceInitiator)service).Dispatcher = null;

            if (service is IServiceEventSource)
            {
                ((IServiceEventSource)service).EventSource = null;
            }

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
