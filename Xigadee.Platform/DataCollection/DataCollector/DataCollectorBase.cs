using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This abstract class is used to implement data collectors.
    /// </summary>
    public abstract class DataCollectorBase: IDataCollectorComponent
    {
        protected DataCollectorBase(string name, DataCollectionSupport support = DataCollectionSupport.All)
        {
            Name = name;
            Support = support;
        }

        /// <summary>
        /// This returns the type of supported data collection
        /// </summary>
        public DataCollectionSupport Support { get; }
        /// <summary>
        /// Returns true if the requested type is supported.
        /// </summary>
        /// <param name="support">The data collection type</param>
        /// <returns></returns>
        public bool IsSupported(DataCollectionSupport support)
        {
            return (Support & support) == support;
        }

        public string Name
        {
            get;
        }

        public string OriginatorId
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
