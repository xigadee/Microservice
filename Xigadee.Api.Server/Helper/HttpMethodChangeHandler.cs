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
