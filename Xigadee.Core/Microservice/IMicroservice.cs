using System;
using System.Collections.Generic;

namespace Xigadee
{
    public interface IMicroserviceConfigure: IService
    {
        string ExternalServiceId { get; }
        string Name { get; }
        string ServiceId { get; }

        IEnumerable<ICommand> Commands { get; }

        Channel RegisterChannel(Channel channel);
        ICommand RegisterCommand(ICommand command);
        IListener RegisterDeadLetterListener(IListener deadLetter);
        IEventSource RegisterEventSource(IEventSource eventSource);
        IListener RegisterListener(IListener listener);
        ILogger RegisterLogger(ILogger logger);
        IPayloadSerializer RegisterPayloadSerializer(IPayloadSerializer serializer);
        ISender RegisterSender(ISender sender);
        ITelemetry RegisterTelemetry(ITelemetry telemetry);
    }

    public interface IMicroservice: IMicroserviceConfigure
    {
        event EventHandler<ProcessRequestErrorEventArgs> ProcessRequestError;
        event EventHandler<ProcessRequestUnresolvedEventArgs> ProcessRequestUnresolved;
        event EventHandler<MicroserviceStatusEventArgs> ComponentStatusChange;

        void Process(TransmissionPayload payload);
        void Process(ServiceMessage message, ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal, Action<bool, Guid> release = null, bool isDeadLetterMessage = false);
        void Process(ServiceMessageHeader header, object package = null, int ChannelPriority = 1, ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal, Action<bool, Guid> release = null, bool isDeadLetterMessage = false);
        void Process(string ChannelId, string MessageType = null, string ActionType = null, object package = null, int ChannelPriority = 1, ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal, Action<bool, Guid> release = null, bool isDeadLetterMessage = false);
        void Process<C>(object package = null, int ChannelPriority = 1, ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal, Action<bool, Guid> release = null, bool isDeadLetterMessage = false) where C : IMessageContract;

    }
}