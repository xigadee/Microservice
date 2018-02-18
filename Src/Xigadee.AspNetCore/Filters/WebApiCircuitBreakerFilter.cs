//#region using
//using System;
//using System.Net;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.Filters;
//#endregion
//namespace Xigadee
//{
//    /// <summary>
//    /// This filter can be applied to specific method calls and interacts with the Xigadee Resource Tracker.
//    /// The filter is used to limit to stop incoming requests in a Microservice system, when there are problems
//    /// with downstream resources.
//    /// </summary>
//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
//    public class WebApiCircuitBreakerFilterAttribute: ActionFilterAttribute
//    {
//        /// <summary>
//        /// This random function is used to provide random support for the half open state that allow some messages to progress
//        /// when the service is validating connectivity to the resource.
//        /// </summary>
//        protected readonly Random mRand = new Random(Environment.TickCount);
//        /// <summary>
//        /// This is the profile Id for the resource we wish to track.
//        /// </summary>
//        protected readonly string mResourceProfileId;
//        /// <summary>
//        /// This property specifies whether the profile id that has caused the error is disclosed in the HTTP response.
//        /// </summary>
//        protected readonly bool mDiscloseStatusinHTTPResponse;
//        /// <summary>
//        /// This is the default constructor.
//        /// </summary>
//        /// <param name="resourceProfileId">The resource id.</param>
//        /// <param name="discloseStatusinHTTPResponse">This property specifies whether the profile id and status that has caused the error is disclosed in the HTTP response. The default it true.</param>
//        public WebApiCircuitBreakerFilterAttribute(string resourceProfileId, bool discloseStatusinHTTPResponse = true)
//        {
//            mResourceProfileId = resourceProfileId;
//            mDiscloseStatusinHTTPResponse = discloseStatusinHTTPResponse;
//        }
//        /// <summary>
//        /// This method is called before the method is executed to verify that the resource is available.
//        /// </summary>
//        /// <param name="actionContext">The action context.</param>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <returns>Async method.</returns>
//        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
//        {
//            var ms = actionContext.ToMicroservice();

//            ResourceStatus status = ms?.ResourceMonitor.ResourceStatusGet(mResourceProfileId);

//            switch (status?.State ?? CircuitBreakerState.Closed)
//            {
//                case CircuitBreakerState.Closed: //Closed: so keep executing
//                    break;
//                case CircuitBreakerState.HalfOpen:// Check the probability function to randomly let some messages through, this is based on a 0-100% filter. 
//                    if (mRand.Next(0,100) <= status.FilterPercentage)
//                        break;
//                    actionContext.Response = GenerateRequest(status, actionContext.Request);
//                    return;
//                case CircuitBreakerState.Open: //Return a 429 error
//                    actionContext.Response = GenerateRequest(status, actionContext.Request);
//                    return;
//            }      
            
//            await base.OnActionExecutingAsync(actionContext, cancellationToken);
//        }

//        private HttpResponseMessage GenerateRequest(ResourceStatus status, HttpRequestMessage request)
//        {
//            HttpResponseMessage response = request.CreateResponse((HttpStatusCode)429);

//            if (mDiscloseStatusinHTTPResponse)
//                response.ReasonPhrase = $"Too many requests. Circuit breaker thrown for {mResourceProfileId} at {status.State} for {status.FilterPercentage}%";
//            else
//                response.ReasonPhrase = $"Too many requests.";

//            if (status.RetryInSeconds.HasValue)
//                response.Headers.Add("Retry-After", status.RetryInSeconds.Value.ToString());

//            return response;
//        }
//    }
//}
