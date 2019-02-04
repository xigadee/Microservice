namespace Xigadee
{
    /// <summary>
    /// This enum specifies the behaviour to take when receiving an unhandled message.
    /// </summary>
    public enum DispatcherUnhandledAction
    {
        /// <summary>
        /// An exception will be raised and logged.
        /// </summary>
        Exception,
        /// <summary>
        /// The message will be logged and ignored.
        /// </summary>
        Ignore,
        /// <summary>
        /// The dispatcher will attempt to send a 500 error response to the originating sender.
        /// </summary>
        AttemptResponseFailMessage
    }
}
