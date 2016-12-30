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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    internal class DataCollectionWrapper: WrapperBase, IMicroserviceDataCollection
    {
        /// <summary>
        /// This collection holds the loggers for the Microservice.
        /// </summary>
        private DataCollectionContainer mDataCollection;

        public DataCollectionWrapper(DataCollectionContainer dataCollection, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mDataCollection = dataCollection;
        }

        //Data Collector
        #region Register(IDataCollectorComponent collector)
        /// <summary>
        /// This method is used to register a collector.
        /// </summary>
        /// <param name="collector">The collector component.</param>
        /// <returns>Returns the collector passed through the registration.</returns>
        public IDataCollectorComponent Register(IDataCollectorComponent collector)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(collector);
            return collector;
        }
        #endregion
        #region RegisterEventSource(IEventSourceComponent eventSource)
        /// <summary>
        /// This method can be used to manually register an EventSource.
        /// </summary>
        public virtual IEventSourceComponent RegisterEventSource(IEventSourceComponent eventSource)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(eventSource);
            return eventSource;
        }
        #endregion
        #region RegisterLogger(ILogger logger)
        /// <summary>
        /// This method can be used to manually register an Collector?.
        /// </summary>
        public virtual ILogger RegisterLogger(ILogger logger)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(logger);
            return logger;
        }
        #endregion
        #region RegisterBoundaryLogger(IBoundaryLoggerComponent logger)
        /// <summary>
        /// This method can be used to manually register an Collector?.
        /// </summary>
        public virtual IBoundaryLoggerComponent RegisterBoundaryLogger(IBoundaryLoggerComponent logger)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(logger);
            return logger;
        }
        #endregion
    }
}
