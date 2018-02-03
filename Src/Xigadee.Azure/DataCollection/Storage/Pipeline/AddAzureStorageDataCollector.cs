using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static partial class AzureExtensionMethods
    {
        /// <summary>
        /// Adds the azure storage data collector.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creds">The Azure storage credentials.</param>
        /// <param name="adjustPolicy">The adjust policy.</param>
        /// <param name="resourceProfile">The resource profile.</param>
        /// <param name="handler">The encryption handler id.</param>
        /// <param name="onCreate">The on create.</param>
        /// <param name="context">The context.</param>
        /// <returns>The pipeline.</returns>
        /// <exception cref="EncryptionHandlerNotResolvedException"></exception>
        public static P AddAzureStorageDataCollector<P>(this P pipeline
            , StorageCredentials creds = null
            , Action<AzureStorageDataCollectorPolicy> adjustPolicy = null
            , ResourceProfile resourceProfile = null
            , EncryptionHandlerId handler = null
            , Action<AzureStorageDataCollector> onCreate = null
            , OperationContext context = null
            )
            where P : IPipeline
        {
            AzureStorageDataCollectorPolicy policy = new AzureStorageDataCollectorPolicy();

            if (handler != null)
            {
                if (!pipeline.Service.ServiceHandlers.Encryption.Contains(handler.Id))
                    throw new EncryptionHandlerNotResolvedException(handler.Id);
            }

            adjustPolicy?.Invoke(policy);

            if (creds == null)
                creds = pipeline.Configuration.AzureStorageCredentials(true);

            var component = new AzureStorageDataCollector(creds, policy
                , context: context
                , encryptionId:handler);

            onCreate?.Invoke(component);

            pipeline.AddDataCollector(component);

            return pipeline;
        }
    }
}
