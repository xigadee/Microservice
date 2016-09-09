using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Xigadee
{
    public abstract class ApplicationInsightBase
    {
        //https://azure.microsoft.com/en-gb/documentation/articles/app-insights-api-custom-events-metrics/
        private TelemetryClient telemetry;

        protected ApplicationInsightBase(string key, bool developerMode)
        {
            var config = new TelemetryConfiguration();
            config.InstrumentationKey = key;
            config.TelemetryChannel.DeveloperMode = developerMode;

            telemetry = new TelemetryClient();
            telemetry.InstrumentationKey = key;
        }
    }
}
