using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the root context it contains shared functionality within the .NET Standard capabilities.
    /// </summary>
    public abstract class ApiStartUpContextRoot<HE> : IApiStartupContextBase, IHostedService
        where HE : HostingContainerBase
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
        #region Host
        /// <summary>
        /// This is the hosting container environment.
        /// </summary>
        public HE Host { get; protected set; }
        #endregion
        #region PipelineExtensions
        /// <summary>
        /// These are the specific extensions such as HealthCheck that are inserted outside of the standard ASP.NET controller flow.
        /// </summary>
        public List<IAspNetPipelineExtension> PipelineExtensions { get; } = new List<IAspNetPipelineExtension>();
        #endregion

        #region PipelineComponents
        /// <summary>
        /// This is the internal list of supported pipeline extensions.
        /// </summary>
        public Dictionary<Type, IAspNetPipelineComponent> PipelineComponents { get; } = new Dictionary<Type, IAspNetPipelineComponent>();
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
        public virtual IConfiguration Configuration { get => Host?.Configuration; }
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

        #region CXA => Initialize(IHostingEnvironment env)/Initialize()
        /// <summary>
        /// Initializes the context.
        /// </summary>
        /// <param name="cont">The hosting container.</param>
        public virtual void Initialize(HE cont)
        {
            Host = cont;

            Initialize();
        }
        /// <summary>
        /// Initializes the context.
        /// </summary>
        public virtual void Initialize()
        {
            Build();
            Bind();
            Pipeline();
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
        protected abstract void Bind();
        #endregion

        #region 3.Pipeline()
        /// <summary>
        /// This method sets the pipeline components and extensions.
        /// </summary>
        protected virtual void Pipeline()
        {
            PipelineComponentsSet();

            PipelineSecuritySet();

            PipelineExtensionsSet();
        }
        #endregion
        #region 3.A PipelineComponentsSet()
        /// <summary>
        /// This method is used to set the pipeline extensions for the context.
        /// </summary>
        protected abstract void PipelineComponentsSet();
        #endregion
        #region 3.B PipelineSecuritySet()
        /// <summary>
        /// This method is used to set the pipeline extensions for the context.
        /// </summary>
        protected virtual void PipelineSecuritySet() { }
        #endregion
        #region 3.C PipelineExtensionsSet()
        /// <summary>
        /// This method is used to set the pipeline extensions for the context.
        /// </summary>
        protected virtual void PipelineExtensionsSet() { }
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

            //Set the logger for the pipeline extensions.
            PipelineComponents.ForEach(ext =>
            {
                ext.Value.Logger = Logger;
                ext.Value.Host = Host;
            });
        }
        #endregion

        #region StartAsync/StopAsync
        /// <summary>
        /// This override starts any registered module that have the start stop attribute set in the context.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public virtual Task StartAsync(CancellationToken cancellationToken) =>
            Task.WhenAll(Directives.ModuleStartStopExtract().Select(m => StartAsyncComponent(m, cancellationToken)));
        /// <summary>
        /// This method is used to start a specific component.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task StartAsyncComponent(IApiModuleService m, CancellationToken cancellationToken) => m.Start(cancellationToken);

        /// <summary>
        /// This method stops any modules that have been marked for start stop.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public virtual Task StopAsync(CancellationToken cancellationToken) =>
            Task.WhenAll(Directives.ModuleStartStopExtract().Select(m => StopAsyncComponent(m, cancellationToken)));
        /// <summary>
        /// This method is used to stop a specific component.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task StopAsyncComponent(IApiModuleService m, CancellationToken cancellationToken) => m.Stop(cancellationToken);

        #endregion

        #region PipelineComponents ...
        /// <summary>
        /// Specifies if the model supports a specific interface.
        /// </summary>
        /// <typeparam name="I">This is the interface type.</typeparam>
        /// <returns>Returns true if the incoming model supported this interface.</returns>
        public bool SupportsPipelineComponent<I>() where I : IAspNetPipelineComponent
            => PipelineComponents.ContainsKey(typeof(I));
        /// <summary>
        /// This method gets the pipeline extension.
        /// </summary>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <returns>Retuns the extension class.</returns>
        public I PipelineComponentGet<I>() where I : IAspNetPipelineComponent
            => (I)PipelineComponents[typeof(I)];
        /// <summary>
        /// This method is used to try and get the extension.
        /// </summary>
        /// <typeparam name="I">The extension type.</typeparam>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool PipelineComponentTryGet<I>(out I extension) where I : IAspNetPipelineComponent
        {
            extension = default(I);

            if (SupportsPipelineComponent<I>())
            {
                extension = PipelineComponentGet<I>();
                return true;
            }

            return false;
        }
        /// <summary>
        /// This method sets a specific pipeline extension.
        /// </summary>
        /// <typeparam name="I">The extension type.</typeparam>
        /// <param name="extension">The class that implements the extension.</param>
        protected void PipelineComponentSet<I>(I extension) where I : IAspNetPipelineComponent
        {
            extension.Host = Host;
            PipelineComponents[typeof(I)] = extension;
        }
        #endregion

        #region Statistics
        /// <summary>
        /// This is a copy of the last issues Microservice statistics.
        /// </summary>
        [RegisterAsSingleton]
        public StatisticsHolder Statistics { get; private set; } = new StatisticsHolder();
        #endregion
    }
}
