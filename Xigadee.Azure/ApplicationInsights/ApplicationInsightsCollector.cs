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
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Xigadee.ApplicationInsights;

namespace Xigadee
{
    /// <summary>
    /// This class hooks Application Insights in to the Microservice logging capabilities.
    /// </summary>
    public class ApplicationInsightsDataCollector : DataCollectorHolder
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


        protected override void StartInternal()
        {
            mTelemetry = new TelemetryClient();

            mTelemetry.Context.Device.Id = OriginatorId.ServiceId;
            mTelemetry.Context.Component.Version = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Version.ToString();
            mTelemetry.Context.Properties["ExternalServiceId"] = OriginatorId.ExternalServiceId;
            mTelemetry.Context.Properties["MachineName"] = OriginatorId.MachineName;
            mTelemetry.Context.Properties["ServiceName"] = OriginatorId.Name;
        }

        protected override void StopInternal()
        {
            mTelemetry.Flush();
        }

        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.BoundaryLogger, e => Write((BoundaryEvent)e));
            SupportAdd(DataCollectionSupport.Dispatcher, e => Write((DispatcherEvent)e));
            SupportAdd(DataCollectionSupport.EventSource, e => Write((EventSourceEvent)e));
            SupportAdd(DataCollectionSupport.Statistics, e => Write((MicroserviceStatistics)e));
            SupportAdd(DataCollectionSupport.Logger, e => Write((LogEvent)e));
            SupportAdd(DataCollectionSupport.Telemetry, e => Write((MetricEvent)e));
        }

        private void Write(BoundaryEvent eventData)
        {
            if (eventData == null)
                return;

            try
            {
                var eventTelemetry = new EventTelemetry($"Boundary:{eventData.Payload?.Message?.ChannelId}:{eventData.Payload?.Message?.MessageType}:{eventData.Payload?.Message?.ActionType}:{eventData.Direction}");
                AddPropertyData(eventTelemetry, nameof(eventData.Payload.Message.ChannelId), eventData.Payload?.Message?.ChannelId);
                AddPropertyData(eventTelemetry, nameof(eventData.Payload.Message.MessageType), eventData.Payload?.Message?.MessageType);
                AddPropertyData(eventTelemetry, nameof(eventData.Payload.Message.ActionType), eventData.Payload?.Message?.ActionType);
                AddPropertyData(eventTelemetry, nameof(eventData.Payload.Message.CorrelationKey), eventData.Payload?.Message?.CorrelationKey);
                AddPropertyData(eventTelemetry, nameof(BoundaryEventType), eventData.Type.ToString());
                AddPropertyData(eventTelemetry, nameof(Exception), eventData.Ex?.ToString());
                eventTelemetry.Metrics[$"{nameof(BoundaryEvent)}:{nameof(eventData.Requested)}"] = eventData.Requested;
                eventTelemetry.Metrics[$"{nameof(BoundaryEvent)}:{nameof(eventData.Actual)}"] = eventData.Actual;

                // If we have the payload and a correlation key use this as the operation id
                eventTelemetry.Context.Operation.Id = eventData.Payload?.Message?.ProcessCorrelationKey ?? eventTelemetry.Context.Operation.Id;

                mTelemetry?.TrackEvent(eventTelemetry);
            }
            catch (Exception ex)
            {
                LogTelemetryException(ex);
            }
        }

        private void Write(DispatcherEvent eventData)
        {
            if (eventData == null)
                return;

            try
            {
                var eventTelemetry = new EventTelemetry($"Dispatcher:{eventData.Payload?.Message?.ChannelId}:{eventData.Payload?.Message?.MessageType}:{eventData.Payload?.Message?.ActionType}:{eventData.IsSuccess}");
                AddPropertyData(eventTelemetry, nameof(eventData.Payload.Message.ChannelId), eventData.Payload?.Message?.ChannelId);
                AddPropertyData(eventTelemetry, nameof(eventData.Payload.Message.MessageType), eventData.Payload?.Message?.MessageType);
                AddPropertyData(eventTelemetry, nameof(eventData.Payload.Message.ActionType), eventData.Payload?.Message?.ActionType);
                AddPropertyData(eventTelemetry, nameof(eventData.Payload.Message.CorrelationKey), eventData.Payload?.Message?.CorrelationKey);
                AddPropertyData(eventTelemetry, nameof(PayloadEventType), eventData.Type.ToString());
                AddPropertyData(eventTelemetry, nameof(Exception), eventData.Ex?.ToString());
                eventTelemetry.Metrics[$"{nameof(DispatcherEvent)}:{nameof(eventData.Delta)}"] = eventData.Delta;

                // If we have the payload and a correlation key use this as the operation id
                eventTelemetry.Context.Operation.Id = eventData.Payload?.Message?.ProcessCorrelationKey ?? eventTelemetry.Context.Operation.Id;

                mTelemetry?.TrackEvent(eventTelemetry);
            }
            catch (Exception ex)
            {
                LogTelemetryException(ex);
            }
        }

        private void Write(EventSourceEvent eventData)
        {
            if (eventData?.Entry == null)
                return;

            try
            {
                var eventTelemetry = new EventTelemetry($"EventSource:{eventData.Entry.EntityType}:{eventData.Entry.EventType}");
                AddPropertyData(eventTelemetry, nameof(eventData.Entry.EntityType), eventData.Entry.EntityType);
                AddPropertyData(eventTelemetry, nameof(eventData.Entry.EventType), eventData.Entry.EventType);
                AddPropertyData(eventTelemetry, nameof(eventData.OriginatorId), eventData.OriginatorId);
                AddPropertyData(eventTelemetry, nameof(eventData.Entry.CorrelationId), eventData.Entry.CorrelationId);
                AddPropertyData(eventTelemetry, nameof(eventData.Entry.EntitySource), eventData.Entry.EntitySource);
                AddPropertyData(eventTelemetry, nameof(eventData.Entry.Key), eventData.Entry.Key);
                mTelemetry?.TrackEvent(eventTelemetry);
            }
            catch (Exception ex)
            {
                LogTelemetryException(ex);
            }
        }

        private void Write(LogEvent eventData)
        {
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
                    telemetryProperties = exceptionTelemetry = new ExceptionTelemetry(eventData.Ex);
                }
                else
                {
                    telemetryProperties = eventTelemetry = new EventTelemetry(eventData.Level + (!string.IsNullOrEmpty(eventData.Category) ? $":{eventData.Category}" : string.Empty));
                }

                AddPropertyData(telemetryProperties, nameof(LoggingLevel), eventData.Level.ToString());
                if (eventData.AdditionalData != null || !string.IsNullOrEmpty(eventData.Message))
                {
                    eventData.AdditionalData?.ForEach(kvp => AddPropertyData(telemetryProperties, kvp.Key, kvp.Value));
                    AddPropertyData(telemetryProperties, nameof(eventData.Message), eventData.Message);
                    AddPropertyData(telemetryProperties, nameof(eventData.Category), eventData.Category);
                }

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

        private void Write(MicroserviceStatistics eventData)
        {
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

        private void Write(MetricEvent eventData)
        {
            if (string.IsNullOrEmpty(eventData?.MetricName))
                return;

            try
            {
                mTelemetry?.TrackMetric(eventData.MetricName, eventData.Value);
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