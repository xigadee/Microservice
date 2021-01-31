using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xigadee;

namespace Tests.Xigadee.AspNetCore50.Server
{
    public class Startup : JwtApiMicroserviceStartupBase<StartupContext>
    {
        public Startup(IWebHostEnvironment whEnv, IHostEnvironment hEnv, IConfiguration cfg) : base(whEnv, hEnv, cfg)
        {
        }

        protected override void ConfigureSecurityAuthorization(IServiceCollection services)
        {
            var policy = new AuthorizationPolicyBuilder()
                //.AddAuthenticationSchemes("Cookie, Bearer")
                .RequireAuthenticatedUser()
                .RequireRole("paul")
                //.RequireAssertion(ctx =>
                //{
                //    return ctx.User.HasClaim("editor", "contents") ||
                //            ctx.User.HasClaim("level", "senior");
                //}
                //)
                .Build();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("adminp", policy);
                //policy =>
                //{
                //    policy.RequireAuthenticatedUser();
                //    policy.RequireRole("admin");
                //});

            })
            ;
        }
    }
}
