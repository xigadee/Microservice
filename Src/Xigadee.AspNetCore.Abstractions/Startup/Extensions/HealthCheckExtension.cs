﻿using Microsoft.AspNetCore.Builder;
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
    public class HealthCheckExtension : XigadeeAspNetPipelineExtensionBase
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="id">The microservice id.</param>
        /// <param name="name">The optional application name.</param>
        /// <param name="statistics">This is the function to retrieve the latest statistics from the Microservice.</param>
        public HealthCheckExtension(ConfigHealthCheck config, Guid id, string name, Func<MicroserviceStatistics> statistics = null)
        {
            ConfigHealthCheck = config;
            Id = id;
            Name = string.IsNullOrEmpty(name) ? GetType().Assembly.GetName().Name : name;
            Statistics = statistics;
        } 
        #endregion

        /// <summary>
        /// This function can be used to retrieve the latest statistics from the Microservice.
        /// </summary>
        public virtual Func<MicroserviceStatistics> Statistics { get; set; }
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

        #region ConfigurePipeline(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app)
        /// <summary>
        /// This override registers the extension in the pipeline.
        /// </summary>
        /// <param name="scope">The pipeline.</param>
        /// <param name="app">The application builder.</param>
        public override void ConfigurePipeline(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app)
        {
            if (ConfigHealthCheck?.Enabled ?? false)
            {
                //This sets the path to direct incoming callback requests to the middle-ware.
                app.Map($"/{Path}", (a) => a.Run(d => Process(d, HealthCheckOutputType.Version)));

                if (ConfigHealthCheck?.ShowStatistics ?? false)
                    app.Map($"/{Path}/Statistics", (a) => a.Run(d => Process(d, HealthCheckOutputType.Statistics)));
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
            int statusOut = 404;
            try
            {
                if (!context.Request.QueryString.HasValue)
                    return;

                var query = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
                string id = query.Keys.FirstOrDefault(k => string.Equals("id", k, StringComparison.InvariantCultureIgnoreCase));

                if (id == null && !Validate(query[id]))
                    return;

                switch (type)
                {
                    case HealthCheckOutputType.Version:
                        statusOut = await HealthCheckOutputWriteVersion(context);
                        break;
                    case HealthCheckOutputType.Statistics:
                        statusOut = await HealthCheckOutputWriteStatistics(context);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger?.LogInformation(ex, $"Health check failed.");
            }
            finally
            {
                context.Response.StatusCode = statusOut;
            }

        }
        #endregion

        #region HealthCheckOutputWriteVersion(HttpContext context)
        /// <summary>
        /// This method writes out the healthcheck string.
        /// </summary>
        /// <param name="context">The Http context.</param>
        /// <returns>Returns the https status.</returns>
        protected virtual async Task<int> HealthCheckOutputWriteVersion(HttpContext context)
        {
            await context.Response.WriteAsync(HealthCheckOutput);
            return 200;
        } 
        #endregion
        #region HealthCheckOutputWriteStatistics(HttpContext context)
        /// <summary>
        /// This method writes out the Microservice statistics.
        /// </summary>
        /// <param name="context">The Http context.</param>
        /// <returns>Returns the https status.</returns>
        protected virtual async Task<int> HealthCheckOutputWriteStatistics(HttpContext context)
        {
            var stats = Statistics?.Invoke();
            if (stats == null)
                return 404;

            var json = JsonConvert.SerializeObject(stats);

            await context.Response.WriteAsync(json, Encoding.UTF8);
            return 200;
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
        private string Path => string.IsNullOrWhiteSpace(ConfigHealthCheck?.Path)?"healthcheck": (ConfigHealthCheck.Path.Trim().ToLowerInvariant());

        /// <summary>
        /// Gets the heartbeat output that is sent back to the polling client.
        /// </summary>
        protected virtual string HealthCheckOutput => $"{Name} => {DateTime.UtcNow:s} @ {Id}";

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
