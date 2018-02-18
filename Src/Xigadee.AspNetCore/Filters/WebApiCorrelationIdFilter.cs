//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.Filters;
//namespace Xigadee
//{
//    /// <summary>
//    /// This base class adds a tracking correlation id to each incoming request to allow for tracking and tracing.
//    /// </summary>
//    public class WebApiCorrelationIdFilter: ActionFilterAttribute
//    {
//        /// <summary>
//        /// This is the preferred HTTP key name for the correlation id.
//        /// </summary>
//        protected readonly string mCorrelationIdKeyName;
//        private readonly bool mAddToClaimsPrincipal;

//        /// <summary>
//        /// This is the default constructor.
//        /// </summary>
//        /// <param name="correlationIdKeyName">Correlation Id key in the request/response header</param>
//        /// <param name="addToClaimsPrincipal">Add the correlation key to the claims principal</param>
//        public WebApiCorrelationIdFilter(string correlationIdKeyName = "X-CorrelationId", bool addToClaimsPrincipal = true)
//        {
//            mCorrelationIdKeyName = correlationIdKeyName;
//            mAddToClaimsPrincipal = addToClaimsPrincipal;
//        }

//        #region CorrelationIdGet()
//        /// <summary>
//        /// This method creates a new correlation id.
//        /// </summary>
//        /// <returns>A unique string.</returns>
//        protected virtual string CorrelationIdGet()
//        {
//            return Guid.NewGuid().ToString("N").ToUpperInvariant();
//        }
//        #endregion

//        /// <summary>
//        /// This method adds the correlation id to the request if one is not already found in the request headers.
//        /// </summary>
//        /// <param name="actionContext">The incoming action.</param>
//        public override void OnActionExecuting(ActionExecutingContext actionContext)
//        {
//            try
//            {
//                var request = actionContext.HttpContext.Request;
//                IEnumerable<string> correlationValues;
//                var correlationId = CorrelationIdGet();
//                if (!request.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValues))
//                {
//                    request.Headers.Add(mCorrelationIdKeyName, correlationId);
//                }
//                else
//                {
//                    correlationId = correlationValues.FirstOrDefault() ?? correlationId;
//                }

//                IRequestOptions apiRequest = actionContext.ActionArguments.Values.OfType<IRequestOptions>().FirstOrDefault();

//                if (apiRequest?.Options != null)
//                    apiRequest.Options.CorrelationId = correlationId;

//                // If we have a claims identity then add the correlation id to it (if component configured to do it)
//                var claimsIdentity = actionContext.Principal?.Identity as ClaimsIdentity;
//                if (mAddToClaimsPrincipal && claimsIdentity !=null && !claimsIdentity.HasClaim(c => c.Type == JwtTokenAuthenticationHandler.ClaimProcessCorrelationKey))
//                    claimsIdentity.AddClaim(new Claim(JwtTokenAuthenticationHandler.ClaimProcessCorrelationKey, correlationId));
//            }
//            catch (Exception)
//            {
//                // Don't prevent normal operation of the site where there is an exception
//            }

//            base.OnActionExecuting(actionContext);
//        }

//        /// <summary>
//        /// This method adds the correlationid to the outgoing response if one is found in the request headers.
//        /// </summary>
//        /// <param name="actionExecutedContext">The request and response.</param>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <returns>Async request.</returns>
//        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
//        {
//            var tasks = new List<Task>
//            {
//                base.OnActionExecutedAsync(actionExecutedContext, cancellationToken)
//            };

//            var request = actionExecutedContext?.Response?.RequestMessage;
//            var response = actionExecutedContext?.Response;

//            // Retrieve the correlation id from the request and add to the response
//            IEnumerable<string> correlationValues = null;
//            string correlationId = null;
//            if ((request?.Headers?.TryGetValues(mCorrelationIdKeyName, out correlationValues) ?? false))
//                correlationId = correlationValues?.FirstOrDefault();

//            if (!string.IsNullOrEmpty(correlationId) && response != null && !response.Headers.Contains(mCorrelationIdKeyName))
//                response.Headers.Add(mCorrelationIdKeyName, correlationId);

//            await Task.WhenAll(tasks);
//        }
//    }
//}
