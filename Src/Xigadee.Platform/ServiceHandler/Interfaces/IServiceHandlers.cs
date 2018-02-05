namespace Xigadee
{
    /// <summary>
    /// This interface is used by the service handler collection
    /// </summary>
    public interface IServiceHandlers
    {
        
        /// <summary>
        /// Gets the authentication handler collection.
        /// </summary>
        ServiceHandlerCollection<IServiceHandlerAuthentication> Authentication { get; }
        /// <summary>
        /// Gets the compression handler collection.
        /// </summary>
        ServiceHandlerCollection<IServiceHandlerCompression> Compression { get; }
        /// <summary>
        /// Gets the encryption handler collection.
        /// </summary>
        ServiceHandlerCollection<IServiceHandlerEncryption> Encryption { get; }
        /// <summary>
        /// Gets the serialization handler collection.
        /// </summary>
        ServiceHandlerCollection<IServiceHandlerSerialization> Serialization { get; }
    }
}