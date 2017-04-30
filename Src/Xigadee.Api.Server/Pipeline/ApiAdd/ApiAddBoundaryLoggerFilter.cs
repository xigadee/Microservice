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
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Xigadee
{
    public static partial class WebApiExtensionMethods
    {
        /// <summary>
        /// This pipeline method is used to add boundary logging to the WebApi. 
        /// This means that all incoming requests and outgoing response can be logged to the boundary logger.
        /// </summary>
        /// <typeparam name="P">The IPipelineWebApi type.</typeparam>
        /// <param name="webpipe">The pipe.</param>
        /// <param name="level">The logging level.</param>
        /// <param name="correlationIdKey">The correlation key reftype name.</param>
        /// <param name="addToClaimsPrincipal">Specifies whether the correlation id should be added to the claims principal. The default is yes.</param>
        /// <returns></returns>
        public static P ApiAddBoundaryLoggerFilter<P>(this P webpipe
            , ApiBoundaryLoggingFilterLevel level = ApiBoundaryLoggingFilterLevel.All
            , string correlationIdKey = "X-CorrelationId"
            , bool addToClaimsPrincipal = true)
            where P : IPipelineWebApi
        {
            var ms = webpipe.ToMicroservice();

            var filter = new WebApiBoundaryLoggingFilter(level, correlationIdKey, addToClaimsPrincipal);

            webpipe.HttpConfig.Filters.Add(filter);

            return webpipe;
        }
    }
}
