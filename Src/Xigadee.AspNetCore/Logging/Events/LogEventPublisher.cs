using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// Publishes log events to subscribers.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class LogEventPublisher : ILogEventPublisher
    {
        private readonly HashSet<ILogEventSubscriber> _logEventSubscribers;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventPublisher"/> class.
        /// </summary>
        public LogEventPublisher()
        {
            _logEventSubscribers = new HashSet<ILogEventSubscriber>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _logEventSubscribers.ForEach(logEventSubscriber => logEventSubscriber.Dispose());
        }

        /// <summary>
        /// Publishes the specified log event to all subscribers.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        public void Publish(LogEventApplication logEvent)
        {
            foreach (var logEventSubscriber in _logEventSubscribers)
            {
                logEventSubscriber.Receive(logEvent);
            }
        }

        /// <summary>
        /// Subscribes the specified subscriber to all log events.
        /// </summary>
        /// <param name="logEventSubscriber">The log event subscriber.</param>
        public void Subscribe(ILogEventSubscriber logEventSubscriber)
        {
            _logEventSubscribers.Add(logEventSubscriber);
        }
    }
}
