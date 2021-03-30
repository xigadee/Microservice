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
    public class ApiStartupBase<CTX> : ApiStartUpRootTemp<CTX, AspNetCore5HostingContainer>
        where CTX : ApiStartUpContext, new()
    {
        #region A=>Constructor
        /// <summary>
        /// Initializes a new instance of the API application class.
        /// </summary>
        /// <param name="env">The web host environment.</param>
        /// <param name="hEnv">The host environment</param>
        public ApiStartupBase(IWebHostEnvironment whEnv, IHostEnvironment hEnv, IConfiguration cfg) : base(new AspNetCore5HostingContainer(whEnv, hEnv, cfg))
        {
        }
        #endregion

        #region B=>ConfigureServices(IServiceCollection services)
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>Returns the new service provider.</returns>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            var rs = base.ConfigureServicesBase(services);
        }
        #endregion
    }
}