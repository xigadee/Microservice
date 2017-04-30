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
    public class WebApiBoundaryLoggingFilter : WebApiCorrelationIdFilter, IRequireMicroserviceConnection
    {
        #region Declarations
        private readonly ApiBoundaryLoggingFilterLevel mLevel;
        #endregion

        #region Microservice
        /// <summary>
        /// This is the reference to the Microservice.
        /// </summary>
        public IMicroservice Microservice { get; set; } 
        #endregion

        #region Constructor
        /// <summary>
        /// This filter can be used to log filtered incoming and outgoing Api messages and payloads to the Xigadee DataCollection infrastructure.
        /// </summary>
        /// <param name="ms">The Microservice.</param>
        /// <param name="correlationIdKeyName">The keyname for the correlation id. By default this is X-CorrelationId</param>
        /// <param name="level">The logging level</param>
        /// <param name="addToClaimsPrincipal">Specifies whether the correlation Id should be added to the claims principal</param>
        public WebApiBoundaryLoggingFilter(ApiBoundaryLoggingFilterLevel level = ApiBoundaryLoggingFilterLevel.All
            , string correlationIdKeyName = "X-CorrelationId"
            , bool addToClaimsPrincipal = true) : base(correlationIdKeyName, addToClaimsPrincipal)
        {
            mLevel = level;
        }
        #endregion

        /// <summary>
        /// This override logs the incoming and outgoing transaction to the Microservice Data Collector.
        /// </summary>
        /// <param name="actionExecutedContext">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the pass through task.</returns>
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (Microservice != null && mLevel > ApiBoundaryLoggingFilterLevel.None)
            {
                var bEvent = new ApiBoundaryEvent(actionExecutedContext, ChannelDirection.Incoming, mLevel);

                // Retrieve the correlation id from the request and add to the response

                IEnumerable<string> correlationValuesOut;
                if (actionExecutedContext.Response.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValuesOut))
                    bEvent.CorrelationId = correlationValuesOut.FirstOrDefault();

                //Ok, check the outbound response if the correlation id was not set on the outgoing request.
                if (string.IsNullOrEmpty(bEvent.CorrelationId))
                {
                    IEnumerable<string> correlationValuesin;
                    if (actionExecutedContext.Request.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValuesin))
                        bEvent.CorrelationId = correlationValuesin.FirstOrDefault();
                }

                Microservice.DataCollection.Write(bEvent);
            }

            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}
