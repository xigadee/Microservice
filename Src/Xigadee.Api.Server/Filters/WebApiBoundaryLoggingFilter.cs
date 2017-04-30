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
    /// This class logs the incoming API requests and subsequent responses to the Azure Storage container.
    /// </summary>
    public class WebApiBoundaryLoggingFilter : WebApiCorrelationIdFilter
    {
        #region LoggingFilterLevel
        /// <summary>
        /// This is the logging level.
        /// </summary>
        [Flags]
        public enum LoggingFilterLevel
        {
            /// <summary>
            /// No logging of any information.
            /// </summary>
            None = 0,

            Exception = 1,
            Request = 2,
            Response = 4,
            RequestContent = 8,
            ResponseContent = 16,

            All = 31
        }
        #endregion
        #region Declarations
        private readonly LoggingFilterLevel mLevel;

        private readonly IMicroservice mMs;
        #endregion
        #region Constructor
        /// <summary>
        /// This filter can be used to log filtered incoming and outgoing Api messages and payloads to the Xigadee DataCollection infrastructure.
        /// </summary>
        /// <param name="ms">The Microservice.</param>
        /// <param name="correlationIdKeyName">The keyname for the correlation id. By default this is X-CorrelationId</param>
        /// <param name="level">The logging level</param>
        /// <param name="addToClaimsPrincipal">Specifies whether the correlation Id should be added to the claims principal</param>
        public WebApiBoundaryLoggingFilter(IMicroservice ms
            , LoggingFilterLevel level = LoggingFilterLevel.All
            , string correlationIdKeyName = "X-CorrelationId"
            , bool addToClaimsPrincipal = true) : base(correlationIdKeyName, addToClaimsPrincipal)
        {
            if (ms == null)
                throw new ArgumentNullException("ms","The Microservice cannot be null.");

            mMs = ms;

            mLevel = level;
        }
        #endregion

        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>
            {
                base.OnActionExecutedAsync(actionExecutedContext, cancellationToken)
            };

            if (mLevel > LoggingFilterLevel.None)
            {
                var request = actionExecutedContext.Response.RequestMessage;
                var response = actionExecutedContext.Response;
                var exception = actionExecutedContext.Exception;
                var principal = actionExecutedContext.ActionContext.RequestContext.Principal;
                var folder = DateTime.UtcNow.ToString("yyyy-MM-dd/HH/mm");

                // Retrieve the correlation id from the request and add to the response
                IEnumerable<string> correlationValues;
                string correlationId = null;
                if (request.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValues))
                    correlationId = correlationValues.FirstOrDefault();

                if (!string.IsNullOrEmpty(correlationId))
                    response.Headers.Add(mCorrelationIdKeyName, correlationId);

                //var refDirectory = mEntityContainer.GetDirectoryReference(folder);
                //var refEntityDirectory =
                //    refDirectory.GetDirectoryReference(FormatDirectoryName(correlationId, principal, request.Method, response));

                //if ((mLevel & LoggingFilterLevel.Exception) > 0 && exception != null)
                //    tasks.Add(UploadBlob(refEntityDirectory, exception, $"{correlationId}.exception.json", cancellationToken));

                //if ((mLevel & LoggingFilterLevel.Request) > 0)
                //    tasks.Add(UploadBlob(refEntityDirectory, new HttpRequestWrapper(request, principal),
                //        $"{correlationId}.request.json", cancellationToken));

                //if ((mLevel & LoggingFilterLevel.Response) > 0)
                //    tasks.Add(UploadBlob(refEntityDirectory, new HttpResponseWrapper(response),
                //        $"{correlationId}.response.json", cancellationToken));

                //if ((mLevel & LoggingFilterLevel.RequestContent) > 0)
                //    tasks.Add(UploadContentBlob(refEntityDirectory, request.Content,
                //        $"{correlationId}.request.content", cancellationToken));

                //if ((mLevel & LoggingFilterLevel.ResponseContent) > 0)
                //    tasks.Add(UploadContentBlob(refEntityDirectory, response.Content,
                //        $"{correlationId}.response.content", cancellationToken));
            }

            await Task.WhenAll(tasks);
        }


        private static string FormatDirectoryName(string correlationId, IPrincipal principal, HttpMethod requestMethod, HttpResponseMessage responseMessage)
        {
            var directoryName = "UNKNOWN";
            if (principal != null)
            {
                var apimIdentity = principal.Identity as ApimIdentity;
                if (!string.IsNullOrEmpty(apimIdentity?.Source))
                    directoryName = $"{apimIdentity.Source}.";
            }

            // CMS/GET.200.05.04785AB98F8843C7BC972F27CF1E5C68
            return $"{directoryName}/{requestMethod}.{(responseMessage != null ? (int)responseMessage.StatusCode : 0)}.{DateTime.UtcNow.ToString("ss")}.{correlationId}";
        }

    }
}
