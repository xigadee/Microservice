using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class MetricEvent: EventBase
    {
        public string MetricName { get; set; }

        public double Value { get; set; }
    }
}
