using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is a test collector. It is primarily used for unit testing to ensure the correct logging has occurred.
    /// </summary>
    public class MemoryCollector: DataCollectorBase
    {
        public MemoryCollector() : base(typeof(MemoryCollector).Name)
        {
        }

        public override Guid BatchPoll(int requested, int actual, string channelId)
        {
            throw new NotImplementedException();
        }

        public override Task Log(LogEvent logEvent)
        {
            throw new NotImplementedException();
        }

        public override void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {
            throw new NotImplementedException();
        }

        public override void TrackMetric(string metricName, double value)
        {
            throw new NotImplementedException();
        }

        public override Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            throw new NotImplementedException();
        }

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }
    }
}
