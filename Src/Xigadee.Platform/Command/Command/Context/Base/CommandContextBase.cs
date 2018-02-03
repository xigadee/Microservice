namespace Xigadee
{
    /// <summary>
    /// This is the context base for inline based commands.
    /// </summary>
    public abstract class CommandContextBase<O> : ICommandContext
        where O: IMicroserviceDispatch
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="serviceHandlers">The service handler container.</param>
        /// <param name="collector">The data collector.</param>
        /// <param name="sharedServices">The shared service context.</param>
        /// <param name="originatorId">This is the Microservice identifiers.</param>
        /// <param name="outgoingRequest">This is the outgoing request initiator.</param>
        public CommandContextBase(
              IServiceHandlerContainer serviceHandlers
            , IDataCollection collector
            , ISharedService sharedServices
            , MicroserviceId originatorId
            , O outgoingRequest)
        {
            ServiceHandlers = serviceHandlers;
            Collector = collector;
            SharedServices = sharedServices;
            OriginatorId = originatorId;
            Outgoing = outgoingRequest;
        }
        /// <summary>
        /// The service handlers container.
        /// </summary>
        public IServiceHandlerContainer ServiceHandlers { get; }
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
        public O Outgoing { get; }
    }
}
