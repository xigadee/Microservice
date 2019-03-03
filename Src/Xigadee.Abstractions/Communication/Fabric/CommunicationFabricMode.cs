namespace Xigadee
{
    /// <summary>
    /// This enumeration specifies the mode used for the bridge.
    /// </summary>
    public enum CommunicationFabricMode
    {
        /// <summary>
        /// In round robin mode a response message is only sent to a single recipient.
        /// </summary>
        Queue,
        /// <summary>
        /// In broadcast mode a response message is sent to all recipients.
        /// </summary>
        Broadcast,
        /// <summary>
        /// The mode is not used by the agent.
        /// </summary>
        NotSet
    }
}
