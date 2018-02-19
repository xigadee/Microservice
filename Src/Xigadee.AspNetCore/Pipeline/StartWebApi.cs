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
        /// <summary>
        /// This is the key used to reference the Microservice in the HttpConfig Properties.
        /// </summary>
        public const string MicroserviceKey = "XigadeeMicroservice";

        /// <summary>
        /// This extension method retrieves the Microservice from the HttpConfig Properties.
        /// </summary>
        public static IMicroservice ToMicroservice(this ActionContext actionContext)
        {
            var ms = actionContext.HttpContext.RequestServices.GetService<IMicroservice>();

            return ms;
        }


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
            
            var pipe = new AspNetCoreMicroservicePipeline(app, name, serviceId, description, policy, properties, config, assign, configAssign
                , addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors, serviceVersionId, serviceReference);

            app.Properties.Add("Xigadee", pipe.ToMicroservice());

            return pipe;
        }

        public static IWebHostBuilder UseXigadee(this IWebHostBuilder hostBuilder, string name = null)
        {
            hostBuilder.ConfigureServices((ctx, coll) => coll.AddXigadee(name));
            return hostBuilder;
        }

        

        public static IServiceCollection AddXigadee(this IServiceCollection services, string name = null
            , string serviceId = null
            , string description = null)
        {
            var ms = new XigadeeService(name, serviceId, description);

            services.AddSingleton<IHostedService>(ms);
            services.AddSingleton<IMicroservice>(ms.Service);

            return services;
        }


        /// <summary>
        /// Reverts the specified AspNetCore pipeline to the application..
        /// </summary>
        /// <param name="cpipe">The incoming pipeline.</param>
        /// <returns>The application</returns>
        public static IApplicationBuilder Revert(this IPipelineAspNetCore cpipe)
        {
            return cpipe.App;
        }

        /// <summary>
        /// This is a helper method that identifies the current pipeline. It is useful for autocomplete identification. 
        /// This command does not do anything.
        /// </summary>
        /// <param name="pipe">The pipeline.</param>
        public static void Ability_Is_AspNetCoreMicroservicePipeline(this IPipelineAspNetCore pipe)
        {
        }
    }


}
