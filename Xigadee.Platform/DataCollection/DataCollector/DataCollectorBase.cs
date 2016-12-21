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
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This abstract class allows data collectors to be create without the need for a policy.
    /// </summary>
    public abstract class DataCollectorBase: DataCollectorBase<DataCollectorStatistics, DataCollectorPolicy>
    {
        #region Constructor
        /// <summary>
        /// This constructor passes in the support types for the collector.
        /// </summary>
        protected DataCollectorBase(DataCollectionSupport? supportMap = null) : base(supportMap)
        {
        }
        #endregion
    }

    /// <summary>
    /// This abstract class allows data collectors to be create without the need for a policy.
    /// </summary>
    /// <typeparam name="S">The statistics type.</typeparam>
    public abstract class DataCollectorBase<S>: DataCollectorBase<S, DataCollectorPolicy>
        where S : DataCollectorStatistics, new()
    {
        #region Constructor
        /// <summary>
        /// This constructor passes in the support types for the collector.
        /// </summary>
        protected DataCollectorBase(DataCollectionSupport? supportMap = null) : base(supportMap)
        {
        }
        #endregion
    }

    /// <summary>
    /// This abstract class is used to implement data collectors.
    /// </summary>
    /// <typeparam name="S">The statistics type.</typeparam>
    /// <typeparam name="P">The policy type.</typeparam>
    public abstract class DataCollectorBase<S, P>: ServiceContainerBase<S, P>, IDataCollectorComponent
        where S : DataCollectorStatistics, new()
        where P : DataCollectorPolicy, new()
    {
        #region Declarations
        /// <summary>
        /// This dictionary object holds the action mapping for the logging type.
        /// </summary>
        protected Dictionary<DataCollectionSupport, Action<EventBase>> mSupported;
        /// <summary>
        /// This is the support map which indicates which type of logging is supported by the collector.
        /// </summary>
        protected readonly DataCollectionSupport? mSupportMapSubmitted;
        /// <summary>
        /// This is the actual calculated support map based on the mappings available.
        /// </summary>
        protected DataCollectionSupport mSupportMapActual;
        #endregion
        #region Constructor
        /// <summary>
        /// This constructor passes in the support types for the collector.
        /// </summary>
        protected DataCollectorBase(DataCollectionSupport? supportMap = null) :base()
        {
            mSupportMapSubmitted = supportMap;
        }
        #endregion

        #region Start/Stop ...
        /// <summary>
        /// This method configures the mapping.
        /// </summary>
        protected override void StartInternal()
        {
            mSupported = new Dictionary<DataCollectionSupport, Action<EventBase>>();

            SupportLoadDefault();

            var support = mSupported.Select((k) => k.Key).Aggregate((a, b) => a | b);

            if (mSupportMapSubmitted.HasValue)
                mSupportMapActual = support & mSupportMapSubmitted.Value;
            else
                mSupportMapActual = support;
        }
        /// <summary>
        /// This method stops the collection.
        /// </summary>
        protected override void StopInternal()
        {
            mSupported.Clear();
        } 
        #endregion

        #region SupportLoadDefault()
        /// <summary>
        /// This method loads the support.
        /// </summary>
        protected virtual void SupportLoadDefault()
        {
            throw new NotImplementedException("DataCollectorBase/SupportLoadDefault must be implemented to enable support.");
        }
        #endregion
        #region SupportAdd(DataCollectionSupport eventType, Action<EventBase> eventData)
        /// <summary>
        /// This method adds support for the log event.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventData">The event data.</param>
        protected virtual void SupportAdd(DataCollectionSupport eventType, Action<EventBase> eventData)
        {
            mSupported[eventType] = eventData;
        } 
        #endregion

        #region IsSupported(DataCollectionSupport support)
        /// <summary>
        /// Returns true if the requested type is supported.
        /// </summary>
        /// <param name="support">The data collection type</param>
        /// <returns></returns>
        public virtual bool IsSupported(DataCollectionSupport support)
        {
            return (mSupportMapActual & support)>0;
        }
        #endregion
        #region OriginatorId
        /// <summary>
        /// This is is the Microservice originator information.
        /// </summary>
        public virtual MicroserviceId OriginatorId
        {
            get; set;
        }
        #endregion

        #region Write(DataCollectionSupport eventType, EventBase eventData)
        /// <summary>
        /// This method is the generic write that maps to the collection.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventData">The event data.</param>
        public virtual void Write(DataCollectionSupport eventType, EventBase eventData)
        {
            if (IsSupported(eventType))
                mSupported[eventType](eventData);
        }
        #endregion

        #region Flush ...
        /// <summary>
        /// This method is called to flush the collector, if CanFlush is set to true.
        /// </summary>
        public virtual void Flush()
        {
            //This does nothing by default.
        }

        /// <summary>
        /// This method specifies whether the collector supports flushing. The default is false.
        /// </summary>
        public bool CanFlush { get; set; } 
        #endregion
    }
}
