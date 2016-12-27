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
    public class AzureStorageConnectorQueue: AzureStorageConnectorBinary<QueueRequestOptions, AzureStorageContainerFile>
    {

        public CloudQueueClient Client { get; set; }

        public CloudQueue Queue { get; set; }

        public override Task Write(EventBase e, MicroserviceId id)
        {
            throw new NotImplementedException();


            //// Retrieve a reference to a queue.
            //CloudQueue queue = queueClient.GetQueueReference("myqueue");

            //// Create the queue if it doesn't already exist.
            //queue.CreateIfNotExists();

            //// Create a message and add it to the queue.
            //CloudQueueMessage message = new CloudQueueMessage("Hello, World");
            //queue.AddMessage(message);

            // Async enqueue the message
            //await queue.AddMessageAsync(cloudQueueMessage);
            //Console.WriteLine("Message added");
        }

        public override void Initialize()
        {
            Client = StorageAccount.CreateCloudQueueClient();
            if (RequestOptionsDefault != null)
                Client.DefaultRequestOptions = RequestOptionsDefault;

            Queue = Client.GetQueueReference(RootId);
            Queue.CreateIfNotExists();
        }
    }
}
