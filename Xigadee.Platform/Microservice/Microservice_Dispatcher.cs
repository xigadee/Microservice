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
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    //Dispatcher
    public partial class Microservice
    {
        protected virtual async Task<bool> Send(TransmissionPayload requestPayload)
        {
            bool isSuccess = await mCommunication.Send(requestPayload);
            if (!isSuccess)
            {
                mDataCollection.DispatcherPayloadUnresolved(requestPayload, DispatcherRequestUnresolvedReason.ChannelOutgoing);
                OnProcessRequestUnresolved(requestPayload, DispatcherRequestUnresolvedReason.ChannelOutgoing);
            }
            return isSuccess;
        }

        protected class PayloadWrapper
        {
            public PayloadWrapper(TransmissionPayload payload, int maxTransitCount)
            {
                Payload = payload;
                CurrentOptions = payload.Options;
                MaxTransitCount = maxTransitCount;
            }

            public TransmissionPayload Payload { get;}

            /// <summary>
            /// This provides custom routing instructions to the dispatcher.
            /// </summary>
            public ProcessOptions CurrentOptions { get; set; }

            public int MaxTransitCount { get; }

            public bool ExternalOnly => (Payload.Options & ProcessOptions.RouteInternal) == 0;
            public bool InternalOnly => (Payload.Options & ProcessOptions.RouteExternal) == 0;

            #region IncrementAndVerifyDispatcherTransitCount(TransmissionPayload request.Payload)
            /// <summary>
            /// This method checks that the incoming message has not exceeded the maximum number of hops.
            /// </summary>
            /// <param name="payload">The requestPayload to check.</param>
            public void IncrementAndVerifyDispatcherTransitCount()
            {
                //Increase the Dispatcher transit count.
                Payload.Message.DispatcherTransitCount = Payload.Message.DispatcherTransitCount + 1;
                //Have we exceeded the transit count for the message.
                if (Payload.Message.DispatcherTransitCount > MaxTransitCount)
                {
                    //this is not supported message - the message handler will be null if previously processed.
                    throw new TransitCountExceededException(Payload);
                }
            }
            #endregion
        }

        #region -->Execute(TransmissionPayload requestPayload)
        /// <summary>
        /// This is the core method that messages are sent to to be routed and processed.
        /// You can override this task in your service to help debug the messages that are passing 
        /// though.
        /// </summary>
        /// <param name="requestPayload">The request payload.</param>
        protected virtual async Task Execute(TransmissionPayload requestPayload)
        {
            var request = new PayloadWrapper(requestPayload, PolicyMicroservice.DispatcherTransitCountMax);

            int timerStart = StatisticsInternal.ActiveIncrement();

            bool isSuccess = true;
            try
            {
                request.Payload.Cancel.ThrowIfCancellationRequested();

                //If there is no message then we cannot continue.
                if (request.Payload == null || request.Payload.Message == null)
                    throw new ArgumentNullException("Payload or Message not present");

                //Log the telemtry for the incoming message.
                mDataCollection.DispatcherPayloadIncoming(request.Payload);

                //Check that we have not exceeded the maximum transit count.
                request.IncrementAndVerifyDispatcherTransitCount();

                //Shortcut for external messages
                if (request.ExternalOnly)
                {
                    isSuccess = await Send(request.Payload);
                    return;
                }

                var responsePayloads = new List<TransmissionPayload>();

                //Execute the message against the internal Microservice commands.
                bool resolvedInternal = await mCommands.Execute(request.Payload, responsePayloads);

                //Switch the incoming message to the outgoing collection to be processed
                //by the sender as it has not been processed internally and it is not marked
                //as internal only.
                if (!resolvedInternal && request.InternalOnly)
                {
                    //OK, we have an problem. We log this as an error and get out of here
                    mDataCollection.DispatcherPayloadUnresolved(request.Payload, DispatcherRequestUnresolvedReason.MessageHandler);

                    OnProcessRequestUnresolved(request.Payload, DispatcherRequestUnresolvedReason.MessageHandler);

                    isSuccess = PolicyMicroservice.DispatcherUnhandledMessagesIgnore;

                    return;
                }
                else if (!resolvedInternal)
                {
                    //OK, we are going to send this to the senders, first make sure that this doesn't route back in.
                    request.CurrentOptions = ProcessOptions.RouteExternal;
                    //Switch the incoming message to the response payload so that they are picked up by the senders.
                    responsePayloads.Add(request.Payload);
                }

                //OK, do we have anything to send on, both internal and external messages?
                if (responsePayloads.Count > 0)
                {
                    //Get the payloads that should be processed internally.
                    var internalPayload = responsePayloads
                        .Where((p) => ((request.CurrentOptions & ProcessOptions.RouteInternal) > 0) && mCommands.Resolve(p))
                        .ToList();

                    //OK, send the payloads off to the Dispatcher for processing.
                    internalPayload.ForEach((p) =>
                    {
                        //Mark internal only to stop any looping. 
                        //We can do this as we have checked with the command handler that they will be processed
                        p.Options = ProcessOptions.RouteInternal;
                        mTaskManager.ExecuteOrEnqueue(p, "Dispatcher");
                    });

                    //Extract the payloads that have been processed internally so that we only have the external payloads
                    var externalPayload = responsePayloads.Except(internalPayload).ToList();

                    //Send the external payload to their specific destination.
                    await Task.WhenAll(externalPayload.Select(async (p) => isSuccess &= await Send(p)));
                }

                //Set the message to success
                isSuccess &= true;
            }
            catch (TransmissionPayloadException pyex)
            {
                mDataCollection.DispatcherPayloadException(request.Payload, pyex);
                OnProcessRequestError(pyex.Payload, pyex);
            }
            catch (Exception ex)
            {
                mDataCollection.DispatcherPayloadException(request.Payload, ex);
                OnProcessRequestError(request.Payload, ex);
            }
            finally
            {
                //Signal to the underlying listener that the message can be released.
                if (request.Payload.DispatcherCanSignal)
                    request.Payload.Signal(isSuccess);

                int delta = StatisticsInternal.ActiveDecrement(timerStart);

                //Log the telemtry for the specific message channelId.
                mDataCollection.DispatcherPayloadComplete(request.Payload, delta, isSuccess);

                if (!isSuccess)
                    StatisticsInternal.ErrorIncrement();
            }
        }
        #endregion


    }
}
