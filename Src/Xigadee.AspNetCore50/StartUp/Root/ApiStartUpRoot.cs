using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This start-up class is responsible for managing the ASP.NET Pipline and interleaving it with the
    /// Xigadee service framework.
    /// </summary>
    /// <typeparam name="CTX">The startup context.</typeparam>
    /// <typeparam name="HE">The hosting environment.</typeparam>
    public abstract class ApiStartUpRootTemp<CTX,HE>
        where CTX : ApiStartUpContextRootTemp<HE>, new()
        where HE: HostingContainerBase
    {
        #region A=>Constructor
        /// <summary>
        /// Initializes a new instance of the API application class.
        /// </summary>
        /// <param name="host">The environment host.</param>
        protected ApiStartUpRootTemp(HE host)
        {
            Host = host;
            Initialize();
        }
        #endregion

        #region Host
        /// <summary>
        /// This is the hosting environment.
        /// </summary>
        public HE Host { get; protected set; }
        #endregion

        #region A. Initialize()
        /// <summary>
        /// This method initialize the initial services.
        /// </summary>
        protected virtual void Initialize()
        {
            LoggerProviderCreate();

            ContextCreate();

            ContextInitialize();
        } 
        #endregion
        #region A1. LoggerProviderCreate()
        /// <summary>
        /// This method creates the LoggerProvider to allow processes such as the Microservice to connect
        /// their logging systems early.
        /// </summary>
        protected virtual void LoggerProviderCreate()
        {
            LoggerProvider = new ApplicationLoggerProvider();
            //This method tells the provider to hold messages internally until the release method is called.
            LoggerProvider.Hold();
        }
        #endregion
        #region A2. ContextCreate()
        /// <summary>
        /// Initializes the context
        /// </summary>
        protected virtual void ContextCreate()
        {
            Context = new CTX();
        }
        #endregion
        #region A3. ContextInitialize() -> CXA ->
        /// <summary>
        /// Initializes the context
        /// </summary>
        protected virtual void ContextInitialize() => Context.Initialize(Host);
        #endregion

        #region B=>ConfigureServices(IServiceCollection services)
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>Returns the new service provider.</returns>
        protected virtual IServiceProvider ConfigureServicesBase(IServiceCollection services)
        {
            ConfigureOptions(services);

            ContextModulesCreate(services);

            if (Context.UseMicroservice)
                ConfigureMicroserviceHostedService(services);

            ConfigureSingletons(services);

            ConfigureSecurityAuthentication(services);

            ConfigureSecurityAuthorization(services);

            ConfigureController(services);

            // Add framework services
            return services.BuildServiceProvider();
        }
        #endregion
        #region B1. ConfigureOptions(IServiceCollection services)
        /// <summary>
        /// Configures service options.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureOptions(IServiceCollection services)
        {

        }
        #endregion
        #region B2. ContextModulesCreate(IServiceCollection services) -> CXB ->
        /// <summary>
        /// Calls the context to create and register any modules and services respectively.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ContextModulesCreate(IServiceCollection services)
        {
            Context.ModulesCreate(services);
        }
        #endregion
        #region B3. ConfigureMicroserviceHostedService(IServiceCollection services)
        /// <summary>
        /// Configures the singletons.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureMicroserviceHostedService(IServiceCollection services)
        {
            MicroserviceCreate();

            MicroserviceDataCollectionConnect();

            MicroserviceConfigure();

            MicroserviceHostedServiceCreate();

            services.AddSingleton<IHostedService>(HostedService);
            //Add the context so that it can inform modules of the start up.
            services.AddSingleton<IHostedService>(Context);
        }
        #endregion
        #region B3a. MicroserviceCreate()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceCreate()
        {
            Pipeline = new MicroservicePipeline();

        }
        #endregion
        #region B3b. MicroserviceDataCollectionConnect()
        /// <summary>
        /// This method connects the DataCollection to the Logger Provider.
        /// </summary>
        protected virtual void MicroserviceDataCollectionConnect()
        {
            //This method pipes the incoming data collection events to the ASP.NET Core logger.
            Pipeline.OnDataCollection(MicroserviceOnDataCollection);
        }
        /// <summary>
        /// This method is called when a collection event is raised withing the Microservice.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="ev">The event.</param>
        protected virtual void MicroserviceOnDataCollection(OnDataCollectionContext ctx, EventHolder ev)
        {
            switch (ev.DataType)
            {
                //Let's send the most important events to their respective methods.
                case DataCollectionSupport.Statistics:
                    MicroserviceOnDataCollection_Statistics(ctx, ev);
                    break;
                case DataCollectionSupport.Telemetry:
                    MicroserviceOnDataCollection_Telemetry(ctx, ev);
                    break;
                case DataCollectionSupport.EventSource:
                    MicroserviceOnDataCollection_EventSource(ctx, ev);
                    break;
                case DataCollectionSupport.Logger:
                    if (MicroserviceOnDataCollection_Logger(ctx, ev, out LogEventApplication lev))
                        LoggerProvider.AddLogEventToQueue(lev);
                    break;
                default:
                    MicroserviceOnDataCollection_Other(ctx, ev);
                    break;
            }
        }

        /// <summary>
        /// This method is called for the less common Microservice event types.
        /// </summary>
        /// <param name="ctx">The incoming context.</param>
        /// <param name="ev">The incoming EventHolder.</param>
        protected virtual void MicroserviceOnDataCollection_Other(OnDataCollectionContext ctx, EventHolder ev) { }

        /// <summary>
        /// This method is called when the Microservice logs new statistics.
        /// </summary>
        /// <param name="ctx">The incoming context.</param>
        /// <param name="ev">The incoming EventHolder.</param>
        protected virtual void MicroserviceOnDataCollection_Statistics(OnDataCollectionContext ctx, EventHolder ev) { }

        /// <summary>
        /// This method is called when the Microservice logs new telemetry.
        /// </summary>
        /// <param name="ctx">The incoming context.</param>
        /// <param name="ev">The incoming EventHolder.</param>
        protected virtual void MicroserviceOnDataCollection_Telemetry(OnDataCollectionContext ctx, EventHolder ev) { }

        /// <summary>
        /// This method is called when the Microservice logs new event source messages.
        /// </summary>
        /// <param name="ctx">The incoming context.</param>
        /// <param name="ev">The incoming EventHolder.</param>
        protected virtual void MicroserviceOnDataCollection_EventSource(OnDataCollectionContext ctx, EventHolder ev) { }

        /// <summary>
        /// This method pipes the incoming Microservice logging event in to the ASP.NET Core logging system.
        /// You can override this method to filter out specific messages.
        /// </summary>
        /// <param name="ctx">The incoming context.</param>
        /// <param name="ev">The incoming EventHolder.</param>
        /// <param name="levOut">The outgoing event.</param>
        /// <returns>Returns true if the event should be logged.</returns>
        protected virtual bool MicroserviceOnDataCollection_Logger(OnDataCollectionContext ctx, EventHolder ev
            , out LogEventApplication levOut)
        {
            levOut = null;
            var le = ev.Data as LogEvent;
            //Check for the unexpected.
            if (le == null)
                return false;

            var lev = new LogEventApplication();
            lev.Message = le.Message;
            lev.Exception = le.Ex;
            lev.Name = ctx.OriginatorId.Name;
            lev.State = le;
            if (le.AdditionalData != null && le.AdditionalData.Count > 0)
            {
                if (lev.FormattedParameters == null)
                    lev.FormattedParameters = new Dictionary<string, object>();
                le.AdditionalData.ForEach(kv => lev.FormattedParameters.Add(kv.Key, kv.Value));
            }

            switch (le.Level)
            {
                case LoggingLevel.Fatal:
                    lev.LogLevel = LogLevel.Critical;
                    break;
                case LoggingLevel.Error:
                    lev.LogLevel = LogLevel.Error;
                    break;
                case LoggingLevel.Warning:
                    lev.LogLevel = LogLevel.Warning;
                    break;
                case LoggingLevel.Info:
                    lev.LogLevel = LogLevel.Information;
                    break;
                case LoggingLevel.Trace:
                    lev.LogLevel = LogLevel.Trace;
                    break;
                case LoggingLevel.Status:
                    lev.LogLevel = LogLevel.Information;
                    break;
                default:
                    lev.LogLevel = LogLevel.None;
                    break;
            }

            levOut = lev;
            return true;
        }
        #endregion
        #region B3c. MicroserviceConfigure()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceConfigure() 
        {
            Pipeline.AdjustPolicyTaskManagerForDebug();

            Pipeline.AdjustCommunicationPolicyForSingleListenerClient();

            Pipeline.Service.Events.StatisticsIssued += (object sender, StatisticsEventArgs e) => Context.Statistics.Load(e?.Statistics);

        }
        #endregion
        #region B3d. MicroserviceHostedServiceCreate()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceHostedServiceCreate()
        {
            HostedService = new MicroserviceHostedService(Pipeline);
        }
        #endregion
        #region B4. ConfigureSingletons(IServiceCollection services)
        /// <summary>
        /// Configures the singletons from the SingletonRegistrationAttributes
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureSingletons(IServiceCollection services)
        {
            Context.Directives.SingletonRegistrationsExtract()
                .ForEach((a) => services.AddSingleton(a.sType, a.service));
        }
        #endregion
        #region B5. ConfigureSecurityAuthentication(IServiceCollection services)
        /// <summary>
        /// Configures the authentication, i.e. services.AddJwtAuthentication(Context.SecurityJwt);
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureSecurityAuthentication(IServiceCollection services) 
        {
            if (Context.PipelineComponentTryGet<IAspNetPipelineSecurityAuthentication>(out var ext))
                ext.ConfigureSecurityAuthentication(services);
        }
        #endregion
        #region B6. ConfigureSecurityAuthorization(IServiceCollection services)
        /// <summary>
        /// Configures the authorization. i.e. services.AddAuthorization(options =>
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureSecurityAuthorization(IServiceCollection services)
        {
            if (Context.PipelineComponentTryGet<IAspNetPipelineSecurityAuthorization>(out var ext))
                ext.ConfigureSecurityAuthorization(services);
        }
        #endregion
        #region B7. ConfigureController(IServiceCollection services)
        /// <summary>
        /// Configures the add MVC service.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureController(IServiceCollection services)
        {
            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.BeforeController, (e, scope) => e.ConfigureServices(scope, services));

            if (Context.PipelineComponentTryGet<IAspNetPipelineController>(out var ext))
                ext.ConfigureControllerOptions(services);

            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.AfterController, (e, scope) => e.ConfigureServices(scope, services));
        }
        #endregion

        #region C=>Configure(IApplicationBuilder app)
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public virtual void Configure(IApplicationBuilder app)
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
            if (Context.PipelineComponentTryGet<IAspNetPipelineController>(out exte))
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
        /// This method connects the context to the logging infrastructure.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        protected virtual void ContextConnect(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            Context.Connect(loggerFactory);
        }
        #endregion
        #region C4. ConfigureSecurity(IApplicationBuilder app)
        /// <summary>
        /// Override this method to set authentication using app.UseAuthentication();
        /// </summary>
        /// <param name="app">The application.</param>
        protected virtual void ConfigureSecurity(IApplicationBuilder app) 
        {
            if (Context.PipelineComponentTryGet<IAspNetPipelineSecurityAuthentication>(out var exte))
                exte.ConfigureSecurityAuthentication(app);

            if (Context.PipelineComponentTryGet<IAspNetPipelineSecurityAuthorization>(out var exto))
                exto.ConfigureSecurityAuthorization(app);
        }
        #endregion
        #region C5. ConfigureUseEndpoints(IApplicationBuilder app
        /// <summary>
        /// Use this section to configure the routing
        /// </summary>
        /// <param name="app">The application builder.</param>
        protected virtual void ConfigureUseEndpoints(IApplicationBuilder app)
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
                , (e, scope) => e.ConfigureUseEndpoints(scope, app, endpoints));

            if (Context.PipelineComponentTryGet<IAspNetPipelineController>(out var ext))
                ext.ConfigureUseEndpoints(app, endpoints);

            PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope.AfterController
                , (e, scope) => e.ConfigureUseEndpoints(scope, app, endpoints));
        }
        #endregion

        #region LoggerFactory
        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        protected ILoggerFactory LoggerFactory { get; set; }
        /// <summary>
        /// This is the logger provider for the application.
        /// </summary>
        protected ApplicationLoggerProvider LoggerProvider { get; set; }
        #endregion
        #region Context
        /// <summary>
        /// Gets or sets the Api application context.
        /// </summary>
        public CTX Context { get; protected set; }
        #endregion
        #region Pipeline
        /// <summary>
        /// Gets the pipeline used to configure the Microservice.
        /// </summary>
        public MicroservicePipeline Pipeline { get; protected set; }
        #endregion
        #region Service
        /// <summary>
        /// Gets the Microservice ASP.NET Core hosted service.
        /// </summary>
        public MicroserviceHostedService HostedService { get; protected set; }
        #endregion

        #region PipelineExtensionsExecute ...
        /// <summary>
        /// This helper method filters the extensions that support the specific scope and calls the action.
        /// </summary>
        /// <param name="scope">The scope to filter on.</param>
        /// <param name="execute">The execute action.</param>
        protected void PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope scope, Action<IAspNetPipelineExtension, XigadeeAspNetPipelineExtensionScope> execute)
        {
            foreach (var extension in Context.PipelineExtensions.Where(p => p.Supported(scope)))
                execute(extension, scope);
        } 
        #endregion
    }
}
