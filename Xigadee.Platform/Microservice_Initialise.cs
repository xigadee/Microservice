#region using

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    //TaskManager
    public partial class Microservice
    {
        #region InitialiseResourceTracker()
        /// <summary>
        /// This method creates the default resource tracker for the Microservice.
        /// Resource trackers are used to limit incoming messages that use a particular resource
        /// that is overloaded.
        /// </summary>
        /// <returns>Returns the resource tracker.</returns>
        protected virtual ResourceTracker InitialiseResourceTracker()
        {
            return new ResourceTracker();
        } 
        #endregion

        #region InitialiseQueueTracker()
        /// <summary>
        /// This method returns the default queue trackers, which includes 4 independent priority queues.
        /// </summary>
        /// <returns>The Queue tracker container.</returns>
        protected virtual QueueTrackerContainer<QueueTracker> InitialiseQueueTracker()
        {
            return new QueueTrackerContainer<QueueTracker>(4);
        }
        #endregion

        #region InitialiseComponentContainer()
        /// <summary>
        /// This method creates the component container.
        /// This container holds the jobs, message initiators and handlers and is used to 
        /// assign incoming requests to the appropriate command.
        /// </summary>
        /// <returns>Returns the container.</returns>
        protected virtual CommandContainer InitialiseCommandsContainer()
        {
            return new CommandContainer();
        } 
        #endregion

        #region InitialiseCommunicationContainer()
        /// <summary>
        /// This method creates the communication container. This container comtains all the 
        /// listeners and senders registered on the service, and assigns priority when polling for 
        /// new incoming requests.
        /// </summary>
        /// <returns>The communication container.</returns>
        protected virtual CommunicationContainer InitialiseCommunicationContainer()
        {
            return new CommunicationContainer(PolicyCommunication());
        }
        /// <summary>
        /// This is the policy used to set the communication component settings.
        /// </summary>
        /// <returns></returns>
        protected virtual CommunicationPolicy PolicyCommunication()
        {
            return new CommunicationPolicy();
        }
        #endregion

        #region InitialiseSchedulerContainer()
        /// <summary>
        /// This method returns the default scheduler container.
        /// </summary>
        /// <returns>The default scheduler.</returns>
        protected virtual SchedulerContainer InitialiseSchedulerContainer()
        {
            return new SchedulerContainer(PolicyScheduler());
        }

        protected virtual SchedulerPolicy PolicyScheduler()
        {
            var policy = new SchedulerPolicy();
            
            //policy.DefaultPollInMs = ConfigurationOptions.

            return policy;
        }
        #endregion

        #region InitialiseTaskManager()
        /// <summary>
        /// This method creates the task manager and sets the default bulkhead reservations.
        /// </summary>
        /// <returns></returns>
        protected virtual TaskManager InitialiseTaskManager()
        {
            var taskTracker = new TaskManager(4, Execute, PolicyTaskManager());

            taskTracker.BulkheadReserve(3, 1);
            taskTracker.BulkheadReserve(2, 2);
            taskTracker.BulkheadReserve(1, 8);
            taskTracker.BulkheadReserve(0, 0);

            return taskTracker;
        }

        /// <summary>
        /// This method retrieves the policy for the task manager.
        /// </summary>
        /// <returns></returns>
        protected virtual TaskManagerPolicy PolicyTaskManager()
        {
            var policy = new TaskManagerPolicy();

            policy.ConcurrentRequestsMax = ConfigurationOptions.ConcurrentRequestsMax;
            policy.ConcurrentRequestsMin = ConfigurationOptions.ConcurrentRequestsMin;
            policy.AutotuneEnabled = ConfigurationOptions.SupportAutotune;
            policy.ProcessKillOverrunGracePeriod = ConfigurationOptions.ProcessKillOverrunGracePeriod;
            policy.ProcessorTargetLevelPercentage = ConfigurationOptions.ProcessorTargetLevelPercentage;

            return policy;
        } 
        #endregion

        #region InitialiseEventSourceContainer(List<IEventSource> eventSources)
        /// <summary>
        /// THis method returns the default scheduler container.
        /// </summary>
        /// <returns>The default scheduler.</returns>
        protected virtual EventSourceContainer InitialiseEventSourceContainer(List<IEventSource> eventSources)
        {
            return new EventSourceContainer(PolicyEventSource(), eventSources);
        }
        /// <summary>
        /// This is the eveent source policy.
        /// </summary>
        /// <returns></returns>
        protected virtual EventSourcePolicy PolicyEventSource()
        {
            return new EventSourcePolicy();
        }
        #endregion

        #region InitialiseLoggerContainer(List<ILogger> loggers)
        /// <summary>
        /// THis method returns the default scheduler container.
        /// </summary>
        /// <returns>The default scheduler.</returns>
        protected virtual LoggerContainer InitialiseLoggerContainer(List<ILogger> loggers)
        {
            return new LoggerContainer(loggers);
        }
        #endregion

        #region InitialiseTelemetryContainer(List<ITelemetry> telemetries)
        /// <summary>
        /// THis method returns the default scheduler container.
        /// </summary>
        /// <returns>The default scheduler.</returns>
        protected virtual TelemetryContainer InitialiseTelemetryContainer(List<ITelemetry> telemetries)
        {
            return new TelemetryContainer(telemetries);
        }
        #endregion

        #region InitialiseSerializationContainer(List<IPayloadSerializer> payloadSerializers)
        /// <summary>
        /// THis method returns the default scheduler container.
        /// </summary>
        protected virtual SerializationContainer InitialiseSerializationContainer(List<IPayloadSerializer> payloadSerializers)
        {
            return new SerializationContainer(payloadSerializers);
        }
        #endregion
        
    }
}
