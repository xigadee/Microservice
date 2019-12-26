using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This class contains the specific policy options for the type of logging.
    /// </summary>
    public class AzureStorageDataCollectorPolicy:DataCollectorPolicy
    {
        /// <summary>
        /// This is the Log options.
        /// </summary>
        public AzureStorageDataCollectorOptions Log { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Logger
                , AzureStorageBehaviour.BlobAndTable
                , serializerTable: AzureStorageHelper.ToTableLogEvent
                , binaryMakeId: BinaryContainerHelper.LoggerMakeId
                , binaryMakeFolder: BinaryContainerHelper.LoggerMakeFolder
                , isSupported: AzureStorageHelper.DefaultLogLevelSupport
                );

        /// <summary>
        /// This is the EventSource options.
        /// </summary>
        public AzureStorageDataCollectorOptions EventSource { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.EventSource
                , AzureStorageBehaviour.Blob
                , binaryMakeId: BinaryContainerHelper.EventSourceMakeId
                , binaryMakeFolder: BinaryContainerHelper.EventSourceMakeFolder
                );
        /// <summary>
        /// This is the Statistics options. By default encryption is not set for statistics.
        /// </summary>
        public AzureStorageDataCollectorOptions Statistics { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Statistics
                , AzureStorageBehaviour.Blob
                , makeId: BinaryContainerHelper.StatisticsMakeId
                , binaryMakeId: BinaryContainerHelper.StatisticsMakeId
                , binaryMakeFolder: BinaryContainerHelper.StatisticsMakeFolder
                )
            { EncryptionPolicy = AzureStorageEncryption.None};
        /// <summary>
        /// This is the Dispatcher options.
        /// </summary>
        public AzureStorageDataCollectorOptions Dispatcher { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Dispatcher
                , AzureStorageBehaviour.Table
                , AzureStorageHelper.ToTableDispatcherEvent
                );
        /// <summary>
        /// This is the Boundary Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Boundary { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Boundary
                , AzureStorageBehaviour.Table
                , AzureStorageHelper.ToTableBoundaryEvent
                );
        /// <summary>
        /// This is the Telemetry Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Telemetry { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Telemetry
                , AzureStorageBehaviour.Table
                , AzureStorageHelper.ToTableTelemetryEvent
                );
        /// <summary>
        /// This is the Resource Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Resource { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Resource
                , AzureStorageBehaviour.Table
                , AzureStorageHelper.ToTableGeneric
                );

        /// <summary>
        /// This is the Resource Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Security { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Security
                , AzureStorageBehaviour.Table
                , AzureStorageHelper.ToTableGeneric
                );

        /// <summary>
        /// This is the Custom options.
        /// </summary>
        public AzureStorageDataCollectorOptions Custom { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Custom);

        /// <summary>
        /// This is the Boundary Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions ApiBoundary { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.ApiBoundary
                , AzureStorageBehaviour.BlobAndTable
                , AzureStorageHelper.ToTableBoundaryEvent
                , binaryMakeId: BinaryContainerHelper.BoundaryMakeId
                , binaryMakeFolder: BinaryContainerHelper.BoundaryMakeFolder
                );

        /// <summary>
        /// This is an enumeration of all the options.
        /// </summary>
        public virtual IEnumerable<AzureStorageDataCollectorOptions> Options
        {
            get
            {
                yield return Log;
                yield return EventSource;
                yield return Statistics;
                yield return Dispatcher;
                yield return Boundary;
                yield return Telemetry;
                yield return Resource;
                yield return Custom;
                yield return Security;
                yield return ApiBoundary;
            }
        }
    }
}
