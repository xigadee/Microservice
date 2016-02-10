using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This calss holds the error information.
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// This is the error severity level.
        /// </summary>
        public EventLogEntryType Type;
        /// <summary>
        /// This is the message to be displayed
        /// </summary>
        public string Message;

        public readonly int Priority = Environment.TickCount;
    }
}
