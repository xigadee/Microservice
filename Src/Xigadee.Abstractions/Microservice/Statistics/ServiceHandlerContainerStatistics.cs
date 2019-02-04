namespace Xigadee
{
    /// <summary>
    /// This class holds a reference to the statistics.
    /// </summary>
    public class ServiceHandlerContainerStatistics : StatusBase
    {
        /// <summary>
        /// This is a list of the supported authentication handlers.
        /// </summary>
        public string[] Authentication { get; set; }
        /// <summary>
        /// This is a list of the supported encryption handlers.
        /// </summary>
        public string[] Encryption { get; set; }
        /// <summary>
        /// This is a list of the supported encryption handlers.
        /// </summary>
        public string[] Compression { get; set; }
        /// <summary>
        /// This is a list of the supported encryption handlers.
        /// </summary>
        public string[] Serialization { get; set; }
    }
}
