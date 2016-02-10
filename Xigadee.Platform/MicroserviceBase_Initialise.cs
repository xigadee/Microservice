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
    public partial class MicroserviceBase
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
        /// THis method returns the default scheduler container.
        /// </summary>
        /// <returns>The default scheduler.</returns>
        protected virtual SchedulerContainer InitialiseSchedulerContainer()
        {
            return new SchedulerContainer(PolicyScheduler());
        }

        protected virtual SchedulerPolicy PolicyScheduler()
        {
            return new SchedulerPolicy();;
        }
        #endregion    

        #region InitialiseEventSourceContainer(List<IEventSource> eventSources)
        /// <summary>
        /// THis method returns the default scheduler container.
        /// </summary>
        /// <returns>The default scheduler.</returns>
        protected virtual EventSourceContainer InitialiseEventSourceContainer(List<IEventSource> eventSources)
        {
            return new EventSourceContainer(eventSources);
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
