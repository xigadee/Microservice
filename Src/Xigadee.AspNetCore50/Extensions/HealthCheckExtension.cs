using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This extension is used to return the healthcheck string.
    /// </summary>
    public class HealthCheckExtension : XigadeeAspNetPipelineExtensionBase
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="id">The microservice id.</param>
        /// <param name="name">The optional application name.</param>
        public HealthCheckExtension(ConfigHealthCheck config, Guid id, string name)
        {
            ConfigHealthCheck = config;
            Id = id;
            Name = string.IsNullOrEmpty(name)? GetType().Assembly.GetName().Name : name;
        }

        /// <summary>
        /// This is the config health check settings.
        /// </summary>
        public virtual ConfigHealthCheck ConfigHealthCheck { get; }
        /// <summary>
        /// Specifies whether the extension is enabled.
        /// </summary>
        public override bool Enabled => ConfigHealthCheck?.Enabled ?? false;

        #region Validate(string id)
        /// <summary>
        /// Validates the incoming Id.
        /// </summary>
        /// <param name="settings">The health check settings.</param>
        /// <param name="id">The incoming id.</param>
        /// <returns>Returns true if matched.</returns>
        public bool Validate(string id)
        {
            if (ConfigHealthCheck == null)
                return false;

            if (!ConfigHealthCheck.Enabled)
                return false;

            if (!ConfigHealthCheck.Id.HasValue && string.IsNullOrEmpty(id))
                return true;

            Guid value;
            if (!Guid.TryParse(id, out value))
                return false;

            return ConfigHealthCheck.Id.Value == value;
        } 
        #endregion

        /// <summary>
        /// This override registers the extension in the pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public override void ConfigurePipeline(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app)
        {
            //This sets the path to direct incoming callback requests to the middle-ware.
            app.Map($"/{Path}", (a) => a.Run(d => Process(d)));
        }

        #region Process(HttpContext context)
        /// <summary>
        /// Process an incoming health check request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        private async Task Process(HttpContext context)
        {
            try
            {
                if (context.Request.QueryString.HasValue)
                {
                    var query = QueryHelpers.ParseQuery(context.Request.QueryString.Value);

                    string id = query.Keys.FirstOrDefault(k => string.Equals("id", k, StringComparison.InvariantCultureIgnoreCase));

                    if (id != null && Validate(query[id]))
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync(HealthCheckOutput);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.LogInformation(ex, $"Health check failed.");
            }

            context.Response.StatusCode = 404;
        }
        #endregion

        /// <summary>
        /// The microservice instance id.
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// The application name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// This is the default healthcheck path 'healthcheck'. You can override this if needed.
        /// </summary>
        private string Path { get; set; } = "healthcheck";

        /// <summary>
        /// Gets the heartbeat output that is sent back to the polling client.
        /// </summary>
        protected virtual string HealthCheckOutput => $"{Name} => {DateTime.UtcNow:s} @ {Id}";

    }
}
