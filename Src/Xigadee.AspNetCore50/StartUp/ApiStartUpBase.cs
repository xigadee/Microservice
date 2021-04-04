using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Xigadee
{
    /// <summary>
    /// This class is used by all services for the application.
    /// </summary>
    /// <typeparam name="CTX">The application context type.</typeparam>
    public class ApiStartupBase<CTX> : ApiStartUpRoot<CTX, AspNetCore5HostingContainer>
        where CTX : ApiStartUpContext, new()
    {
        #region A=>Constructor
        /// <summary>
        /// Initializes a new instance of the API application class.
        /// </summary>
        /// <param name="whEnv">The web host environment.</param>
        /// <param name="hEnv">The host environment.</param>
        /// <param name="cfg">The configuration.</param>
        public ApiStartupBase(IWebHostEnvironment whEnv, IHostEnvironment hEnv, IConfiguration cfg) : base(new AspNetCore5HostingContainer(whEnv, hEnv, cfg))
        {
        }
        #endregion

        #region B=>ConfigureServices(IServiceCollection services)
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>Returns the new service provider.</returns>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesRoot(services);

            services.BuildServiceProvider();
        }
        #endregion
        #region B7. ConfigureController(IServiceCollection services)
        /// <summary>
        /// Configures the add MVC service.
        /// </summary>
        /// <param name="services">The services.</param>
        protected override void ConfigureController(IServiceCollection services)
        {
            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.BeforeController, (e, scope) => e.ConfigureServices(scope, services));

            if (Context.PipelineComponentTryGet<IAspNetPipelineController>(out var ext))
                ext.ConfigureControllerOptions(services);

            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.AfterController, (e, scope) => e.ConfigureServices(scope, services));
        }
        #endregion

        #region C5. ConfigureUseEndpoints(IApplicationBuilder app
        /// <summary>
        /// Use this section to configure the routing
        /// </summary>
        /// <param name="app">The application builder.</param>
        protected override void ConfigureUseEndpoints(IApplicationBuilder app)
        {
            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.BeforeController
                , (e, scope) => e.ConfigureUseEndpoints(scope, app));

            if (Context.PipelineComponentTryGet<IAspNetPipelineController>(out var ext))
                ext.ConfigureUseEndpoints(app);

            app.UseEndpoints(endpoints => ConfigureUseEndpoints(app, endpoints));

            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.AfterController
                , (e, scope) => e.ConfigureUseEndpoints(scope, app));

        }

        /// <summary>
        /// This method configures the specific endpoint.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="endpoints">The endpoint route builder.</param>
        protected virtual void ConfigureUseEndpoints(IApplicationBuilder app, IEndpointRouteBuilder endpoints)
        {
            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.BeforeController
                , (e, scope) => ExecuteIfSupportsEndpoints(e, app, endpoints, scope));

            if (Context.PipelineComponentTryGet<IAspNetPipelineController>(out var ext))
                ExecuteIfSupportsEndpoints(ext, app, endpoints);

            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.AfterController
                , (e, scope) => ExecuteIfSupportsEndpoints(e, app, endpoints, scope));
        }

        protected virtual void ExecuteIfSupportsEndpoints(IAspNetPipelineComponent c, IApplicationBuilder app, IEndpointRouteBuilder endpoints, XigadeeAspNetPipelineExtensionScope? scope = null)
        {
            var e = c as IAspNetPipelineSupportsUseEndpoints;
            if (e != null)
               e.ConfigureUseEndpoints(app, endpoints, scope);
        }
        #endregion
    }
}