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
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to create senders for various communication technologies using a 
    /// generic base class.
    /// </summary>
    /// <typeparam name="C">The client type.</typeparam>
    /// <typeparam name="M">The client message type.</typeparam>
    /// <typeparam name="H">The client holder type.</typeparam>
    public class MessagingSenderBase<C, M, H> : MessagingServiceBase<C, M, H, SenderPartitionConfig>, ISender
        where H : ClientHolder<C, M>, new()
        where C : class
    {
        #region ProcessMessage(TransmissionPayload payload)
        /// <summary>
        /// This method resolves the client and processes the message.
        /// </summary>
        /// <param name="payload">The payload to transmit.</param>
        public virtual async Task ProcessMessage(TransmissionPayload payload)
        {
            int? start = null;
            H client = null;
            try
            {
                client = ClientResolve(payload.Message.ChannelPriority);

                start = client.StatisticsInternal.ActiveIncrement();

                await client.Transmit(payload);

                payload.TraceWrite($"Sent: {client.Name}", "MessagingSenderBase/ProcessMessage");
            }
            catch (Exception ex)
            {
                LogExceptionLocation("ProcessMessage (Unhandled)", ex);
                //OK, not sure what happened here, so we need to throw the exception.
                payload.TraceWrite($"Exception: {ex.Message}", "MessagingSenderBase/ProcessMessage");
                if (client != null)
                    client.StatisticsInternal.ErrorIncrement();
                throw;
            }
            finally
            {
                if (client != null && start.HasValue)
                    client.StatisticsInternal.ActiveDecrement(start.Value);
            }
        } 
        #endregion
    }
}
