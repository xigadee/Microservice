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

        /// <summary>
        /// This method makes the queue hold incoming messages in memory until the Release() method is called.
        /// </summary>
        void Hold();
        /// <summary>
        /// This releases the queue and sets it to poll immediately.
        /// </summary>
        void Release();
    }
}
