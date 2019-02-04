//using Microsoft.Extensions.DependencyInjection;

//namespace Xigadee
//{
//    public static partial class AspNetCoreExtensionMethods
//    {
//        /// <summary>
//        /// This is the key used to reference the Microservice in the HttpConfig Properties.
//        /// </summary>
//        public const string MicroserviceKey = "XigadeeMicroservice";

//        /// <summary>
//        /// This extension method retrieves the Microservice from the HttpConfig Properties.
//        /// </summary>
//        public static IMicroservice ToMicroservice(this ActionContext actionContext)
//        {
//            var ms = actionContext.HttpContext.RequestServices.GetService<IMicroservice>();

//            return ms;
//        }

//        /// <summary>
//        /// This is a helper method that identifies the current pipeline. It is useful for autocomplete identification. 
//        /// This command does not do anything.
//        /// </summary>
//        /// <param name="pipe">The pipeline.</param>
//        public static void Ability_Is_AspNetCoreMicroservicePipeline(this IPipelineAspNetCore pipe)
//        {
//        }
//    }


//}
