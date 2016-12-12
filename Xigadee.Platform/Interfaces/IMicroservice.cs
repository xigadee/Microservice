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
    /// This interface defines the Microservice definition.
    /// </summary>
    public interface IMicroservice: IMicroserviceConfigure, IMicroserviceProcess, IMicroserviceEvents, IMicroservicePolicy, IService
    {
        MicroserviceId Id { get; }

        string ExternalServiceId { get; }

        string Name { get; }

        string ServiceId { get; }

        IEnumerable<ICommand> Commands { get; }

        IEnumerable<Channel> Channels { get; }
    }

    /// <summary>
    /// This interface is used by the configuration pipeline to add necessary components to the microservice.
    /// </summary>
    public interface IMicroserviceConfigure
    {
        Channel RegisterChannel(Channel channel);

        ICommand RegisterCommand(ICommand command);

        IEventSourceComponent RegisterEventSource(IEventSourceComponent eventSource);
        ILogger RegisterLogger(ILogger logger);
        IBoundaryLoggerComponent RegisterBoundaryLogger(IBoundaryLoggerComponent logger);
        IDataCollectorComponent RegisterDataCollector(IDataCollectorComponent logger);

        IPayloadSerializer RegisterPayloadSerializer(IPayloadSerializer serializer);
        void ClearPayloadSerializers();

        IListener RegisterListener(IListener listener);
        ISender RegisterSender(ISender sender);

        ISharedService SharedServices { get; }
    }

    /// <summary>
    /// This interface lists the policy options for the Microservice.
    /// </summary>
    public interface IMicroservicePolicy
    {
        MicroservicePolicy PolicyMicroservice { get; }
        TaskManagerPolicy PolicyTaskManager { get; }
        ResourceTrackerPolicy PolicyResourceTracker { get; }
        CommandContainerPolicy PolicyCommandContainer { get; }
        CommunicationPolicy PolicyCommunication { get; }
        SchedulerPolicy PolicyScheduler { get; }
        SecurityPolicy PolicySecurity { get; }
        DataCollectionPolicy PolicyDataCollection { get; }
    }

    /// <summary>
    /// This interface lists the events available from the Microservice.
    /// </summary>
    public interface IMicroserviceEvents
    {
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

    public interface IMicroserviceProcess
    {
        void Process(TransmissionPayload payload);

        void Process(ServiceMessage message, ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal, Action<bool, Guid> release = null, bool isDeadLetterMessage = false);

        void Process(ServiceMessageHeader header, object package = null, int ChannelPriority = 1, ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal, Action<bool, Guid> release = null, bool isDeadLetterMessage = false);

        void Process(string ChannelId, string MessageType = null, string ActionType = null, object package = null, int ChannelPriority = 1, ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal, Action<bool, Guid> release = null, bool isDeadLetterMessage = false);

        void Process<C>(object package = null, int ChannelPriority = 1, ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal, Action<bool, Guid> release = null, bool isDeadLetterMessage = false) 
            where C : IMessageContract;
    }
}