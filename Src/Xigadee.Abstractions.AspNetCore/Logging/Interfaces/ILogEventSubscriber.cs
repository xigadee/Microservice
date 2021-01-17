using System;

namespace Xigadee
{
    /// <summary>
    /// Interface for log event subscribers.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ILogEventSubscriber : IDisposable
    {
        /// <summary>
        /// Receives and actions the specified log event from the queue.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        void Receive(LogEventApplication logEvent);
    }
}