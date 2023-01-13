﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the standard api controller configuration.
    /// </summary>
    public class XigadeeAspNetPipelineController : XigadeeAspNetPipelineComponentBase, 
        IAspNetPipelineController, IAspNetPipelineSupportsUseEndpoints
    {
        protected Type[] _types = null;

        public XigadeeAspNetPipelineController(params Type[] types)
        {
            _types = types;
        }

        public virtual void ConfigureControllerOptions(IServiceCollection services)
        {
            //Add the default API controller options.
            var mvcBuilder = services.AddControllers();

            _types?.ForEach(t => mvcBuilder.AddApplicationPart(t.Assembly));
        }

        public virtual void ConfigurePipeline(IApplicationBuilder app)
        {
            if (Host?.IsDevelopment()??false)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

        }

        public virtual void ConfigurePipelineComplete(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseAuthorization();
        }

        public virtual void ConfigureUseEndpoints(IApplicationBuilder app)
        {

        }

        public virtual void ConfigureUseEndpoints(IApplicationBuilder app, IEndpointRouteBuilder endpoints, XigadeeAspNetPipelineExtensionScope? scope = null)
        {
            endpoints.MapControllers();
        }
    }

    /// <summary>
    /// This class is used to record the application parts that require startup logic.
    /// </summary>
    public class ApplicationPartsLogger : IHostedService
    {
        private readonly ILogger<ApplicationPartsLogger> _logger;
        private readonly ApplicationPartManager _partManager;

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="logger">The root logger.</param>
        /// <param name="partManager">The MVC parts manager.</param>
        public ApplicationPartsLogger(ILogger<ApplicationPartsLogger> logger, ApplicationPartManager partManager)
        {
            _logger = logger;
            _partManager = partManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Get the names of all the application parts. This is the short assembly name for AssemblyParts
            var applicationParts = _partManager.ApplicationParts.Select(x => x.Name);

            // Create a controller feature, and populate it from the application parts
            var controllerFeature = new ControllerFeature();
            _partManager.PopulateFeature(controllerFeature);

            // Get the names of all of the controllers
            var controllers = controllerFeature.Controllers.Select(x => x.Name);

            // Log the application parts and controllers
            _logger.LogInformation("Found the following application parts: '{ApplicationParts}' with the following controllers: '{Controllers}'",
                string.Join(", ", applicationParts), string.Join(", ", controllers));

            return Task.CompletedTask;
        }

        // Required by the interface
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }



}