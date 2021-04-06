namespace Xigadee
{
    /// <summary>
    /// This is the base configuration.
    /// </summary>
    public abstract class ConfigPipelineBase
    {
        /// <summary>
        /// This method sets the overall status of the command.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
