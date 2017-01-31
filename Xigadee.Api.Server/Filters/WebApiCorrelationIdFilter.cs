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
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Xigadee
{
    public class WebApiCorrelationIdFilter: ActionFilterAttribute
    {
        protected readonly string mCorrelationIdKeyName;
        private readonly bool mAddToClaimsPrincipal;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correlationIdKeyName">Correlation Id key in the request/response header</param>
        /// <param name="addToClaimsPrincipal">Add the correlation key to the claims principal</param>
        public WebApiCorrelationIdFilter(string correlationIdKeyName = "X-CorrelationId", bool addToClaimsPrincipal = true)
        {
            mCorrelationIdKeyName = correlationIdKeyName;
            mAddToClaimsPrincipal = addToClaimsPrincipal;
        }

        #region CorrelationIdGet()
        /// <summary>
        /// This method creates the correlation id.
        /// </summary>
        /// <returns>A unique string.</returns>
        protected virtual string CorrelationIdGet()
        {
            return Guid.NewGuid().ToString("N").ToUpperInvariant();
        }
        #endregion

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                var request = actionContext.Request;
                IEnumerable<string> correlationValues;
                var correlationId = CorrelationIdGet();
                if (!request.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValues))
                {
                    actionContext.Request.Headers.Add(mCorrelationIdKeyName, correlationId);
                }
                else
                {
                    correlationId = correlationValues.FirstOrDefault() ?? correlationId;
                }

                IRequestOptions apiRequest = actionContext.ActionArguments.Values.OfType<IRequestOptions>().FirstOrDefault();

                if (apiRequest?.Options != null)
                    apiRequest.Options.CorrelationId = correlationId;

                // If 
                var claimsIdentity = actionContext.RequestContext?.Principal?.Identity as ClaimsIdentity;
                if (mAddToClaimsPrincipal && claimsIdentity !=null && !claimsIdentity.HasClaim(c => c.Type == JwtTokenAuthenticationHandler.ClaimProcessCorrelationKey))
                    claimsIdentity.AddClaim(new Claim(JwtTokenAuthenticationHandler.ClaimProcessCorrelationKey, correlationId));
            }
            catch (Exception)
            {
                // Don't prevent normal operation of the site where there is an exception
            }

            base.OnActionExecuting(actionContext);
        }

        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>
            {
                base.OnActionExecutedAsync(actionExecutedContext, cancellationToken)
            };

            var request = actionExecutedContext.Response.RequestMessage;
            var response = actionExecutedContext.Response;

            // Retrieve the correlation id from the request and add to the response
            IEnumerable<string> correlationValues;
            string correlationId = null;
            if (request.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValues))
                correlationId = correlationValues.FirstOrDefault();

            if (!string.IsNullOrEmpty(correlationId) 
                && !response.Headers.Contains(mCorrelationIdKeyName))
                response.Headers.Add(mCorrelationIdKeyName, correlationId);

            await Task.WhenAll(tasks);
        }

    }
}
