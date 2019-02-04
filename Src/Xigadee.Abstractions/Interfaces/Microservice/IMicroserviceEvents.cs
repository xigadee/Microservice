using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface lists the events available from the Microservice.
    /// </summary>
    public interface IMicroserviceEvents
    {
        /// <summary>
        /// This event can be used to subscribe to status changes.
        /// </summary>
        event EventHandler<StatusChangedEventArgs> StatusChanged;
        /// <summary>
        /// This event handler can be used to inspect an incoming message before it executes.
        /// </summary>
        event EventHandler<TransmissionPayloadState> ExecuteBegin;
        /// <summary>
        /// This event handler can be used to inspect an incoming message after it has executed.
        /// </summary>
        event EventHandler<TransmissionPayloadState> ExecuteComplete;

        /// <summary>
        /// This event is raised when the service start begins
        /// </summary>
        event EventHandler<StartEventArgs> StartRequested;
        /// <summary>
        /// This event is raised when the service start completes.
        /// </summary>
        event EventHandler<StartEventArgs> StartCompleted;
        /// <summary>
        /// This event is raised when a stop request begins.
        /// </summary>
        event EventHandler<StopEventArgs> StopRequested;
        /// <summary>
        /// This event is raised when the service has stopped.
        /// </summary>
        event EventHandler<StopEventArgs> StopCompleted;
        /// <summary>
        /// This event is raised when the service has stopped.
        /// </summary>
        event EventHandler<MicroserviceStatusEventArgs> ComponentStatusChange;
        /// <summary>
        /// This event will be triggered when the Microservice statistics have been calculated.
        /// </summary>
        event EventHandler<StatisticsEventArgs> StatisticsIssued;
        /// <summary>
        /// This event will be thrown if an incoming message cannot be resolved against a command or an outgoing channel.
        /// </summary>
        event EventHandler<DispatcherRequestUnresolvedEventArgs> ProcessRequestUnresolved;
        /// <summary>
        /// This event will be raised when a process request errors.
        /// </summary>
        event EventHandler<ProcessRequestErrorEventArgs> ProcessRequestError;
    }
}
