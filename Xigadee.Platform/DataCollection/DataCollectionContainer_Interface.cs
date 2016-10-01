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


        /// <summary>
        /// This is the external method to submit events to the event source.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="originatorId"></param>
        /// <param name="entry"></param>
        /// <param name="utcTimeStamp"></param>
        /// <param name="sync"></param>
        /// <returns></returns>
        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            Write(new EventSourceEvent { OriginatorId = originatorId, Entry = entry, UtcTimeStamp = utcTimeStamp}, DataCollectionSupport.EventSource, sync);
        }

        public Guid BoundaryBatchPoll(int requested, int actual, string channelId)
        {
            var id = Guid.NewGuid();

            Write(new BoundaryEvent { Type = BoundaryEventType.Poll, Id = id, Requested = requested, Actual = actual, ChannelId = channelId }, DataCollectionSupport.BoundaryLogger);

            return id;
        }

        public void BoundaryLog(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {
            Write(new BoundaryEvent { Type = BoundaryEventType.Boundary, Direction = direction, Payload = payload, Ex = ex, Id = batchId }, DataCollectionSupport.BoundaryLogger);
        }



        public virtual void DispatcherPayloadComplete(TransmissionPayload payload, int delta, bool isSuccess)
        {
            Write(new DispatcherEvent { Type = PayloadEventType.Complete, Payload = payload, Delta = delta, IsSuccess = isSuccess }, DataCollectionSupport.Dispatcher);
        }

        public virtual void DispatcherPayloadException(TransmissionPayload payload, Exception pex)
        {
            Write(new DispatcherEvent { Type = PayloadEventType.Exception, Payload = payload, Ex = pex }, DataCollectionSupport.Dispatcher);
        }

        public virtual void DispatcherPayloadIncoming(TransmissionPayload payload)
        {
            Write(new DispatcherEvent { Type = PayloadEventType.Incoming, Payload = payload }, DataCollectionSupport.Dispatcher);
        }

        public virtual void DispatcherPayloadUnresolved(TransmissionPayload payload, DispatcherRequestUnresolvedReason reason)
        {
            Write(new DispatcherEvent { Type = PayloadEventType.Unresolved, Payload = payload, Reason = reason }, DataCollectionSupport.Dispatcher);
        }

        public virtual void MicroserviceStatisticsIssued(MicroserviceStatistics statistics)
        {
            Write(statistics, DataCollectionSupport.Statistics);
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

            Write(new MetricEvent { MetricName = metricName, Value = value }, DataCollectionSupport.Telemetry);
        }
        #endregion

        #region Log(LogEvent logEvent)
        /// <summary>
        /// This method logs the event. 
        /// </summary>
        /// <param name="logEvent">The incoming event</param>
        public async Task Log(LogEvent logEvent)
        {
            Write(logEvent, DataCollectionSupport.Logger);
        }
        #endregion

        //Extended logging methods
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
        #endregion
    }
}
