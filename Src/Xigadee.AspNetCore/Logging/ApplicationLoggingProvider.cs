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
        #region Declarations
        private readonly ConcurrentDictionary<string, ApplicationLogger> _loggers = new ConcurrentDictionary<string, ApplicationLogger>();
        private readonly ILogEventQueue _logEventQueue;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLoggerProvider"/> class.
        /// </summary>
        public ApplicationLoggerProvider()
        {
            Publisher = new LogEventPublisher();
            _logEventQueue = new LogEventQueue(Publisher);
        }
        #endregion

        #region Hold()
        /// <summary>
        /// This method tells the provider to hold messages internally until the release method is called.
        /// </summary>
        public void Hold() => _logEventQueue.Hold();
        #endregion
        #region Release()
        /// <summary>
        /// This method releases any held messages.
        /// These messages were initially held while the logging infrastructure was being set up.
        /// This is to ensure that we don't loose our initial set up logging.
        /// </summary>
        public void Release() => _logEventQueue.Release(); 
        #endregion

        #region Publisher
        /// <summary>
        /// This is the publisher which can be used to add additional logging agents.
        /// </summary>
        public ILogEventPublisher Publisher { get; }
        #endregion

        #region CreateLogger(string category)
        /// <summary>
        /// Creates and registers a logger for the specified category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The created logger</returns>
        public ILogger CreateLogger(string category)
        {
            return _loggers.GetOrAdd(category, name => new ApplicationLogger(name, AddLogEventToQueue));
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => _logEventQueue.Dispose();
        #endregion

        #region AddLogEventToQueue(LogEventApplication logEvent)
        /// <summary>
        /// This method can be used to inject a LogEventApplication message in to the logging system directly.
        /// </summary>
        /// <param name="logEvent"></param>
        public void AddLogEventToQueue(LogEventApplication logEvent) => _logEventQueue.Add(logEvent); 
        #endregion
    }
}
