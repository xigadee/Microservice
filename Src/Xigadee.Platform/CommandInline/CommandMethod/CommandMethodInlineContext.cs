using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This context is used to hold the necessary data for an inline command request.
    /// </summary>
    [DebuggerDisplay("{Id}/{CorrellationId}")]
    public class CommandMethodInlineContext: CommandContextBase
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <param name="rsCol">The outgoing response collection.</param>
        /// <param name="serializer">The serialization container.</param>
        /// <param name="collector">The data collector.</param>
        /// <param name="sharedServices">The shared service context.</param>
        /// <param name="originatorId">This is the Microservice identifiers.</param>
        /// <param name="outgoingRequest">This is the outgoing request initiator.</param>
        public CommandMethodInlineContext(TransmissionPayload rq, List<TransmissionPayload> rsCol
            , IPayloadSerializationContainer serializer
            , IDataCollection collector
            , ISharedService sharedServices
            , MicroserviceId originatorId
            , ICommandOutgoing outgoingRequest) :base(serializer, collector, sharedServices, originatorId, outgoingRequest)
        {
            Request = rq;
            Responses = rsCol;
        }
        /// <summary>
        /// The incoming request.
        /// </summary>
        public TransmissionPayload Request { get; }
        /// <summary>
        /// The outgoing responses.
        /// </summary>
        public List<TransmissionPayload> Responses { get; }

        /// <summary>
        /// This is the request Id.
        /// </summary>
        public Guid Id => Request.Id;
        /// <summary>
        /// This is the internal message.
        /// </summary>
        public ServiceMessage Message => Request.Message;
        /// <summary>
        /// This is the request Id.
        /// </summary>
        public string CorrellationId => Request.Message.ProcessCorrelationKey;
    }

    /// <summary>
    /// This is the context base for inline based commands.
    /// </summary>
    public abstract class CommandContextBase
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="serializer">The serialization container.</param>
        /// <param name="collector">The data collector.</param>
        /// <param name="sharedServices">The shared service context.</param>
        /// <param name="originatorId">This is the Microservice identifiers.</param>
        /// <param name="outgoingRequest">This is the outgoing request initiator.</param>
        public CommandContextBase(
              IPayloadSerializationContainer serializer
            , IDataCollection collector
            , ISharedService sharedServices
            , MicroserviceId originatorId
            , ICommandOutgoing outgoingRequest)
        {
            PayloadSerializer = serializer;
            Collector = collector;
            SharedServices = sharedServices;
            OriginatorId = originatorId;
            Outgoing = outgoingRequest;
        }
        /// <summary>
        /// The serialization container.
        /// </summary>
        public IPayloadSerializationContainer PayloadSerializer { get; }
        /// <summary>
        /// This is the data collector.
        /// </summary>
        public IDataCollection Collector { get; }
        /// <summary>
        /// This is the shared service collection.
        /// </summary>
        public ISharedService SharedServices { get; }
        /// <summary>
        /// This is the service originator.
        /// </summary>
        public MicroserviceId OriginatorId { get; }
        /// <summary>
        /// Gets the outgoing request initiator.
        /// </summary>
        public ICommandOutgoing Outgoing { get; }
    }
}
