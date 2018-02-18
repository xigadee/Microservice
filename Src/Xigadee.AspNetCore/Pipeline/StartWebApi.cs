using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Xigadee
{
    public static partial class AspNetCoreExtensionMethods
    {
        /// <summary>
        /// This is the key used to reference the Microservice in the HttpConfig Properties.
        /// </summary>
        public const string MicroserviceKey = "XigadeeMicroservice";

        ///// <summary>
        ///// This extension method retrieves the Microservice from the HttpConfig Properties.
        ///// </summary>
        //public static IMicroservice ToMicroservice(this ActionContext actionContext)
        //{
        //    object value;
        //    //actionContext.ControllerContext.Configuration.Properties.TryGetValue(WebApiExtensionMethods.MicroserviceKey, out value);
        //    actionContext..(WebApiExtensionMethods.MicroserviceKey, out value);
        //    return value as IMicroservice;
        //}

        //public static IMicroservice ToMicroservice(this HttpActionExecutedContext actionExecutedContext)
        //{
        //    object value;
        //    actionExecutedContext.ActionContext.ControllerContext.Configuration.Properties.TryGetValue(WebApiExtensionMethods.MicroserviceKey, out value);
        //    return value as IMicroservice;
        //}

        ///// <summary>
        ///// This method can be used to start the web api pipeline using the 
        ///// HttpConfiguration embedded in the pipeline.
        ///// </summary>
        ///// <typeparam name="P">The pipeline type.</typeparam>
        ///// <param name="webpipe">The pipeline based on the WebApi.</param>
        ///// <param name="app">The app builder reference.</param>
        ///// <returns>Returns the pipeline</returns>
        //public static void StartWebApi<P>(this P webpipe, IAppBuilder app)
        //    where P : IPipelineWebApi
        //{
        //    app.UseWebApi(webpipe.HttpConfig);

        //    webpipe.HttpConfig.EnsureInitialized();

        //    webpipe.HttpConfig.Properties.GetOrAdd(MicroserviceKey, webpipe.ToMicroservice());

        //    Task.Run(() => webpipe.Start());
        //}

        //
        // Summary:
        //     Captures synchronous and asynchronous System.Exception instances from the pipeline
        //     and generates HTML error responses.
        //
        // Parameters:
        //   app:
        //     The Microsoft.AspNetCore.Builder.IApplicationBuilder.
        //
        // Returns:
        //     A reference to the app after the operation has completed.
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

        public static IWebHostBuilder UseXigadee(this IWebHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(XigadeeConfigure);
            return hostBuilder;
        }

        private static void XigadeeConfigure(WebHostBuilderContext ctx, IServiceCollection coll)
        {

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

    /// <summary>
    /// This extension pipeline is used by the AspNetCore pipeline.
    /// </summary>
    public class AspNetCoreMicroservicePipeline: MicroservicePipeline, IPipelineAspNetCore
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor for the pipeline.
        /// </summary>
        /// <param name="app">The AspNetCore application.</param>
        /// <param name="name">The Microservice name.</param>
        /// <param name="serviceId">The service id.</param>
        /// <param name="description">This is an optional Microservice description.</param>
        /// <param name="policy">The policy settings collection.</param>
        /// <param name="properties">Any additional property key/value pairs.</param>
        /// <param name="config">The environment config object</param>
        /// <param name="assign">The action can be used to assign items to the microservice.</param>
        /// <param name="configAssign">This action can be used to adjust the config settings.</param>
        /// <param name="addDefaultJsonPayloadSerializer">This property specifies that the default Json payload serializer should be added to the Microservice, set this to false to disable this.</param>
        /// <param name="addDefaultPayloadCompressors">This method ensures the Gzip and Deflate compressors are added to the Microservice.</param>
        /// <param name="serviceVersionId">This is the version id of the calling assembly as a string.</param>
        /// <param name="serviceReference">This is a reference type used to identify the version id of the root assembly.</param>
        public AspNetCoreMicroservicePipeline(IApplicationBuilder app
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
            ) : base(name, serviceId, description, policy, properties, config, assign, configAssign, addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors, serviceVersionId, serviceReference)
        {
            App = app ?? throw new ArgumentNullException("app");
        }

        /// <summary>
        /// This is the default pipeline.
        /// </summary>
        /// <param name="service">The microservice.</param>
        /// <param name="config">The microservice configuration.</param>
        /// <param name="app">The AspNetCore application.</param>
        public AspNetCoreMicroservicePipeline(IMicroservice service
            , IEnvironmentConfiguration config
            , IApplicationBuilder app) : base(service, config)
        {
            App = app ?? throw new ArgumentNullException("app");
        }
        #endregion

        #region App
        /// <summary>
        /// This is the AspNetCore application
        /// </summary>
        public IApplicationBuilder App { get; protected set; }
        #endregion
    }
}
