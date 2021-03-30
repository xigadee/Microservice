using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Xigadee
{
    public interface IXigadeeAspNetPipelineSecurityAuthorization : IXigadeeAspNetPipelineComponent
    {
        void ConfigureSecurityAuthorization(IApplicationBuilder app);

        void ConfigureSecurityAuthorization(IServiceCollection services);
    }
}
