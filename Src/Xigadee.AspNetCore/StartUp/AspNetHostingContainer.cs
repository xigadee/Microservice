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
    public class AspNetHostingContainer: HostingContainerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hEnv"></param>
        /// <param name="cfg"></param>
        public AspNetHostingContainer(Microsoft.AspNetCore.Hosting.IHostingEnvironment hEnv, IConfiguration cfg)
        {
            HostingEnvironment = hEnv;
            Configuration = cfg;
        }

        /// <summary>
        /// The underlying host.
        /// </summary>
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// The content root path.
        /// </summary>
        public override string ContentRootPath => HostingEnvironment?.ContentRootPath;
        /// <summary>
        /// The current environment name.
        /// </summary>
        public override string EnvironmentName => HostingEnvironment?.EnvironmentName;
        /// <summary>
        /// This is the application name.
        /// </summary>
        public override string ApplicationName => HostingEnvironment?.ApplicationName;
    }
}
