namespace Xigadee
{
    /// <summary>
    /// This enumeration specifies how the helper processes messages, i.e. incoming, outgoing or both.
    /// </summary>
    public enum UdpHelperMode
    {
        /// <summary>
        /// The helper is in listener mode.
        /// </summary>
        Listener,
        /// <summary>
        /// The helper is in sender mode.
        /// </summary>
        Sender,
        /// <summary>
        /// The helper is bidirectional
        /// </summary>
        Bidirectional
    }
}
