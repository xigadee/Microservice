using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This is the core logger that logs to the supported endpoints.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Logging.ILogger" />
    public class ApplicationLogger : ILogger
    {
        /// <summary>
        /// The internal name.
        /// </summary>
        private readonly string _loggerName;

        /// <summary>
        /// The action reference, used to insert a logging entity in to the queue.
        /// </summary>
        private readonly Action<LogEventApplication> _logEventAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLogger"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="logEventAction">This is the action to logEventAction the incoming log requests.</param>
        public ApplicationLogger(string name, Action<LogEventApplication> logEventAction)
        {
            _loggerName = name;
            _logEventAction = logEventAction ?? throw new ArgumentException("logEventAction cannot be null");
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>
        /// An IDisposable that ends the logical operation scope on dispose.
        /// </returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel" /> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns>
        ///   <c>true</c> if enabled.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <c>string</c> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var logEvent = new LogEventApplication
            {
                Name = _loggerName,
                EventId = eventId,
                LogLevel = logLevel,
                Exception = exception,
                Message = formatter?.Invoke(state, exception),
                ParentId = Activity.Current?.ParentId,
                OperationId = Activity.Current?.RootId
            };

            // Extract formatted values excluding the original format value which is the unformatted message
            var logValues = state as IReadOnlyList<KeyValuePair<string, object>>;
            logValues?.Where(lv => lv.Key.IndexOf("OriginalFormat", StringComparison.InvariantCultureIgnoreCase) < 0)
                .ForEach(lv => logEvent.FormattedParameters[lv.Key] = lv.Value);

            _logEventAction(logEvent);
        }

        private class NoopDisposable : IDisposable
        {
            public static readonly NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}
