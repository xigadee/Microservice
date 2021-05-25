using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Xigadee
{
    //Application Builder
    public abstract partial class ApiStartUpContextRoot<HE>
    {
        #region C=>Configure(IApplicationBuilder app)
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public virtual void ConfigureApplication(IApplicationBuilder app)
        {
            LoggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();

            ConfigurePipeline(app);

            ConfigureLogging(app);

            ContextConnect(app, LoggerFactory);

            ConfigureSecurity(app);

            ConfigureUseEndpoints(app);
        }
        #endregion
        #region C1. ConfigurePipeline(IApplicationBuilder app)
        /// <summary>
        /// Configures the ASP.NET pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        protected virtual void ConfigurePipeline(IApplicationBuilder app)
        {
            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.BeforeController
                , (ext, scope) => ext.ConfigurePipeline(scope, app));

            IAspNetPipelineController exte = null;
            if (PipelineComponentTryGet(out exte))
                exte.ConfigurePipeline(app);

            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.AfterController
                , (ext, scope) => ext.ConfigurePipeline(scope, app));

            exte?.ConfigurePipelineComplete(app);

        }
        #endregion
        #region C2. ConfigureLogging(IApplicationBuilder app)
        /// <summary>
        /// Configures the logging provide for the application.
        /// </summary>
        /// <param name="app">The application.</param>
        protected virtual void ConfigureLogging(IApplicationBuilder app)
        {
            ConfigureLoggingSubscribers(app, LoggerProvider);

            //Add our default logger with the default configuration.
            LoggerFactory.AddProvider(LoggerProvider);

            //This method releases any held messages.
            //These messages were initially held while the logging infrastructure was being set up.
            //This is to ensure that we don't loose our initial set up logging.
            LoggerProvider.Release();
        }
        #endregion
        #region C2a. ConfigureLoggingSubscribers(IApplicationBuilder app, ApplicationLoggerProvider provider)
        /// <summary>
        /// Use this override to add your specific logging subscribers to published events.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="provider">The logger provider.</param>
        protected virtual void ConfigureLoggingSubscribers(IApplicationBuilder app, ApplicationLoggerProvider provider)
        {

        }
        #endregion
        #region C3. ContextConnect(IApplicationBuilder app, ILoggerFactory loggerFactory) -> CXC ->
        /// <summary>
        /// Connects the application components and registers the relevant services.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="lf">The logger factory.</param>
        public virtual void ContextConnect(IApplicationBuilder app, ILoggerFactory lf)
        {
            Logger = lf.CreateLogger<IApiStartupContextBase>();

            //Set the logger for the pipeline extensions.
            PipelineComponents.ForEach(ext =>
            {
                ext.Value.Logger = Logger;
                ext.Value.Host = Host;
            });

            AttributeModulesConnect(lf);
        }
        #endregion
        #region C4. ConfigureSecurity(IApplicationBuilder app)
        /// <summary>
        /// Override this method to set authentication using app.UseAuthentication();
        /// </summary>
        /// <param name="app">The application.</param>
        protected virtual void ConfigureSecurity(IApplicationBuilder app)
        {
            if (PipelineComponentTryGet<IAspNetPipelineSecurityAuthentication>(out var exte))
                exte.ConfigureSecurityAuthentication(app);

            if (PipelineComponentTryGet<IAspNetPipelineSecurityAuthorization>(out var exto))
                exto.ConfigureSecurityAuthorization(app);
        }
        #endregion
        #region C5. ConfigureUseEndpoints(IApplicationBuilder app
        /// <summary>
        /// Use this section to configure the routing
        /// </summary>
        /// <param name="app">The application builder.</param>
        protected abstract void ConfigureUseEndpoints(IApplicationBuilder app);
        #endregion
    }
}
