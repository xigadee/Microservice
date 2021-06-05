﻿using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
namespace Xigadee
{
    /// <summary>
    /// This is the default start up context.
    /// </summary>
    public class ApiStartUpContext : ApiStartUpContextRoot<AspNetCore5HostingContainer>//, IApiStartupContext
    {
        #region 1.Build()
        /// <summary>
        /// Builds and sets the default configuration using the appsettings.json file and the appsettings.{Environment.EnvironmentName}.json file.
        /// </summary>
        protected override void Build()
        {
            var builder = new ConfigurationBuilder();

            builder
                .SetBasePath(Host.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Host.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Host.Configuration = builder.Build();
        }
        #endregion
        #region 2.Bind()
        /// <summary>
        /// Creates and binds specific configuration components required by the application.
        /// </summary>
        protected override void Bind()
        {
            BindAttributesConfigurationSet();

            BindConfigApplication();

            BindConfigMicroservice();

            BindConfigHealthCheck();

            ServiceIdentitySet();
        }
        #endregion
        #region 2.A BindConfigApplication()
        /// <summary>
        /// This in the application config binding.
        /// </summary>
        /// <summary>
        /// This in the application config binding.
        /// </summary>
        protected virtual void BindConfigApplication()
        {
            ConfigApplication = new ConfigApplication();
            if (!string.IsNullOrWhiteSpace(BindNameConfigApplication))
            {
                Configuration.Bind(BindNameConfigApplication, ConfigApplication);

                ConfigApplication.Connections = Configuration.GetSection("ConnectionStrings").GetChildren().ToDictionary((e) => e.Key, (e) => e.Value);
            }

        }
        #endregion
        #region 2.B BindConfigMicroservice()
        /// <summary>
        /// This is the microservice config binding.
        /// </summary>
        /// <summary>
        /// This is the microservice config binding.
        /// </summary>
        protected virtual void BindConfigMicroservice()
        {
            ConfigMicroservice = new ConfigMicroservice();
            if (UseMicroservice)
            {
                if (!string.IsNullOrEmpty(BindNameConfigMicroservice))
                    Configuration.Bind(BindNameConfigMicroservice, ConfigMicroservice);
            }
        }
        #endregion
        #region 2.C BindConfigHealthCheck()
        /// <summary>
        /// This is the config health check creation and binding.
        /// </summary>
        protected virtual void BindConfigHealthCheck()
        {
            ConfigHealthCheck = new ConfigHealthCheck();
            if (!string.IsNullOrWhiteSpace(BindNameConfigHealthCheck))
            {
                Configuration.Bind(BindNameConfigHealthCheck, ConfigHealthCheck);
            }
        }
        #endregion
        #region 2.D ServiceIdentitySet()
        /// <summary>
        /// This method sets the service identity for the application.
        /// This is primarily used for logging and contains the various parameters needed
        /// to identity this instance when debugging and logging.
        /// </summary>
        protected virtual void ServiceIdentitySet()
        {
            //Set the Microservice Identity
            string instanceId = System.Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
            string siteName = System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
            string siteHostname = System.Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");

            var url = string.IsNullOrEmpty(siteName) ? "http://localhost" : $"https://{siteName}.azurewebsites.net/";

            var ass = GetType().Assembly;

            ServiceIdentity = new ApiServiceIdentity(
                  Id
                , Host.MachineName
                , Host.ApplicationName
                , ass.GetName().Version.ToString()
                , url
                , instanceId
                , Host.EnvironmentName);
        }
        #endregion

        #region AttributeConfigurationSetBind(string configName, object obj)
        /// <summary>
        /// This override is used to bind the specific attributes.
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="obj"></param>
        public override void AttributeConfigurationSetBind(string configName, object obj) => Configuration.Bind(configName, obj); 
        #endregion


        #region 3.A PipelineComponentsSet()
        /// <summary>
        /// This method is used to set the pipeline extensions for the context.
        /// </summary>
        protected override void PipelineComponentsSet()
        {
            //Set the default Api settings.
            PipelineComponentSet<IAspNetPipelineController>(new XigadeeAspNetPipelineController(GetType()));
        }
        #endregion
        #region 3.C PipelineExtensionsSet()
        /// <summary>
        /// This method is used to set the pipeline extensions for the context.
        /// </summary>
        protected override void PipelineExtensionsSet()
        {
            //Set the default Api settings.
            if (ConfigHealthCheck?.Enabled ?? false)
                PipelineExtensionRegister(new HealthCheckExtension(ConfigHealthCheck));
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

            if (PipelineComponentTryGet<IAspNetPipelineController>(out var ext))
                ext.ConfigureControllerOptions(services);

            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.AfterController, (e, scope) => e.ConfigureServices(scope, services));
        }
        #endregion

        #region PipelineExtensionRegister(IXigadeeAspNetPipelineExtension extension)
        /// <summary>
        /// This method sets the extension logger and then adds it to the collection.
        /// </summary>
        /// <param name="extension">The extension to add.</param>
        public void PipelineExtensionRegister(IAspNetPipelineExtension extension)
        {
            extension.Logger = Logger;
            extension.Host = Host;
            PipelineExtensions.Add(extension);
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

            if (PipelineComponentTryGet<IAspNetPipelineController>(out var ext))
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

            if (PipelineComponentTryGet<IAspNetPipelineController>(out var ext))
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
