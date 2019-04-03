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
    /// <summary>
    /// This is the base application class.
    /// </summary>
    /// <seealso cref="Xigadee.ApiMicroserviceStartupBase{Tests.Xigadee.TestStartupContext}" />
    public class TestStartup : ApiMicroserviceStartupBase<TestStartupContext>
    {
        public TestStartup(IHostingEnvironment env) : base(env)
        {
        }

        PersistenceClient<Guid, MondayMorningBlues> _mmbClient;

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


        protected override void ConfigureSingletons(IServiceCollection services)
        {
            //services.adds
            services.AddSingleton<IRepositoryAsync<Guid, MondayMorningBlues>>(_mmbClient);
            services.AddSingleton<IApiUserSecurityModule>(Context.UserSecurityModule);

            services.AddSingleton(Context.SecurityJwt);

            services.AddJwtAuthentication(Context.SecurityJwt);
        }

    }
}
