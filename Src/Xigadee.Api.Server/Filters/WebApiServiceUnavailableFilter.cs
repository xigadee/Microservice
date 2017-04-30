#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
            RetryInSeconds = retryInSeconds;
        }

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

        /// <summary>
        /// This method is called when the incoming request is executed.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continuation">The continuation function.</param>
        /// <returns>Returns a response message based on the current Microservice status.</returns>
        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var ms = actionContext.ToMicroservice();

            var status = ms?.Status ?? ServiceStatus.Faulted;

            //Ok, the service is running, so keep going.
            if (status == ServiceStatus.Running)
                return await continuation();

            //Service has not yet started, so returns a service unavailable response.
            var request = actionContext.Request;
            HttpResponseMessage response = request.CreateResponse(HttpStatusCode.ServiceUnavailable);
            response.ReasonPhrase = $"Status: {status.ToString()}";

            if (RetryInSeconds.HasValue)
                response.Headers.Add("Retry-After", RetryInSeconds.Value.ToString());

            actionContext.Response = response;

            return response;
        }
    }

}
