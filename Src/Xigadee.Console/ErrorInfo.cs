namespace Xigadee
{
    /// <summary>
    /// This class holds the error information.
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// This is the error severity level.
        /// </summary>
        public LoggingLevel Type;
        /// <summary>
        /// This is the message to be displayed
        /// </summary>
        public string Message;
        /// <summary>
        /// This is the incremental logging id.
        /// </summary>
        public long LoggingId;
    }
}
