using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Xigadee
{
    /// <summary>
    /// This class is used by all services for the application.
    /// </summary>
    /// <typeparam name="CTX">The application context type.</typeparam>
    public abstract class ApiStartupBase<CTX> : IStartup
        where CTX : ApiStartUpContext, new()
    {
        #region A=>Constructor
        /// <summary>
        /// Initializes a new instance of the API application class.
        /// </summary>
        /// <param name="env">The environment.</param>
        protected ApiStartupBase(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            HostingEnvironment = env;

            LoggerProviderCreate();

            ContextCreate();

            ContextInitialize();
        }
        #endregion
        #region 1. LoggerProviderCreate()
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
        #region 2. ContextCreate()
        /// <summary>
        /// Initializes the context
        /// </summary>
        protected virtual void ContextCreate()
        {
            Context = new CTX();
        }
        #endregion
        #region 3. ContextInitialize() -> CXA ->
        /// <summary>
        /// Initializes the context
        /// </summary>
        protected virtual void ContextInitialize()
        {
            Context.Initialize(HostingEnvironment);
        }
        #endregion

        #region B=>ConfigureServices(IServiceCollection services)
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>Returns the new service provider.</returns>
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ConfigureOptions(services);

            ContextModulesCreate(services);

            if (Context.UseMicroservice)
                ConfigureMicroserviceHostedService(services);

            ConfigureSingletons(services);

            ConfigureSecurityAuthentication(services);

            ConfigureSecurityAuthorization(services);

            ConfigureAddMvc(services);

            // Add framework services
            return services.BuildServiceProvider();
        }
        #endregion
        #region 1. ConfigureOptions(IServiceCollection services)
        /// <summary>
        /// Configures service options.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureOptions(IServiceCollection services)
        {

        }
        #endregion
        #region 2. ContextModulesCreate(IServiceCollection services) -> CXB ->
        /// <summary>
        /// Calls the context to create and register any modules and services respectively.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ContextModulesCreate(IServiceCollection services)
        {
            Context.ModulesCreate(services);
        }
        #endregion
        #region 3. ConfigureMicroserviceHostedService(IServiceCollection services)
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
        #region 3a. MicroserviceCreate()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceCreate()
        {
            Pipeline = new MicroservicePipeline();

        }
        #endregion
        #region 3b. MicroserviceDataCollectionConnect()
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
        protected virtual void MicroserviceOnDataCollection_Telemetry(OnDataCollectionContext ctx, EventHolder ev){}

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
        #region 3c. MicroserviceConfigure()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceConfigure() { }
        #endregion
        #region 3d. MicroserviceHostedServiceCreate()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceHostedServiceCreate()
        {
            HostedService = new MicroserviceHostedService(Pipeline);
        }
        #endregion
        #region 4. ConfigureSingletons(IServiceCollection services)
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
        #region 5. ConfigureSecurityAuthentication(IServiceCollection services)
        /// <summary>
        /// Configures the authentication, i.e. services.AddJwtAuthentication(Context.SecurityJwt);
        /// </summary>
        /// <param name="services">The services.</param>
        protected abstract void ConfigureSecurityAuthentication(IServiceCollection services);
        #endregion
        #region 6. ConfigureSecurityAuthorization(IServiceCollection services)
        /// <summary>
        /// Configures the authorization. i.e. services.AddAuthorization(options =>
        /// </summary>
        /// <param name="services">The services.</param>
        protected abstract void ConfigureSecurityAuthorization(IServiceCollection services);
        #endregion
        #region 7. ConfigureAddMvc(IServiceCollection services)
        /// <summary>
        /// Configures the add MVC service.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureAddMvc(IServiceCollection services)
        {
            //services.AddMvcCore();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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

            ConfigureCustomRouting(app);

            ConfigureUseMvc(app);
        }
        #endregion
        #region 1. ConfigurePipeline(IApplicationBuilder app)
        /// <summary>
        /// Configures the ASP.NET pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        protected virtual void ConfigurePipeline(IApplicationBuilder app)
        {
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Append("x-oh-fuck", "22");
            //    //if (!string.IsNullOrEmpty(Context?.Identity?.ServiceVersionId))
            //    //    context.Response.Headers.Append("x-api-ver", Context.Identity.ServiceVersionId);

            //    //if (!string.IsNullOrEmpty(Activity.Current?.RootId))
            //    //    context.Response.Headers.Append("x-api-cid", Activity.Current.RootId);

            //    await next();
            //});
        }
        #endregion
        #region 2. ConfigureLogging(IApplicationBuilder app)
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
        #region 2a. ConfigureLoggingSubscribers(IApplicationBuilder app, ApplicationLoggerProvider provider)
        /// <summary>
        /// Use this override to add your specific logging subscribers to published events.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="provider">The logger provider.</param>
        protected virtual void ConfigureLoggingSubscribers(IApplicationBuilder app, ApplicationLoggerProvider provider)
        {
            
        }
        #endregion
        #region 3. ContextConnect(IApplicationBuilder app, ILoggerFactory loggerFactory) -> CXC ->
        /// <summary>
        /// Override this method to configure the UseMvc command, or to stop it being set.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        protected virtual void ContextConnect(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            Context.Connect(loggerFactory);
        }
        #endregion
        #region 4. ConfigureSecurity(IApplicationBuilder app)
        /// <summary>
        /// Override this method to set authentication using app.UseAuthentication();
        /// </summary>
        /// <param name="app">The application.</param>
        protected abstract void ConfigureSecurity(IApplicationBuilder app);
        #endregion
        #region 5. ConfigureCustomRouting(IApplicationBuilder app)
        /// <summary>
        /// Override this method to configure the UseMvc command, or to stop it being set.
        /// </summary>
        /// <param name="app">The application.</param>
        protected virtual void ConfigureCustomRouting(IApplicationBuilder app)
        {
            if (Context.ConfigHealthCheck?.Enabled ?? false)
            {
                //This sets the path to direct incoming callback requests to the middle-ware.
                app.Map($"/{HealthCheckPath}", (a) => a.Run(d => HealthCheck(d)));
            }
        }
        #endregion
        #region 6. ConfigureUseMvc(IApplicationBuilder app)
        /// <summary>
        /// Override this method to configure the UseMvc command, or to stop it being set.
        /// </summary>
        /// <param name="app">The application.</param>
        protected virtual void ConfigureUseMvc(IApplicationBuilder app) => app.UseMvc();
        #endregion

        #region HostingEnvironment
        /// <summary>
        /// Gets the AspNet Core hosting environment.
        /// </summary>
        Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment { get; }
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
        /// Gets or sets the API application context.
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

        #region HealthCheck(HttpContext context)
        /// <summary>
        /// Process an incoming health check request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        protected virtual async Task HealthCheck(HttpContext context)
        {
            try
            {
                if (context.Request.QueryString.HasValue)
                {
                    var query = QueryHelpers.ParseQuery(context.Request.QueryString.Value);

                    string id = query.Keys.FirstOrDefault(k => string.Equals("id", k, StringComparison.InvariantCultureIgnoreCase));

                    if (id != null && Context.ConfigHealthCheck.Validate(query[id]))
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync(HealthCheckOutput);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Context?.Logger.LogInformation(ex, $"Health check failed.");
            }

            context.Response.StatusCode = 404;
        }

        /// <summary>
        /// This is the default healthcheck path 'healthcheck'. You can override this if needed.
        /// </summary>
        protected string HealthCheckPath { get; set; } = "healthcheck";
        /// <summary>
        /// Gets the heartbeat output that is sent back to the polling client.
        /// </summary>
        protected virtual string HealthCheckOutput => $"{Context.ConfigApplication?.Name} => {DateTime.UtcNow:s} @ {Context.Id}";
        #endregion
    }
}
