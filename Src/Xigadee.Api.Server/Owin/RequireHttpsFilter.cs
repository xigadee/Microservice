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
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Xigadee
{
    /// <summary>
    /// This filter is used to force the API to only allow secure connections.
    /// </summary>
    public class RequireHttpsFilter : IAuthenticationFilter
    {
        public bool AllowMultiple => true;

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (context.Request.RequestUri.Scheme == Uri.UriSchemeHttps)
                return Task.CompletedTask;

            var allowHttpTraffic =
                context.ActionContext.ActionDescriptor.GetCustomAttributes<AllowHttpTrafficAttribute>().FirstOrDefault() ??
                context.ActionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowHttpTrafficAttribute>().FirstOrDefault();
            
            // If we can allow http traffic then check whether it is local only and if this is local traffic 
            if (allowHttpTraffic != null && (!allowHttpTraffic.LocalOnly || context.Request.RequestUri.IsLoopback))
                return Task.CompletedTask;

            context.ErrorResult = new StatusResult(HttpStatusCode.Forbidden, context.Request, "Invalid Scheme");
            return Task.CompletedTask;
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
