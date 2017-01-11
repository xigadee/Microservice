#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class centrally holds all the logging, telemetry and event source support.
    /// </summary>
    public partial class DataCollectionContainer: ServiceContainerBase<DataCollectionStatistics, DataCollectionPolicy>
        , IDataCollection, IEventSource, IServiceOriginator
        , ITaskManagerProcess, IRequireSharedServices, IRequireSecurityService
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
        public DataCollectionContainer(DataCollectionPolicy policy) : base(policy)
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
        public IDataCollectorComponent Add(IDataCollectorComponent component)
        {
            mCollectors.Add(component);
            return component;
        }

        public ILogger Add(ILogger component)
        {
            var legacy = new DataCollectorLegacySupport<LogEvent, ILogger>(
                DataCollectionSupport.Logger, component, (l,e) => l.Log(e));

            Add(legacy);

            return component;
        }

        public IEventSource Add(IEventSourceComponent component)
        {
            //mEventSource.Add(component);
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
                if ((service as IServiceOriginator) != null)
                    ((IServiceOriginator)service).OriginatorId = OriginatorId;

                if ((service as IRequireSharedServices) != null)
                    ((IRequireSharedServices)service).SharedServices = SharedServices;

                if ((service as IRequireSecurityService) != null)
                    ((IRequireSecurityService)service).Security = Security;

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

        #region Security
        /// <summary>
        /// This is the security service primarily used for encryption.
        /// </summary>
        public ISecurityService Security
        {
            get; set;
        } 
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
    }
}
