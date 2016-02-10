using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface ILoggerExtended : ILogger
    {
        void LogException(Exception ex);
        void LogException(string message, Exception ex);
        void LogMessage(string message);
        void LogMessage(LoggingLevel level, string message);
        void LogMessage(LoggingLevel level, string message, string category);
    }
}
