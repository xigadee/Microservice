using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

namespace Xigadee
{
    /// <summary>
    /// This is the base interface with non-platform specific properties.
    /// </summary>
    public interface IApiStartupContextBase
    {
        /// <summary>
        /// Connects the application components and registers the relevant services.
        /// </summary>
        /// <param name="lf">The logger factory.</param>
        void ContextConnect(IApplicationBuilder app, ILoggerFactory lf);

        /// <summary>
        /// Initializes and create the modules dynamically.
        /// </summary>
        /// <param name="services">The services.</param>
        void ModulesCreate(IServiceCollection services);

        /// <summary>
        /// Gets or sets the application configuration.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// This is the service identity for the Api.
        /// </summary>
        IApiServiceIdentity ServiceIdentity { get; }

        /// <summary>
        /// This is the Xigadee service pipeline.
        /// </summary>
        MicroservicePipeline MicroservicePipeline { get; set; }
        /// <summary>
        /// This is the microservice hosted service.
        /// </summary>
        MicroserviceHostedService MicroserviceHostedService { get; set; }

        #region LoggerFactory
        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        ILoggerFactory LoggerFactory { get; set; }
        /// <summary>
        /// This is the logger provider for the application.
        /// </summary>
        ApplicationLoggerProvider LoggerProvider { get; set; }
        #endregion
    }

    /// <summary>
    /// This is the root API-based microservice application interface.
    /// </summary>
    public interface IApiStartupContext<HE> : IHostedService, IApiStartupContextBase
    {
        /// <summary>
        /// Initializes the module with the application environment settings.
        /// </summary>
        /// <param name="env">The environment.</param>
        void Initialize(HE env);


        /// <summary>
        /// Gets or sets the hosting environment.
        /// </summary>
        HE Environment { get; set; }

    }
}
