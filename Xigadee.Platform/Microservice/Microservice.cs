#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
    public partial class Microservice: ServiceBase<MicroserviceStatistics>, IMicroservice
    {
        #region Declarations
        /// <summary>
        /// This collection holds the serializer
        /// </summary>
        protected SerializationContainer mSerializer;
        /// <summary>
        /// This collection holds the loggers for the Microservice.
        /// </summary>
        protected DataCollectionContainer mDataCollection;
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
        /// This class is used to track resource starvation and to rate limit incoming requests.
        /// </summary>
        protected ResourceTracker mResourceTracker;
        /// <summary>
        /// This contains the supported serializers.
        /// </summary>
        protected Dictionary<byte[],IPayloadSerializer> mPayloadSerializers;
        /// <summary>
        /// This is the collection of policy settings for the Microservice.
        /// </summary>
        protected List<PolicyBase> mPolicySettings;

        #endregion
        #region Constructors
        /// <summary>
        /// This is the Microservice constructor.
        /// </summary>
        /// <param name="name">The Microservice name.</param>
        /// <param name="serviceId">The service id.</param>
        /// <param name="policySettings">The policy settings.</param>
        /// <param name="properties">Any additional property key.</param>
        public Microservice(
              string name = null
            , string serviceId = null
            , IEnumerable<PolicyBase> policySettings = null
            , IEnumerable<Tuple<string,string>> properties = null)
            : base(name)
        {
            mPolicySettings = policySettings?.ToList()??new List<PolicyBase>();

            if (string.IsNullOrEmpty(name))
                name = GetType().Name;

            Id = new MicroserviceId(name
                , serviceId: serviceId
                , serviceVersionId: Assembly.GetCallingAssembly().GetName().Version.ToString()
                , serviceEngineVersionId: Assembly.GetExecutingAssembly().GetName().Version.ToString()
                , properties: properties);

            mSecurity = InitialiseSecurityContainer();
            mCommunication = InitialiseCommunicationContainer();
            mCommands = InitialiseCommandContainer();
            mResourceTracker = InitialiseResourceTracker();
            mDataCollection = InitialiseDataCollectionContainer();
            mPayloadSerializers = new Dictionary<byte[], IPayloadSerializer>();
        }
        #endregion

        #region Id
        /// <summary>
        /// This contains the set of identifiers for the Microservice.
        /// </summary>
        public MicroserviceId Id { get; } 
        #endregion

        #region ConfigurationInitialise()
        /// <summary>
        /// This method is used to set the dynamic system configuration parameters such and max and min concurrent jobs
        /// before the core system starts.
        /// </summary>
        protected virtual void ConfigurationInitialise()
        {
            var tmPolicy = PolicyTaskManager;

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
                , PolicyMicroservice.FrequencyStatisticsGeneration, "Status Poll", TimeSpan.FromSeconds(0), isInternal:true);

            //Set the status log frequency.
            if (PolicyMicroservice.FrequencyAutotune.HasValue)
                mScheduler.Register(async (s, cancel) => await mTaskManager.Autotune()
                    , PolicyMicroservice.FrequencyAutotune.Value, "Autotune", TimeSpan.FromSeconds(10), isInternal: true);

            // Flush the accumulated telemetry 
            mScheduler.Register(async (s, cancel) => await mDataCollection.Flush()
                , PolicyMicroservice.FrequencyDataCollectionFlush, "Data Collection Flush", TimeSpan.FromSeconds(10), isInternal: true);

            // Kills any overrunning tasks
            mScheduler.Register(async (s, cancel) => await mTaskManager.TaskTimedoutKill()
                , PolicyMicroservice.FrequencyTasksTimeout, "Tasks timed-out: kill", TimeSpan.FromSeconds(1), isInternal: true);
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

                //This method initialises the serialization container.
                EventStart(() => ServiceStart(mSerializer), "Serialization");

                //Start data collection.
                EventStart(() => ServiceStart(mDataCollection), "Data Collection");

                //OK, register the resource tracker
                EventStart(() => ServiceStart(mResourceTracker), "Resource Tracker");

                //This method connects any components that require Shared Service together before they start.
                EventStart(() => mCommands.SharedServicesConnect(), "Command Shared Services");

                //Start the channel controller.
                EventStart(() => ServiceStart(mSecurity), "Security Container");

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
            EventStop(() => ServiceStop(mSecurity), "Security Container");

            EventStop(() => ServiceStop(mResourceTracker), "Resource Tracker");

            EventStop(() => ServiceStop(mDataCollection), "Data Collection");

            EventStop(() => ServiceStop(mSerializer), "Serialization");

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
                if (service is IRequireDataCollector)
                    ((IRequireDataCollector)service).Collector = mDataCollection;

                if (service is IServiceOriginator)
                    ((IServiceOriginator)service).OriginatorId = Id;

                if (service is IServiceEventSource)
                    ((IServiceEventSource)service).EventSource = mDataCollection;

                if (service is IPayloadSerializerConsumer)
                    ((IPayloadSerializerConsumer)service).PayloadSerializer = mSerializer;

                if (service is IRequireScheduler)
                    ((IRequireScheduler)service).Scheduler = mScheduler;

                if (service is IRequireSecurityService)
                    ((IRequireSecurityService)service).Security = mSecurity;

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

        #region Name
        /// <summary>
        /// This is the public name of the service.
        /// </summary>
        public string Name
        {
            get
            {
                return Id.Name;
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
                return Id.ServiceId;
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
                return Id.ExternalServiceId;
            }
        }
        #endregion

        #region Collector
        /// <summary>
        /// This is the internal Collector?. This is exposed to allow external services to log using the Microservice logging services.
        /// </summary>
        public IDataCollection Collector { get { return mDataCollection; } } 
        #endregion
    }
}
