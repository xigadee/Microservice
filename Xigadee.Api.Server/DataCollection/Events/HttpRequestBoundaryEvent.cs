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

#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json.Linq;
#endregion

namespace Xigadee
{
    public class HttpRequestBoundaryEvent : ApiBoundaryEvent
    {
        private readonly HttpRequestMessage mRequestMessage;
        private readonly IPrincipal mRequestPrincipal;

        public HttpRequestHeaders Headers => mRequestMessage.Headers;

        public HttpContentHeaders ContentHeaders => mRequestMessage.Content?.Headers;

        public HttpMethod Method => mRequestMessage.Method;

        public Uri RequestUri => mRequestMessage.RequestUri;

        public IIdentity Identity => mRequestPrincipal?.Identity;

        public string ClientIPAddress
        {
            get
            {
                try
                {
                    if (mRequestMessage.Properties.ContainsKey("MS_HttpContext"))
                        return ((HttpContextWrapper)mRequestMessage.Properties["MS_HttpContext"]).Request.UserHostAddress;

                    if (mRequestMessage.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                        return ((RemoteEndpointMessageProperty)mRequestMessage.Properties[RemoteEndpointMessageProperty.Name]).Address;
                }
                catch (Exception)
                {
                    return null;
                }

                return null;
            }
        }

        public HttpRequestBoundaryEvent(HttpRequestMessage requestMessage, IPrincipal requestPrincipal)
        {
            mRequestMessage = requestMessage;
            mRequestPrincipal = requestPrincipal;
        }
    }

}
