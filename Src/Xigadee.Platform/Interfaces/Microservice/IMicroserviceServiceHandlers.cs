using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is responsible for the Microservice security.
    /// </summary>
    public interface IMicroserviceServiceHandlers
    {
        /// <summary>
        /// Gets the authentication collection.
        /// </summary>
        ServiceHandlerCollection<IServiceHandlerAuthentication> Authentication { get; }
        /// <summary>
        /// Gets the compression collection.
        /// </summary>
        ServiceHandlerCollection<IServiceHandlerCompression> Compression { get; }
        /// <summary>
        /// Gets the encryption collection.
        /// </summary>
        ServiceHandlerCollection<IServiceHandlerEncryption> Encryption { get; }
        /// <summary>
        /// Gets the serialization collection.
        /// </summary>
        ServiceHandlerCollection<IServiceHandlerSerialization> Serialization { get; }
    }
}
