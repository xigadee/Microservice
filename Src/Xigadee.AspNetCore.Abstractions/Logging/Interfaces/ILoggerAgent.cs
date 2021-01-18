using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// Interface for microservice logger agents.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ILoggerAgent : IDisposable
    {
        /// <summary>
        /// Logs the event data to the underlying provider.
        /// </summary>
        /// <param name="logEvent">The event data.</param>
        void LogItem(LogEventApplication logEvent);
    }
}
