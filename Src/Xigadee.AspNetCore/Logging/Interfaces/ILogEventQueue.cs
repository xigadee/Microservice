using System;

namespace Xigadee
{
    /// <summary>
    /// Interface for log event queues
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ILogEventQueue : IDisposable
    {
        /// <summary>
        /// Adds the specified log event to the queue.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        void Add(LogEventApplication logEvent);
    }
}
