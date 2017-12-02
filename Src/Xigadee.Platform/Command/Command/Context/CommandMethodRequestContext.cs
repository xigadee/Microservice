using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This context is used to hold the necessary data for an in-line command request.
    /// </summary>
    public class CommandMethodRequestContext: CommandRequestContextBase<ICommandOutgoing>
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
        public CommandMethodRequestContext(TransmissionPayload rq, List<TransmissionPayload> rsCol
            , IPayloadSerializationContainer serializer
            , IDataCollection collector
            , ISharedService sharedServices
            , MicroserviceId originatorId
            , ICommandOutgoing outgoingRequest) :base(rq, rsCol, serializer, collector, sharedServices, originatorId, outgoingRequest)
        {
        }
    }
}
