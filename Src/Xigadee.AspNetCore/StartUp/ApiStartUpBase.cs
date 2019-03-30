using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Xigadee
{
    /// <summary>
    /// This class is used by all services for the application.
    /// </summary>
    /// <typeparam name="CTX">The application context type.</typeparam>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-2.2" />
    public abstract class ApiStartupBase<CTX> : IStartup
        where CTX : class, IApiMicroservice, new()
    {
        #region --> Constructor
        /// <summary>
        /// Initializes a new instance of the API application class.
        /// </summary>
        /// <param name="env">The environment.</param>
        protected ApiStartupBase(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            HostingEnvironment = env;

            ContextCreate();

            ContextInitialize();

            MicroserviceCreate();

            MicroserviceConfigure();

            MicroserviceHostedServiceCreate();
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
        #region 2. ContextInitialize()
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
        protected abstract void MicroserviceConfigure();
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

        #region --> Configure(IApplicationBuilder app)
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
        }
        #endregion
        #region 3. ContextConnect(IApplicationBuilder app, ILoggerFactory loggerFactory)
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
        #region 4. ConfigureUseMvc(IApplicationBuilder app)
        /// <summary>
        /// Override this method to configure the UseMvc command, or to stop it being set.
        /// </summary>
        /// <param name="app">The application.</param>
        protected virtual void ConfigureUseMvc(IApplicationBuilder app)
        {
            app.UseMvc();
        }
        #endregion

        #region --> ConfigureServices(IServiceCollection services)
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>Returns the new service provider.</returns>
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ConfigureSingletons(services);

            //Add the microservice as a hosted service.
            services.AddSingleton<IHostedService>(HostedService);

            ConfigureAddMvc(services);

            // Add framework services
            return services.BuildServiceProvider();
        }
        #endregion

        protected virtual void ConfigureSingletons(IServiceCollection services)
        {
            //// Add the heartbeat configuration.
            //services.AddSingleton(Context.CertificateModule);

            //services.AddSingleton(Context.Identity);
        }



        protected virtual void ConfigureAddMvc(IServiceCollection services)
        {
            //services.AddMvcCore();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_0);
        }

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
