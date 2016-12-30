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
        event EventHandler<Microservice.TransmissionPayloadState> ExecuteBegin;
        /// <summary>
        /// This event handler can be used to inspect an incoming message after it has executed.
        /// </summary>
        event EventHandler<Microservice.TransmissionPayloadState> ExecuteComplete;

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
        /// This event will be thrown if an incoming message cannot be resolved against a command.
        /// </summary>
        event EventHandler<DispatcherRequestUnresolvedEventArgs> ProcessRequestUnresolved;
        /// <summary>
        /// This event will be raised when a process request errors.
        /// </summary>
        event EventHandler<ProcessRequestErrorEventArgs> ProcessRequestError;
    }
}
