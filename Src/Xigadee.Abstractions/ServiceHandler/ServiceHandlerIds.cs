namespace Xigadee
{
    /// <summary>
    /// This class holds the service handlers.
    /// </summary>
    public class ServiceHandlerIdCollection
    {
        /// <summary>
        /// Gets or sets the preferred authentication identifier.
        /// </summary>
        public AuthenticationHandlerId Authentication { get; set; }
        /// <summary>
        /// Gets or sets the preferred serializer identifier.
        /// </summary>
        public SerializationHandlerId Serializer { get; set; }
        /// <summary>
        /// Gets or sets the preferred compression identifier.
        /// </summary>
        public CompressionHandlerId Compression { get; set; }
        /// <summary>
        /// Gets or sets the preferred encryption identifier.
        /// </summary>
        public EncryptionHandlerId Encryption { get; set; }
    }
}
