using System;
using System.Linq;
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

        public static IPipelineAspNetCore GetXigadeePipeline(this IApplicationBuilder app)
        {
            XigadeeHostedService service = app.GetXigadeeService();

            return service?.Pipeline;
        }

        public static IPipelineAspNetCore GetXigadeePipeline(this IServiceCollection services)
        {
            XigadeeHostedService service = services.GetXigadeeService();

            return service?.Pipeline;
        }


        public static IMicroservice GetXigadee(this IServiceCollection services)
        {
            XigadeeHostedService service = services.GetXigadeeService();

            return service?.Pipeline?.Service;
        }

        public static IMicroservice GetXigadee(this IApplicationBuilder app)
        {
            XigadeeHostedService service = app.GetXigadeeService();

            return service?.Pipeline?.Service;
        }


        public static XigadeeHostedService GetXigadeeService(this IApplicationBuilder app)
        {
            var service = app.ApplicationServices
                .GetServices<IHostedService>()
                .FirstOrDefault((s) => s is XigadeeHostedService);

            return service as XigadeeHostedService;
        }

        public static XigadeeHostedService GetXigadeeService(this IServiceCollection services)
        {
            var service = services
                .Where((s) => s.ServiceType == typeof(IHostedService))
                .FirstOrDefault((s) => s.ImplementationInstance is XigadeeHostedService);

            return service?.ImplementationInstance as XigadeeHostedService;
        }

        public static IPipelineAspNetCore AddXigadee(this IServiceCollection services, string name = null
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

            XigadeeHostedService service = services.GetXigadeeService();

            if (service != null)
                return service.Pipeline;

            var ms = services.XigadeeCreate(name, serviceId, description, policy
                , properties, config, assign
                , configAssign, addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors
                , serviceVersionId, serviceReference);

            return ms.Pipeline;
        }
    }
}
