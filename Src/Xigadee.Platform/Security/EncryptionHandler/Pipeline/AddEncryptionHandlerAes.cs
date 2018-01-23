using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method adds the encryption handler to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="identifier">The encryption type identifier. 
        /// This is will be used when assigning the handler to a channel or collector.</param>
        /// <param name="key">Encryption Key</param>
        /// <param name="keySize">Encryption Key Size</param>
        /// <param name="action">The action on the handler.</param>
        /// <param name="useCompression">Compress payload prior to encryption</param>
        /// <returns>The pipeline.</returns>
        public static P AddEncryptionHandlerAes<P>(this P pipeline
            , string identifier
            , byte[] key
            , bool useCompression = true
            , int? keySize = null
            , Action<IEncryptionHandler> action = null)
            where P : IPipeline
        {
            var handler = new AesEncryptionHandler(identifier, key, useCompression, keySize);

            action?.Invoke(handler);

            pipeline.Service.Security.RegisterEncryptionHandler(identifier, handler);

            return pipeline;
        }

        /// <summary>
        /// This method adds the encryption handler to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="identifier">The encryption type identifier.</param> 
        /// <param name="creator">This function is used to create the handler from the configuration collection.</param>
        /// <param name="action">The action on the handler.</param>
        /// <returns>The pipeline.</returns>
        public static P AddEncryptionHandlerAes<P>(this P pipeline
            , string identifier
            , Func<IEnvironmentConfiguration, IEncryptionHandler> creator
            , Action<IEncryptionHandler> action = null)
            where P : IPipeline
        {
            var handler = creator(pipeline.Configuration);

            action?.Invoke(handler);

            pipeline.Service.Security.RegisterEncryptionHandler(identifier, handler);

            return pipeline;
        }
    }
}
