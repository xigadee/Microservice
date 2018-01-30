using System;
using System.Collections;
using System.Collections.Generic;

namespace Xigadee
{
    internal class ServiceHandlerWrapper: WrapperBase, IMicroserviceServiceHandlers
    {
        /// <summary>
        /// This container is used to hold the security infrastructure for the Microservice.
        /// </summary>
        private ServiceHandlerContainer mContainer;

        internal ServiceHandlerWrapper(ServiceHandlerContainer container, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mContainer = container ?? throw new ArgumentNullException("container");
        }
        /// <summary>
        /// Gets the authentication collection.
        /// </summary>
        public ServiceHandlerCollection<IServiceHandlerAuthentication> Authentication => mContainer.Authentication;
        /// <summary>
        /// Gets the compression collection.
        /// </summary>
        public ServiceHandlerCollection<IServiceHandlerCompression> Compression => mContainer.Compression;
        /// <summary>
        /// Gets the encryption collection.
        /// </summary>
        public ServiceHandlerCollection<IServiceHandlerEncryption> Encryption => mContainer.Encryption;
        /// <summary>
        /// Gets the serialization collection.
        /// </summary>
        public ServiceHandlerCollection<IServiceHandlerSerialization> Serialization => mContainer.Serialization;
    }
}
