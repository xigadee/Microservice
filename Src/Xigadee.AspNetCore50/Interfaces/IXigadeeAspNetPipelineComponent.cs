using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This is the root interface for pipeline extensions. It does not implement any methods.
    /// </summary>
    public interface IXigadeeAspNetPipelineComponent
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        ILogger Logger { get; set; }
    }
}
