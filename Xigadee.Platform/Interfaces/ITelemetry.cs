using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface ITelemetry
    {
        void TrackMetric(string metricName, double value);
    }
}
