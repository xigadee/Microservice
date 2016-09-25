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
        , ILogger, IEventSource, ITelemetry, IServiceOriginator, ILoggerExtended, ITaskManagerProcess
    {
        #region Declarations
        private List<IDataCollectorComponent> mCollectors;
        private List<ILogger> mLoggers;
        private List<IEventSourceComponent> mEventSource;
        private List<IBoundaryLoggerComponent> mBoundaryLoggers;
        private List<ITelemetry> mTelemetry;

        private Action<TaskTracker> mTaskSubmit;
        private ITaskAvailability mTaskAvailability;
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
            mEventSource = new List<IEventSourceComponent>();
            mBoundaryLoggers = new List<IBoundaryLoggerComponent>();
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
            mCollectors.ForEach((c) => ServiceStop(c));
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
            mLoggers.Add(component);
            return component;
        }

        public IEventSource Add(IEventSourceComponent component)
        {
            mEventSource.Add(component);
            return component;
        }

        public IBoundaryLoggerComponent Add(IBoundaryLoggerComponent component)
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

        #region CanProcess()
        /// <summary>
        /// This method checks whether there are overloaded services.
        /// </summary>
        /// <returns>Returns true if any of the queues need additional processing.</returns>
        public bool CanProcess()
        {
            return mContainerEventSource.CanProcess() || mContainerLogger.CanProcess();
        }
        #endregion
        #region Process()
        /// <summary>
        /// This method attempts to process the overload.
        /// </summary>
        public void Process()
        {
            ProcessCheck(mContainerEventSource);
            ProcessCheck(mContainerLogger);
        }
        #endregion
        #region ProcessCheck(ITaskManagerProcess process)
        /// <summary>
        /// This method checks whether an overload is set.
        /// </summary>
        /// <param name="process">The process to check.</param>
        private void ProcessCheck(ITaskManagerProcess process)
        {
            if (process.CanProcess())
                process.Process();
        }
        #endregion

        #region TaskSubmit
        /// <summary>
        /// This action is used to submit a task to the tracker for overflow 
        /// </summary>
        public Action<TaskTracker> TaskSubmit
        {
            get { return mTaskSubmit; }
            set
            {
                mTaskSubmit = value;
                mContainerEventSource.TaskSubmit = value;
                mContainerLogger.TaskSubmit = value;
            }
        }
        #endregion
        #region TaskAvailability
        /// <summary>
        /// This method is used to signal that a process has a task that needs processing.
        /// </summary>
        public ITaskAvailability TaskAvailability
        {
            get { return mTaskAvailability; }
            set
            {
                mTaskAvailability = value;
                mContainerEventSource.TaskAvailability = value;
                mContainerLogger.TaskAvailability = value;
            }
        } 
        #endregion
    }
}
