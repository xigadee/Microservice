using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// Logger provider for microservices.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Logging.ILoggerProvider" />
    public class ApplicationLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ApplicationLogger> _loggers = new ConcurrentDictionary<string, ApplicationLogger>();
        private readonly ILogEventQueue _logEventQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLoggerProvider"/> class.
        /// </summary>
        public ApplicationLoggerProvider()
        {
            Publisher = new LogEventPublisher();
            _logEventQueue = new LogEventQueue(Publisher);
        }

        /// <summary>
        /// This is the publisher which can be used to add additional logging agents.
        /// </summary>
        public ILogEventPublisher Publisher { get; }

        /// <summary>
        /// Creates and registers a logger for the specified category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The created logger</returns>
        public ILogger CreateLogger(string category)
        {
            return _loggers.GetOrAdd(category, name => new ApplicationLogger(name, AddLogEventToQueue));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => _logEventQueue.Dispose(); 

        private void AddLogEventToQueue(LogEventApplication logEvent) => _logEventQueue.Add(logEvent);


    }

}
