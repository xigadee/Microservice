using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
    public abstract class ApiStartupBase<CTX> : ApiStartUpRoot<CTX>
        where CTX : ApiStartUpContext, new()
    {
        #region A=>Constructor
        /// <summary>
        /// Initializes a new instance of the API application class.
        /// </summary>
        /// <param name="env">The environment.</param>
        protected ApiStartupBase(IWebHostEnvironment whEnv, IHostEnvironment hEnv, IConfiguration cfg) 
        {
            //            IWebHostEnvironment
            //            IHostEnvironment
            //IConfiguration

            WebHostEnvironment = whEnv;
            HostEnvironment = hEnv;
            Configuration = cfg;


            //HostingEnvironment = env;

            Initialize();
        }
        #endregion

        #region 3. ContextInitialize() -> CXA ->
        /// <summary>
        /// Initializes the context
        /// </summary>
        protected override void ContextInitialize()
        {
            Context.Initialize(WebHostEnvironment, HostEnvironment, Configuration);
        }
        #endregion

        #region B7. ConfigureAddMvc(IServiceCollection services)
        /// <summary>
        /// Configures the add MVC service.
        /// </summary>
        /// <param name="services">The services.</param>
        protected override void ConfigureAddMvc(IServiceCollection services)
        {
            //services.AddMvcCore();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }
        #endregion

        private IWebHostEnvironment WebHostEnvironment { get; }

        private IHostEnvironment HostEnvironment { get; }

        private IConfiguration Configuration { get; }



    }
}
