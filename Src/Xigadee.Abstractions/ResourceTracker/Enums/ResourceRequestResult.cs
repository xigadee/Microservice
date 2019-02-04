namespace Xigadee
{
    /// <summary>
    /// This enumeration is used to record the reason for the end of the resource track.
    /// </summary>
    public enum ResourceRequestResult
    {
        /// <summary>
        /// The track was successful.
        /// </summary>
        Success,
        /// <summary>
        /// The track resulted in an exception.
        /// </summary>
        Exception,
        /// <summary>
        /// The task was cancelled.
        /// </summary>
        TaskCancelled,
        /// <summary>
        /// The number of retry attempts was exceeded.
        /// </summary>
        RetryExceeded,
        /// <summary>
        /// The reason for was not in the other values.
        /// </summary>
        Unknown
    }
}
