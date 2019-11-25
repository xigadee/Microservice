using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// Standard log event for microservices.
    /// </summary>
    public class LogEventApplication
    {
        public LogLevel LogLevel { get; set; }

        public EventId EventId { get; set; }

        public object State { get; set; }

        public Exception Exception { get; set; }

        public string Message { get; set; }

        public string Name { get; set; }

        public string OperationId { get; set; }

        public string ParentId { get; set; }

        public Dictionary<string, object> FormattedParameters { get; set; } = new Dictionary<string, object>();
    }
}
