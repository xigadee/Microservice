using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This is the logging agent base configuration
    /// </summary>
    public class ConfigLoggerAgentBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether logging is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the minimum level that should be logged. Leave this null to not filter by a level.
        /// </summary>
        public LogLevel? Level { get; set; }

        /// <summary>
        /// The loop pause time in ms. The default is 1s.
        /// </summary>
        public int? LoopPauseTimeInMs = 1000;

        /// <summary>
        /// Determines whether the specified log level is enabled.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>
        ///   <c>true</c> if the specified log level is enabled; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return Enabled && (!Level.HasValue || (Level.Value <= logLevel));
        }
    }
}
