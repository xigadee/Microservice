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
    public class AzureStorageEventSource: AzureStorageLoggingBase<EventSourceEntryBase>, IEventSource
    {
        public AzureStorageEventSource(StorageCredentials credentials, string serviceName, string containerName = "eventsource"
            , ResourceProfile resourceProfile = null)
            :base(credentials, containerName, serviceName, resourceProfile:resourceProfile)
        {
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
