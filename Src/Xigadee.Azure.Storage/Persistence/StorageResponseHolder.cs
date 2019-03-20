using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Xigadee
{
    public class StorageRequestHolder: StorageHolderBase
    {
        public StorageRequestHolder(string key, CancellationToken? cancel, string directory = null)
        {
            SafeKey = AzureSafeKey(key);
            Id = key;
            CancelSet = cancel ?? new CancellationToken();
            Directory = directory ?? "";
        }

        public static string AzureSafeKey(string key)
        {
            return key;
        }

        public string SafeKey { get; set; }

        public string Directory { get; set; }

        public CancellationToken CancelSet { get; set; }

        public CloudBlockBlob Blob { get; set; }
    }

    public abstract class StorageHolderBase: IResponseHolder
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

        public byte[] Data { get; set; }

        public Dictionary<string, string> Fields { get; set; }

        public DateTime? LastUpdated { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsCacheHit { get; set; }

        public string Content
        {
            get
            {
                if (Data == null)
                    return null;

                return Encoding.UTF8.GetString(Data);
            }

            set
            {
                Data = Encoding.UTF8.GetBytes(value);
            }
        }

        public Exception Ex
        {
            get;set;
        }

        public bool IsSuccess { get; set; }

        public bool IsTimeout { get; set; }

        public int StatusCode { get; set; }

        public void CopyTo(StorageHolderBase copyto)
        {
            copyto.Id = Id;
            copyto.VersionId = VersionId;
            copyto.ContentType = ContentType;
            copyto.ContentEncoding = ContentEncoding;
            copyto.Fields = Fields.Select((k) => new { k.Key, k.Value }).ToDictionary((l) => l.Key, (l) => l.Value);
        }
    }

    public class StorageResponseHolder: StorageHolderBase
    {


    }

    public class StorageResponseHolder<O>: StorageResponseHolder
        where O : class
    {
        public StorageResponseHolder():base()
        {

        }

        public O Entity { get; set; }
    }
}
