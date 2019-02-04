using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Xigadee
{
    /// <summary>
    /// This class is used by all services for the application.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    /// <typeparam name="TModule">The user security module</typeparam>
    /// <seealso cref="Microsoft.AspNetCore.Hosting.IStartup" />
    public abstract class StartupBase<TContext, TModule> : IStartup
        where TContext : class, IMicroserviceContext<TModule>, new()
        where TModule : IUserSecurityModule
    {
        protected StartupBase(IHostingEnvironment env)
        {
            Context = new TContext();
            Context.Initialize(env);
        }

        protected ILoggerFactory LoggerFactory { get; set; }

        public virtual void Configure(IApplicationBuilder app)
        {
            LoggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();

            //app.Use(async (context, next) =>
            //{
            //    if (Context?.Identity?.Version != null)
            //        context.Response.Headers.Append("x-apv", Context.Identity.Version);

            //    if (!string.IsNullOrEmpty(Activity.Current?.RootId))
            //        context.Response.Headers.Append("x-correlation-Id", Activity.Current.RootId);

            //    await next();
            //});

            //ConfigureLogging(app);
            //Context.MetricLogger = new ApplicationInsightsMetricLogger(Context.ConfigurationAiTelemetry, Context.Identity);

            //Context.ConnectModules(LoggerFactory);

            //if (Context.ConfigurationHeartbeat?.Enabled ?? false)
            //{
            //    //This sets the path to direct incoming callback requests to the middle-ware.
            //    app.Map("/heartbeat", (a) => a.Run(d => Heartbeat(d)));
            //}
        }


        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>Returns the new service provider.</returns>
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add the heartbeat configuration.
            //services.AddSingleton(Context.ConfigurationHeartbeat);

            //services.AddSingleton(Context.CertificateModule);

            //services.AddSingleton(Context.Identity);

            //// App Insights config
            //if (Context.ConfigurationMicroserviceLogging.ApplicationInsights.Enabled)
            //{
            //    // Disable the automatic adding of the quick pulse stream as we will add it after custom processors
            //    // That filter out some of the telemetry noise i.e. frequent calls to DB / Table storage etc.
            //    var aiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
            //    {
            //        EnableQuickPulseMetricStream = false,
            //        EnableAdaptiveSampling = false
            //    };
            //    services.AddApplicationInsightsTelemetry(aiOptions);
            //}

            // Add framework services
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Gets or sets the web application context.
        /// </summary>
        public TContext Context { get; }

    }
}
