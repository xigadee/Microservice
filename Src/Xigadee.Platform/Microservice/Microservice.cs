#region using
using System;
using System.Collections.Generic;
using System.Reflection;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class enables the communication fabric to be plugged in to existing code with minimal effort.
    /// </summary>
    public partial class Microservice: ServiceBase<Microservice.Statistics>, IMicroservice
    {
        #region Declarations
        /// <summary>
        /// This collection holds the loggers for the Microservice.
        /// </summary>
        protected DataCollectionContainer mDataCollection;
        /// <summary>
        /// This container is used to hold the service handler infrastructure for the Microservice.
        /// </summary>
        protected ServiceHandlerContainer mServiceHandlers;
        /// <summary>
        /// This container holds the components that do work on the system.
        /// </summary>
        protected CommandContainer mCommands;
        /// <summary>
        /// This container holds the communication components.
        /// </summary>
        protected CommunicationContainer mCommunication;
        /// <summary>
        /// This class is used to track resource starvation and to rate limit incoming requests.
        /// </summary>
        protected ResourceContainer mResourceMonitor;
        /// <summary>
        /// This wrapper holds the events for the Microservice.
        /// </summary>
        EventsWrapper mEventsWrapper;
        /// <summary>
        /// This class contains the running tasks and provides a breakdown of the current availability for new tasks.
        /// </summary>
        private TaskManager mTaskManager;
        /// <summary>
        /// This is the scheduler container.
        /// </summary>
        private SchedulerContainer mScheduler;
        #endregion
        #region Constructors
        /// <summary>
        /// This is the Microservice constructor.
        /// </summary>
        /// <param name="name">The Microservice name.</param>
        /// <param name="serviceId">The service id.</param>
        /// <param name="description">An optional description for the Microservice.</param>
        /// <param name="policySettings">The policy settings.</param>
        /// <param name="properties">Any additional property key.</param>
        /// <param name="serviceVersionId">This is the version id of the calling assembly as a string.</param>
        public Microservice(
              string name = null
            , string serviceId = null
            , string description = null
            , IEnumerable<PolicyBase> policySettings = null
            , IEnumerable<Tuple<string,string>> properties = null
            , string serviceVersionId = null
            )
            : base(name)
        {
            Policies = new PolicyWrapper(policySettings, () => Status);
            //Id
            if (string.IsNullOrEmpty(name))
                name = GetType().Name;

            Id = new MicroserviceId(name
                , serviceId: serviceId
                , description: description
                , serviceVersionId: serviceVersionId ?? Assembly.GetCallingAssembly().GetName().Version.ToString()
                , serviceEngineVersionId: GetType().Assembly.GetName().Version.ToString()
                , properties: properties);

            //Service Handlers
            mServiceHandlers = InitialiseServiceHandlerContainer();
            ServiceHandlers = new ServiceHandlerWrapper(mServiceHandlers, () => Status);
            //Communication
            mCommunication = InitialiseCommunicationContainer();
            Communication = new CommunicationWrapper(mCommunication, () => Status);
            //Commands
            mCommands = InitialiseCommandContainer();
            Commands = new CommandWrapper(mCommands, () => Status);
            //Resources
            mResourceMonitor = InitialiseResourceMonitor();
            ResourceMonitor = new ResourceWrapper(mResourceMonitor, () => Status);
            //Data Collection
            mDataCollection = InitialiseDataCollectionContainer();
            DataCollection = new DataCollectionWrapper(mDataCollection, () => Status);
            //Events
            mEventsWrapper = new EventsWrapper(this, mDataCollection, () => Status);
            Events = mEventsWrapper;
        }
        #endregion

        #region ConfigurationInitialise()
        /// <summary>
        /// This method is used to set the dynamic system configuration parameters such and max and min concurrent jobs
        /// before the core system starts.
        /// </summary>
        protected virtual void ConfigurationInitialise()
        {
            var tmPolicy = Policies.TaskManager;

            //Do some sanity checking on the Max/Min settings. 
            if (tmPolicy.ConcurrentRequestsMin < 0)
                tmPolicy.ConcurrentRequestsMin = 0;

            if (tmPolicy.ConcurrentRequestsMax < tmPolicy.ConcurrentRequestsMin)
                tmPolicy.ConcurrentRequestsMax = tmPolicy.ConcurrentRequestsMin;

            if (tmPolicy.ConcurrentRequestsMax == 0)
                tmPolicy.ConcurrentRequestsMax = 1;

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
                , Policies.Microservice.FrequencyStatisticsGeneration, "Statistics: Output", TimeSpan.FromSeconds(0), isInternal:true);

            //Set the status log frequency.
            if (Policies.Microservice.FrequencyAutotune.HasValue)
                mScheduler.Register(async (s, cancel) => await mTaskManager.Autotune()
                    , Policies.Microservice.FrequencyAutotune.Value, "TaskManager: Autotune", TimeSpan.FromSeconds(10), isInternal: true);

            // Flush the accumulated telemetry 
            mScheduler.Register(async (s, cancel) => await mDataCollection.Flush()
                , Policies.Microservice.FrequencyDataCollectionFlush, "Data Collection Flush", TimeSpan.FromSeconds(10), isInternal: true);

            // Kills any overrunning tasks
            mScheduler.Register(async (s, cancel) => await mTaskManager.TaskTimedoutKill()
                , Policies.Microservice.FrequencyTasksTimeout, "TaskManager: Tasks timed-out kill", TimeSpan.FromSeconds(1), isInternal: true);
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
                mEventsWrapper.OnStartRequested();

                //This method initialises the configuration.
                EventStart(() => ConfigurationInitialise(), "Configuration");

                //This initialises the process loop.
                EventStart(() => CoreEngineInitialize(), "Core Engine Initialization");

                //This method initialises the serialization container.
                EventStart(() => ServiceStart(mServiceHandlers), "Service Handlers Container");

                //Start data collection.
                EventStart(() => ServiceStart(mDataCollection), "Data Collection");

                //OK, register the resource tracker
                EventStart(() => ServiceStart(mResourceMonitor), "Resource Tracker");

                //This method connects any components that require Shared Service together before they start.
                EventStart(() => mCommands.SharedServicesConnect(), "Command Shared Services");

                //Ensure that the communication handler is working.
                EventStart(() => ServiceStart(mCommunication), "Communication Container");

                //Start the senders
                EventStart(() => mCommunication.SendersStart(), "Communication Senders");

                //Ensure that any handlers are registered.
                EventStart(() => ServiceStart(mCommands), "Commands Container");

                //OK, start the loop to start processing requests and picking up messages from the listeners.
                EventStart(() => TaskManagerStart(), "Task Manager");

                //Finally register the housekeeping schedules.
                EventStart(() => SchedulesRegister(), "Scheduler");

                //OK start the commands in parallel at the same priority group.
                EventStart(() => mCommands.CommandsStart(ServiceStart), "Commands");

                //Now start the listeners and deadletter listeners
                EventStart(() => mCommunication.ListenersStart(), "Communication Listeners");

                //OK start the commands in parallel at the same priority group.
                EventStart(() => Dispatch = new DispatchWrapper(mServiceHandlers, mTaskManager.ExecuteOrEnqueue, () => Status
                    , Policies.TaskManager.TransmissionPayloadTraceEnabled)
                    , "Dispatch Wrapper");

                //Signal that start has completed.
                mEventsWrapper.OnStartCompleted();

                //Do a final log now that everything has started up.
                LogStatistics().Wait(TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                try
                {
                    //Throw the original exception.
                    mDataCollection?.LogException("StartInternal unhandled exception thrown - service is stopping", ex);
                    //Just try and tidy up where possible.
                    StopInternal();
                }
                catch (Exception)
                {
                    // Nothing do be done here
                }

                //Throw the original exception out to the initiating party
                throw ex;
            }
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This override sends the service stop message.
        /// </summary>
        protected override void StopInternal()
        {
            mEventsWrapper.OnStopRequested();

            LogStatistics().Wait(TimeSpan.FromSeconds(15));

            EventStop(() => mCommands.CommandsStop(ServiceStop), "Commands");

            EventStop(() => mCommunication.ListenersStop(), "Communication Listeners");

            EventStop(() => ServiceStop(mCommands), "Command Container");

            EventStop(() => TaskManagerStop(), "Task Manager");

            //Stop the sender
            EventStop(() => mCommunication.SendersStop(), "Communication Senders");

            EventStop(() => ServiceStop(mCommunication), "Communication Container");

            //Stop the channel controller.
            EventStop(() => ServiceStop(mResourceMonitor), "Resource Tracker");

            EventStop(() => ServiceStop(mDataCollection), "Data Collection");

            EventStop(() => ServiceStop(mServiceHandlers), "Service Handler Container");

            mEventsWrapper.OnStopCompleted();
        }
        #endregion

        #region EventStart/EventStop
        /// <summary>
        /// This wrapper is used for starting
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="title">The section title.</param>
        internal virtual void EventStart(Action action, string title)
        {
            mEventsWrapper.EventGeneric(action, title, MicroserviceComponentStatusChangeAction.Starting);
        }
        /// <summary>
        /// This wrapper is used for stopping
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="title">The section title.</param>
        internal virtual void EventStop(Action action, string title)
        {
            mEventsWrapper.EventGeneric(action, title, MicroserviceComponentStatusChangeAction.Stopping);
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
                if (service is IRequireDataCollector)
                    ((IRequireDataCollector)service).Collector = mDataCollection;

                if (service is IRequireServiceOriginator)
                    ((IRequireServiceOriginator)service).OriginatorId = Id;

                if (service is IRequireServiceHandlers)
                    ((IRequireServiceHandlers)service).ServiceHandlers = mServiceHandlers;

                if (service is IRequireScheduler)
                    ((IRequireScheduler)service).Scheduler = mScheduler;

                if (service is IRequireSharedServices)
                    ((IRequireSharedServices)service).SharedServices = mCommands.SharedServices;

                if (service is IContainerService)
                    ((IContainerService)service).Services.ForEach(s => ServiceStart(s));

                //Set the transmission action.
                if (service is ICommand)
                    ((ICommand)service).TaskManager = mTaskManager.ExecuteOrEnqueue;

                base.ServiceStart(service);
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException(string.Format("Service Start failed for service {0}", service.GetType().Name), ex);
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

        #region TaskManagerStart()
        /// <summary>
        /// This method starts the processing process loop.
        /// </summary>
        protected virtual void TaskManagerStart()
        {
            TaskManagerRegisterProcesses();

            ServiceStart(mTaskManager);

            ServiceStart(mScheduler);
        }
        #endregion
        #region TaskManagerStop()
        /// <summary>
        /// This method stops the process loop.
        /// </summary>
        protected virtual void TaskManagerStop()
        {
            ServiceStop(mScheduler);

            ServiceStop(mTaskManager);
        }
        #endregion
        #region TaskManagerProcessRegister()
        /// <summary>
        /// 
        /// </summary>
        protected virtual void TaskManagerRegisterProcesses()
        {
            mTaskManager.ProcessRegister("SchedulesProcess"
                , 5, mScheduler);

            mTaskManager.ProcessRegister("ListenersProcess"
                , 4, mCommunication);

            mTaskManager.ProcessRegister("Overload Check"
                , 3, mDataCollection);

        }
        #endregion

        //Identifiers
        #region Id
        /// <summary>
        /// This contains the set of identifiers for the Microservice.
        /// </summary>
        public MicroserviceId Id { get; } 
        #endregion

        #region Collector
        /// <summary>
        /// This is the internal Collector?. This is exposed to allow external services to log using the Microservice logging services.
        /// </summary>
        public IDataCollection Collector { get { return mDataCollection; } }
        #endregion

        /// <summary>
        /// This is the events collection for the microservice.
        /// </summary>
        public IMicroserviceEvents Events { get; }
        /// <summary>
        /// This is the set of policy settings for the microservice.
        /// </summary>
        public IMicroservicePolicy Policies { get; }
        /// <summary>
        /// This is the service handler wrapper.
        /// </summary>
        public IMicroserviceServiceHandlers ServiceHandlers { get; }
        /// <summary>
        /// This is the communication wrapper.
        /// </summary>
        public IMicroserviceCommunication Communication { get; }
        /// <summary>
        /// This wrapper is used to send requests direct to
        /// </summary>
        public IMicroserviceDispatch Dispatch { get; protected set;}
        /// <summary>
        /// This is the command wrapper used for interacting with the command collection.
        /// </summary>
        public IMicroserviceCommand Commands { get; }
        /// <summary>
        /// This is the data collection container.
        /// </summary>
        public IMicroserviceDataCollection DataCollection { get; } 
        /// <summary>
        /// This is the resource monitoring wrapper.
        /// </summary>
        public IMicroserviceResourceMonitor ResourceMonitor { get; }
    }
}
