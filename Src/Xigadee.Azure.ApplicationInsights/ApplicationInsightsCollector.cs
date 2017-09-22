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
using System.Reflection;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Xigadee
{
    /// <summary>
    /// This class hooks Application Insights in to the Microservice logging capabilities.
    /// </summary>
    public class ApplicationInsightsDataCollector : DataCollectorBase<DataCollectorStatistics>
    {
        #region Declarations
        //https://azure.microsoft.com/en-gb/documentation/articles/app-insights-api-custom-events-metrics/
        private TelemetryClient mTelemetry;
        private readonly LoggingLevel mLoggingLevel;
        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">This is the application insights key.</param>
        /// <param name="loggingLevel">This is the minium level at which to log events.</param>
        public ApplicationInsightsDataCollector(string key, LoggingLevel loggingLevel = LoggingLevel.Warning)
        {
            mLoggingLevel = loggingLevel;
            TelemetryConfiguration.Active.InstrumentationKey = key;
            TelemetryConfiguration.Active.TelemetryInitializers.Add(new CommandCorrelationInitializer());
        }

        #endregion

        /// <summary>
        /// Starts the collector.
        /// </summary>
        protected override void StartInternal()
        {
            base.StartInternal();

            mTelemetry = new TelemetryClient();

            mTelemetry.Context.Device.Id = OriginatorId.ServiceId;
            mTelemetry.Context.Component.Version = OriginatorId.ServiceVersionId ?? (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Version.ToString();
            mTelemetry.Context.Properties["ExternalServiceId"] = OriginatorId.ExternalServiceId;
            mTelemetry.Context.Properties["MachineName"] = OriginatorId.MachineName;
            mTelemetry.Context.Properties["ServiceName"] = OriginatorId.Name;

            mTelemetry.TrackEvent($"Startup:{OriginatorId.Name}");
        }
        /// <summary>
        /// Stops the collector.
        /// </summary>
        protected override void StopInternal()
        {
            mTelemetry.TrackEvent($"Shutdown:{OriginatorId.Name}");
            mTelemetry.Flush();

            base.StopInternal();
        }

        /// <summary>
        /// Configures the event mappers.
        /// </summary>
        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.Boundary, WriteBoundaryEvent);
            SupportAdd(DataCollectionSupport.Dispatcher, WriteDispatcherEvent);
            SupportAdd(DataCollectionSupport.EventSource, WriteEventSourceEvent);
            SupportAdd(DataCollectionSupport.Statistics, WriteMicroserviceStatistics);
            SupportAdd(DataCollectionSupport.Logger, WriteLogEvent);
            SupportAdd(DataCollectionSupport.Telemetry, WriteTelemetryEvent);
            SupportAdd(DataCollectionSupport.Security, WriteSecurityEvent);
        }

        private void WriteBoundaryEvent(EventHolder eventHolder)
        {
            var eventData = eventHolder.Data as BoundaryEvent;
            if (eventData == null || (mLoggingLevel > LoggingLevel.Info && eventData.Ex == null))
                return;

            try
            {
                EventTelemetry eventTelemetry = null;
                ExceptionTelemetry exceptionTelemetry = null;
                ISupportProperties telemetryProperties;

                // If we have en exception log as such
                if (eventData.Ex != null)
                {
                    telemetryProperties = exceptionTelemetry = AddTelemetryContext(new ExceptionTelemetry(eventData.Ex) { Message = eventData.Ex.Message }, eventHolder);
                }
                else
                {
                    telemetryProperties = eventTelemetry = AddTelemetryContext(new EventTelemetry($"Boundary:{eventData.Payload?.Message?.ChannelId}:{eventData.Payload?.Message?.MessageType}:{eventData.Payload?.Message?.ActionType}:{eventData.Direction}"), eventHolder);
                }

                AddPropertyData(telemetryProperties, nameof(eventData.Payload.Message.ChannelId), eventData.Payload?.Message?.ChannelId);
                AddPropertyData(telemetryProperties, nameof(eventData.Payload.Message.MessageType), eventData.Payload?.Message?.MessageType);
                AddPropertyData(telemetryProperties, nameof(eventData.Payload.Message.ActionType), eventData.Payload?.Message?.ActionType);
                AddPropertyData(telemetryProperties, nameof(eventData.Payload.Message.CorrelationKey), eventData.Payload?.Message?.CorrelationKey);
                AddPropertyData(telemetryProperties, nameof(eventData.Payload.Message.FabricDeliveryCount), eventData.Payload?.Message?.FabricDeliveryCount.ToString());
                AddPropertyData(telemetryProperties, nameof(BoundaryEventType), eventData.Type.ToString());

                // If we have the payload and a correlation key use this as the operation id
                var telemetry = (ITelemetry)eventTelemetry ?? exceptionTelemetry;
                if (telemetry != null)
                    telemetry.Context.Operation.Id = eventData.Payload?.Message?.ProcessCorrelationKey ?? telemetry.Context.Operation.Id;

                if (exceptionTelemetry != null)
                {
                    mTelemetry?.TrackException(exceptionTelemetry);
                }
                else if (eventTelemetry != null)
                {
                    eventTelemetry.Metrics[$"{nameof(BoundaryEvent)}:{nameof(eventData.Requested)}"] = eventData.Requested;
                    eventTelemetry.Metrics[$"{nameof(BoundaryEvent)}:{nameof(eventData.Actual)}"] = eventData.Actual;
                    mTelemetry?.TrackEvent(eventTelemetry);
                }
            }
            catch (Exception ex)
            {
                LogTelemetryException(ex);
            }
        }

        private void WriteDispatcherEvent(EventHolder eventHolder)
        {
            var eventData = eventHolder.Data as DispatcherEvent;
            if (eventData == null || (mLoggingLevel > LoggingLevel.Info && eventData.Ex == null))
                return;

            try
            {
                EventTelemetry eventTelemetry = null;
                ExceptionTelemetry exceptionTelemetry = null;
                ISupportProperties telemetryProperties;

                // If we have en exception log as such
                if (eventData.Ex != null)
                {
                    telemetryProperties = exceptionTelemetry = AddTelemetryContext(new ExceptionTelemetry(eventData.Ex) { Message = eventData.Ex.Message }, eventHolder);
                }
                else
                {
                    telemetryProperties = eventTelemetry = AddTelemetryContext(new EventTelemetry($"Dispatcher:{eventData.Payload?.Message?.ChannelId}:{eventData.Payload?.Message?.MessageType}:{eventData.Payload?.Message?.ActionType}:{eventData.IsSuccess}"), eventHolder);
                }

                AddPropertyData(telemetryProperties, nameof(eventData.Reason), eventData.Reason?.ToString());
                AddPropertyData(telemetryProperties, nameof(eventData.Payload.Message.ChannelId), eventData.Payload?.Message?.ChannelId);
                AddPropertyData(telemetryProperties, nameof(eventData.Payload.Message.MessageType), eventData.Payload?.Message?.MessageType);
                AddPropertyData(telemetryProperties, nameof(eventData.Payload.Message.ActionType), eventData.Payload?.Message?.ActionType);
                AddPropertyData(telemetryProperties, nameof(eventData.Payload.Message.CorrelationKey), eventData.Payload?.Message?.CorrelationKey);
                AddPropertyData(telemetryProperties, nameof(eventData.Payload.Message.FabricDeliveryCount), eventData.Payload?.Message?.FabricDeliveryCount.ToString());
                AddPropertyData(telemetryProperties, nameof(PayloadEventType), eventData.Type.ToString());

                // If we have the payload and a correlation key use this as the operation id
                var telemetry = (ITelemetry) eventTelemetry ?? exceptionTelemetry;
                if (telemetry != null)
                    telemetry.Context.Operation.Id = eventData.Payload?.Message?.ProcessCorrelationKey ?? telemetry.Context.Operation.Id;

                if (exceptionTelemetry != null)
                {
                    mTelemetry?.TrackException(exceptionTelemetry);
                }
                else if (eventTelemetry != null)
                {
                    eventTelemetry.Metrics[$"{nameof(DispatcherEvent)}:{nameof(eventData.Delta)}"] = eventData.Delta;
                    mTelemetry?.TrackEvent(eventTelemetry);
                }
            }
            catch (Exception ex)
            {
                LogTelemetryException(ex);
            }
        }

        private void WriteEventSourceEvent(EventHolder eventHolder)
        {
            var eventData = eventHolder.Data as EventSourceEvent;
            if (eventData?.Entry == null || mLoggingLevel > LoggingLevel.Info)
                return;

            try
            {
                var eventTelemetry = AddTelemetryContext(new EventTelemetry($"EventSource:{eventData.Entry.EntityType}:{eventData.Entry.EventType}"), eventHolder);
                AddPropertyData(eventTelemetry, nameof(eventData.Entry.EntityType), eventData.Entry.EntityType);
                AddPropertyData(eventTelemetry, nameof(eventData.Entry.EventType), eventData.Entry.EventType);
                AddPropertyData(eventTelemetry, nameof(eventData.OriginatorId), eventData.OriginatorId);
                AddPropertyData(eventTelemetry, nameof(eventData.Entry.CorrelationId), eventData.Entry.CorrelationId);
                AddPropertyData(eventTelemetry, nameof(eventData.Entry.EntitySource), eventData.Entry.EntitySource);
                AddPropertyData(eventTelemetry, nameof(eventData.Entry.Key), eventData.Entry.Key);
                AddPropertyData(eventTelemetry, nameof(eventData.TraceId), eventData.TraceId);
                mTelemetry?.TrackEvent(eventTelemetry);
            }
            catch (Exception ex)
            {
                LogTelemetryException(ex);
            }
        }

        private void WriteLogEvent(EventHolder eventHolder)
        {

            var eventData = eventHolder.Data as LogEvent;
            if (eventData == null || eventData.Level < mLoggingLevel)
                return;

            try
            {
                EventTelemetry eventTelemetry = null;
                ExceptionTelemetry exceptionTelemetry = null;
                ISupportProperties telemetryProperties;

                // Don't log non errors that have exceptions as exceptions i.e. warnings / info
                if (eventData.Ex != null && eventData.Level >= LoggingLevel.Error)
                {
                    telemetryProperties = exceptionTelemetry = AddTelemetryContext(new ExceptionTelemetry(eventData.Ex), eventHolder);
                }
                else
                {
                    telemetryProperties = eventTelemetry = AddTelemetryContext(new EventTelemetry(eventData.Level + (!string.IsNullOrEmpty(eventData.Category) ? $":{eventData.Category}" : string.Empty)), eventHolder);
                }

                AddPropertyData(telemetryProperties, nameof(LoggingLevel), eventData.Level.ToString());
                AddPropertyData(telemetryProperties, nameof(eventData.TraceId), eventData.TraceId);
                AddPropertyData(telemetryProperties, nameof(eventData.Message), eventData.Message);
                AddPropertyData(telemetryProperties, nameof(eventData.Category), eventData.Category);
                AddPropertyData(telemetryProperties, "Exception.Message", eventData.Ex?.Message);
                eventData.AdditionalData?.ForEach(kvp => AddPropertyData(telemetryProperties, kvp.Key, kvp.Value));

                if (exceptionTelemetry != null)
                {
                    mTelemetry?.TrackException(exceptionTelemetry);
                }
                else
                {
                    AddPropertyData(telemetryProperties, nameof(Exception), eventData.Ex?.ToString());
                    mTelemetry?.TrackEvent(eventTelemetry);
                }
            }
            catch (Exception ex)
            {
                LogTelemetryException(ex);
            }
        }

        private void WriteMicroserviceStatistics(EventHolder eventHolder)
        {
            var eventData = eventHolder.Data as MicroserviceStatistics;
            if (string.IsNullOrEmpty(eventData?.Name))
                return;

            try
            {
                mTelemetry?.TrackMetric($"{eventData.Name}.Tasks.Active", eventData.Tasks?.Availability?.Active ?? 0);
                mTelemetry?.TrackMetric($"{eventData.Name}.Tasks.SlotsAvailable", eventData.Tasks?.Availability?.SlotsAvailable ?? 0);
                mTelemetry?.TrackMetric($"{eventData.Name}.Tasks.Killed", eventData.Tasks?.Availability?.Killed ?? 0);
                mTelemetry?.TrackMetric($"{eventData.Name}.Tasks.KilledDidReturn", eventData.Tasks?.Availability?.KilledDidReturn ?? 0);
                mTelemetry?.TrackMetric($"{eventData.Name}.Tasks.KilledTotal", (eventData.Tasks?.Availability?.Killed ?? 0) + (eventData.Tasks?.Availability?.KilledDidReturn ?? 0));
                mTelemetry?.TrackMetric($"{eventData.Name}.Cpu.ServicePercentage", eventData.Tasks?.Cpu?.ServicePercentage ?? 0);
            }
            catch (Exception ex)
            {
                LogTelemetryException(ex);
            }
        }

        private void WriteTelemetryEvent(EventHolder eventHolder)
        {
            var eventData = eventHolder.Data as TelemetryEvent;
            if (string.IsNullOrEmpty(eventData?.MetricName))
                return;

            try
            {
                mTelemetry?.TrackMetric(eventData.MetricName, eventData.Value, eventData.AdditionalData);
            }
            catch (Exception ex)
            {
                LogTelemetryException(ex);
            }
        }

        private void WriteSecurityEvent(EventHolder eventHolder)
        {
            var eventData = eventHolder.Data as SecurityEvent;
            if (eventData == null)
                return;

            try
            {
                var traceId = new KeyValuePair<string,string>(nameof(eventData.TraceId), eventData.TraceId);
                if (eventData.Ex != null)
                {
                    mTelemetry?.TrackException(AddTelemetryContext(new ExceptionTelemetry(eventData.Ex) { Message = $"Security Event - {eventData.Direction}", Properties = { traceId } }, eventHolder));
                }
                else
                {
                    mTelemetry?.TrackEvent(AddTelemetryContext(new EventTelemetry($"{LoggingLevel.Error}:{eventData.Direction}") { Properties = { traceId }}, eventHolder));
                }
            }
            catch (Exception ex)
            {
                LogTelemetryException(ex);
            }
        }

        /// <summary>
        /// Flush the underlying telemetry.
        /// </summary>
        public override void Flush()
        {
            mTelemetry?.Flush();
        }

        /// <summary>
        /// Add properties if not null or empty to a telemetry object that supports properties
        /// </summary>
        /// <param name="propertiesObject"></param>
        /// <param name="dataName"></param>
        /// <param name="dataValue"></param>
        private void AddPropertyData(ISupportProperties propertiesObject, string dataName, string dataValue)
        {
            if (propertiesObject == null || string.IsNullOrEmpty(dataName) || string.IsNullOrEmpty(dataValue))
                return;

            propertiesObject.Properties[dataName] = dataValue;
        }

        /// <summary>
        /// Update telemetry context based on meta data held in the event holder
        /// </summary>
        /// <param name="telemetry"></param>
        /// <param name="eventHolder"></param>
        private T AddTelemetryContext<T>(T telemetry, EventHolder eventHolder) where T:ITelemetry
        {
            if (telemetry == null || eventHolder?.Claims == null)
                return telemetry;

            var correlationClaim = eventHolder.Claims.FindFirst(JwtTokenAuthenticationHandler.ClaimProcessCorrelationKey);
            if (!string.IsNullOrEmpty(correlationClaim?.Value))
                telemetry.Context.Operation.Id = correlationClaim.Value;

            if (!string.IsNullOrEmpty(eventHolder.Claims?.Identity?.Name))
                telemetry.Context.User.AuthenticatedUserId = eventHolder.Claims?.Identity?.Name;

            return telemetry;
        }

        /// <summary>
        /// Attempt to log the exception details to telemetry
        /// </summary>
        /// <param name="ex"></param>
        private void LogTelemetryException(Exception ex)
        {
            try
            {
                mTelemetry?.TrackException(ex, new Dictionary<string, string> { { "Message", "Unable to log correctly" } });
            }
            catch (Exception)
            {
                // Not much we can do here
            }
        }
    }
}