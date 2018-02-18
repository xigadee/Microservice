using Microsoft.AspNetCore.Builder;

namespace Xigadee
{
    /// <summary>
    /// This pipeline is used to set Xigadee configuration parameters during the AspNet configuration.
    /// </summary>
    /// <seealso cref="Xigadee.IPipeline" />
    public interface IPipelineAspNetCore: IPipeline
    {
        /// <summary>
        /// Gets the AspNetCore application.
        /// </summary>
        IApplicationBuilder App { get; }
    }
}
