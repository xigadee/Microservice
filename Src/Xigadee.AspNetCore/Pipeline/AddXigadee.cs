//using System;
//using System.Linq;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//namespace Xigadee
//{
//    public static partial class AspNetCoreExtensionMethods
//    {

//        public static IPipelineAspNetCore GetXigadeePipeline(this IApplicationBuilder app)
//        {
//            MicroserviceHostedService service = app.GetXigadeeService();

//            return service?.Pipeline;
//        }

//        public static IPipelineAspNetCore GetXigadeePipeline(this IServiceCollection services)
//        {
//            MicroserviceHostedService service = services.GetXigadeeService();

//            return service?.Pipeline;
//        }


//        public static IMicroservice GetXigadee(this IServiceCollection services)
//        {
//            MicroserviceHostedService service = services.GetXigadeeService();

//            return service?.Pipeline?.Service;
//        }

//        public static IMicroservice GetXigadee(this IApplicationBuilder app)
//        {
//            MicroserviceHostedService service = app.GetXigadeeService();

//            return service?.Pipeline?.Service;
//        }


//        public static MicroserviceHostedService GetXigadeeService(this IApplicationBuilder app)
//        {
//            var service = app.ApplicationServices
//                .GetServices<IHostedService>()
//                .FirstOrDefault((s) => s is MicroserviceHostedService);

//            return service as MicroserviceHostedService;
//        }

//        public static MicroserviceHostedService GetXigadeeService(this IServiceCollection services)
//        {
//            var service = services
//                .Where((s) => s.ServiceType == typeof(IHostedService))
//                .FirstOrDefault((s) => s.ImplementationInstance is MicroserviceHostedService);

//            return service?.ImplementationInstance as MicroserviceHostedService;
//        }

//        public static IPipelineAspNetCore AddXigadee(this IServiceCollection services, string name = null
//            , string serviceId = null
//            , string description = null
//            , IEnumerable<PolicyBase> policy = null
//            , IEnumerable<Tuple<string, string>> properties = null
//            , IEnvironmentConfiguration config = null
//            , Action<IMicroservice> assign = null
//            , Action<IEnvironmentConfiguration> configAssign = null
//            , bool addDefaultJsonPayloadSerializer = true
//            , bool addDefaultPayloadCompressors = true
//            , string serviceVersionId = null
//            , Type serviceReference = null)
//        {

//            MicroserviceHostedService service = services.GetXigadeeService();

//            if (service != null)
//                return service.Pipeline;

//            var ms = services.XigadeeCreate(name, serviceId, description, policy
//                , properties, config, assign
//                , configAssign, addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors
//                , serviceVersionId, serviceReference);

//            return ms.Pipeline;
//        }
//    }
//}
