namespace Xigadee
{
    /// <summary>
    /// This class holds the service handlers and their keys.
    /// </summary>
    public class ServiceHandlerCollectionContext
    {
        /// <summary>
        /// Identifies whether a serializer is specified.
        /// </summary>
        public bool HasSerialization => Serialization != null;
        /// <summary>
        /// Gets or sets the preferred serializer identifier.
        /// </summary>
        public SerializationHandlerId Serialization { get; set; }
        /// <summary>
        /// This is the serialization module.
        /// </summary>
        public IServiceHandlerSerialization Serializer { get; set; }

        /// <summary>
        /// Identifies whether compression is specified.
        /// </summary>
        public bool HasCompression => Compression != null;
        /// <summary>
        /// Gets or sets the preferred compression identifier.
        /// </summary>
        public CompressionHandlerId Compression { get; set; }
        /// <summary>
        /// This is the compressor module.
        /// </summary>
        public IServiceHandlerCompression Compressor { get; set; }

        /// <summary>
        /// Identifies whether authentication is specified.
        /// </summary>
        public bool HasAuthentication => Authentication != null;
        /// <summary>
        /// Gets or sets the preferred authentication identifier.
        /// </summary>
        public AuthenticationHandlerId Authentication { get; set; }
        /// <summary>
        /// This is the authentication module.
        /// </summary>
        public IServiceHandlerAuthentication Authenticator { get; set; }

        /// <summary>
        /// Identifies whether encrpytion is specified.
        /// </summary>
        public bool HasEncryption => Encryption != null;
        /// <summary>
        /// Gets or sets the preferred encryption identifier.
        /// </summary>
        public EncryptionHandlerId Encryption { get; set; }
        /// <summary>
        /// This is the encrpytor module.
        /// </summary>
        public IServiceHandlerEncryption Encryptor { get; set; }
    }
}
