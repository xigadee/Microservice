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


        public static IServiceCollection AddXigadee(this IServiceCollection services, string name = null
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

            services.AddSingleton<IHostedService>(ms);
            services.AddSingleton(ms.Pipeline);
            services.AddSingleton<IMicroservice>(ms.Pipeline.Service);

            return services;
        }
    }
}
