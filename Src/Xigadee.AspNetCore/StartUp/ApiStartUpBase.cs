using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Xigadee
{
    /// <summary>
    /// This class is used by all services for the application.
    /// </summary>
    /// <typeparam name="CTX">The application context type.</typeparam>
    /// <typeparam name="MODSEC">The user security module type.</typeparam>
    /// <typeparam name="CONATEN">The authentication configuration type..</typeparam>
    /// <typeparam name="CONATHZ">The authorization configuration type.</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Hosting.IStartup" />
    public abstract class ApiStartupBase<CTX, MODSEC, CONATEN, CONATHZ> : IStartup
        where CTX : class, IApiMicroservice<MODSEC, CONATEN, CONATHZ>, new()
        where MODSEC : IApiUserSecurityModule
        where CONATEN : ConfigAuthentication, new()
        where CONATHZ : ConfigAuthorization, new()
    {
        /// <summary>
        /// Initializes a new instance of the API application class.
        /// </summary>
        /// <param name="env">The environment.</param>
        protected ApiStartupBase(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            Context = new CTX();
            Context.Initialize(env);

            CreateMicroservicePipeline();

            ConfigureMicroservicePipeline(Pipeline);

            Service = new MicroserviceHostedService(Pipeline);
        }

        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        protected ILoggerFactory LoggerFactory { get; set; }

        #region Configure(IApplicationBuilder app)
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public virtual void Configure(IApplicationBuilder app)
        {
            LoggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();

            app.Use(async (context, next) =>
            {
                if (!string.IsNullOrEmpty(Context?.Identity?.ServiceVersionId))
                    context.Response.Headers.Append("x-api-ver", Context.Identity.ServiceVersionId);

                if (!string.IsNullOrEmpty(Activity.Current?.RootId))
                    context.Response.Headers.Append("x-api-cid", Activity.Current.RootId);

                await next();
            });

            ConfigureLogging(app);

            Context.ConnectModules(LoggerFactory);
        } 
        #endregion

        /// <summary>
        /// Configures the logging provide for the application.
        /// </summary>
        /// <param name="app">The application.</param>
        protected virtual void ConfigureLogging(IApplicationBuilder app)
        {
        }

        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void CreateMicroservicePipeline()
        {
            Pipeline = new MicroservicePipeline();
        }

        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected abstract void ConfigureMicroservicePipeline(MicroservicePipeline pipeline);

        #region ConfigureServices(IServiceCollection services)
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>Returns the new service provider.</returns>
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add the heartbeat configuration.
            services.AddSingleton(Context.CertificateModule);

            services.AddSingleton(Context.Identity);

            //Add the microservice as a hosted service.
            services.AddSingleton<IHostedService>(Service);

            // Add framework services
            return services.BuildServiceProvider();
        } 
        #endregion

        /// <summary>
        /// Gets or sets the API application context.
        /// </summary>
        public CTX Context { get; }

        /// <summary>
        /// Gets the pipeline used to configure the Microservice.
        /// </summary>
        public MicroservicePipeline Pipeline { get; protected set; }
        /// <summary>
        /// Gets the Microservice ASP.NET Core hosted service.
        /// </summary>
        public MicroserviceHostedService Service { get; protected set; }
    }
}
