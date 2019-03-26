namespace Xigadee
{
    /// <summary>
    /// This class holds the service setting
    /// </summary>
    public class ConfigModuleMode
    {
        /// <summary>
        /// Gets or sets the reference service mode: in-memory, live et.c
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Gets or sets the reference connection string.
        /// </summary>
        public string Connection { get; set; }
    }
}
