using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// Standard log event for microservices.
    /// </summary>
    [DebuggerDisplay("{LogLevel} {Name}: {Message}")]
    public class LogEventApplication
    {
        /// <summary>
        /// This is the logging level of the event.
        /// </summary>
        public LogLevel LogLevel { get; set; }
        /// <summary>
        /// This is the event id
        /// </summary>
        public EventId EventId { get; set; }
        /// <summary>
        /// This is the optional state object that can be set to the event 
        /// </summary>
        public object State { get; set; }
        /// <summary>
        /// This is the optional exception that was raised for the event.
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// The is the event message.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// This is the event name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// This is the operation id of the event.
        /// </summary>
        public string OperationId { get; set; }
        /// <summary>
        /// This is the parent id of the event.
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// This dictionary contains any additional formatted parameters for the event.
        /// </summary>
        public Dictionary<string, object> FormattedParameters { get; set; } = new Dictionary<string, object>();
    }
}
