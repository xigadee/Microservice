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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    public partial class CommunicationContainer
    {
        //Senders
        #region --> SenderAdd(ISender sender)
        /// <summary>
        /// This method adds a registered sender to the communication collection.
        /// </summary>
        /// <param name="sender">The sender to add.</param>
        public void SenderAdd(ISender sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender", "sender cannot be null.");

            if (!sender.BoundaryLoggingActive.HasValue)
                sender.BoundaryLoggingActive = mPolicy.BoundaryLoggingActiveDefault;

            mSenders.Add(sender);

            mMessageSenderMap.Clear();
        }
        #endregion

        #region SendersStart()
        /// <summary>
        /// This method starts the registered senders in the container.
        /// </summary>
        public void SendersStart()
        {
            try
            {
                mSenders.ForEach(l => ServiceStart(l));
            }
            catch (Exception ex)
            {
                Collector?.LogException("Communication/SendersStart", ex);
                throw;
            }
        }
        #endregion
        #region SendersStop()
        /// <summary>
        /// This method stops the senders in the container.
        /// </summary>
        public void SendersStop()
        {
            mSenders.ForEach(l => ServiceStop(l));
        }
        #endregion

        #region Send(TransmissionPayload requestPayload)
        /// <summary>
        /// This method transmits the messages to the relevant senders.
        /// </summary>
        /// <param name="payload">The payload messages to externalOnly</param>
        public virtual async Task<bool> Send(TransmissionPayload payload)
        {
            payload.TraceConfigure(mPolicy.TransmissionPayloadTraceEnabled);
            payload.TraceWrite("Outgoing", "CommunicationContainer/Send");

            try
            {
                Channel channel = PayloadOutgoingRedirectChecks(payload);

                PayloadOutgoingSecurity(payload);

                //No, we want to send the message externally.
                List<ISender> messageSenders = null;
                //Get the supported message handler
                if (channel != null && !mMessageSenderMap.TryGetValue(channel.Id, out messageSenders))
                    messageSenders = MessageSenderResolve(payload);

                //If there are no supported senders for the particular channelId then throw an exception
                if (messageSenders == null || messageSenders.Count == 0)
                {
                    Collector?.LogMessage(LoggingLevel.Info, string.Format("Unable to resolve sender for message {0}", payload != null ? payload.Message : null), "Communication");
                    payload.TraceWrite("Senders Unresolved", "CommunicationContainer/Send");
                    return false;
                }

                //Set the outgoing originator if not set.
                if (string.IsNullOrEmpty(payload.Message.OriginatorServiceId))
                    payload.Message.OriginatorServiceId = OriginatorId.ExternalServiceId;

                //Send the message to the supported senders.
                await Task.WhenAll(messageSenders.Select(s => s.ProcessMessage(payload)));
            }
            catch (Exception ex)
            {
                Collector?.LogException(string.Format("Unable to send message {0}", payload != null ? payload.Message : null), ex);
                payload.TraceWrite($"Exception: {ex.Message}", "CommunicationContainer/Send");
                return false;
            }

            return true;
        }
        #endregion
        #region MessageSenderResolve(TransmissionPayload payload)
        /// <summary>
        /// This message resolves the specific handler that can process the incoming message.
        /// </summary>
        /// <param name="payload">The incoming message payload.</param>
        /// <returns>Returns the supported handlers or null.</returns>
        protected virtual List<ISender> MessageSenderResolve(TransmissionPayload payload)
        {
            var message = payload.Message;

            string channelId = message.ChannelId;
            List<ISender> newMap = mSenders.Where(h => h.SupportsChannel(channelId)).ToList();

            //Make sure that the handler is queueAdded as a null value to stop further resolution attemps
            mMessageSenderMap.AddOrUpdate(channelId, newMap, (k, u) => newMap.Count == 0 ? null : newMap);

            return newMap;
        }
        #endregion

    }
}
