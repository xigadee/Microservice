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
    public class ApiStartUpContextRootTemp<HE>: IApiStartupContextBase, IHostedService
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
        public ApiStartUpContextRootTemp()
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

        #region PipelineComponents
        /// <summary>
        /// This is the internal list of supported pipeline extensions.
        /// </summary>
        public Dictionary<Type, IAspNetPipelineComponent> PipelineComponents { get; } = new Dictionary<Type, IAspNetPipelineComponent>();
        #endregion        
        #region PipelineExtensions
        /// <summary>
        /// These are the specific extensions such as HealthCheck that are inserted outside of the standard ASP.NET controller flow.
        /// </summary>
        public List<IAspNetPipelineExtension> PipelineExtensions { get; } = new List<IAspNetPipelineExtension>();
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
        protected virtual void Build()
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

            var ass = GetType().Assembly;

            ServiceIdentity = new ApiServiceIdentity(
                  Id
                , Host.MachineName
                , Host.ApplicationName
                , ass.GetName().Version.ToString()
                , ""//Host.Url
                , ""//Host.InstanceId
                , Host.EnvironmentName);
        }
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
        protected virtual void PipelineComponentsSet()
        {
            //Set the default Api settings.
            PipelineComponentSet<IAspNetPipelineController>(new XigadeeAspNetPipelineController());
        }
        #endregion
        #region 3.B PipelineSecuritySet()
        /// <summary>
        /// This method is used to set the pipeline extensions for the context.
        /// </summary>
        protected virtual void PipelineSecuritySet()
        {
        }
        #endregion
        #region 3.C PipelineExtensionsSet()
        /// <summary>
        /// This method is used to set the pipeline extensions for the context.
        /// </summary>
        protected virtual void PipelineExtensionsSet()
        {
            //Set the default Api settings.
            if (ConfigHealthCheck?.Enabled ?? false)
                PipelineExtensionRegister(new HealthCheckExtension(ConfigHealthCheck, Id, ConfigApplication?.Name ?? ""));
        }
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
            PipelineComponents.ForEach(ext => ext.Value.Logger = Logger);
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
            => PipelineComponents[typeof(I)] = extension;
        #endregion
        #region PipelineExtensionRegister(IXigadeeAspNetPipelineExtension extension)
        /// <summary>
        /// This method sets the extension logger and then adds it to the collection.
        /// </summary>
        /// <param name="extension">The extension to add.</param>
        public void PipelineExtensionRegister(IAspNetPipelineExtension extension)
        {
            extension.Logger = Logger;
            PipelineExtensions.Add(extension);
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
