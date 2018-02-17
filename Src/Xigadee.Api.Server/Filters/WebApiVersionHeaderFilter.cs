using System;
using System.Web.Http.Filters;

namespace Xigadee
{
    /// <summary>
    /// This action filter adds the version header to the http output.
    /// </summary>
    public class WebApiVersionHeaderFilter: ActionFilterAttribute
    {
        private readonly string mHeaderName;

        /// <summary>
        /// This is the constructor.
        /// </summary>
        /// <param name="headerName">The HTTP key name. The default is X-XigadeeApiVersion.</param>
        public WebApiVersionHeaderFilter(string headerName = "X-XigadeeApiVersion") => mHeaderName = headerName;

        /// <summary>
        /// This method adds the API version to the outgoing response.
        /// </summary>
        /// <param name="actionExecutedContext">The outgoing executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                var assName = actionExecutedContext?.ActionContext?.ControllerContext?.Controller?.GetType().Assembly.GetName();
                actionExecutedContext?.Response?.Headers?.Add(mHeaderName, assName?.Version.ToString() ?? "Unknown");
            }
            catch(Exception)
            {
                actionExecutedContext?.Response?.Headers?.Add(mHeaderName, "Error");
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
