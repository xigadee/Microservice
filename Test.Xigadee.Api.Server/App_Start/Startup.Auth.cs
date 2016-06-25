using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Test.Xigadee.Api.Server.Providers;
using Test.Xigadee.Api.Server.Models;

namespace Test.Xigadee.Api.Server
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            //app.UseWindowsAzureActiveDirectoryBearerAuthentication(
            //    new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            //    {
            //        Tenant = ConfigurationManager.AppSettings["ida:Tenant"],
            //        TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidAudience = ConfigurationManager.AppSettings["ida:Audience"]
            //        },
            //    });
        }
    }
}
