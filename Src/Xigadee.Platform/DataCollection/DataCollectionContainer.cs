using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class centrally holds all the logging, telemetry and event source support.
    /// </summary>
    public partial class DataCollectionContainer: ServiceContainerBase<DataCollectionContainer.Statistics, DataCollectionContainer.Policy>
        , IDataCollection, IRequireServiceOriginator
        , ITaskManagerProcess, IRequireSharedServices, IRequireServiceHandlers
    {
        #region Declarations
        /// <summary>
        /// This is a collection of the supported collectors. You cannot add the same collector more than once.
        /// </summary>
        private HashSet<IDataCollectorComponent> mCollectors = new HashSet<IDataCollectorComponent>();

        private Action<TaskTracker> mTaskSubmit;

        private ITaskAvailability mTaskAvailability;

        private Dictionary<DataCollectionSupport, HashSet<IDataCollectorComponent>> mCollectorSupported;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="policy">The policy.</param>
        public DataCollectionContainer(DataCollectionContainer.Policy policy) : base(policy)
        {
        }
        #endregion

        #region StartInternal/StopInternal
        /// <summary>
        /// This method starts the data collector.
        /// </summary>
        protected override void StartInternal()
        {
            mCollectors.ForEach((c) => ServiceStart(c));

            CollectorSupportSet();

            StartQueue();
        }

        /// <summary>
        /// This method builds the collector dictionary for all the supported collection types.
        /// </summary>
        private void CollectorSupportSet()
        {
            mCollectorSupported = new Dictionary<DataCollectionSupport, HashSet<IDataCollectorComponent>>();

            var dataTypes = Enum.GetValues(typeof(DataCollectionSupport)).Cast<DataCollectionSupport>();

            foreach (var enumitem in dataTypes)
            {
                var items = mCollectors.Where((i) => i.IsSupported(enumitem)).Distinct().ToList();

                mCollectorSupported.Add(enumitem, items.Count == 0?null:new HashSet<IDataCollectorComponent>(items));
            }
        }
        /// <summary>
        /// This method stops the data collector.
        /// </summary>
        protected override void StopInternal()
        {
            StopQueue();

            mCollectors.ForEach((c) => ServiceStop(c));

            mCollectorSupported.Clear();
        }
        #endregion

        #region Add...        
        /// <summary>
        /// Adds the specified data collector to the collection.
        /// </summary>
        /// <param name="component">The data collector component.</param>
        /// <returns>Returns the component.</returns>
        public IDataCollectorComponent Add(IDataCollectorComponent component)
        {
            mCollectors.Add(component);
            return component;
        }
        #endregion

        #region ServiceStart(object service)
        /// <summary>
        /// This override sets the originator for the internal components.
        /// </summary>
        /// <param name="service">The service to start</param>
        protected override void ServiceStart(object service)
        {
            try
            {
                if ((service as IRequireServiceOriginator) != null)
                    ((IRequireServiceOriginator)service).OriginatorId = OriginatorId;

                if ((service as IRequireSharedServices) != null)
                    ((IRequireSharedServices)service).SharedServices = SharedServices;

                if ((service as IRequireServiceHandlers) != null)
                    ((IRequireServiceHandlers)service).ServiceHandlers = ServiceHandlers;

                if ((service as IRequireDataCollector) != null)
                    ((IRequireDataCollector)service).Collector = this;

                base.ServiceStart(service);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error starting data collection service [{0}]: {1}", service.GetType().Name, ex.ToString());
                throw;
            }
        } 
        #endregion

        #region OriginatorId
        /// <summary>
        /// This is the unique id for the underlying Microservice.
        /// </summary>
        public MicroserviceId OriginatorId
        {
            get; set;
        }
        #endregion
        #region SharedServices
        /// <summary>
        /// This is the shared service collection
        /// </summary>
        public ISharedService SharedServices
        {
            get;
            set;
        }
        #endregion
        #region ServiceHandlers
        /// <summary>
        /// This is the system wide service handler collection.
        /// </summary>
        public IServiceHandlers ServiceHandlers { get; set; } 
        #endregion

        #region Flush()
        /// <summary>
        /// This method calls the underlying collectors and initiates a flush of any pending data.
        /// </summary>
        public async Task Flush()
        {
            try
            {
                mCollectors?.Where((c) => c.CanFlush).ForEach((c) => c.Flush());
            }
            catch (Exception ex)
            {
                this.LogException("DataCollectionContainer/Flush failed.", ex);
            }
        }
        #endregion

        #region Class -> Policy
        /// <summary>
        /// This class holds the data collection policy.
        /// </summary>
        public class Policy: PolicyBase
        {
            /// <summary>
            /// This is the maximum time that an overload process should run.
            /// </summary>
            public int OverloadProcessTimeInMs { get; set; } = 10000; //10s
            /// <summary>
            /// This is the maximum number of overload tasks that should be run concurrently.
            /// </summary>
            public int OverloadMaxTasks { get; set; } = 2;
            /// <summary>
            /// This is the threshold at which point the overload tasks will be triggered.
            /// </summary>
            public int? OverloadThreshold { get; set; } = 1000;
            /// <summary>
            /// This is the number of retry attempts to be made if the write fails.
            /// </summary>
            public int RetryLimit { get; set; } = 0;

            /// <summary>
            /// This is the event source retry limit.
            /// </summary>
            public int EventSourceRetryLimit { get; set; } = 3;
        }
        #endregion
        #region Class -> Statistics
        /// <summary>
        /// This is the base data collection statistics.
        /// </summary>
        public class Statistics: MessagingStatistics
        {
            public int ItemCount { get; set; }

            public int QueueLength { get; set; }

            public bool Overloaded { get; set; }

            public int OverloadProcessCount { get; set; }

            public int? OverloadThreshold { get; set; }

            public long OverloadProcessHits { get; set; }

        } 
        #endregion
    }
}
