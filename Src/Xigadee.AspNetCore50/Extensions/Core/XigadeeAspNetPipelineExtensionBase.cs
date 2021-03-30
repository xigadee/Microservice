using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This base class can be used to create a pipeline component extension.
    /// </summary>
    public abstract class XigadeeAspNetPipelineExtensionBase : XigadeeAspNetPipelineComponentBase, IAspNetPipelineExtension
    {
        public abstract bool Enabled { get; }

        public bool Supported(XigadeeAspNetPipelineExtensionScope scope)
            => Enabled && ((Scope & scope) > 0);

        public virtual XigadeeAspNetPipelineExtensionScope Scope { get; protected set; } = XigadeeAspNetPipelineExtensionScope.AfterController;

        public virtual void ConfigureServices(XigadeeAspNetPipelineExtensionScope scope, IServiceCollection services)
        {
        }

        public virtual void ConfigurePipeline(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app)
        {
        }

        public virtual void ConfigureUseEndpoints(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app)
        {
        }
        
        public virtual void ConfigureUseEndpoints(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app, IEndpointRouteBuilder endpoints)
        {
        }
    }
}
