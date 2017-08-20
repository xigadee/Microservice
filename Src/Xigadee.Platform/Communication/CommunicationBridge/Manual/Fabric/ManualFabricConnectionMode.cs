namespace Xigadee
{   
    /// <summary>
    /// This enumeration contains the supported modes for a manual connection.
    /// </summary>
    public enum ManualFabricConnectionMode
    {
        /// <summary>
        /// The queue 1-1 mode.
        /// </summary>
        Queue,
        /// <summary>
        /// The subscription 1-to-many receive mode.
        /// </summary>
        Subscription,
        /// <summary>
        /// The transmit only mode.
        /// </summary>
        Transmit
    }
}
