namespace Xigadee
{
    /// <summary>
    /// This enumeration contains the logging levels used throughout the Microservice.
    /// </summary>
    public enum LoggingLevel
    {
        /// <summary>
        /// The generic low level trace information
        /// </summary>
        Trace = 10,
        /// <summary>
        /// The low level information event
        /// </summary>
        Info = 11,
        /// <summary>
        /// The status level, used for logging service status information
        /// </summary>
        Status = 12,
        /// <summary>
        /// The error warning level
        /// </summary>
        Warning = 13,
        /// <summary>
        /// The error level
        /// </summary>
        Error = 14,
        /// <summary>
        /// The fatal error level.
        /// </summary>
        Fatal = 15
    }
}
