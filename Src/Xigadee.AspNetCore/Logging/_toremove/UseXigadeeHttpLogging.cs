//using System;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Options;

//namespace Xigadee
//{
//    public static partial class AspNetCoreExtensionMethods
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="app">The application builder.</param>
//        /// <param name="level">The specified logging level. The default value is to log all the data.</param>
//        /// <param name="correlationIdKey">The correlation Id header. The default is X-CorrelationId</param>
//        /// <param name="addToClaimsPrincipal">Specifies whether to use this id to the claims principal.</param>
//        /// <param name="filter">A function that can be used to filter out specific requests from logging.</param>
//        /// <returns>The application builder.</returns>
//        public static IApplicationBuilder UseXigadeeHttpBoundaryLogging(this IApplicationBuilder app
//            , ApiBoundaryLoggingFilterLevel level = ApiBoundaryLoggingFilterLevel.All
//            , string correlationIdKey = "X-CorrelationId"
//            , bool addToClaimsPrincipal = true
//            , Func<HttpContext, bool> filter = null)
//        {
//            app.UseMiddleware< XigadeeHttpBoundaryLogger >(
//                Options.Create(new XigadeeHttpBoundaryLoggerOptions
//                    {
//                          Level = level
//                        , CorrelationIdKey = correlationIdKey
//                        , AddToClaimsPrincipal = addToClaimsPrincipal
//                        , Microservice = app.GetXigadee()
//                        , Filter = filter
//                    })
//                );

//            return app;
//        }

//    }





//}
