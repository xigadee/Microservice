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
            , AzureStorageBehaviour behavior = AzureStorageBehaviour.None)
        {
            Support = support;
            Behaviour = behavior;
            var id = AzureStorageDCExtensions.GetEnum<DataCollectionSupport>(support).StringValue;
            ConnectorBlob.RootId = id;
            ConnectorTable.RootId = id;
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
        /// The encryption type.
        /// </summary>
        public AzureStorageEncryption Encryption { get; set; } = AzureStorageEncryption.BlobWhenPresent;

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
