using System;

namespace Xigadee
{
    /// <summary>
    /// This class contains the storage configuration options for the specific DataCollection type.
    /// </summary>
    public class EventHubDataCollectorOptions
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="support">The collection support type.</param>
        /// <param name="serializerBinary">Serializer used to serialize the event object</param>
        /// <param name="entityPath">Function to determine the entity path that will be used when sending messages to event hub</param>
        /// <param name="isSupported">Function to determine whether the event is supported</param>
        public EventHubDataCollectorOptions(DataCollectionSupport support
            , Func<EventHolder, MicroserviceId, AzureStorageBinary> serializerBinary = null
            , Func<EventHolder, MicroserviceId, string> entityPath = null
            , Func<EventHolder, bool> isSupported = null)
        {
            Support = support;

            SerializerBinary = serializerBinary ?? AzureStorageHelper.DefaultJsonBinarySerializer;
            EntityPath = entityPath ?? ((ev, ms) => $"{ms.Name}_{support.ToString()}");

            IsSupported = isSupported ?? (ev => true);
        }

        /// <summary>
        /// This function can be set to provide the event hub entity patch
        /// </summary>
        public Func<EventHolder, MicroserviceId, string> EntityPath { get; set; }

        /// <summary>
        /// This function can be set to provide specific binary serialization.
        /// </summary>
        public Func<EventHolder, MicroserviceId, AzureStorageBinary> SerializerBinary { get; set; }
        
        /// <summary>
        /// This function can be used to filter out specific event from being written.
        /// </summary>
        public Func<EventHolder, bool> IsSupported { get; set; }

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
        public AzureStorageEncryption EncryptionPolicy { get; set; } = AzureStorageEncryption.None;

        /// <summary>
        /// Specifies whether the profiler should be used for the write action.
        /// </summary>
        public bool ShouldProfile { get; set; } = true;
    }
}
