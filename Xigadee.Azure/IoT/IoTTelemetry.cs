﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class TelemetryIoT: ITelemetry
    {
        public void TrackMetric(string metricName, double value)
        {
            throw new NotImplementedException();
        }
    }
}