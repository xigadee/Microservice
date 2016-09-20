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
using Microsoft.ApplicationInsights.Extensibility;

namespace Xigadee
{
    /// <summary>
    /// This class hooks Application Insights in to the Microservice logging capabilities.
    /// </summary>
    public class ApplicationInsightsDataCollector: DataCollectorBase
    {
        //https://azure.microsoft.com/en-gb/documentation/articles/app-insights-api-custom-events-metrics/
        private TelemetryClient mTelemetry;
        private readonly string mKey;

        protected ApplicationInsightsDataCollector(string key
            , string name = null
            , bool developerMode = false
            , DataCollectionSupport support = DataCollectionSupport.All)
            :base(name ?? "ApplicationInsightsDataCollector", support)
        {
            var config = new TelemetryConfiguration();
            config.InstrumentationKey = key;
            config.TelemetryChannel.DeveloperMode = developerMode;
            
            mKey = key;
        }


        protected override void StartInternal()
        {
            
            mTelemetry = new TelemetryClient();

            mTelemetry.Context.Device.Id = OriginatorId.ServiceId;

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




        public override Guid BatchPoll(int requested, int actual, string channelId)
        {
            var id = Guid.NewGuid();

            return id;
        }

        public override void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {

   //         // Set up some properties and metrics:
   //         var properties = new Dictionary<string, string>
   //{{"game", currentGame.Name}, {"difficulty", currentGame.Difficulty}};
   //         var metrics = new Dictionary<string, double>
   //{{"Score", currentGame.Score}, {"Opponents", currentGame.OpponentCount}};

   //         // Send the event:
   //         mTelemetry.TrackEvent("WinGame", properties, metrics);

        }

        public override async Task Log(LogEvent logEvent)
        {
            if (logEvent == null)
                return;

            switch (logEvent.Level)
            {
                case LoggingLevel.Info:
                    break;
                case LoggingLevel.Status:
                    break;
                case LoggingLevel.Fatal:
                case LoggingLevel.Error:
                case LoggingLevel.Warning:
                    break;
            }
        }



        public override void TrackMetric(string metricName, double value)
        {
            mTelemetry?.TrackMetric(metricName, value);
        }

        public override async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
        }


    }
}

