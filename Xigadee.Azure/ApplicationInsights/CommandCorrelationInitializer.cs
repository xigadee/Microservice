using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;

namespace Xigadee.ApplicationInsights
{
    public class CommandCorrelationInitializer : ITelemetryInitializer
    {
        public void Initialize(Microsoft.ApplicationInsights.Channel.ITelemetry telemetry)
        {
            //telemetry.Context.Operation.Id = CommandContext.CorrelationKey ?? telemetry.Context.Operation.Id;
        }
    }
}
