//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//namespace Xigadee
//{
//    public static partial class AspNetCoreExtensionMethods
//    {
//        /// <summary>
//        /// Sets the XIgadee Microservice in the root pipeline.
//        /// </summary>
//        /// <param name="hostBuilder">The host builder.</param>
//        /// <param name="name">The Microservice name.</param>
//        /// <param name="serviceId">The service id.</param>
//        /// <param name="description">This is an optional Microservice description.</param>
//        /// <param name="policy">The policy settings collection.</param>
//        /// <param name="properties">Any additional property key/value pairs.</param>
//        /// <param name="config">The environment config object</param>
//        /// <param name="assign">The action can be used to assign items to the microservice.</param>
//        /// <param name="configAssign">This action can be used to adjust the config settings.</param>
//        /// <param name="addDefaultJsonPayloadSerializer">This property specifies that the default Json payload serializer should be added to the Microservice, set this to false to disable this.</param>
//        /// <param name="addDefaultPayloadCompressors">This method ensures the Gzip and Deflate compressors are added to the Microservice.</param>
//        /// <param name="serviceVersionId">This is the version id of the calling assembly as a string.</param>
//        /// <param name="serviceReference">This is a reference type used to identify the version id of the root assembly.</param>
//        /// <param name="configure">The optional configuration action.</param>
//        /// <returns>Returns the web host builder.</returns>
//        public static IWebHostBuilder UseXigadee(this IWebHostBuilder hostBuilder
//            , string name = null
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
//            , Type serviceReference = null
//            , Action<IPipelineAspNetCore> configure = null
//            )
//        {
//            hostBuilder.ConfigureServices((ctx, coll) => 
//                coll.XigadeeCreate(name, serviceId, description, policy
//                    , properties, config, assign
//                    , configAssign, addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors
//                    , serviceVersionId, serviceReference));

//            return hostBuilder;
//        }

//        /// <summary>
//        /// The extension method creates the container and returns it.
//        /// </summary>
//        /// <param name="coll">The coll.</param>
//        /// <param name="name">The Microservice name.</param>
//        /// <param name="serviceId">The service id.</param>
//        /// <param name="description">This is an optional Microservice description.</param>
//        /// <param name="policy">The policy settings collection.</param>
//        /// <param name="properties">Any additional property key/value pairs.</param>
//        /// <param name="config">The environment config object</param>
//        /// <param name="assign">The action can be used to assign items to the microservice.</param>
//        /// <param name="configAssign">This action can be used to adjust the config settings.</param>
//        /// <param name="addDefaultJsonPayloadSerializer">This property specifies that the default Json payload serializer should be added to the Microservice, set this to false to disable this.</param>
//        /// <param name="addDefaultPayloadCompressors">This method ensures the Gzip and Deflate compressors are added to the Microservice.</param>
//        /// <param name="serviceVersionId">This is the version id of the calling assembly as a string.</param>
//        /// <param name="serviceReference">This is a reference type used to identify the version id of the root assembly.</param>
//        /// <param name="configure">The optional configuration action.</param>
//        /// <returns>Returns the XigadeeHostedService container.</returns>
//        public static MicroserviceHostedService XigadeeCreate(this IServiceCollection coll
//            , string name = null
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
//            , Type serviceReference = null
//            , Action<IPipelineAspNetCore> configure = null
//            )
//        {
//            var ms = new MicroserviceHostedService(name, serviceId, description, policy
//                , properties, config, assign
//                , configAssign, addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors
//                , serviceVersionId, serviceReference);

//            coll.XigadeeRegister(ms);

//            configure?.Invoke(ms.Pipeline);

//            return ms;
//        }
//        /// <summary>
//        /// Registers the Xigadee Hosted Service Container.
//        /// </summary>
//        /// <param name="coll">The service collection</param>
//        /// <param name="ms">The hosted service container..</param>
//        public static void XigadeeRegister(this IServiceCollection coll, MicroserviceHostedService ms)
//        {
//            coll.AddSingleton<IHostedService>(ms);
//            coll.AddSingleton<IMicroservice>(ms.Pipeline.Service);
//        }
//    }
//}
