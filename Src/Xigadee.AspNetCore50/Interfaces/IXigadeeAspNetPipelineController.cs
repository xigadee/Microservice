using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Xigadee
{
    /// <summary>
    /// 
    /// </summary>
    public interface IXigadeeAspNetPipelineController : IXigadeeAspNetPipelineComponent
    {
        void ConfigureControllerOptions(IServiceCollection services);

        void ConfigurePipeline(IApplicationBuilder app);

        void ConfigurePipelineComplete(IApplicationBuilder app);

        void ConfigureUseEndpoints(IApplicationBuilder app);

        void ConfigureUseEndpoints(IApplicationBuilder app, IEndpointRouteBuilder endpoints);
    }
}
