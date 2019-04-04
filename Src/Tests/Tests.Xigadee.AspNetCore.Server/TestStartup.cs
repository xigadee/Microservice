using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Test.Xigadee;
using Xigadee;

namespace Tests.Xigadee
{
    /// <summary>
    /// This is the base application class.
    /// </summary>
    /// <seealso cref="Xigadee.ApiMicroserviceStartupBase{Tests.Xigadee.TestStartupContext}" />
    public class TestStartup : JwtApiMicroserviceStartupBase<TestStartupContext>
    {
        PersistenceClient<Guid, MondayMorningBlues> _mmbClient;

        public TestStartup(IHostingEnvironment env) : base(env)
        {
        }

        protected override void MicroserviceConfigure()
        {
            Pipeline
                .AdjustPolicyTaskManagerForDebug()
                .AddChannelIncoming("testin")
                    .AttachPersistenceManagerHandlerMemory(
                        (MondayMorningBlues e) => e.Id
                        , s => new Guid(s)
                        , versionPolicy: ((e) => $"{e.VersionId:N}", (e) => e.VersionId = Guid.NewGuid(), true)
                        , propertiesMaker: (e) => e.ToReferences2()
                        , searches: new[] { new RepositoryMemorySearch<Guid, MondayMorningBlues>("default") }
                        , searchIdDefault: "default")
                    .AttachPersistenceClient(out _mmbClient)
                    .Revert()                
                ;

            Pipeline.Service.Events.StartCompleted += Service_StartCompleted; 
        }

        private void Service_StartCompleted(object sender, StartEventArgs e)
        {
            var rs = _mmbClient.Create(new MondayMorningBlues() { Id = new Guid("9A2E3F6D-3B98-4C2C-BD45-74F819B5EDFC") }).Result;
        }

        /// <summary>
        /// Adds the MondayMorningBlues test repo to the collection..
        /// </summary>
        /// <param name="services">The services.</param>
        protected override void ConfigureSingletons(IServiceCollection services)
        {
            base.ConfigureSingletons(services);
            services.AddSingleton<IRepositoryAsync<Guid, MondayMorningBlues>>(_mmbClient);
        }

        /// <summary>
        /// Configures the authorization policy
        /// </summary>
        /// <param name="services">The services.</param>
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
