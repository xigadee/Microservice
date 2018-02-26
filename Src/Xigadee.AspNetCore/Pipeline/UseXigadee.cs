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
        public static IPipelineAspNetCore UseXigadee(this IApplicationBuilder app
            , string name = null
            , string serviceId = null
            , string description = null
            , IEnumerable<PolicyBase> policy = null
            , IEnumerable<Tuple<string, string>> properties = null
            , IEnvironmentConfiguration config = null
            , Action<IMicroservice> assign = null
            , Action<IEnvironmentConfiguration> configAssign = null
            , bool addDefaultJsonPayloadSerializer = true
            , bool addDefaultPayloadCompressors = true
            , string serviceVersionId = null
            , Type serviceReference = null
            )
        {

            var pipe = new AspNetCoreMicroservicePipeline(name, serviceId, description, policy, properties, config, assign, configAssign
                , addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors, serviceVersionId, serviceReference, app);

            return pipe;
        }

        public static IWebHostBuilder UseXigadee(this IWebHostBuilder hostBuilder, string name = null)
        {
            hostBuilder.ConfigureServices((ctx, coll) => coll.AddXigadee(name));
            return hostBuilder;
        }
    }
}
