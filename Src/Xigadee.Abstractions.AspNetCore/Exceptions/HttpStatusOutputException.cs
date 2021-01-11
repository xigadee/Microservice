using Microsoft.Extensions.Logging;
using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used to throw a defined http error code exception.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class HttpStatusOutputException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpStatusOutputException"/> class.
        /// </summary>
        /// <param name="code">The http status code.</param>
        /// <param name="description">The optional error description.</param>
        /// <param name="level">The logging level. The default is 'Error'</param>
        /// <param name="ex">The optional inner exception.</param>
        /// <param name="eid">The event id. The default is default(EventId).</param>
        /// <param name="statusSubcode">Gets the status error sub code. This can be used to give a more detailed error state.</param>
        /// <param name="errorPayload">This is the optional error payload. If this is specified it is returned in the HTTP response.</param>
        public HttpStatusOutputException(int code, string description = null
            , LogLevel level = LogLevel.Warning
            , Exception ex = null
            , EventId eid = default(EventId)
            , int? statusSubcode = null
            , object errorPayload = null
            ) : base(description, ex)
        {
            StatusCode = code;
            EventId = eid;
            Level = level;
            StatusSubcode = statusSubcode;
            ErrorPayload = errorPayload;
        }

        /// <summary>
        /// Gets the status error code.
        /// </summary>
        public int StatusCode { get; }
        /// <summary>
        /// Gets the status error sub code. This can be used to give a more detailed error state.
        /// </summary>
        public int? StatusSubcode { get; }
        /// <summary>
        /// Gets the log level for the exception.
        /// </summary>
        public LogLevel Level { get; }
        /// <summary>
        /// Gets or sets the event id.
        /// </summary>
        public EventId EventId { get; }

        /// <summary>
        /// This is the optional payload to be returned to the user in the HTTP response.
        /// </summary>
        public object ErrorPayload { get; }

        /// <summary>
        /// Logs the specified details to the logger..
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        public void Log(ILogger log, Func<Exception, string> message)
        {
            Log<object>(log, message, null);
        }
        /// <summary>
        /// Logs the exception to the logger.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="state">The state.</param>
        public void Log<T>(ILogger log, Func<Exception, string> message, T state = default(T))
        {
            log.Log(Level, EventId, state, InnerException, (t, ex) => message(ex));
        }
    }
}
