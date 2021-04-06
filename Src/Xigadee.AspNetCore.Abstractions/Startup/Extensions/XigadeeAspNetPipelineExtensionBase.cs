using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Xigadee
{
    /// <summary>
    /// This is the base pipeline extension.
    /// </summary>
    /// <typeparam name="CFG">The generic configuration type.</typeparam>
    public abstract class XigadeeAspNetPipelineExtensionBase<CFG> : XigadeeAspNetPipelineExtensionBase
        where CFG : ConfigPipelineBase
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="config"></param>
        protected XigadeeAspNetPipelineExtensionBase(CFG config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }
        /// <summary>
        /// This is the default configuration for the extension.
        /// </summary>
        protected virtual CFG Config { get; }
        /// <summary>
        /// This is the enabled flag that is inherited from the configuration.
        /// </summary>
        public override bool Enabled => Config.Enabled;
    }

    /// <summary>
    /// This base class can be used to create a pipeline component extension.
    /// </summary>
    public abstract class XigadeeAspNetPipelineExtensionBase : XigadeeAspNetPipelineComponentBase
        , IAspNetPipelineExtension
    {
        /// <summary>
        /// Specifies whether the extension is enabled.
        /// </summary>
        public abstract bool Enabled { get; }
        /// <summary>
        /// Specifies that the scope required is enabled.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>Returns true if enabled.</returns>
        public bool Supported(XigadeeAspNetPipelineExtensionScope scope) => Enabled && ((Scope & scope) > 0);
        /// <summary>
        /// The defined scope.
        /// </summary>
        public virtual XigadeeAspNetPipelineExtensionScope Scope { get; protected set; } = XigadeeAspNetPipelineExtensionScope.AfterController;

        /// <summary>
        /// This method can be used to configure and service.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="services"></param>
        public virtual void ConfigureServices(XigadeeAspNetPipelineExtensionScope scope, IServiceCollection services)
        {
        }
        /// <summary>
        /// This method can be used to configure the pipeline.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="app">The application builder.</param>
        public virtual void ConfigurePipeline(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app)
        {
        }
        /// <summary>
        /// This method can be used to configure generic endpoints.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="app">The application builder.</param>
        public virtual void ConfigureUseEndpoints(XigadeeAspNetPipelineExtensionScope scope, IApplicationBuilder app)
        {
        }
        
    }
}
