using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Xigadee
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAspNetPipelineController : IAspNetPipelineComponent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        void ConfigureControllerOptions(IServiceCollection services);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        void ConfigurePipeline(IApplicationBuilder app);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        void ConfigurePipelineComplete(IApplicationBuilder app);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        void ConfigureUseEndpoints(IApplicationBuilder app);
    }
}
