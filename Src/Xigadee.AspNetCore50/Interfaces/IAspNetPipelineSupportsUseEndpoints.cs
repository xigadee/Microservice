using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Xigadee
{
    /// <summary>
    /// THis interface is supported by components that use extended endpoints.
    /// This is due to the fact that the IEndpointRouteBuilder is not supported in the abstract classes and can only
    /// be implemented in the Core libraries.
    /// </summary>
    public interface IAspNetPipelineSupportsUseEndpoints
    {
        /// <summary>
        /// THis is the extended endpoint support.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="endpoints"></param>
        /// <param name="scope">The optional scope.</param>
        void ConfigureUseEndpoints(IApplicationBuilder app, IEndpointRouteBuilder endpoints, XigadeeAspNetPipelineExtensionScope? scope = null);
    }
}
