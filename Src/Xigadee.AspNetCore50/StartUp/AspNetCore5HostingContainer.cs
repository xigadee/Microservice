using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the hosting container.
    /// </summary>
    public class AspNetCore5HostingContainer: HostingContainerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="whEnv"></param>
        /// <param name="hEnv"></param>
        /// <param name="cfg"></param>
        public AspNetCore5HostingContainer(IWebHostEnvironment whEnv, IHostEnvironment hEnv, IConfiguration cfg)
        {
            WebHostEnvironment = whEnv;
            HostEnvironment = hEnv;
            Configuration = cfg;
        }

        /// <summary>
        /// The web host environment.
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; }
        /// <summary>
        /// The underlying host.
        /// </summary>
        public IHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// The content root path.
        /// </summary>
        public override string ContentRootPath => HostEnvironment?.ContentRootPath;
        /// <summary>
        /// The current environment name.
        /// </summary>
        public override string EnvironmentName => HostEnvironment?.EnvironmentName;
    }
}
