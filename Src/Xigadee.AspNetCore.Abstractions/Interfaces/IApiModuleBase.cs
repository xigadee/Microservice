using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This interface contains the base settings for all modules.
    /// </summary>
    public interface IApiModuleBase
    {
        /// <summary>
        /// Gets or sets the module logger.
        /// </summary>
        ILogger Logger { get; set; }
    }

    /// <summary>
    /// This is the base interface implemented by a config class for a specific module.
    /// </summary>
    public interface IApiModuleConfigBase
    {
        /// <summary>
        /// This property specifies whether the module component is enabled.
        /// </summary>
        public bool Enabled { get; set; }
    }
}
