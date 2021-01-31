using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the root context with shared functionality with the .NET Standard capabilities.
    /// </summary>
    public abstract class ApiStartUpContextRoot: IApiStartupContextBase, IHostedService
    {
        #region Id
        /// <summary>
        /// This is the unique service Id which is regenerated each time the service starts up.
        /// </summary>
        public readonly Guid Id = Guid.NewGuid();

        /// <summary>
        /// This is the service identity for the Api.
        /// </summary>
        public IApiServiceIdentity ServiceIdentity { get; protected set; }
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default construct that creates the directive class.
        /// </summary>
        public ApiStartUpContextRoot()
        {
            Directives = new ContextDirectives(this);
        }
        #endregion
        #region Directives
        /// <summary>
        /// This collection contains the list of repository directives for the context.
        /// This can be used to populate the repositories and run time from a central method.
        /// Useful when you want to set as memory backed for testing.
        /// </summary>
        public ContextDirectives Directives { get; }
        #endregion

        #region ConfigHealthCheck
        /// <summary>
        /// This is the config health check settings.
        /// </summary>
        public virtual ConfigHealthCheck ConfigHealthCheck { get; set; }

        /// <summary>
        /// This is the bind name for the health check. Override this if you need to change it.
        /// </summary>
        protected virtual string BindNameConfigHealthCheck => "ConfigHealthCheck";
        #endregion
        #region Configuration
        /// <summary>
        /// Gets or sets the application configuration.
        /// </summary>
        public virtual IConfiguration Configuration { get; set; }
        #endregion        
        #region Logger
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public virtual ILogger Logger { get; set; }
        #endregion

        #region UseMicroservice
        /// <summary>
        /// Gets a value indicating whether the service should create an initialize a Microservice Pipeline
        /// </summary>
        public virtual bool UseMicroservice => true;
        #endregion
        #region ConfigMicroservice
        /// <summary>
        /// Gets or sets the microservice configuration.
        /// </summary>
        public virtual ConfigMicroservice ConfigMicroservice { get; set; }
        /// <summary>
        /// Gets the bind section for ConfigMicroservice.
        /// </summary>
        protected virtual string BindNameConfigMicroservice => "ConfigMicroservice";
        #endregion
        #region ConfigApplication
        /// <summary>
        /// Gets or sets the microservice configuration.
        /// </summary>
        public virtual ConfigApplication ConfigApplication { get; set; }
        /// <summary>
        /// Gets the bind section for ConfigMicroservice.
        /// </summary>
        protected virtual string BindNameConfigApplication => "ConfigApplication";
        #endregion

        #region CXA => Initialize()
        /// <summary>
        /// Initializes the context.
        /// </summary>
        /// <param name="env">The hosting environment.</param>
        public virtual void Initialize()
        {
            Build();
            Bind();
        }
        #endregion
        #region 1.Build()
        /// <summary>
        /// Builds and sets the default configuration using the appsettings.json file and the appsettings.{Environment.EnvironmentName}.json file.
        /// </summary>
        protected abstract void Build();
        #endregion

        #region 2.Bind()
        /// <summary>
        /// Creates and binds specific configuration components required by the application.
        /// </summary>
        protected virtual void Bind()
        {
            ConfigApplication = new ConfigApplication();
            BindConfigApplication();

            ConfigMicroservice = new ConfigMicroservice();
            BindConfigMicroservice();

            ConfigHealthCheck = new ConfigHealthCheck();
            BindConfigHealthCheck();

            ServiceIdentitySet();
        }
        #endregion

        //2.Bind
        #region BindConfigApplication()
        /// <summary>
        /// This in the application config binding.
        /// </summary>
        protected abstract void BindConfigApplication();
        #endregion
        #region BindConfigMicroservice()
        /// <summary>
        /// This is the microservice config binding.
        /// </summary>
        protected abstract void BindConfigMicroservice();
        #endregion
        #region BindConfigHealthCheck()
        /// <summary>
        /// This is the config health check creation and binding.
        /// </summary>
        protected abstract void BindConfigHealthCheck();

        #endregion
        #region ServiceIdentitySet()
        /// <summary>
        /// This method sets the service identity for the application.
        /// This is primarily used for logging and contains the various parameters needed
        /// to identity this instance when debugging and logging.
        /// </summary>
        protected abstract void ServiceIdentitySet();
        #endregion

        #region CXB => ModulesCreate(IServiceCollection services)
        /// <summary>
        /// Connects the application components and registers the relevant services.
        /// </summary>
        /// <param name="services">The services.</param>
        public virtual void ModulesCreate(IServiceCollection services)
        {
        }
        #endregion
        #region CXC => Connect(ILoggerFactory lf)
        /// <summary>
        /// Connects the application components and registers the relevant services.
        /// </summary>
        /// <param name="lf">The logger factory.</param>
        public virtual void Connect(ILoggerFactory lf)
        {
            Logger = lf.CreateLogger<IApiStartupContextBase>();
        }
        #endregion

        #region StartAsync/StopAsync
        /// <summary>
        /// This override starts any registered module that have the start stop attribute set in the context.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(Directives.ModuleStartStopExtract().Select(m => m.Start(cancellationToken)));
        }
        /// <summary>
        /// This method stops any modules that have been marked for start stop.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(Directives.ModuleStartStopExtract().Select(m => m.Stop(cancellationToken)));
        }
        #endregion
    }
}
