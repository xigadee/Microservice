using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is responsible for the Microservice security.
    /// </summary>
    public interface IMicroserviceSecurity
    {
        /// <summary>
        /// This method registers an encryption handler with the Security container, which can encrypt and decrypt a binary blob.
        /// </summary>
        /// <param name="identifier">The identifier. This is used to identify the handler so that it can be assigned to multiple channels.</param>
        /// <param name="handler">The actual handler.</param>
        void RegisterEncryptionHandler(string identifier, IServiceHandlerEncryption handler);

        /// <summary>
        /// This method specifies whether the microservice has the encryption handler registered.
        /// </summary>
        /// <param name="identifier">The identifier for the handler.</param>
        /// <returns>Returns true if the handler is registered.</returns>
        bool HasEncryptionHandler(string identifier);

        /// <summary>
        /// This method registers an encryption handler with the Security container, which can encrypt and decrypt a binary blob.
        /// </summary>
        /// <param name="identifier">The identifier. This is used to identify the handler so that it can be assigned to multiple channels.</param>
        /// <param name="handler">The actual handler.</param>
        void RegisterAuthenticationHandler(string identifier, IServiceHandlerAuthentication handler);

        /// <summary>
        /// This method specifies whether the microservice has the encryption handler registered.
        /// </summary>
        /// <param name="identifier">The identifier for the handler.</param>
        /// <returns>Returns true if the handler is registered.</returns>
        bool HasAuthenticationHandler(string identifier);
    }
}
