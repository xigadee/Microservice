using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Xigadee
{
    /// <summary>
    /// This class is used by all services for the application.
    /// </summary>
    /// <typeparam name="CTX">The application context type.</typeparam>
    public abstract class ApiStartupBase<CTX> : ApiStartUpRoot<CTX, AspNetCore5HostingContainer>
        where CTX : ApiStartUpContext, new()
    {
        #region A=>Constructor
        /// <summary>
        /// Initializes a new instance of the API application class.
        /// </summary>
        /// <param name="env">The web host environment.</param>
        /// <param name="hEnv">The host environment</param>
        protected ApiStartupBase(IWebHostEnvironment whEnv, IHostEnvironment hEnv, IConfiguration cfg) : base(new AspNetCore5HostingContainer(whEnv, hEnv, cfg))
        {
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
            services.AddMvc();
        }
        #endregion

    }
}