using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

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
        void Connect(ILoggerFactory lf);

        /// <summary>
        /// Initializes a new instance of the <see cref="IApiStartupContext" /> interface.
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
        /// This collection contains the list of attribute directives for the context.
        /// This can be used to process and set the repositories and the singleton registrations.
        /// </summary>
        ContextDirectives Directives { get; }

        /// <summary>
        /// This is the service identity for the Api.
        /// </summary>
        IApiServiceIdentity ServiceIdentity { get; }
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
