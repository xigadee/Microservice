using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    [DebuggerDisplay("Metric={MetricName}/{Value}")]
    public class MetricEvent: EventBase
    {
        public string MetricName { get; set; }

        public double Value { get; set; }
    }
}
