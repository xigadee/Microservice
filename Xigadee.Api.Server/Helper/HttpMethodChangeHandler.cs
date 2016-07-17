using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace Xigadee
{
    /// <summary>
    /// This method changes the incoming Http method to the specified method.
    /// </summary>
    public class HttpMethodChangeHandler: HttpMessageHandler
    {
        private readonly HttpConfiguration mConfiguration;
        private readonly string mVerbChange;

        public HttpMethodChangeHandler(HttpConfiguration configuration, string newVerb)
        {
            mConfiguration = configuration;
            mVerbChange = newVerb;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IHttpRouteData routeData = request.GetRouteData();

            IHttpControllerSelector ControllerSelector = mConfiguration.Services.GetHttpControllerSelector();
            HttpControllerDescriptor httpControllerDescriptor = ControllerSelector.SelectController(request);
            IHttpController httpController = httpControllerDescriptor.CreateController(request);

            request.Method = new HttpMethod(mVerbChange);
            // Create context
            HttpControllerContext controllerContext = new HttpControllerContext(mConfiguration, routeData, request);
            controllerContext.Controller = httpController;
            controllerContext.ControllerDescriptor = httpControllerDescriptor;

            return httpController.ExecuteAsync(controllerContext, cancellationToken);
        }
    }
}
