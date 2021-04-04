using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by components that add in to the pipeline between the standard controller declarations.
    /// </summary>
    public interface IAspNetPipelineExtension: IAspNetPipelineComponent
    {
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
        /// <param name="scope">The scope to check.</param>
        /// <param name="app">The application builder.</param>
        void ConfigurePipeline(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app);

        /// <summary>
        /// This is the pipeline components.
        /// </summary>
        /// <param name="scope">The scope to check.</param>
        /// <param name="app">The application builder.</param>
        void ConfigureUseEndpoints(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app);
    }

    /// <summary>
    /// This is the pipeline application scope.
    /// </summary>
    [Flags]
    public enum XigadeeAspNetPipelineExtensionScope
    {
        /// <summary>
        /// Before the controller declarations
        /// </summary>
        BeforeController = 1,
        /// <summary>
        /// After the controller declarations.
        /// </summary>
        AfterController = 2
    }
}