using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace Xigadee
{
    /// <summary>
    /// This connector uses the Azure Queue for event writing.
    /// </summary>
    public class AzureStorageConnectorQueue: AzureStorageConnectorBase<QueueRequestOptions, BinaryContainer>
    {
        /// <summary>
        /// This is the queue client.
        /// </summary>
        public CloudQueueClient Client { get; set; }
        /// <summary>
        /// This is the queue.
        /// </summary>
        public CloudQueue Queue { get; set; }
        /// <summary>
        /// This method writes to the incoming event to the underlying storage technology.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <param name="id">The microservice metadata.</param>
        /// <returns>
        /// This is an async task.
        /// </returns>
        public override async Task Write(EventHolder e, MicroserviceId id)
        {
            var output = Serializer(e, id);

            //Encrypt the payload when required.
            if (EncryptionPolicy != AzureStorageEncryption.None && Encryptor != null)
            {
                //The checks for always encrypt are done externally.
                output.Blob = Encryptor(output.Blob);
            }

            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(Convert.ToBase64String(output.Blob));

            // Async enqueue the message
            await Queue.AddMessageAsync(message);
        }
        /// <summary>
        /// This method initializes the connector.
        /// </summary>
        public override void Initialize()
        {
            Client = StorageAccount.CreateCloudQueueClient();
            if (RequestOptionsDefault != null)
                Client.DefaultRequestOptions = RequestOptionsDefault;

            if (ContainerId == null)
                ContainerId = AzureStorageHelper.GetEnum<DataCollectionSupport>(Support).StringValue;

            ContainerId = StorageServiceBase.ValidateAzureContainerName(ContainerId);

            Queue = Client.GetQueueReference(ContainerId);
            Queue.CreateIfNotExistsAsync().Wait();
        }
        /// <summary>
        /// This method is used to check that the specific event should be written to the underlying storage.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <returns>
        /// Returns true if the event should be written.
        /// </returns>
        public override bool ShouldWrite(EventHolder e)
        {
            return Options.IsSupported(AzureStorageBehaviour.Queue, e);
        }
    }
}
