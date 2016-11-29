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

using System.Diagnostics;
namespace Xigadee
{
    /// <summary>
    /// This is the wrapper that holds the response data and the deserialized object.
    /// </summary>
    /// <typeparam name="RS">The response type.</typeparam>
    [DebuggerDisplay("Response={ResponseCode}/{ResponseMessage}")]
    public class ResponseWrapper<RS>
    {
        /// <summary>
        /// This is the standard message response constructor.
        /// </summary>
        /// <param name="payload">The transmission payload.</param>
        public ResponseWrapper(TransmissionPayload payload)
        {
            ResponseCode = int.Parse(payload.Message.Status);
            ResponseMessage = payload.Message.StatusDescription;
            Payload = payload;
        }
        /// <summary>
        /// This is the status only constructor. Usually called in an error scenario.
        /// </summary>
        /// <param name="responseCode">The response status code.</param>
        /// <param name="responseMessage">The response message.</param>
        public ResponseWrapper(int responseCode, string responseMessage)
        {
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
            Payload = null;
        }
        /// <summary>
        /// The response code.
        /// </summary>
        public int? ResponseCode { get; }
        /// <summary>
        /// The response message.
        /// </summary>
        public string ResponseMessage { get; }
        /// <summary>
        /// The response payload.
        /// </summary>
        public TransmissionPayload Payload { get; }
        /// <summary>
        /// The response obejct.
        /// </summary>
        public RS Response { get; set; }
    }
}
