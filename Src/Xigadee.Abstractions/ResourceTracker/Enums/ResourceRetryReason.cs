namespace Xigadee
{
    /// <summary>
    /// This is the reason for a retry attempt on a resource
    /// </summary>
    public enum ResourceRetryReason
    {
        /// <summary>
        /// The underlying resource is throttling traffic.
        /// </summary>
        Throttle,
        /// <summary>
        /// These was a timeout when accessing the resource.
        /// </summary>
        Timeout,
        /// <summary>
        /// An exception occurred when accessing the resource.
        /// </summary>
        Exception,
        /// <summary>
        /// The retry reason was not specified.
        /// </summary>
        Other
    }
}
