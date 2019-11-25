

namespace Xigadee
{
    /// <summary>
    /// Subscribes an agent to log events.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class LogEventSubscriber : ILogEventSubscriber
    {
        private readonly ILoggerAgent _microserviceLoggerAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventSubscriber"/> class.
        /// </summary>
        /// <param name="microserviceLoggerAgent">The microservice logger agent.</param>
        public LogEventSubscriber(ILoggerAgent microserviceLoggerAgent)
        {
            _microserviceLoggerAgent = microserviceLoggerAgent;
        }

        /// <summary>
        /// Receives and actions the specified log event from the queue.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        public void Receive(LogEventApplication logEvent)
        {
            _microserviceLoggerAgent.LogItem(logEvent);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _microserviceLoggerAgent?.Dispose();
        }
    }
}
