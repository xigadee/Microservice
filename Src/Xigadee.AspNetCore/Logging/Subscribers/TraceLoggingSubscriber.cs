using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This is the default trace logger.
    /// </summary>
    public class TraceLoggingSubscriber : ILogEventSubscriber
    {
        /// <summary>
        /// The default subscriber.
        /// </summary>
        /// <param name="config"></param>
        public TraceLoggingSubscriber(ConfigTraceLogging config = null)
        {
            //Set the config, we enable everything if this is not set.
            Configuration = config ?? new ConfigTraceLogging() { Enabled = true };
        }

        ConfigTraceLogging Configuration { get; }

        /// <summary>
        /// This is a stateless class.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Reveive the log event.
        /// </summary>
        /// <param name="logEvent">The event to process.</param>
        public void Receive(LogEventApplication logEvent)
        {
            try
            {
                if (logEvent == null || !(Configuration?.IsEnabled(logEvent.LogLevel) ?? true))
                    return;

                string output = logEvent.Message;

                switch (logEvent.LogLevel)
                {
                    case LogLevel.Critical:
                    case LogLevel.Error:
                        Trace.TraceError(output);
                        break;
                    case LogLevel.Warning:
                        Trace.TraceWarning(output);
                        break;
                    case LogLevel.Information:
                        Trace.TraceInformation(output);
                        break;
                    default:
                        Trace.WriteLine(output);
                        break;
                }
            }
            catch (Exception)
            {
                //We do not want to bubble exceptions out.
            }
        }
    }
}
