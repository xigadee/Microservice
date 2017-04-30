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
    /// <summary>
    /// This filter can be applied to specific method calls and interacts with the Xigadee Resource Tracker.
    /// The filter is used to limit ot stop incoming requests in a Microservice system, when there are problems
    /// with downstream resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class WebApiCircuitBreakerFilterAttribute: ActionFilterAttribute,IRequireMicroserviceConnection
    {
        public WebApiCircuitBreakerFilterAttribute()
        {
        }

        public IMicroservice Microservice { get; set; }
    }
}
