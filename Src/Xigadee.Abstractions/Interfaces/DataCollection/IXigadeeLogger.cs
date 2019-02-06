using System;
using System.Threading.Tasks;
namespace Xigadee
{
    /// <summary>
    /// This is the root logger interface to support async logging support through the framework.
    /// </summary>
    [Obsolete("Use the new data collection interfaces")]
    public interface IXigadeeLogger
    {
        /// <summary>
        /// This method asynchronously logs an event.
        /// </summary>
        /// <param name="logEvent">The event to log.</param>
        /// <returns>This is an async task.</returns>
        Task Log(LogEvent logEvent);
    }
}
