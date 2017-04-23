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

namespace Xigadee
{
    public class AzureStorageConnectorTable: AzureStorageConnectorBase<TableRequestOptions, ITableEntity>
    {
        public CloudTableClient Client { get; set; }

        public CloudTable Table { get; set; }

        public override async Task Write(EventHolder e, MicroserviceId id)
        {
            var tableId = MakeId(e,id);

            var output = Serializer(e, id);

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insert = TableOperation.Insert(output);

            // Execute the insert operation.
            await Table.ExecuteAsync(insert);
        }

        public override void Initialize()
        {
            Client = StorageAccount.CreateCloudTableClient();

            if (RequestOptionsDefault != null)
                Client.DefaultRequestOptions = RequestOptionsDefault;

            if (ContainerId == null)
                ContainerId = AzureStorageHelper.GetEnum<DataCollectionSupport>(Support).StringValue;

            ContainerId = StorageServiceBase.ValidateAzureContainerName(ContainerId);

            // Retrieve a reference to the table.
            Table = Client.GetTableReference(ContainerId);

            // Create the table if it doesn't exist.
            Table.CreateIfNotExists();
        }

        public override bool ShouldWrite(EventHolder e)
        {
            return Options.IsSupported?.Invoke(AzureStorageBehaviour.Table, e)??false;
        }
    }
}
