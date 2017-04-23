#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
#endregion
namespace IMGroup.Microservice
{
    public abstract class StorageHolderBase
    {
        public StorageHolderBase()
        {
            Fields = new Dictionary<string, string>();
        }

        public string Id { get; set; }

        public string ETag { get; set; }

        public string VersionId { get; set; }

        public string ContentType { get; set; }

        public string ContentEncoding { get; set; }

        public byte[] Content { get; set; }

        public Dictionary<string, string> Fields { get; set; }

        public DateTime? LastUpdated { get; set; }

        public bool IsDeleted { get; set; }

        public void CopyTo(StorageHolderBase copyto)
        {
            copyto.Id = Id;
            copyto.VersionId = VersionId;
            copyto.ContentType = ContentType;
            copyto.ContentEncoding = ContentEncoding;
            copyto.Fields = Fields.Select((k) => new { k.Key, k.Value }).ToDictionary((l) => l.Key, (l) => l.Value);
        }
    }

}
