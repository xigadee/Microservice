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

#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the default persistence event.
    /// </summary>
    /// <seealso cref="Xigadee.PayloadLogEvent" />
    public class PersistencePayloadLogEvent : PayloadLogEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistencePayloadLogEvent"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="level">The level.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="processingTime">The processing time.</param>
        public PersistencePayloadLogEvent(TransmissionPayload payload, RepositoryHolder request, RepositoryHolder response, LoggingLevel? level = null
            , Exception ex = null, DispatcherLoggerDirection? direction = null, TimeSpan? processingTime = null)
            :base(payload, level, ex, direction, processingTime)
        {
            if (request != null)
            {
                RequestTraceId = request.TraceId;

                if (request.Settings != null)
                {
                    BatchId = request.Settings.BatchId;
                    Prefer = request.Settings.Prefer;
                    Headers = request.Settings.Headers;
                }
            }

            if (response != null)
            {
                ResponseCode = response.ResponseCode;
                ResponseMessage = response.ResponseMessage;
            }
        }
        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        public string BatchId { get; set; }

        /// <summary>
        /// Gets or sets the request trace identifier from the incoming request.
        /// </summary>
        public string RequestTraceId { get; set; }
        /// <summary>
        /// Gets or sets the prefer headers.
        /// </summary>
        public Dictionary<string, string> Prefer { get; set; }
        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        public virtual int ResponseCode { get; set; }
        /// <summary>
        /// Gets or sets the response message.
        /// </summary>
        public virtual string ResponseMessage { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format("{0} TraceId={1} BatchId={2} ReponseCode={3} ReponseMessage={4}", base.Message, RequestTraceId, BatchId, ResponseCode, ResponseMessage);
            }
            set
            {
                base.Message = value;
            }
        }
        /// <summary>
        /// Gets the additional data for the request.
        /// </summary>
        public override Dictionary<string, string> AdditionalData
        {
            get
            {
                var additionalData = base.AdditionalData;
                additionalData["RequestTraceId"] = RequestTraceId;
                additionalData["BatchId"] = BatchId;
                additionalData["ResponseCode"] = ResponseCode.ToString();
                additionalData["ResponseMessage"] = ResponseMessage;
                return additionalData;
            }
        }
    }
}
