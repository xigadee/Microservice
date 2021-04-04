using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class provides a number of extension helpers.
    /// </summary>
    public static class StartUpExtensionHelper
    {
        /// <summary>
        /// This extension method is used to shortcut to the context for a standard Microservice.
        /// </summary>
        /// <typeparam name="CTX"></typeparam>
        /// <param name="hostBuilder">The web host builder.</param>
        /// <returns>Returns an connect the startup class.</returns>
        public static IWebHostBuilder UseStartupXigadee<CTX>(this IWebHostBuilder hostBuilder)
            where CTX : ApiStartUpContext, new()
        {
            return hostBuilder.UseStartup<ApiStartupBase<CTX>>();
        }
    }
}
