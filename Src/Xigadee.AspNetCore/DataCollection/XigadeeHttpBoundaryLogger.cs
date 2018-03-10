using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace Xigadee
{
    /// <summary>
    /// This is the boundary logger.
    /// </summary>
    public class XigadeeHttpBoundaryLogger
    {
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

        /// <summary>
        /// Gets the create options.
        /// </summary>
        private XigadeeHttpBoundaryLoggerOptions Options { get; }

        /// <summary>
        /// Gets the next jump in the pipeline chain.
        /// </summary>
        private RequestDelegate Next { get; }

        /// <summary>
        /// This method is called as part of the ASP.Net Core pipeline
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>This is an async process.</returns>
        public async Task Invoke(HttpContext context)
        {
            Exception logEx = null;
            IMicroservice ms = Options.Microservice ?? context.RequestServices.GetService<IMicroservice>();

            try
            {
                await Next(context);
            }
            catch (Exception ex)
            {
                logEx = ex;
                throw;
            }
            finally
            {
                if (ms != null
                    && Options.Level != ApiBoundaryLoggingFilterLevel.None
                    && (Options.Filter?.Invoke(context) ?? true)
                    )
                {
                    var boundaryLog = new AspNetCoreBoundaryEvent(context, Options.Level, logEx);
                    ms.DataCollection.Write(boundaryLog, DataCollectionSupport.ApiBoundary, false);
                }
            }
        }
    }
}
