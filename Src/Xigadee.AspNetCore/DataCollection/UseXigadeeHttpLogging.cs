using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Xigadee
{
    public static partial class AspNetCoreExtensionMethods
    {
        public static IApplicationBuilder UseXigadeeHttpBoundaryLogging(this IApplicationBuilder app
            , ApiBoundaryLoggingFilterLevel level = ApiBoundaryLoggingFilterLevel.All
            , string correlationIdKey = "X-CorrelationId"
            , bool addToClaimsPrincipal = true)
        {
            app.UseMiddleware< XigadeeHttpBoundaryLogger >(
                Options.Create(
                    new XigadeeHttpBoundaryLoggerOptions
                    {
                          Level = level
                        , CorrelationIdKey = correlationIdKey
                        , AddToClaimsPrincipal = addToClaimsPrincipal
                        , Microservice = app.GetXigadee()
                    })
                );

            return app;
        }

    }

    public class XigadeeHttpBoundaryLoggerOptions
    {
        public ApiBoundaryLoggingFilterLevel Level { get; set; } = ApiBoundaryLoggingFilterLevel.All;

        public string CorrelationIdKey { get; set; } = "X-CorrelationId";

        public bool AddToClaimsPrincipal { get; set; } = true;

        public IMicroservice Microservice { get; set; }
    }

    /// <summary>
    /// This is the boundary logger.
    /// </summary>
    public class XigadeeHttpBoundaryLogger
    {
        /// <summary>
        /// Gets the create options.
        /// </summary>
        private XigadeeHttpBoundaryLoggerOptions Options { get; }

        /// <summary>
        /// Gets the next jump in the pipeline chain.
        /// </summary>
        private RequestDelegate Next { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="XigadeeHttpBoundaryLogger"/> class.
        /// </summary>
        /// <param name="next">The next jump in the pipeline.</param>
        /// <param name="options">The incoming options.</param>
        public XigadeeHttpBoundaryLogger(RequestDelegate next, IOptions<XigadeeHttpBoundaryLoggerOptions> options)
        {
            Next = next;
            Options = options?.Value ?? new XigadeeHttpBoundaryLoggerOptions();
        }

        public async Task Invoke(HttpContext context)
        {

            await Next(context);
            
            if (Options.Microservice == null)
                Options.Microservice = context.RequestServices.GetService<IMicroservice>();

            if (Options.Microservice != null)
            {
                var boundaryLog = new AspNetCoreBoundaryEvent(context, Options.Level);
                Options.Microservice.DataCollection.Write(boundaryLog, DataCollectionSupport.ApiBoundary, false);
            }
        }
    }

    /// <summary>
    /// This is the logging level.
    /// </summary>
    [Flags]
    public enum ApiBoundaryLoggingFilterLevel
    {
        /// <summary>
        /// No logging of any information.
        /// </summary>
        None = 0,
        /// <summary>
        /// Include the exception event in the log
        /// </summary>
        Exception = 1,
        /// <summary>
        /// Include the  request event in the log
        /// </summary>
        Request = 2,
        /// <summary>
        /// Include the response event in the log
        /// </summary>
        Response = 4,
        /// <summary>
        /// Include the request content in the log
        /// </summary>
        RequestContent = 8,
        /// <summary>
        /// Include the response content in the log
        /// </summary>
        ResponseContent = 16,
        /// <summary>
        /// Include all data in the log
        /// </summary>
        All = 31
    }
}
