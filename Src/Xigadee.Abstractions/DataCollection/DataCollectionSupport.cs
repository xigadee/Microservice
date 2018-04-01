using System;

namespace Xigadee
{
    /// <summary>
    /// This enumeration is used to specify the type of data collection supported by the component.
    /// </summary>
    [Flags]
    public enum DataCollectionSupport:int
    {
        /// <summary>
        /// This is a default setting.
        /// </summary>
        None = 0,
        /// <summary>
        /// The data is a log message.
        /// </summary>
        Logger = 1,
        /// <summary>
        /// The data is a event source record.
        /// </summary>
        EventSource = 2,
        /// <summary>
        /// The data is a boundary log event.
        /// </summary>
        Boundary = 4,
        /// <summary>
        /// The data is telemetry.
        /// </summary>
        Telemetry = 8,
        /// <summary>
        /// The data is a dispatcher transit event.
        /// </summary>
        Dispatcher = 16,
        /// <summary>
        /// The data is Microservice statistics
        /// </summary>
        Statistics = 32,
        /// <summary>
        /// The data is a resource event.
        /// </summary>
        Resource = 64,
        /// <summary>
        /// The data is the custom event.
        /// </summary>
        Custom = 128,
        /// <summary>
        /// The data is a security event.
        /// </summary>
        Security = 256,
        /// <summary>
        /// The data is a API boundary event.
        /// </summary>
        ApiBoundary = 512
    }
}
