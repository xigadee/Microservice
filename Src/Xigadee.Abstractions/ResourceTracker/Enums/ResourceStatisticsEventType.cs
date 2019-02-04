namespace Xigadee
{
    /// <summary>
    /// This is the resource event action types.
    /// </summary>
    public enum ResourceStatisticsEventType
    {
        /// <summary>
        /// The resource counter has been created.
        /// </summary>
        Created,
        /// <summary>
        /// This is a regular poll that records the current metrics
        /// </summary>
        KeepAlive
    }
}
