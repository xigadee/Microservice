using System;

namespace Xigadee
{
    /// <summary>
    /// Interface for log event publishers.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ILogEventPublisher : IDisposable
    {
        /// <summary>
        /// Publishes the specified log event to all subscribers.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        void Publish(LogEventApplication logEvent);

        /// <summary>
        /// Subscribes the specified subscriber to all log events.
        /// </summary>
        /// <param name="logEventSubscriber">The log event subscriber.</param>
        void Subscribe(ILogEventSubscriber logEventSubscriber);
    }
}