using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Xigadee
{
    public interface IAspNetPipelineSecurityAuthorization : IAspNetPipelineComponent
    {
        void ConfigureSecurityAuthorization(IApplicationBuilder app);

        void ConfigureSecurityAuthorization(IServiceCollection services);
    }
}
