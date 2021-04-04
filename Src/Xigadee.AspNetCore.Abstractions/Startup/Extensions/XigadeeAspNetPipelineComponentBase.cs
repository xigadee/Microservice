using Microsoft.Extensions.Logging;
namespace Xigadee
{
    /// <summary>
    /// This is the base component class for extensions.
    /// </summary>
    public class XigadeeAspNetPipelineComponentBase : IAspNetPipelineComponent
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }
    }
}
