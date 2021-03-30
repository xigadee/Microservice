using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by components that add in to the pipeline between the standard controller declarations.
    /// </summary>
    public interface IXigadeeAspNetPipelineExtension
    {
        /// <summary>
        /// The system logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Specifies whether it is enabled and should be registered.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// This is the scope that determines whether the call is made before or after the controller call.
        /// </summary>
        XigadeeAspNetPipelineExtensionScope Scope { get; }

        /// <summary>
        /// Specifies 
        /// </summary>
        /// <param name="scope">The scope to check.</param>
        /// <returns>Returns true if supported.</returns>
        bool Supported(XigadeeAspNetPipelineExtensionScope scope);

        /// <summary>
        /// Configures any services.
        /// </summary>
        /// <param name="scope">The execution scope.</param>
        /// <param name="services">The setrvice collection.</param>
        void ConfigureServices(XigadeeAspNetPipelineExtensionScope scope, IServiceCollection services);

        /// <summary>
        /// This method is called through the main pipeline process.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="app"></param>
        void ConfigurePipeline(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app);

        /// <summary>
        /// This is the pipeline components.
        /// </summary>
        /// <param name="app"></param>
        void ConfigureUseEndpoints(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app);

        /// <summary>
        /// Configures the pipeline interaction.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="endpoints">The endpoints.</param>
        void ConfigureUseEndpoints(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app, IEndpointRouteBuilder endpoints);
    }

    [Flags]
    public enum XigadeeAspNetPipelineExtensionScope
    {
        BeforeController = 1,
        AfterController = 2
    }
}