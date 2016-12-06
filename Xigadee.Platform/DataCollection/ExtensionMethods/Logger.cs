using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// These extension methods simplify the logging of complex data to a consistent framework.
    /// </summary>
    public static partial class DataCollectionExtensionMethodsLogger
    {
        public static async Task Log(this IDataCollection collector, LogEvent logEvent)
        {
            collector.Write(logEvent);
        }

        public static void LogException(this IDataCollection collector, Exception ex)
        {
            collector.Write(new LogEvent(ex));
        }

        public static void LogException(this IDataCollection collector, string message, Exception ex)
        {
            collector.Write(new LogEvent(message, ex));
        }

        public static void LogMessage(this IDataCollection collector, string message)
        {
            collector.Write(new LogEvent(message));
        }

        public static void LogMessage(this IDataCollection collector, LoggingLevel level, string message)
        {
            collector.Write(new LogEvent(level, message));
        }

        public static void LogMessage(this IDataCollection collector, LoggingLevel level, string message, string category)
        {
            collector.Write(new LogEvent(level, message, category));
        }
    }

    public class Fuck: ILogger
    {
        public Task Log(LogEvent logEvent)
        {
            throw new NotImplementedException();
        }
    }
}
