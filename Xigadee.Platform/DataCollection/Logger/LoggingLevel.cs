using System;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This enumeration contains the logging levels used throughout the Microservice.
    /// </summary>
    public enum LoggingLevel
    {
        Trace = 10,
        Info = 11,
        Status = 12,
        Warning = 13,
        Error = 14,
        Fatal = 15
    }
}
