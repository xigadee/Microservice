using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
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
