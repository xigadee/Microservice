#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This enum defines when the message is being logged.
    /// </summary>
    public enum DispatcherLoggerDirection
    {
        /// <summary>
        /// The message was logged as it entered the dispatcher.
        /// </summary>
        Incoming,
        /// <summary>
        /// The message was logged as it left the dispatcher.
        /// </summary>
        Outgoing
    }
}
