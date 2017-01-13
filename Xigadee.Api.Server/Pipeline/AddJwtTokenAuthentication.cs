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
using System.Threading.Tasks;
using System.IdentityModel.Claims;
using System.Security.Claims;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace Xigadee
{
    public static partial class WebApiExtensionMethods
    {
        public static P AddJwtTokenAuthentication<P>(this P webpipe
            , JwtHashAlgorithm algo
            , string base64Secret
            , string audience = "api"
            , Action<IAuthenticationFilter> action = null)
            where P : IPipelineWebApi
        {
            var policy = new JwtTokenVerificationPolicy
            {
                  Algorithm = algo
                , Audience = audience
                , Secret = Convert.FromBase64String(base64Secret)
            };

            return webpipe.AddJwtTokenAuthentication(policy, action);
        }

        public static P AddJwtTokenAuthentication<P>(this P webpipe
           , Func<IEnvironmentConfiguration, JwtTokenVerificationPolicy> creator
           , Action<IAuthenticationFilter> action = null)
           where P : IPipelineWebApi
        {
            var policy = creator(webpipe.Configuration);

            return webpipe.AddJwtTokenAuthentication(policy, action);
        }

        public static P AddJwtTokenAuthentication<P>(this P webpipe
           , JwtTokenVerificationPolicy policy
           , Action<IAuthenticationFilter> action = null)
           where P : IPipelineWebApi
        {
            var filter = new JwtAuthenticationFilter(policy);

            action?.Invoke(filter);

            webpipe.HttpConfig.Filters.Add(filter);

            return webpipe;
        }

    }

}
