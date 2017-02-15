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
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.IO;
using Microsoft.ServiceBus;
using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the generic Azure Service Bus client holder class. 
    /// This allows a common client for difference between a QueueClient and SubscriptionClient.
    /// </summary>
    /// <typeparam name="C">The client type.</typeparam>
    public class AzureClientHolder<C, M> : ClientHolder<C, M>
        where C : ClientEntity
    {
        #region Transmit(TransmissionPayload payload, int retry = 0)
        /// <summary>
        /// This method will transmit a message.
        /// </summary>
        /// <param name="message">The message to transmit.</param>
        /// <param name="retry">The current number of retries.</param>
        public override async Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            bool tryAgain = false;
            bool fail = true;
            try
            {
                LastTickCount = Environment.TickCount;

                if (retry > MaxRetries)
                    throw new RetryExceededTransmissionException();

                var message = MessagePack(payload);
                await MessageTransmit(message);
                if (BoundaryLoggingActive)
                    Collector?.BoundaryLog(ChannelDirection.Outgoing, payload, ChannelId, Priority);
                fail = false;
            }
            catch (NoMatchingSubscriptionException nex)
            {
                //OK, this happens when the remote transmitting party has closed or recycled.
                LogException($"The sender has closed: {payload.Message.CorrelationServiceId}", nex);
                if (BoundaryLoggingActive)
                    Collector?.BoundaryLog(ChannelDirection.Outgoing, payload, ChannelId, Priority, nex);
            }
            catch (TimeoutException tex)
            {
                LogException("TimeoutException (Transmit)", tex);
                tryAgain = true;
                if (BoundaryLoggingActive)
                    Collector?.BoundaryLog(ChannelDirection.Outgoing, payload, ChannelId, Priority, tex);
            }
            catch (MessagingException dex)
            {
                //OK, something has gone wrong with the Azure fabric.
                LogException("Messaging Exception (Transmit)", dex);
                //Let's reinitialise the client
                if (ClientReset == null)
                    throw;

                ClientReset(dex);
                tryAgain = true;
            }
            catch (Exception ex)
            {
                LogException("Unhandled Exception (Transmit)", ex);
                if (BoundaryLoggingActive)
                    Collector?.BoundaryLog(ChannelDirection.Outgoing, payload, ChannelId, Priority, ex);
                throw;
            }
            finally
            {
                if (fail)
                    StatisticsInternal.ExceptionHitIncrement();
            }

            if (tryAgain)
                await Transmit(payload, ++retry);
        }
        #endregion

        #region MessagesPull(int? count, int? wait)
        /// <summary>
        /// This method pulls a set of messages from the fabric and unpacks them in to TransmissionPayload messages.
        /// </summary>
        /// <param name="count">The number of messages to pull.</param>
        /// <param name="wait">The maximum wait time.</param>
        /// <returns>Returns a list of messages or null if the request times out.</returns>
        public override async Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            List<TransmissionPayload> batch = null;
            Guid? batchId = null;
            try
            {
                var intBatch = (await MessageReceive(count, wait))?.ToList() ?? new List<M>();
                if (BoundaryLoggingActive)
                    batchId = Collector?.BoundaryBatchPoll(count ?? -1, intBatch.Count, mappingChannel ?? ChannelId, Priority);

                batch = intBatch.Select(m => TransmissionPayloadUnpack(m, Priority, mappingChannel, batchId)).ToList();
            }
            catch (MessagingException dex)
            {
                //OK, something has gone wrong with the Azure fabric.
                LogException("Messaging Exception (Pull)", dex);
                //Let's reinitialise the client
                if (ClientReset == null)
                    throw;

                ClientReset(dex);
                batch = batch ?? new List<TransmissionPayload>();
            }
            catch (TimeoutException tex)
            {
                LogException("MessagesPull (Timeout)", tex);
                batch = batch ?? new List<TransmissionPayload>();
            }

            LastTickCount = Environment.TickCount;

            return batch;
        }
        #endregion
        #region TransmissionPayloadUnpack(M message, int priority, string mappingChannel = null)
        /// <summary>
        /// This method wraps the incoming fabric message in to a generic payload.
        /// </summary>
        /// <param name="message">The incoming fabric message.</param>
        /// <returns>Returns the payload with the service message.</returns>
        protected virtual TransmissionPayload TransmissionPayloadUnpack(M message, int priority, string mappingChannel = null, Guid? batchId = null)
        {
            ServiceMessage serviceMessage = MessageUnpack(message);
            serviceMessage.ChannelPriority = priority;

            if (mappingChannel != null)
                serviceMessage.ChannelId = mappingChannel;

            //Get the payload message with the associated metadata for transmission
            var payload =  PayloadRegisterAndCreate(message, serviceMessage);

            //Get the boundary logger to log the metadata.
            if (BoundaryLoggingActive)
                Collector?.BoundaryLog(ChannelDirection.Incoming, payload, ChannelId, Priority, batchId: batchId);

            return payload;
        }
        #endregion
    }
}
