using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Xigadee
{
    //Configuration
    public abstract partial class ApiStartUpContextRoot<HE>
    {
        #region B=>ConfigureServices/ConfigureServicesRoot(IServiceCollection services)
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>Returns the new service provider.</returns>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigureOptions(services);

            ModulesCreate(services);

            ModulesLoad(services);

            MicroserviceConfigure(services);

            ConfigureSingletons(services);

            ConfigureSecurityAuthentication(services);

            ConfigureSecurityAuthorization(services);

            ConfigureController(services);
        }
        #endregion

        #region B1. ConfigureOptions(IServiceCollection services)
        /// <summary>
        /// Configures service options.
        /// </summary>
        /// <param name="services">The services.</param>
        public virtual void ConfigureOptions(IServiceCollection services)
        {

        }
        #endregion

        #region B4. ConfigureSingletons(IServiceCollection services)
        /// <summary>
        /// Configures the singletons from the SingletonRegistrationAttributes
        /// </summary>
        /// <param name="services">The services.</param>
        public virtual void ConfigureSingletons(IServiceCollection services)
        {
            //Add the context so that it can inform modules of the application start up.
            services.AddSingleton<IHostedService>(this);

            AttributeSingletonRegistrationsExtract()
                .ForEach((a) => services.AddSingleton(a.sType, a.service));
        }
        #endregion
        #region B5. ConfigureSecurityAuthentication(IServiceCollection services)
        /// <summary>
        /// Configures the authentication, i.e. services.AddJwtAuthentication(Context.SecurityJwt);
        /// </summary>
        /// <param name="services">The services.</param>
        public virtual void ConfigureSecurityAuthentication(IServiceCollection services)
        {
            if (PipelineComponentTryGet<IAspNetPipelineSecurityAuthentication>(out var ext))
                ext.ConfigureSecurityAuthentication(services);
        }
        #endregion
        #region B6. ConfigureSecurityAuthorization(IServiceCollection services)
        /// <summary>
        /// Configures the authorization. i.e. services.AddAuthorization(options =>
        /// </summary>
        /// <param name="services">The services.</param>
        public virtual void ConfigureSecurityAuthorization(IServiceCollection services)
        {
            if (PipelineComponentTryGet<IAspNetPipelineSecurityAuthorization>(out var ext))
                ext.ConfigureSecurityAuthorization(services);
        }
        #endregion
        #region B7. ConfigureController(IServiceCollection services)
        /// <summary>
        /// Configures the add MVC service.
        /// </summary>
        /// <param name="services">The services.</param>
        protected abstract void ConfigureController(IServiceCollection services);
        #endregion

        #region PipelineExtensionsExecute ...
        /// <summary>
        /// This helper method filters the extensions that support the specific scope and calls the action.
        /// </summary>
        /// <param name="scope">The scope to filter on.</param>
        /// <param name="execute">The execute action.</param>
        public virtual void PipelineExtensionsExecute(XigadeeAspNetPipelineExtensionScope scope, Action<IAspNetPipelineExtension, XigadeeAspNetPipelineExtensionScope> execute)
        {
            foreach (var extension in PipelineExtensions.Where(p => p.Supported(scope)))
                execute(extension, scope);
        }
        #endregion
    }
}