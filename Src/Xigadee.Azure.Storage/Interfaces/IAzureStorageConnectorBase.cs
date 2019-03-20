using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace Xigadee
{
    /// <summary>
    /// This is the common interface for storage connectors.
    /// </summary>
    public interface IAzureStorageConnectorBase
    {
        /// <summary>
        /// This is the default timeout.
        /// </summary>
        TimeSpan? DefaultTimeout { get; set; }
        /// <summary>
        /// This is the specific EventBase type supported for the connector.
        /// </summary>
        DataCollectionSupport Support { get; set; }
        /// <summary>
        /// This is the cloud storage account used for all connectivity.
        /// </summary>
        CloudStorageAccount StorageAccount { get; set; }
        /// <summary>
        /// This is the specific storage options.
        /// </summary>
        AzureStorageDataCollectorOptions Options { get; set; }
        /// <summary>
        /// This is the Azure storage operation context.
        /// </summary>
        OperationContext Context { get; set; }
        /// <summary>
        /// This is the binary encryption handler.
        /// </summary>
        Func<byte[], byte[]> Encryptor { get; set; }
        /// <summary>
        /// The encryption storage policy.
        /// </summary>
        AzureStorageEncryption EncryptionPolicy { get; set; }
        /// <summary>
        /// This method initializes the connector.
        /// </summary>
        void Initialize();
        /// <summary>
        /// This method writes to the incoming event to the underlying storage technology.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <param name="id">The microservice metadata.</param>
        /// <returns>This is an async task.</returns>
        Task Write(EventHolder e, MicroserviceId id);
        /// <summary>
        /// This method is used to check that the specific event should be written to the underlying storage.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <returns>Returns true if the event should be written.</returns>
        bool ShouldWrite(EventHolder e);
    }
}
