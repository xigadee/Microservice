//#region using
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//#endregion
//namespace Xigadee
//{
//    /// <summary>
//    /// This class logs the incoming API requests and subsequent responses to the Azure Storage container.
//    /// </summary>
//    public class WebApiBoundaryLoggingFilter : WebApiCorrelationIdFilter
//    {
//        #region Declarations
//        private readonly ApiBoundaryLoggingFilterLevel mLevel;
//        #endregion

//        #region Constructor
//        /// <summary>
//        /// This filter can be used to log filtered incoming and outgoing Api messages and payloads to the Xigadee DataCollection infrastructure.
//        /// </summary>
//        /// <param name="ms">The Microservice.</param>
//        /// <param name="correlationIdKeyName">The keyname for the correlation id. By default this is X-CorrelationId</param>
//        /// <param name="level">The logging level</param>
//        /// <param name="addToClaimsPrincipal">Specifies whether the correlation Id should be added to the claims principal</param>
//        public WebApiBoundaryLoggingFilter(ApiBoundaryLoggingFilterLevel level = ApiBoundaryLoggingFilterLevel.All
//            , string correlationIdKeyName = "X-CorrelationId"
//            , bool addToClaimsPrincipal = true) : base(correlationIdKeyName, addToClaimsPrincipal)
//        {
//            mLevel = level;
//        }
//        #endregion

//        /// <summary>
//        /// This override logs the incoming and outgoing transaction to the Microservice Data Collector.
//        /// </summary>
//        /// <param name="actionExecutedContext">The context.</param>
//        /// <param name="cancellationToken">The cancellation token.</param>
//        /// <returns>Returns the pass through task.</returns>
//        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
//        {
//            var ms = actionExecutedContext.ToMicroservice();

//            if (ms != null && mLevel > ApiBoundaryLoggingFilterLevel.None)
//            {
//                var bEvent = new ApiBoundaryEvent(actionExecutedContext, mLevel);

//                // Retrieve the correlation id from the request and add to the response

//                IEnumerable<string> correlationValuesOut;
//                if (actionExecutedContext.Response.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValuesOut))
//                    bEvent.CorrelationId = correlationValuesOut.FirstOrDefault();

//                //Ok, check the outbound response if the correlation id was not set on the outgoing request.
//                if (string.IsNullOrEmpty(bEvent.CorrelationId))
//                {
//                    IEnumerable<string> correlationValuesin;
//                    if (actionExecutedContext.Request.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValuesin))
//                        bEvent.CorrelationId = correlationValuesin.FirstOrDefault();
//                }

//                ms.DataCollection.Write(bEvent, DataCollectionSupport.ApiBoundary, false);
//            }

//            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
//        }
//    }
//}
