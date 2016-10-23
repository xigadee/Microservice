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
        public static TransmissionPayload ToResponse(this TransmissionPayload incoming)
        {
            var rsMessage = incoming.Message.ToResponse();

            rsMessage.ChannelId = incoming.Message.ResponseChannelId;
            rsMessage.ChannelPriority = incoming.Message.ResponseChannelPriority;
            rsMessage.MessageType = incoming.Message.MessageType;
            rsMessage.ActionType = "";

            return new TransmissionPayload(rsMessage);
        }
    }
}
