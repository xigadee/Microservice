using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

            ContextCreate();

            ContextInitialize();

            if (Context.UseMicroservice)
            {
                MicroserviceCreate();

                MicroserviceConfigure();

                MicroserviceHostedServiceCreate();
            }
        }
        #endregion
        #region 1. ContextCreate()
        /// <summary>
        /// Initializes the context
        /// </summary>
        protected virtual void ContextCreate()
        {
            Context = new CTX();
        }
        #endregion
        #region 2. ContextInitialize() -> CXA ->
        /// <summary>
        /// Initializes the context
        /// </summary>
        protected virtual void ContextInitialize()
        {
            Context.Initialize(HostingEnvironment);
        }
        #endregion
        #region 3. MicroserviceCreate()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceCreate()
        {
            Pipeline = new MicroservicePipeline();
        }
        #endregion
        #region 4. MicroserviceConfigure()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceConfigure() { }
        #endregion
        #region 5. MicroserviceHostedServiceCreate()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceHostedServiceCreate()
        {
            HostedService = new MicroserviceHostedService(Pipeline);
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

            ConfigureSingletons(services);

            if (Context.UseMicroservice)
                ConfigureMicroserviceHostedService(services);

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
            //Add the microservice as a hosted service.
            if (HostedService != null)
                services.AddSingleton<IHostedService>(HostedService);
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
            //Add our default logger with the default configuration.
            //LoggerFactory.AddProvider(...

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
        }
        #endregion
        #region 6. ConfigureUseMvc(IApplicationBuilder app)
        /// <summary>
        /// Override this method to configure the UseMvc command, or to stop it being set.
        /// </summary>
        /// <param name="app">The application.</param>
        protected virtual void ConfigureUseMvc(IApplicationBuilder app)
        {
            app.UseMvc();
        }
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
    }

}
