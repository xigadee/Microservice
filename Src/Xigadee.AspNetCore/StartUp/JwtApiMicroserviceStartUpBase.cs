using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Xigadee
{
    /// <summary>
    /// This class is used by all services for the application.
    /// </summary>
    /// <typeparam name="CTX">The application context type.</typeparam>
    public abstract class JwtApiMicroserviceStartupBase<CTX> : ApiStartupBase<CTX>
        where CTX : JwtApiMicroserviceStartUpContext, new()
    {
        #region A=>Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtApiMicroserviceStartupBase{CTX}"/> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        protected JwtApiMicroserviceStartupBase(Microsoft.AspNetCore.Hosting.IHostingEnvironment env) : base(env)
        {
        }
        #endregion

        #region ConfigureSecurity(IApplicationBuilder app)
        /// <summary>
        /// This override turns authentication on.
        /// </summary>
        /// <param name="app">The application.</param>
        protected override void ConfigureSecurity(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
        #endregion
        #region ConfigureSecurityAuthentication(IServiceCollection services)
        /// <summary>
        /// Configures the authentication using the Jwt token configuration.
        /// </summary>
        /// <param name="services">The services.</param>
        protected override void ConfigureSecurityAuthentication(IServiceCollection services)
        {
            services.AddJwtAuthentication(Context.SecurityJwt);
        }
        #endregion
    }

}
