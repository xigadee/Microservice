#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
        /// This method initiialises the table storage connector.
        /// </summary>
        public override void Initialize()
        {
            Client = StorageAccount.CreateCloudTableClient();

            if (RequestOptionsDefault != null)
                Client.DefaultRequestOptions = RequestOptionsDefault ?? 
                    new TableRequestOptions()
                    {
                        RetryPolicy = new LinearRetry(TimeSpan.FromMilliseconds(200), 5)
                        , ServerTimeout = DefaultTimeout ?? TimeSpan.FromSeconds(1)
                        //, ParallelOperationThreadCount = 64 
                    };

            if (ContainerId == null)
                ContainerId = AzureStorageHelper.GetEnum<DataCollectionSupport>(Support).StringValue;

            ContainerId = StorageServiceBase.ValidateAzureContainerName(ContainerId);

            // Retrieve a reference to the table.
            Table = Client.GetTableReference(ContainerId);

            // Create the table if it doesn't exist.
            Table.CreateIfNotExists();
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
