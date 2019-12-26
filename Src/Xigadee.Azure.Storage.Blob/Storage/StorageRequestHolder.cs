using Microsoft.Azure.Storage.Blob;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This class holds the data for a Blob storage request
    /// </summary>
    public class StorageRequestHolder : StorageHolderBase
    {
        /// <summary>
        /// Sets the core data fields.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="directory">The entity directory. if this is not set, then the root key is used.</param>
        public StorageRequestHolder(string key, CancellationToken? cancel, string directory = null)
        {
            SafeKey = AzureSafeKey(key);
            Id = key;
            CancelSet = cancel ?? new CancellationToken();
            Directory = directory ?? "";
        }

        /// <summary>
        /// This method converts the key in to an Azure storage safe key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AzureSafeKey(string key)
        {
            //var uriSafekey = WebUtility.UrlEncode(key);
            //if (string.IsNullOrEmpty(uriSafekey))
            //    return null;

            return key;
        }

        /// <summary>
        /// The safe key.
        /// </summary>
        public string SafeKey { get; set; }
        /// <summary>
        /// The directory.
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        /// The cancellation token.
        /// </summary>
        public CancellationToken CancelSet { get; set; }
        /// <summary>
        /// The cloud blob.
        /// </summary>
        public CloudBlockBlob Blob { get; set; }
    }

}
