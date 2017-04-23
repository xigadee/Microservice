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

}
