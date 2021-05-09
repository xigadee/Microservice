using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Xigadee;

namespace Tests.Xigadee.AspNetCore50
{
    /// <summary>
    /// This is the default startup class. This is needed as ASP.NET picks up the resolution of views and suck-like from the calling assembly.
    /// </summary>
    public class Startup : ApiStartupBase<StartupContext>
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="whEnv">The web host environment.</param>
        /// <param name="hEnv">The host environment.</param>
        /// <param name="cfg">The configuration.</param>
        public Startup(IWebHostEnvironment whEnv, IHostEnvironment hEnv, IConfiguration cfg) : base(whEnv, hEnv, cfg)
        {

        }

        #region MicroserviceConfigure()
        /// <summary>
        /// This method configures the Microservice for the application.
        /// </summary>
        protected override void MicroserviceConfigure()
        {
            base.MicroserviceConfigure();
        }
        #endregion



    }
}
