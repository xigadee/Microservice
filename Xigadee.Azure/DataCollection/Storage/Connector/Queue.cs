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
    public class AzureStorageConnectorQueue: AzureStorageConnectorBase<QueueRequestOptions, AzureStorageBinary>
    {
        public CloudQueueClient Client { get; set; }

        public CloudQueue Queue { get; set; }

        public override async Task Write(EventBase e, MicroserviceId id)
        {
            var output = Serializer(e, id);

            //Encrypt the payload when required.
            if (EncryptionPolicy != AzureStorageEncryption.None && EncryptionHandler != null)
            {
                //The checks for always encrypt are done externally.
                output.Blob = EncryptionHandler.Encrypt(output.Blob);
            }

            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(output.Blob);

            // Async enqueue the message
            await Queue.AddMessageAsync(message);
        }

        public override void Initialize()
        {
            Client = StorageAccount.CreateCloudQueueClient();
            if (RequestOptionsDefault != null)
                Client.DefaultRequestOptions = RequestOptionsDefault;

            if (ContainerId == null)
                ContainerId = AzureStorageHelper.GetEnum<DataCollectionSupport>(Support).StringValue;

            ContainerId = StorageServiceBase.ValidateAzureContainerName(ContainerId);

            Queue = Client.GetQueueReference(ContainerId);
            Queue.CreateIfNotExists();
        }

        public override bool ShouldWrite(EventBase e)
        {
            return Options.IsSupported(AzureStorageBehaviour.Queue, e);
        }
    }
}
