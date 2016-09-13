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
    public partial class DataCollectionContainer: ServiceContainerBase<DataCollectionStatistics, DataCollectionPolicy>, IDataCollector, ILoggerExtended, ITaskManagerProcess
    {
        #region Declarations
        private List<IDataCollectorComponent> mCollectors;
        private List<ILogger> mLoggers;
        private List<IEventSource> mEventSource;
        private List<IBoundaryLogger> mBoundaryLoggers;
        private List<ITelemetry> mTelemetry;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="policy">The policy.</param>
        public DataCollectionContainer(DataCollectionPolicy policy) : base(policy)
        {
            mCollectors = new List<IDataCollectorComponent>();
            mLoggers = new List<ILogger>();
            mEventSource = new List<IEventSource>();
            mBoundaryLoggers = new List<IBoundaryLogger>();
            mTelemetry = new List<ITelemetry>();
        }
        #endregion

        #region StartInternal/StopInternal
        protected override void StartInternal()
        {
            mCollectors.ForEach((c) => ServiceStart(c));
            StartTelemetry();
            StartEventSource();
            StartLogger();
        }

        protected override void StopInternal()
        {
            StopTelemetry();
            StopEventSource();
            StopLogger();
        }
        #endregion

        #region Add...
        public IDataCollector Add(IDataCollectorComponent component)
        {
            mCollectors.Add(component);
            return component;
        }

        public ILogger Add(ILogger component)
        {
            mLoggers.Add(component);
            return component;
        }

        public IEventSource Add(IEventSource component)
        {
            mEventSource.Add(component);
            return component;
        }

        public IBoundaryLogger Add(IBoundaryLogger component)
        {
            mBoundaryLoggers.Add(component);
            return component;
        }

        public ITelemetry Add(ITelemetry component)
        {
            mTelemetry.Add(component);
            return component;
        }
        #endregion

        protected override void ServiceStart(object service)
        {
            try
            {
                if ((service as IServiceOriginator) != null)
                    ((IServiceOriginator)service).OriginatorId = OriginatorId;

                base.ServiceStart(service);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error starting data collection service [{0}]: {1}", service.GetType().Name, ex.ToString());
                throw;
            }

        }

        public string OriginatorId
        {
            get; set;
        }


        public bool CanProcess()
        {
            return false;
        }

        public void Process()
        {
            
        }

        public Action<TaskTracker> TaskSubmit
        {
            get;set;
        }

        public ITaskAvailability TaskAvailability
        {
            get;set;
        }

        public DataCollectionSupport Support
        {
            get
            {
                return DataCollectionSupport.All;
            }
        }

        public bool IsSupported(DataCollectionSupport support)
        {
            return true;
        }

    }
}
