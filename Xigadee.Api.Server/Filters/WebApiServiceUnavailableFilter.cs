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
    /// This filter is used to stop requests when the system is not fully started.
    /// </summary>
    public class WebApiServiceUnavailableFilter: IAuthorizationFilter
    {
        public WebApiServiceUnavailableFilter(int retryInSeconds = 10)
        {
            StatusCurrent = ServiceStatus.Stopped;
            RetryInSeconds = retryInSeconds;
        }

        public ServiceStatus StatusCurrent { get; set; }

        public int? RetryInSeconds { get; set; }

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
            response.ReasonPhrase = $"Microservice Status: {StatusCurrent.ToString()}";

            if (RetryInSeconds.HasValue)
                response.Headers.Add("Retry-After", RetryInSeconds.Value.ToString());

            actionContext.Response = response;

            return response;
        }
    }

}
