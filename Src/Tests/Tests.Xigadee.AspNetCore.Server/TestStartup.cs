using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Test.Xigadee;
using Xigadee;

namespace Tests.Xigadee
{
    public class TestStartup : ApiStartupBase<ApiStartUpContext>//<TestStartupContext, ConfigApplication, IApiUserSecurityModule, ConfigAuthorization>
    {
        public TestStartup(IHostingEnvironment env) : base(env)
        {
        }

        PersistenceClient<Guid, MondayMorningBlues> __client;

        protected override void ConfigureMicroservicePipeline(MicroservicePipeline pipeline)
        {
            pipeline
                .AdjustPolicyTaskManagerForDebug()
                .AddChannelIncoming("testin")
                    .AttachPersistenceManagerHandlerMemory((MondayMorningBlues e) => e.Id, s => new Guid(s))
                    .AttachPersistenceClient(out __client)
                    .Revert()                
                ;
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //services.adds
            services.AddSingleton<IRepositoryAsync<Guid, MondayMorningBlues>>(__client);

            return base.ConfigureServices(services);
        }


    }
}
