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

        public static IWebHostBuilder UseXigadee(this IWebHostBuilder hostBuilder
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
            , Type serviceReference = null)
        {
            hostBuilder.ConfigureServices((ctx, coll) => 
                coll.XigadeeConfigure(name, serviceId, description, policy
                    , properties, config, assign
                    , configAssign, addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors
                    , serviceVersionId, serviceReference));

            return hostBuilder;
        }


        public static XigadeeHostedService XigadeeConfigure(this IServiceCollection coll
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
            , Type serviceReference = null)
        {
            var ms = new XigadeeHostedService(name, serviceId, description, policy
                , properties, config, assign
                , configAssign, addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors
                , serviceVersionId, serviceReference);

            return coll.XigadeeConfigure(ms);
        }

        public static XigadeeHostedService XigadeeConfigure(this IServiceCollection coll, XigadeeHostedService ms)
        {
            coll.AddSingleton<IHostedService>(ms);
            coll.AddSingleton(ms.Pipeline);
            coll.AddSingleton<IMicroservice>(ms.Pipeline.Service);

            return ms;
        }
    }
}
