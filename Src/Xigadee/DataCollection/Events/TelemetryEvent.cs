using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This event type is used to measure values within the application.
    /// </summary>
    [DebuggerDisplay("Metric={MetricName}/{Value}")]
    public class TelemetryEvent: EventBase
    {
        /// <summary>
        /// This is the metric name.
        /// </summary>
        public string MetricName { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Addition data to associate when logging the metric
        /// </summary>
        public virtual Dictionary<string, string> AdditionalData { get; } = new Dictionary<string, string>();
    }
}
