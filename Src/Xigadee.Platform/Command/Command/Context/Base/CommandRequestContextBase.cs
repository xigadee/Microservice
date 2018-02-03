using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This context is used to hold the necessary data for an in-line command request.
    /// </summary>
    [DebuggerDisplay("{Id}/{CorrelationId}")]
    public abstract class CommandRequestContextBase<O> : CommandContextBase<O>, ICommandRequestContext where O: IMicroserviceDispatch
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <param name="rsCol">The outgoing response collection.</param>
        /// <param name="serviceHandlers">The service handlers container.</param>
        /// <param name="collector">The data collector.</param>
        /// <param name="sharedServices">The shared service context.</param>
        /// <param name="originatorId">This is the Microservice identifiers.</param>
        /// <param name="outgoingRequest">This is the outgoing request initiator.</param>
        public CommandRequestContextBase(TransmissionPayload rq, List<TransmissionPayload> rsCol
            , IServiceHandlerContainer serviceHandlers
            , IDataCollection collector
            , ISharedService sharedServices
            , MicroserviceId originatorId
            , O outgoingRequest) : base(serviceHandlers, collector, sharedServices, originatorId, outgoingRequest)
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
        public string CorrelationId => Request.Message.ProcessCorrelationKey;
    }
}
