namespace Xigadee
{
    /// <summary>
    /// This class holds a reference to the statistics.
    /// </summary>
    public class SecurityContainerStatistics: StatusBase
    {
        /// <summary>
        /// This is a list of the supported authentication handlers.
        /// </summary>
        public string[] AuthenticationHandlers { get; set; }
        /// <summary>
        /// This is a list of the supported encryption handlers.
        /// </summary>
        public string[] EncryptionHandlers { get; set; }
    }
}
