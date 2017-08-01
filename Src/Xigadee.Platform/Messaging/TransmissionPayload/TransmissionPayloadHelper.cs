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
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This class provides static extension methods for the transmission payload.
    /// </summary>
    public static class TransmissionPayloadHelper
    {
        #region ToResponse(this TransmissionPayload incoming)
        /// <summary>
        /// This helper method turns round an incoming payload request in to its corresponding response payload.
        /// </summary>
        /// <param name="incoming">The incoming payload.</param>
        /// <returns></returns>
        public static TransmissionPayload ToResponse(this TransmissionPayload incoming)
        {
            var m = incoming.Message;
            var rsMessage = m.ToResponse();

            rsMessage.ChannelId = m.ResponseChannelId;
            rsMessage.ChannelPriority = m.ResponseChannelPriority;
            rsMessage.MessageType = m.ResponseMessageType;
            rsMessage.ActionType = m.ResponseActionType;

            var outgoing = new TransmissionPayload(rsMessage, traceEnabled: incoming.TraceEnabled);

            if (incoming.TraceEnabled)
                outgoing.TraceSet(new TransmissionPayloadTraceEventArgs());

            return outgoing;
        }
        #endregion
        #region CanRespond(this TransmissionPayload incoming)
        /// <summary>
        /// Determines whether the payload instance has a response channel set.
        /// </summary>
        public static bool CanRespond(this TransmissionPayload incoming)
        {
            var m = incoming.Message;

            return !string.IsNullOrEmpty(m.ResponseChannelId);
        }
        #endregion

        /// <summary>
        /// Traces the set.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="hello">The hello.</param>
        public static void TraceSet(this TransmissionPayload payload, string hello)
        {

        }
    }
}
