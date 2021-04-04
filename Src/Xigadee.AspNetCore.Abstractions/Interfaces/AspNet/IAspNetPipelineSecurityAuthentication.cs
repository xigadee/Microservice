using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Xigadee
{
    /// <summary>
    /// This extension is specific for the authentiation.
    /// </summary>
    public interface IAspNetPipelineSecurityAuthentication : IAspNetPipelineComponent
    {
        /// <summary>
        /// This method enabled authentication. 
        /// </summary>
        /// <param name="app">The application builder.</param>
        void ConfigureSecurityAuthentication(IApplicationBuilder app);
        /// <summary>
        /// This method sets the authentication.
        /// </summary>
        /// <param name="services">The service collection.</param>
        void ConfigureSecurityAuthentication(IServiceCollection services);
    }

}
