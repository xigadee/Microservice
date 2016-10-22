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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Xigadee
{
    /// <summary>
    /// This class hooks Application Insights in to the Microservice logging capabilities.
    /// </summary>
    public class ApplicationInsightsDataCollector: DataCollectorHolder
    {
        #region Declarations
        //https://azure.microsoft.com/en-gb/documentation/articles/app-insights-api-custom-events-metrics/
        private TelemetryClient mTelemetry;
        private readonly string mKey;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">This is the application insights key.</param>
        /// <param name="name"></param>
        /// <param name="developerMode"></param>
        /// <param name="support"></param>
        protected ApplicationInsightsDataCollector(string key, bool developerMode = false)
        {
            var config = new TelemetryConfiguration();
            config.InstrumentationKey = key;
            config.TelemetryChannel.DeveloperMode = developerMode;

            mKey = key;
        } 
        #endregion


        protected override void StartInternal()
        {
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
            mTelemetry = new TelemetryClient();

            //mTelemetry.Context.Component.Version = 
            mTelemetry.Context.Device.Id = OriginatorId.ServiceId;

            //mTelemetry.Context.Operation.Id = 

            mTelemetry.Context.Component.Version = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Version.ToString();
            mTelemetry.Context.Properties["ExternalServiceId"] = OriginatorId.ExternalServiceId;
            mTelemetry.Context.Properties["MachineName"] = OriginatorId.MachineName;
            mTelemetry.Context.Properties["ServiceName"] = OriginatorId.Name;

            mTelemetry.InstrumentationKey = mKey;
        }

        protected override void StopInternal()
        {
            mTelemetry.Flush();
            mTelemetry = null;
        }

        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.BoundaryLogger, (e) => Write((BoundaryEvent)e));
            SupportAdd(DataCollectionSupport.Dispatcher, (e) => Write((DispatcherEvent)e));
            SupportAdd(DataCollectionSupport.EventSource, (e) => Write((EventSourceEvent)e));
            SupportAdd(DataCollectionSupport.Logger, (e) => Write((LogEvent)e));
            SupportAdd(DataCollectionSupport.Statistics, (e) => Write((MicroserviceStatistics)e));
            SupportAdd(DataCollectionSupport.Telemetry, (e) => Write((MetricEvent)e));
        }

        private void Write(BoundaryEvent eventData)
        {
            //switch (eventData.

            //mTelemetry.TrackException(new ExceptionTelemetry() { 

            //mTelemetry.TrackEvent(new EventTelemetry() { 
        }

        private void Write(DispatcherEvent eventData)
        {

        }

        private void Write(EventSourceEvent eventData)
        {

        }

        private void Write(LogEvent eventData)
        {

        }

        private void Write(MicroserviceStatistics eventData)
        {

        }

        private void Write(MetricEvent eventData)
        {

        }

        /// <summary>
        /// Flush the underlying telemetry.
        /// </summary>
        public override void Flush()
        {
            mTelemetry?.Flush();
        }


        //public override void BoundaryLog(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        //{

        //    //         // Set up some properties and metrics:
        //    //         var properties = new Dictionary<string, string>
        //    //{{"game", currentGame.Name}, {"difficulty", currentGame.Difficulty}};
        //    //         var metrics = new Dictionary<string, double>
        //    //{{"Score", currentGame.Score}, {"Opponents", currentGame.OpponentCount}};

        //    //         // Send the event:
        //    //         mTelemetry.TrackEvent("WinGame", properties, metrics);

        //    var eventAI = new EventTelemetry();

        //    eventAI.Name = "WinGame";

        //    //eventAI.Metrics ["processingTime"] = stopwatch.Elapsed.TotalMilliseconds;
        //    //eventAI.Properties ["game"] = currentGame.Name;
        //    //eventAI.Properties ["difficulty"] = currentGame.Difficulty;
        //    //eventAI.Metrics ["Score"] = currentGame.Score;
        //    //eventAI.Metrics ["Opponents"] = currentGame.Opponents.Length;

        //    mTelemetry?.TrackEvent(eventAI);
        //    var mTelem = new MetricTelemetry();
        //    mTelemetry?.TrackMetric(mTelem);

        //    var rTelem = new RequestTelemetry();
        //    //rTelem.
        //    //mTelemetry?.TrackRequest(PerformanceCounterTelemetry);
        //    //mTelem.Count
        //}

        //public override async Task Log(LogEvent logEvent)
        //{
        //    if (logEvent == null)
        //        return;

        //    switch (logEvent.Level)
        //    {
        //        case LoggingLevel.Info:
        //            break;
        //        case LoggingLevel.Status:
        //            break;
        //        case LoggingLevel.Fatal:
        //        case LoggingLevel.Error:
        //        case LoggingLevel.Warning:
        //            break;
        //    }
        //}



        //public override void TrackMetric(string metricName, double value)
        //{
        //    mTelemetry?.TrackMetric(metricName, value);
        //}

        //public override async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        //{
        //}


    }
}

