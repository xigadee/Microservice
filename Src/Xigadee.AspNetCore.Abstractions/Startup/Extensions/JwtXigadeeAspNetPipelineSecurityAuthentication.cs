using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{

    /// <summary>
    /// This method configures Jwt authentication for the application.
    /// </summary>
    public class JwtXigadeeAspNetPipelineSecurityAuthentication : XigadeeAspNetPipelineComponentBase, IAspNetPipelineSecurityAuthentication
    {
        public JwtXigadeeAspNetPipelineSecurityAuthentication(ConfigAuthenticationJwt SecurityJwt)
        {
            this.SecurityJwt = SecurityJwt;
        }

        /// <summary>
        /// This is the Jwt configuration.
        /// </summary>
        public ConfigAuthenticationJwt SecurityJwt { get; }

        /// <summary>
        /// This method enabled authentication. 
        /// </summary>
        /// <param name="app">The application builder.</param>
        public void ConfigureSecurityAuthentication(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
        /// <summary>
        /// This method sets the authentication.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureSecurityAuthentication(IServiceCollection services)
        {
            services.AddJwtAuthentication(SecurityJwt);
        }
    }
}
