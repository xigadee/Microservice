using System;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    /// <summary>
    /// This is the base class for Azure Queue communication.
    /// </summary>
    public class AzureQueueFabricBridge : FabricBridgeBase<ICommunicationBridge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureQueueFabricBridge"/> class.
        /// Which is used to provides connectivity using the Azure Storage Queue.
        /// </summary>
        public AzureQueueFabricBridge(StorageCredentials credentials)
        {
            Credentials = credentials;
        }

        /// <summary>
        /// Gets the Azure storage credentials.
        /// </summary>
        protected StorageCredentials Credentials { get; }

        /// <summary>
        /// Gets the <see cref="ICommunicationBridge"/> with the specified mode.
        /// </summary>
        /// <value>
        /// The <see cref="ICommunicationBridge"/>.
        /// </value>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Only queue is supported. Broadcast is not available on Azure Storage.</exception>
        public override ICommunicationBridge this[FabricMode mode]
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }
}
