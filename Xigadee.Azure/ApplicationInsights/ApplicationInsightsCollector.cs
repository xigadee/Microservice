
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
    public class ApplicationInsightsCollector: IDataCollector
    {
        //https://azure.microsoft.com/en-gb/documentation/articles/app-insights-api-custom-events-metrics/
        private TelemetryClient telemetry;

        protected ApplicationInsightsCollector(string key, bool developerMode)
        {
            var config = new TelemetryConfiguration();
            config.InstrumentationKey = key;
            config.TelemetryChannel.DeveloperMode = developerMode;
            
            telemetry = new TelemetryClient();
            telemetry.Context.Component.Version = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Version.ToString();
            telemetry.Context.Properties["MachineName"] = "";
            telemetry.Context.Properties["ServiceName"] = "";
            telemetry.Context.Properties["MachineName"] = "";
            telemetry.Context.Properties["MachineName"] = "";
            telemetry.Context.Properties["MachineName"] = "";

            telemetry.InstrumentationKey = key;
        }

        public string Name
        {
            get
            {
                return "";
            }
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

        public void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {

        }

        public async Task Log(LogEvent logEvent)
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



        public void TrackMetric(string metricName, double value)
        {
        }

        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
        }

    }
}

