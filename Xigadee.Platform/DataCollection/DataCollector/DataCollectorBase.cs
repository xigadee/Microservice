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
    public abstract class DataCollectorBase: DataCollectorBase<DataCollectorStatistics>
    {
        public DataCollectorBase(DataCollectionSupport support = DataCollectionSupport.All) : base(support)
        {
        }
    }

    /// <summary>
    /// This abstract class is used to implement data collectors.
    /// </summary>
    public abstract class DataCollectorBase<S>: ServiceBase<S>, IDataCollectorComponent
        where S : DataCollectorStatistics, new()
    {
        #region Constructor
        /// <summary>
        /// This constructor passes in the support types for the collector.
        /// </summary>
        /// <param name="support">The support types - all by default.</param>
        protected DataCollectorBase(DataCollectionSupport support = DataCollectionSupport.All)
        {
            Support = support;
        } 
        #endregion

        /// <summary>
        /// This returns the type of supported data collection
        /// </summary>
        public virtual DataCollectionSupport Support { get; }

        /// <summary>
        /// Returns true if the requested type is supported.
        /// </summary>
        /// <param name="support">The data collection type</param>
        /// <returns></returns>
        public virtual bool IsSupported(DataCollectionSupport support)
        {
            return (Support & support) == support;
        }

        /// <summary>
        /// This is is the Microservice originator information.
        /// </summary>
        public virtual MicroserviceId OriginatorId
        {
            get; set;
        }

        public abstract void Write(EventSourceEvent eventData);

        public abstract void Write(MetricEvent eventData);

        public abstract void Write(LogEvent eventData);

        public abstract void Write(PayloadEvent eventData);

        public abstract void Write(BoundaryEvent eventData);

        public abstract void Write(MicroserviceStatistics eventData);
    }


    //public abstract class DataCollectorObjectBase<S>: DataCollectorBase<S>
    //    where S : DataCollectorStatistics, new()
    //{
    //    public DataCollectorObjectBase(string name, DataCollectionSupport support = DataCollectionSupport.All) : base(name, support)
    //    {
    //    }




    //    public override async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
    //    {
    //        Write(new EventSourceEvent { OriginatorId = originatorId, Entry = entry, UtcTimeStamp = utcTimeStamp, Sync = sync });
    //    }



    //}
}
