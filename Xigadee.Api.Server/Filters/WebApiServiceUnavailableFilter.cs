using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Xigadee
{
    /// <summary>
    /// This auth filter is used to stop requests from being processed when the system is not fully started.
    /// </summary>
    public class WebApiServiceUnavailableFilter: IAuthorizationFilter
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="retryInSeconds">The default retry time in seconds.</param>
        public WebApiServiceUnavailableFilter(int retryInSeconds = 10)
        {
            StatusCurrent = ServiceStatus.Stopped;
            RetryInSeconds = retryInSeconds;
        }

        /// <summary>
        /// The current server status.
        /// </summary>
        public ServiceStatus StatusCurrent { get; set; }
        /// <summary>
        /// The default retry time in seconds.
        /// </summary>
        public int? RetryInSeconds { get; set; }

        /// <summary>
        /// This is an override from the auth interface definition. We do not allow multiple.
        /// </summary>
        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (StatusCurrent == ServiceStatus.Running)
                return await continuation();

            var request = actionContext.Request;
            HttpResponseMessage response = request.CreateResponse(HttpStatusCode.ServiceUnavailable);
            response.ReasonPhrase = $"Status: {StatusCurrent.ToString()}";

            if (RetryInSeconds.HasValue)
                response.Headers.Add("Retry-After", RetryInSeconds.Value.ToString());

            actionContext.Response = response;

            return response;
        }
    }

}
