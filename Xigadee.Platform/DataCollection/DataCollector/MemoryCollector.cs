using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is a test collector.
    /// </summary>
    public class MemoryCollector: IDataCollector
    {
        public string Name
        {
            get;
            protected set;
        }

        public string OriginatorId
        {
            get;set;
        }

        public Guid BatchPoll(int requested, int actual, string channelId)
        {
            var id = Guid.NewGuid();

            return id;
        }

        public async Task Log(LogEvent logEvent)
        {

        }

        public void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {

        }

        public void TrackMetric(string metricName, double value)
        {

        }

        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {

        }
    }
}
