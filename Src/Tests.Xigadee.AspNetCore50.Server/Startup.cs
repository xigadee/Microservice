using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee.AspNetCore50.Server
{
    public class Startup : ApiStartUpRoot<StartupContext>
    {
        protected override void ConfigureSecurity(IApplicationBuilder app)
        {
            throw new NotImplementedException();
        }

        protected override void ConfigureSecurityAuthentication(IServiceCollection services)
        {
            throw new NotImplementedException();
        }

        protected override void ConfigureSecurityAuthorization(IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}
