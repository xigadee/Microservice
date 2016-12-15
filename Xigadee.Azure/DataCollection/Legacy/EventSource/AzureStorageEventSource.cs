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

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class AzureStorageEventSource: AzureStorageLoggingBase<EventSourceEntryBase>, IEventSourceComponent
    {
        public AzureStorageEventSource(StorageCredentials credentials, string serviceName, string containerName = "eventsource", ResourceProfile resourceProfile = null, IEncryptionHandler encryption = null)
            :base(credentials, containerName, serviceName, resourceProfile:resourceProfile, encryption:encryption)
        {
        }

        public string Name
        {
            get
            {
                return "AzureBlobStorage";
            }
        }

        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            await Output(mIdMaker(entry), mDirectoryMaker(entry), entry);
        }

        protected override string DirectoryMaker(EventSourceEntryBase data)
        {
            return string.Format("{0}/{1:yyyy-MM-dd}/{2}" , mServiceName , data.UTCTimeStamp, data.EntityType);
        }

        protected override string IdMaker(EventSourceEntryBase data)
        {
            return string.Format("{0}.json", string.Join("_", data.Key.Split(Path.GetInvalidFileNameChars())));
        }
    }
}
