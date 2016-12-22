using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class contains the storage configuration options for the specific DataCollection type.
    /// </summary>
    public class AzureStorageDataCollectorOptions
    {
        public AzureStorageDataCollectorOptions(DataCollectionSupport support
            , StorageBehaviour behavior = StorageBehaviour.None
            )
        {
            Support = support;
            TableId = AzureStorageDCExtensions.GetEnum<DataCollectionSupport>(support).StringValue;
            Behaviour = behavior;
        }

        public DataCollectionSupport Support { get; }
        /// <summary>
        /// This is the Boundary Logging Table storage table.
        /// </summary>
        public string TableId { get; set; }


        public Func<MicroserviceId, EventBase, AzureStorageContainerBlob> BlobConverter { get; set; } = null;

        public Func<MicroserviceId, EventBase, AzureStorageContainerTable> TableConverter { get; set; } = null;

        /// <summary>
        /// The storage action
        /// </summary>
        public StorageBehaviour Behaviour { get; set; }

        /// <summary>
        /// The encryption type.
        /// </summary>
        public StorageEncryption Encryption { get; set; } = StorageEncryption.BlobWhenPresent;

        /// <summary>
        /// This enumeration determines the specific storage behavior for the data type.
        /// </summary>
        [Flags]
        public enum StorageBehaviour
        {
            None = 0,
            Blob = 1,
            Table = 2,
            BlobAndTable = 3
        }

        /// <summary>
        /// This settings determines the specific encryption settings for the data type.
        /// </summary>
        public enum StorageEncryption
        {
            None,
            BlobWhenPresent,
            BlobAlwaysWithException
        }
    }
}
