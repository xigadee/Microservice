namespace Xigadee
{
    /// <summary>
    /// This is the base interface for the command context
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        /// Gets the collector.
        /// </summary>
        IDataCollection Collector { get; }
        /// <summary>
        /// Gets the originator identifier.
        /// </summary>
        MicroserviceId OriginatorId { get; }
        /// <summary>
        /// Gets the payload serializer.
        /// </summary>
        IPayloadSerializationContainer PayloadSerializer { get; }
        /// <summary>
        /// Gets the shared services collection.
        /// </summary>
        ISharedService SharedServices { get; }
    }
}