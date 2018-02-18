using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Xigadee
{
    public static partial class WebApiExtensionMethods
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
        public static IApplicationBuilder UseXigadee(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
