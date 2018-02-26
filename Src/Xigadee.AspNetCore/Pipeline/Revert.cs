using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Xigadee
{
    public static partial class AspNetCoreExtensionMethods
    {
        /// <summary>
        /// Reverts the specified AspNetCore pipeline to the application..
        /// </summary>
        /// <param name="cpipe">The incoming pipeline.</param>
        /// <returns>The application</returns>
        public static IApplicationBuilder Revert(this IPipelineAspNetCore cpipe)
        {
            return cpipe.App;
        }
    }
}
