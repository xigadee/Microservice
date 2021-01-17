using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to log metric values for the application.
    /// </summary>
    public interface ILogMetrics
    {
        /// <summary>
        /// This method is used to log a metric valie.
        /// </summary>
        /// <param name="metricName">The metric name.</param>
        /// <param name="value">The metric value.</param>
        /// <param name="additionalData">Any additional key-value pair data fields.</param>
        void LogMetric(string metricName, double value, Dictionary<string, string> additionalData = null);

    }
}
