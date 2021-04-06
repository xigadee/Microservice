using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This extension is used to return the healthcheck string.
    /// </summary>
    public class HealthCheckExtension : XigadeeAspNetPipelineExtensionBase<ConfigHealthCheck>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public HealthCheckExtension(ConfigHealthCheck config):base(config)
        {
        } 
        #endregion

        #region Validate(string id)
        /// <summary>
        /// Validates the incoming Id.
        /// </summary>
        /// <param name="id">The incoming id.</param>
        /// <returns>Returns true if matched.</returns>
        public bool Validate(string id)
        {
            if (Config == null)
                return false;

            if (!Config.Enabled)
                return false;

            if (!Config.Id.HasValue && string.IsNullOrEmpty(id))
                return true;

            Guid value;
            if (!Guid.TryParse(id, out value))
                return false;

            return Config.Id.Value == value;
        }
        #endregion

        #region ConfigurePipeline(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app)
        /// <summary>
        /// This override registers the extension in the pipeline.
        /// </summary>
        /// <param name="scope">The pipeline.</param>
        /// <param name="app">The application builder.</param>
        public override void ConfigurePipeline(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app)
        {
            if (Config?.Enabled ?? false)
            {
                if (Config?.ShowStatistics ?? false)
                    app.Map($"/{Path}/statistics", (a) => a.Run(d => Process(d, HealthCheckOutputType.Statistics)));

                //This sets the path to direct incoming callback requests to the middle-ware.
                app.Map($"/{Path}", (a) => a.Run(d => Process(d, HealthCheckOutputType.Version)));

            }
        }
        #endregion

        #region Process(HttpContext context)
        /// <summary>
        /// Process an incoming health check request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="type">The HTTP context.</param>
        private async Task Process(HttpContext context, HealthCheckOutputType type)
        {
            try
            {
                context.Response.StatusCode = 404;
                if (!context.Request.QueryString.HasValue)
                    return;

                var query = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
                string id = query.Keys.FirstOrDefault(k => string.Equals("id", k, StringComparison.InvariantCultureIgnoreCase));

                if (id == null && !Validate(query[id]))
                    return;

                switch (type)
                {
                    case HealthCheckOutputType.Version:
                        await HealthCheckOutputWriteVersion(context);
                        return;
                    case HealthCheckOutputType.Statistics:
                        await HealthCheckOutputWriteStatistics(context);
                        return;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger?.LogWarning(ex, $"Health check failed.");
                context.Response.StatusCode = 500;
            }
        }
        #endregion

        #region HealthCheckOutputWriteVersion(HttpContext context)
        /// <summary>
        /// This method writes out the healthcheck string.
        /// </summary>
        /// <param name="context">The Http context.</param>
        /// <returns>Returns the https status.</returns>
        protected virtual async Task HealthCheckOutputWriteVersion(HttpContext context)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/markdown";
            await context.Response.WriteAsync(HealthCheckOutput, Encoding.UTF8);
        } 
        #endregion
        #region HealthCheckOutputWriteStatistics(HttpContext context)
        /// <summary>
        /// This method writes out the Microservice statistics.
        /// </summary>
        /// <param name="context">The Http context.</param>
        /// <returns>Returns the https status.</returns>
        protected virtual async Task HealthCheckOutputWriteStatistics(HttpContext context)
        {
            try
            {
                var stats = Host.StatisticsHolder;
                if (stats?.Statistics == null)
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                var json = JsonConvert.SerializeObject(stats);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error retrieving statistics.");
                context.Response.StatusCode = 500;
            }
        } 
        #endregion

        /// <summary>
        /// This is the default healthcheck path 'healthcheck'. You can override this if needed.
        /// </summary>
        private string Path => string.IsNullOrWhiteSpace(Config?.Path)?"healthcheck": (Config.Path.Trim().ToLowerInvariant());

        /// <summary>
        /// Gets the heartbeat output that is sent back to the polling client.
        /// </summary>
        protected virtual string HealthCheckOutput => $"{Host.ApplicationName} => {DateTime.UtcNow:s} @ {Host.InstanceId ?? Host.MicroserviceId?.ExternalServiceId}";

    }

    /// <summary>
    /// This is the type of healthcheck output
    /// </summary>
    public enum HealthCheckOutputType
    {
        /// <summary>
        /// The output is a version string.
        /// </summary>
        Version,
        /// <summary>
        /// The output is the current running statistics.
        /// </summary>
        Statistics
    }
}
