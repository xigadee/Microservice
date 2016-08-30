using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This logger outputs event to the trace output.
    /// </summary>
    public class TraceEventLogger : ILogger
    {
        ConcurrentDictionary<LoggingLevel, LogConvert> mContains;

        private class LogConvert
        {
            public LogConvert(LoggingLevel level)
            {
                Level = level;
            }

            public LoggingLevel Level { get; }

            public bool Enabled { get; set; } = true;

            public Func<LogEvent, string> Output {get;set;} = null;
        }

        public TraceEventLogger()
        {
            var items = Enum.GetValues(typeof(LoggingLevel))
                .Cast<LoggingLevel>()
                .Select((l) => new KeyValuePair<LoggingLevel, LogConvert>(l, new LogConvert(l)));

            mContains = new ConcurrentDictionary<LoggingLevel, LogConvert>(items);
        }

        /// <summary>
        /// This method logs the output to the Trace stream.
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        public async Task Log(LogEvent logEvent)
        {
            try
            {
                var logStatus = mContains[logEvent.Level];

                if (!logStatus.Enabled)
                    return;

                string output;
                if (logStatus.Output != null)
                    output = logStatus.Output(logEvent);
                else
                    output = logEvent.Message;

                switch (logEvent.Level)
                {
                    case LoggingLevel.Fatal:
                        Trace.TraceError(output);
                        break;
                    case LoggingLevel.Error:
                        Trace.TraceError(output);
                        break;
                    case LoggingLevel.Warning:
                        Trace.TraceWarning(output);
                        break;
                    case LoggingLevel.Info:
                        Trace.TraceInformation(output);
                        break;
                    case LoggingLevel.Status:
                        Trace.TraceInformation(output);
                        break;
                    case LoggingLevel.Trace:
                        Trace.TraceInformation(output);
                        break;
                    default:
                        Trace.WriteLine(output);
                        break;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
