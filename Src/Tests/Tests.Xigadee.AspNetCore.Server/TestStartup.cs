using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Xigadee;

namespace Tests.Xigadee
{
    public class TestStartup : JwtApiStartUpBase<TestStartupContext, ConfigApiService, IApiUserSecurityModule, ConfigAuthorization>
    {
        public TestStartup(IHostingEnvironment env) : base(env)
        {
        }

        protected override void ConfigureMicroservicePipeline(MicroservicePipeline pipeline)
        {
            pipeline
                .AdjustPolicyTaskManagerForDebug();

        }
    }
}
