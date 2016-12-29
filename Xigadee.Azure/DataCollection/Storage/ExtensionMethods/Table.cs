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
using Microsoft.WindowsAzure.Storage.Table;

namespace Xigadee
{
    public static partial class AzureStorageHelper
    {
        public static void AddPayloadCommon(this Dictionary<string, EntityProperty> dict, TransmissionPayload payload)
        {
            if (payload == null)
                return;

            dict.Add("PayloadId", new EntityProperty(payload.Id));
            dict.Add("PayloadSource", new EntityProperty(payload.Source));
            dict.Add("PayloadOptions", new EntityProperty(payload.Options.ToString()));
            dict.Add("PayloadCorrelationKey", new EntityProperty(payload.Message?.ProcessCorrelationKey));
            dict.Add("PayloadOriginatorKey", new EntityProperty(payload.Message?.OriginatorKey));
            dict.Add("PayloadOriginatorUTC", new EntityProperty(payload.Message?.OriginatorUTC));
            dict.Add("PayloadKey", new EntityProperty(payload.Message?.ToKey()));
            dict.Add("PayloadResponseKey", new EntityProperty(payload.Message?.ToResponseKey()));
        }

        public static ITableEntity ToTableGeneric(EventBase e, MicroserviceId id)
        {
            var dict = new Dictionary<string, EntityProperty>();

            return new DynamicTableEntity(e.GetType().Name + DatePartition(), e.TraceId, "*", dict);
        }

        public static ITableEntity ToTableLogEvent(EventBase e, MicroserviceId id)
        {
            var ev = e as LogEvent;

            var dict = new Dictionary<string, EntityProperty>();

            dict.Add("Machine", new EntityProperty(id.MachineName));
            dict.Add("Name", new EntityProperty(id.Name));
            dict.Add("ServiceId", new EntityProperty(id.ServiceId));

            dict.Add("Level", GetEnum<LoggingLevel>(ev.Level));
            dict.Add("Category", new EntityProperty(ev.Category));
            dict.Add("Message", new EntityProperty(ev.Message));

            dict.Add("Ex", new EntityProperty(ev.Ex?.Message));

            //ETag: Set this value to '*' to blindly overwrite an entity as part of an update operation.
            return new DynamicTableEntity("Logger" + DatePartition(), ev.TraceId, "*", dict);
        }

        public static ITableEntity ToTableDispatcherEvent(EventBase e, MicroserviceId msId)
        {
            var ev = e as DispatcherEvent;

            var dict = new Dictionary<string, EntityProperty>();
            dict.Add("IsSuccess", new EntityProperty(ev.IsSuccess));
            dict.Add("Type", GetEnum<PayloadEventType>(ev.Type));
            dict.Add("Reason", GetEnum<DispatcherRequestUnresolvedReason>(ev.Reason));
            dict.Add("Delta", new EntityProperty(ev.Delta));
            dict.Add("Ex", new EntityProperty(ev.Ex?.Message));
            dict.AddPayloadCommon(ev.Payload);

            //ETag: Set this value to '*' to blindly overwrite an entity as part of an update operation.
            return new DynamicTableEntity("Dispatcher" + DatePartition(), ev.TraceId, "*", dict);
        }

        public static ITableEntity ToTableBoundaryEvent(EventBase e, MicroserviceId msId)
        {
            var ev = e as BoundaryEvent;

            var dict = new Dictionary<string, EntityProperty>();
            dict.Add("ChannelId", new EntityProperty(ev.ChannelId));
            dict.Add("Direction", GetEnum<ChannelDirection>(ev.Direction));
            dict.Add("Type", GetEnum<BoundaryEventType>(ev.Type));
            dict.Add("Requested", new EntityProperty(ev.Requested));
            dict.Add("Actual", new EntityProperty(ev.Actual));
            dict.Add("BatchId", new EntityProperty(ev.Id));
            dict.Add("Ex", new EntityProperty(ev.Ex?.Message));
            dict.AddPayloadCommon(ev.Payload);

            return new DynamicTableEntity("Boundary" + DatePartition(), ev.TraceId, "*", dict);
        }

        public static ITableEntity ToTableTelemetryEvent(EventBase e, MicroserviceId msId)
        {
            var ev = e as TelemetryEvent;

            var dict = new Dictionary<string, EntityProperty>();
            dict.Add("Metric", new EntityProperty(ev.MetricName));
            dict.Add("Value", new EntityProperty(ev.Value));

            return new DynamicTableEntity("Telemetry" + DatePartition(), ev.TraceId, "*", dict);
        }
    }
}
