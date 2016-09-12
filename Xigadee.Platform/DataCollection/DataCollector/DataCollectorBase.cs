using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class DataCollectorBase: IDataCollector
    {
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string OriginatorId
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

        public Guid BatchPoll(int requested, int actual, string channelId)
        {
            throw new NotImplementedException();
        }

        public Task Log(LogEvent logEvent)
        {
            throw new NotImplementedException();
        }

        public void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception ex)
        {
            throw new NotImplementedException();
        }

        public void LogException(string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void LogMessage(string message)
        {
            throw new NotImplementedException();
        }

        public void LogMessage(LoggingLevel level, string message)
        {
            throw new NotImplementedException();
        }

        public void LogMessage(LoggingLevel level, string message, string category)
        {
            throw new NotImplementedException();
        }

        public void TrackMetric(string metricName, double value)
        {
            throw new NotImplementedException();
        }

        public Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            throw new NotImplementedException();
        }
    }
}
