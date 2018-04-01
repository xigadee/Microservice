namespace Xigadee
{
    /// <summary>
    /// The is the job status enumeration.
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// The job is created.
        /// </summary>
        Created,
        /// <summary>
        /// The job has experienced an error during index-up
        /// </summary>
        Faulted,

        /// <summary>
        /// The job is starting.
        /// </summary>
        Starting,
        /// <summary>
        /// The job is starting.
        /// </summary>

        Running,
        /// <summary>
        /// The job is running.
        /// </summary>
        Stopped,
        /// <summary>
        /// The job is stopped.
        /// </summary>
        Stopping,
        /// <summary>
        /// The job is stopping.
        /// </summary>
        Pausing,
        /// <summary>
        /// The job is pausing. This is not currently supported.
        /// </summary>
        Paused,
        /// <summary>
        /// The job is resuming. This is not currently supported.
        /// </summary>
        Resuming
    }
}
