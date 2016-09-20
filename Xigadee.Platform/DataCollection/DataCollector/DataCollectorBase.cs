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
        public DataCollectorBase(string name, DataCollectionSupport support = DataCollectionSupport.All) : base(name, support)
        {
        }
    }

    /// <summary>
    /// This abstract class is used to implement data collectors.
    /// </summary>
    public abstract class DataCollectorBase<S>: ServiceBase<S>, IDataCollectorComponent
        where S:DataCollectorStatistics, new()
    {
        protected DataCollectorBase(string name, DataCollectionSupport support = DataCollectionSupport.All)
        {
            Name = name;
            Support = support;
        }

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
        /// This is the name of the data collector.
        /// </summary>
        public virtual string Name
        {
            get;
        }

        /// <summary>
        /// This is is the Microservice originator information.
        /// </summary>
        public virtual MicroserviceId OriginatorId
        {
            get;set;
        }

        public abstract Guid BatchPoll(int requested, int actual, string channelId);

        public abstract Task Log(LogEvent logEvent);

        public abstract void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?));

        public abstract void TrackMetric(string metricName, double value);

        public abstract Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false);
    }
}
