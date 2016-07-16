using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="containerName"></param>
        /// <param name="correlationIdKeyName"></param>
        /// <param name="level"></param>
        public WebApiCorrelationIdFilter(string correlationIdKeyName = "X-CorrelationId")
        {
            mCorrelationIdKeyName = correlationIdKeyName;
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
                    actionContext.Request.Headers.Add(mCorrelationIdKeyName, correlationId);

                IRequestOptions apiRequest = actionContext.ActionArguments.Values.FirstOrDefault(
                        aa => aa != null && aa is IRequestOptions) as IRequestOptions;

                if (apiRequest?.Options != null)
                    apiRequest.Options.CorrelationId = correlationId;
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

            if (!string.IsNullOrEmpty(correlationId))
                response.Headers.Add(mCorrelationIdKeyName, correlationId);

            await Task.WhenAll(tasks);
        }

    }
}
