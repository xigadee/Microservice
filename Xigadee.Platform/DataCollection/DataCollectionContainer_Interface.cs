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
    public partial class DataCollectionContainer
    {

        public void BoundaryLog(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {
            Write(new BoundaryEvent { Type = BoundaryEventType.Boundary, Direction = direction, Payload = payload, Ex = ex, Id = batchId });
        }

        public Guid BoundaryBatchPoll(int requested, int actual, string channelId)
        {
            var id = Guid.NewGuid();

            Write(new BoundaryEvent { Type = BoundaryEventType.Poll, Id = id, Requested = requested, Actual = actual, ChannelId = channelId });

            return id;
        }

        public virtual void DispatcherPayloadComplete(TransmissionPayload payload, int delta, bool isSuccess)
        {
            Write(new PayloadEvent { Type = PayloadEventType.Complete, Payload = payload, Delta = delta, IsSuccess = isSuccess });
        }

        public virtual void DispatcherPayloadException(TransmissionPayload payload, Exception pex)
        {
            Write(new PayloadEvent { Type = PayloadEventType.Exception, Payload = payload, Ex = pex });
        }

        public virtual void DispatcherPayloadIncoming(TransmissionPayload payload)
        {
            Write(new PayloadEvent { Type = PayloadEventType.Incoming, Payload = payload });
        }

        public virtual void DispatcherPayloadUnresolved(TransmissionPayload payload, DispatcherRequestUnresolvedReason reason)
        {
            Write(new PayloadEvent { Type = PayloadEventType.Unresolved, Payload = payload, Reason = reason });
        }

        public virtual void MicroserviceStatisticsIssued(MicroserviceStatistics statistics)
        {
            Write(statistics);
        }

        #region TrackMetric(string metricName, double value)
        /// <summary>
        /// Track the metric for each of the tranckers.
        /// </summary>
        /// <param name="metricName"></param>
        /// <param name="value"></param>
        public void TrackMetric(string metricName, double value)
        {
            //mTelemetry.ForEach((c) =>
            //{
            //    try
            //    {
            //        c.TrackMetric(metricName, value);
            //    }
            //    catch { }
            //});

            Write(new MetricEvent { MetricName = metricName, Value = value });
        }
        #endregion

        #region Log(LogEvent logEvent)
        /// <summary>
        /// This method logs the event. 
        /// </summary>
        /// <param name="logEvent">The incoming event</param>
        public async Task Log(LogEvent logEvent)
        {
            Write(logEvent);
        }
        #endregion

        //Extended logging methods
        #region LogException...
        public void LogException(Exception ex)
        {
            Write(new LogEvent(ex));
        }

        public void LogException(string message, Exception ex)
        {
            Write(new LogEvent(message, ex));
        }
        #endregion
        #region LogMessage...
        public void LogMessage(string message)
        {
            Write(new LogEvent(message));
        }

        public void LogMessage(LoggingLevel logLevel, string message)
        {
            Write(new LogEvent(logLevel, message));
        }

        public void LogMessage(LoggingLevel logLevel, string message, string category)
        {
            Write(new LogEvent(logLevel, message, category));
        }
        #endregion
    }
}
