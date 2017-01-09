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
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Xigadee
{
    public class AzureStorageConnectorBlob: AzureStorageConnectorBase<BlobRequestOptions, AzureStorageBinary>
    {
        public CloudBlobContainer Container { get; set; }

        public CloudBlobClient Client { get; set; }

        public BlobContainerPublicAccessType BlobAccessType { get; set; } = BlobContainerPublicAccessType.Off;

        /// <summary>
        /// This function is used to create the folder for the entity;
        /// </summary>
        public Func<EventBase, MicroserviceId, string> MakeFolder { get; set; }

        public override async Task Write(EventBase e, MicroserviceId id)
        {
            string storageId = MakeId(e, id);
            string storageFolder = MakeFolder(e, id);
            var output = Serializer(e, id);

            //Encrypt the payload when required.
            if (EncryptionPolicy != AzureStorageEncryption.None && Encryptor!=null)
            {
                //The checks for always encrypt are done externally.
                output.Blob = Encryptor(output.Blob);
            }

            var refEntityDirectory = Container.GetDirectoryReference(storageFolder);

            var Blob = refEntityDirectory.GetBlockBlobReference(storageId);

            Blob.Properties.ContentType = output.ContentType;

            Blob.Properties.ContentEncoding = output.ContentEncoding;

            await Blob.UploadFromByteArrayAsync(output.Blob, 0, output.Blob.Length);
        }

        /// <summary>
        /// This method initializes the blob client and container.
        /// </summary>
        public override void Initialize()
        {
            Client = StorageAccount.CreateCloudBlobClient();

            if (RequestOptionsDefault == null)
                RequestOptionsDefault = new BlobRequestOptions()
                {
                    RetryPolicy = new LinearRetry(TimeSpan.FromMilliseconds(200), 5)
                        , ServerTimeout = DefaultTimeout ?? TimeSpan.FromSeconds(1)
                    //, ParallelOperationThreadCount = 64 
                };

            Client.DefaultRequestOptions = RequestOptionsDefault;

            if (ContainerId == null)
                ContainerId = AzureStorageHelper.GetEnum<DataCollectionSupport>(Support).StringValue;

            ContainerId = StorageServiceBase.ValidateAzureContainerName(ContainerId);

            Container = Client.GetContainerReference(ContainerId);
                                                                                                                                           
            Container.CreateIfNotExists(BlobAccessType, RequestOptionsDefault, Context);
        }

        public override bool ShouldWrite(EventBase e)
        {
            return Options.IsSupported?.Invoke(AzureStorageBehaviour.Blob, e)??false;
        }
    }
}
