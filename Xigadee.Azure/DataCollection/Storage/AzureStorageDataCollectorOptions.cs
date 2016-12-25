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
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="support">The collection support type.</param>
        /// <param name="behavior">The default storage behaviour.</param>
        public AzureStorageDataCollectorOptions(DataCollectionSupport support
            , AzureStorageBehaviour behavior = AzureStorageBehaviour.None
            )
        {
            Support = support;
            TableId = AzureStorageDCExtensions.GetEnum<DataCollectionSupport>(support).StringValue;
            Behaviour = behavior;
        }

        /// <summary>
        /// This is the support type for the options handler, i.e. LogEvent, EventSource, etc.
        /// </summary>
        public DataCollectionSupport Support { get; }

        /// <summary>
        /// This is the name of the storage table.
        /// </summary>
        public string TableId { get; set; }
        /// <summary>
        /// This is the name of the storage queue.
        /// </summary>
        public string QueueId { get; set; }

        /// <summary>
        /// Specifies whether the profiler should be used for the write action.
        /// </summary>
        public bool ShouldProfile { get; set; }=true;

        /// <summary>
        /// This method converts the entity in to a blob ready for blob storage
        /// </summary>
        public Func<MicroserviceId, EventBase, AzureStorageContainerBlob> BlobConverter { get; set; } = null;
        /// <summary>
        /// This method converts the entity in to a dynamic Table entity.
        /// </summary>
        public Func<MicroserviceId, EventBase, AzureStorageContainerTable> TableConverter { get; set; } = null;

        /// <summary>
        /// The storage behaviour, i.e. currently blob, table or both.
        /// </summary>
        public AzureStorageBehaviour Behaviour { get; set; }

        /// <summary>
        /// The encryption type.
        /// </summary>
        public AzureStorageEncryption Encryption { get; set; } = AzureStorageEncryption.BlobWhenPresent;
    }
}
