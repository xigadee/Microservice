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
        /// This event is fired when an exception is raised during the processing of a request by the Dispatcher
        /// </summary>
        event EventHandler<ProcessRequestErrorEventArgs> ProcessRequestError;

        event EventHandler<DispatcherRequestUnresolvedEventArgs> ProcessRequestUnresolved;

        event EventHandler<MicroserviceStatusEventArgs> ComponentStatusChange;
        /// <summary>
        /// This event handler can be used to inspect an incoming message before it executes.
        /// </summary>
        event EventHandler<Microservice.TransmissionPayloadState> OnExecuteBegin;
        /// <summary>
        /// This event handler can be used to inspect an incoming message after it has executed.
        /// </summary>
        event EventHandler<Microservice.TransmissionPayloadState> OnExecuteComplete;
    }
}
