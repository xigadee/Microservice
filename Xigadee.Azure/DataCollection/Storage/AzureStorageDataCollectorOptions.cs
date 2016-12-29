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
            , Func<EventBase, MicroserviceId, ITableEntity> serializerTable = null
            , Func<EventBase, MicroserviceId, AzureStorageBinary> serializerBinary = null
            , Func<EventBase, MicroserviceId, string> makeId = null
            , Func<EventBase, MicroserviceId, string> binaryMakeId = null
            , Func<EventBase, MicroserviceId, string> binaryMakeFolder = null
            , Func<AzureStorageBehaviour, EventBase, bool> isSupported = null
            )
        {
            Support = support;
            Behaviour = behavior;
            SerializerTable = serializerTable;
            SerializerBinary = serializerBinary ?? AzureStorageHelper.DefaultJsonBinarySerializer;

            MakeId = makeId ?? ((EventBase e, MicroserviceId i) => e.TraceId);
            BinaryMakeId = binaryMakeId ?? MakeId;
            BinaryMakeFolder = binaryMakeFolder;

            IsSupported = isSupported ?? ((b,e) => true);
        }

        public Func<EventBase, MicroserviceId, string> MakeId { get; set; }

        public Func<EventBase, MicroserviceId, string> BinaryMakeId { get; set; }

        public Func<EventBase, MicroserviceId, string> BinaryMakeFolder { get; set; }

        /// <summary>
        /// This function can be set to provide specific table serialization.
        /// </summary>
        public Func<EventBase, MicroserviceId, ITableEntity> SerializerTable { get; set; }

        /// <summary>
        /// This function can be set to provide specific binary serialization.
        /// </summary>
        public Func<EventBase, MicroserviceId, AzureStorageBinary> SerializerBinary { get; set; }
        /// <summary>
        /// This function can be used to filter out specific event from writing for certain storage support.
        /// </summary>
        public Func<AzureStorageBehaviour, EventBase, bool> IsSupported { get; set; }
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

    }
}
