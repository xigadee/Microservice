using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Xigadee
{
    /// <summary>
    /// This class is used by all services for the application.
    /// </summary>
    /// <typeparam name="CTX">The application context type.</typeparam>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-2.2" />
    public abstract class ApiMicroserviceStartupBase<CTX> : ApiStartupBase<CTX>
        where CTX : ApiMicroserviceStartUpContext, new()
    {
        #region A=>Constructor
        /// <summary>
        /// Initializes a new instance of the API application class.
        /// </summary>
        /// <param name="env">The environment.</param>
        protected ApiMicroserviceStartupBase(Microsoft.AspNetCore.Hosting.IHostingEnvironment env):base(env)
        {
            MicroserviceCreate();

            //Check in case we do not need a Microservice.
            if (Pipeline != null)
            {
                MicroserviceConfigure();

                MicroserviceHostedServiceCreate();
            }
        }
        #endregion
        #region 3. MicroserviceCreate()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceCreate()
        {
            Pipeline = new MicroservicePipeline();
        }
        #endregion
        #region 4. MicroserviceConfigure()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceConfigure() { }
        #endregion
        #region 5. MicroserviceHostedServiceCreate()
        /// <summary>
        /// Creates and configures the Xigadee microservice pipeline.
        /// </summary>
        protected virtual void MicroserviceHostedServiceCreate()
        {
            HostedService = new MicroserviceHostedService(Pipeline);
        }
        #endregion

        #region B=>ConfigureServices(IServiceCollection services)
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>Returns the new service provider.</returns>
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ConfigureOptions(services);

            ConfigureSingletons(services);

            ConfigureMicroserviceHostedService(services);

            ContextModulesCreate(services);

            ConfigureSecurityAuthentication(services);

            ConfigureSecurityAuthorization(services);

            ConfigureAddMvc(services);

            // Add framework services
            return services.BuildServiceProvider();
        }
        #endregion
        #region 2b. ConfigureMicroserviceHostedService(IServiceCollection services)
        /// <summary>
        /// Configures the singletons.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureMicroserviceHostedService(IServiceCollection services)
        {
            //Add the microservice as a hosted service.
            if (HostedService != null)
                services.AddSingleton<IHostedService>(HostedService);
        }
        #endregion
    }

    /// <summary>
    /// This is the default start up context.
    /// </summary>
    public class ApiMicroserviceStartUpContext : ApiStartUpContext
    {
        #region 2.Bind()
        /// <summary>
        /// Creates and binds specific configuration components required by the application.
        /// </summary>
        protected override void Bind()
        {
            base.Bind();

            ConfigMicroservice = new ConfigMicroservice();
            Configuration.Bind("ConfigMicroservice", ConfigMicroservice);
        }
        #endregion

        /// <summary>
        /// Gets or sets the microservice configuration.
        /// </summary>
        public ConfigMicroservice ConfigMicroservice { get; set; }

        //public MicroserviceId Id
    }
}
