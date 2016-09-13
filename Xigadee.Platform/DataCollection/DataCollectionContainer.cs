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
        protected List<IDataCollector> mCollectors;
        protected List<ILogger> mLoggers;
        protected List<IEventSource> mEventSource;
        protected List<IBoundaryLogger> mBoundaryLoggers;
        protected List<ITelemetry> mTelemetry;

        /// <summary>
        /// This collection holds the loggers for the Microservice.
        /// </summary>
        protected LoggerContainer mContainerLogger;
        /// <summary>
        /// This collection holds the event sources for the Microservice.
        /// </summary>
        protected EventSourceContainer mContainerEventSource;
        /// <summary>
        /// This collection holds the telemetry components.
        /// </summary>
        protected TelemetryContainer mContainerTelemetry;
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="policy">The policy.</param>
        public DataCollectionContainer(DataCollectionPolicy policy) : base(policy)
        {
            mCollectors = new List<IDataCollector>();
            mLoggers = new List<ILogger>();
            mEventSource = new List<IEventSource>();
            mBoundaryLoggers = new List<IBoundaryLogger>();
            mTelemetry = new List<ITelemetry>();
        } 
        #endregion

        protected override void StartInternal()
        {
            mContainerLogger = new LoggerContainer(mLoggers);
            mContainerEventSource = new EventSourceContainer(mEventSource);
            mContainerTelemetry = new TelemetryContainer(mTelemetry);

            
        }

        protected override void StopInternal()
        {
        }

        public async Task TelemetryFlush()
        {

        }

        #region Add...
        public IDataCollector Add(IDataCollector component)
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

        public string OriginatorId
        {
            get; set;
        }

        #region LogException...
        public void LogException(Exception ex)
        {
            Log(new LogEvent(ex));
        }

        public void LogException(string message, Exception ex)
        {
            Log(new LogEvent(message, ex));
        }
        #endregion
        #region LogMessage...
        public void LogMessage(string message)
        {
            Log(new LogEvent(message));
        }

        public void LogMessage(LoggingLevel logLevel, string message)
        {
            Log(new LogEvent(logLevel, message));
        }

        public void LogMessage(LoggingLevel logLevel, string message, string category)
        {
            Log(new LogEvent(logLevel, message, category));
        }
        public async Task Log(LogEvent logEvent)
        {
        }
        #endregion

        public void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {

        }

        public Guid BatchPoll(int requested, int actual, string channelId)
        {
            var id = Guid.NewGuid();

            return id;
        }


        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
        }

        public void TrackMetric(string metricName, double value)
        {
        }

        public bool CanProcess()
        {
            return false;
        }

        public void Process()
        {
            
        }


        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }



        public Action<TaskTracker> TaskSubmit
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ITaskAvailability TaskAvailability
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
