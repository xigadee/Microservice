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
        /// This is the consructor.
        /// </summary>
        /// <param name="headerName">The HTTP key name. The default is X-XigadeeApiVersion.</param>
        public WebApiVersionHeaderFilter(string headerName = "X-XigadeeApiVersion")
        {
            mHeaderName = headerName;
        }

        /// <summary>
        /// This method adds the api version to the outgoing response.
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
