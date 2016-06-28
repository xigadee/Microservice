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

        public WebApiVersionHeaderFilter(string headerName = "X-XigadeeApiVersion")
        {
            mHeaderName = headerName;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                var assName = actionExecutedContext.ActionContext.ControllerContext.Controller.GetType().Assembly.GetName();

                actionExecutedContext.Response.Headers.Add(mHeaderName, assName.Version.ToString());
            }
            catch(Exception)
            {
                actionExecutedContext.Response.Headers.Add(mHeaderName, "Error");
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
