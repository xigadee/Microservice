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
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Net;

namespace Xigadee
{
    public class JwtAuthenticationFilter: IAuthenticationFilter
    {
        private readonly IJwtTokenVerificationPolicy mPolicy;

        public JwtAuthenticationFilter(IJwtTokenVerificationPolicy policy)
        {
            mPolicy = policy;
        }

        public bool AllowMultiple => false;

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            try
            {
                // Verify that the action / controller isn't set to allow anonymous access
                if (context.ActionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() ||
                    context.ActionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                    return Task.CompletedTask;

                // Look for credentials in the request.
                AuthenticationHeaderValue auth = context.Request.Headers.Authorization;

                // If there aren't any credentials - or the filter does not recognize the authentication scheme - do nothing.
                if (auth == null || !auth.Scheme.Equals("bearer", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (mPolicy.DenyByDefault)
                        context.ErrorResult = new StatusResult(HttpStatusCode.Forbidden, context.Request);

                    return Task.CompletedTask;
                }

                var token = mPolicy.Validate(auth.Parameter);

                context.Principal = new MicroserviceSecurityPrincipal(token);

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                // Ignore exceptions
            }

            //On error, set to unauthorized.
            context.ErrorResult = new StatusResult(HttpStatusCode.Forbidden, context.Request);
            return Task.CompletedTask;
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// This class is used to assign the status.
    /// </summary>
    public class StatusResult: IHttpActionResult
    {
        public StatusResult(HttpStatusCode status, HttpRequestMessage request, string reasonPhrase = null)
        {
            Status = status;
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        public HttpStatusCode Status { get; private set; }

        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(Status);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase ?? Enum.GetName(typeof(HttpStatusCode), Status);
            return response;
        }
    }

}
