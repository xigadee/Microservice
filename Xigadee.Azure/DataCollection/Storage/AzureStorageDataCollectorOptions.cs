using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

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
            , Func<EventBase, MicroserviceId, ITableEntity> tableSerializer = null)
        {
            Support = support;
            Behaviour = behavior;

            var id = AzureStorageHelper.GetEnum<DataCollectionSupport>(support).StringValue;
            ConnectorBlob.RootId = id;

            ConnectorTable.RootId = id;
            ConnectorTable.Serializer = tableSerializer;

            ConnectorQueue.RootId = id;
            ConnectorFile.RootId = id;
        }

        /// <summary>
        /// This is the support type for the options handler, i.e. LogEvent, EventSource, etc.
        /// </summary>
        public DataCollectionSupport Support { get; }

        /// <summary>
        /// The storage behaviour, i.e. currently blob, table or both.
        /// </summary>
        public AzureStorageBehaviour Behaviour { get; set; }

        /// <summary>
        /// The encryption storage policy.
        /// </summary>
        public AzureStorageEncryption EncryptionPolicy { get; set; } = AzureStorageEncryption.BlobWhenPresent;

        /// <summary>
        /// Specifies whether the profiler should be used for the write action.
        /// </summary>
        public bool ShouldProfile { get; set; }=true;

        public AzureStorageConnectorBlob ConnectorBlob { get; set; } = new AzureStorageConnectorBlob();
        public AzureStorageConnectorTable ConnectorTable { get; set; } = new AzureStorageConnectorTable();
        public AzureStorageConnectorQueue ConnectorQueue { get; set; } = new AzureStorageConnectorQueue();
        public AzureStorageConnectorFile ConnectorFile { get; set; } = new AzureStorageConnectorFile();

    }
}
