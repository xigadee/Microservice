using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Xigadee
{
    /// <summary>
    /// This is the default connector class for Azure Table Storage.
    /// </summary>
    public class AzureStorageConnectorTable: AzureStorageConnectorBase<TableRequestOptions, ITableEntity>
    {
        /// <summary>
        /// This is the table client.
        /// </summary>
        public CloudTableClient Client { get; set; }
        /// <summary>
        /// This is the table.
        /// </summary>
        public CloudTable Table { get; set; }

        /// <summary>
        /// This method writes the event holder to table storage.
        /// </summary>
        /// <param name="e">The event holder to write to table storage.</param>
        /// <param name="id">The service id.</param>
        /// <returns>This is an async process.</returns>
        public override async Task Write(EventHolder e, MicroserviceId id)
        {
            //Create the output.
            var output = Serializer(e, id);

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insert = TableOperation.Insert(output);

            // Execute the insert operation.
            await Table.ExecuteAsync(insert);
        }

        /// <summary>
        /// This method initialises the table storage connector.
        /// </summary>
        public override void Initialize()
        {
            Client = StorageAccount.CreateCloudTableClient();

            Client.DefaultRequestOptions = RequestOptionsDefault ?? 
                new TableRequestOptions()
                {
                    RetryPolicy = new LinearRetry(TimeSpan.FromMilliseconds(200), 5)
                    , ServerTimeout = DefaultTimeout ?? TimeSpan.FromSeconds(1)
                };

            if (ContainerId == null)
                ContainerId = AzureStorageHelper.GetEnum<DataCollectionSupport>(Support).StringValue;

            ContainerId = StorageServiceBase.ValidateAzureContainerName(ContainerId);

            // Retrieve a reference to the table.
            Table = Client.GetTableReference(ContainerId);

            // Create the table if it doesn't exist.
            Table.CreateIfNotExistsAsync().Wait();
        }

        /// <summary>
        /// This method specifies whether the event should be written.
        /// </summary>
        /// <param name="e">The event holder.</param>
        /// <returns>Returns true if the entity should be written.</returns>
        public override bool ShouldWrite(EventHolder e)
        {
            return Options.IsSupported?.Invoke(AzureStorageBehaviour.Table, e)??false;
        }
    }
}
