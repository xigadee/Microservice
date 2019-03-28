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

        PersistenceClient<Guid, MondayMorningBlues> _client;

        protected override void ConfigureMicroservicePipeline(MicroservicePipeline pipeline)
        {
            pipeline
                .AdjustPolicyTaskManagerForDebug()
                .AddChannelIncoming("testin")
                    .AttachPersistenceManagerHandlerMemory(
                        (MondayMorningBlues e) => e.Id
                        , s => new Guid(s)
                        , versionPolicy: ((e) => $"{e.VersionId:N}", (e) => e.VersionId = Guid.NewGuid(), true)
                        , propertiesMaker: (e) => e.ToReferences2()
                        , searches: new[] { new RepositoryMemorySearch<Guid, MondayMorningBlues>("default") }
                        , searchIdDefault: "default")
                    .AttachPersistenceClient(out _client)
                    .Revert()                
                ;

            pipeline.Service.Events.StartCompleted += Service_StartCompleted; 
        }

        private void Service_StartCompleted(object sender, StartEventArgs e)
        {
            var rs = _client.Create(new MondayMorningBlues() { Id = new Guid("9A2E3F6D-3B98-4C2C-BD45-74F819B5EDFC") }).Result;
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //services.adds
            services.AddSingleton<IRepositoryAsync<Guid, MondayMorningBlues>>(_client);

            return base.ConfigureServices(services);
        }


    }
}
