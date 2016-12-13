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
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    public partial class CommunicationContainer
    {
        //Listener
        #region --> ListenerAdd(IListener listener)
        /// <summary>
        /// This method adds a listener or a deadletter listener to the collection.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void ListenerAdd(IListener listener)
        {
            mListener.Add(listener);
        }
        #endregion

        #region ListenersStart()
        /// <summary>
        /// This method starts the registered listeners and deadletter listeners.
        /// </summary>
        public void ListenersStart()
        {
            mSupportedMessages = mSupportedMessageTypes.SupportedMessages;

            try
            {
                mListener.ForEach(l => ServiceStart(l));

                //Create the client priority collection.
                ListenersPriorityRecalculate(true).Wait();

                if (mPolicy.ListenerClientPollAlgorithm.PriorityRecalculateFrequency.HasValue)
                    //Set the reschedule priority.
                    mClientRecalculateSchedule = Scheduler.Register(async (s, cancel) => await ListenersPriorityRecalculate(false)
                        , mPolicy.ListenerClientPollAlgorithm.PriorityRecalculateFrequency.Value
                        , "Communication: Listeners Priority Recalculate"
                        , TimeSpan.FromMinutes(1)
                        , isInternal: true);

            }
            catch (Exception ex)
            {
                Collector?.LogException("Communication/ListenersStart", ex);
                throw;
            }
        }
        #endregion
        #region ListenersStop()
        /// <summary>
        /// This method stops the listeners and deadletter listeners.
        /// </summary>
        public void ListenersStop()
        {
            if (mClientRecalculateSchedule != null)
                Scheduler?.Unregister(mClientRecalculateSchedule);

            mClientCollection?.Close();

            mListener?.ForEach(l => ServiceStop(l));
        }
        #endregion

        #region ListenersPriorityRecalculate(bool rebuild = true)
        /// <summary>
        /// This method recalculate the new poll chain for the listener and deadletter listener collection 
        /// and swaps the new collection in atomically. This is done on a schedule to ensure that the collection priority
        /// does not become stale, and that the most active clients receive the most amount of attention.
        /// </summary>
        /// <param name="rebuild">This boolean property specifies whether the collection should be rebuilt or reordered. True rebuilds the collection.</param>
        private async Task ListenersPriorityRecalculate(bool rebuild = true)
        {
            if (Status != ServiceStatus.Running)
                return;

            if (!rebuild && mClientCollection != null)
                mClientCollection.Reprioritise();

            try
            {
                //We do an atomic switch to add in a new priority list.
                var newColl = new ClientPriorityCollection(mListener
                    , mResourceTracker
                    , mPolicy.ListenerClientPollAlgorithm
                    , Interlocked.Increment(ref mListenersPriorityIteration));

                //Switch out the old collection for the new collection atomically
                var oldColl = Interlocked.Exchange(ref mClientCollection, newColl);

                //Close the old collection, note that it will be null the first time.
                oldColl?.Close();

                Collector?.LogMessage(LoggingLevel.Trace, $"ListenersPriorityRecalculate completed {mListenersPriorityIteration}.");
            }
            catch (Exception ex)
            {
                Collector?.LogException("ListenersPriorityCalculate failed. Using the old collection.", ex);
            }
        }
        #endregion

        #region ProcessClients(bool pastDue)
        /// <summary>
        /// This method processes the active clients in order of their priority an allocates available slots to them.
        /// </summary>
        /// <param name="pastDue">A boolean that specifies to the polling logic to poll any clients that have waited longer then the maximum time. 
        /// This stops higher priority clients hogging all the processing bandwidth.</param>
        protected void ProcessClients(bool pastDue)
        {
            try
            {
                //Process each priority level in decending priority. The Levels property is already ordered correctly.
                var currentColl = mClientCollection;
                //Get a holder in the priority needed.
                foreach (ClientPriorityHolder context in currentColl.TakeNext(TaskAvailability, pastDue))
                {
                    TrackerSubmitFromClientPriorityHolder(context);
                    //Check whether we can continue. Note the current collection may be rebuilt and closed 
                    //during this poll.
                    if (!CanProcess() || currentColl.IsClosed)
                        break;
                }
            }
            catch (Exception ex)
            {
                Collector?.LogException("CommunicationContainer/Process", ex);
                throw;
            }
        }
        #endregion

        #region TrackerSubmitFromClientPriorityHolder(ClientPriorityHolder context)
        /// <summary>
        /// This method builds the task tracker for the listener poll.
        /// </summary>
        /// <param name="context">The client priority holder context.</param>
        /// <returns>Returns a tracker of type listener poll.</returns>
        private void TrackerSubmitFromClientPriorityHolder(ClientPriorityHolder context)
        {
            TaskTracker tracker = new TaskTracker(TaskTrackerType.ListenerPoll, TimeSpan.FromSeconds(30))
            {
                Priority = TaskTracker.PriorityInternal,
                Context = context,
                Name = context.Name
            };

            tracker.Execute = async t =>
            {
                var currentContext = ((ClientPriorityHolder)tracker.Context);

                var payloads = await currentContext.Poll();
                
                if (payloads != null && payloads.Count > 0)
                    foreach (var payload in payloads)
                        PayloadSubmit(currentContext.ClientId, payload);
            };

            tracker.ExecuteComplete = (tr, failed, ex) =>
            {
                var currentContext = ((ClientPriorityHolder)tr.Context);
                TaskAvailability.ReservationRelease(currentContext.Id);
                currentContext.Release(failed);
            };

            TaskSubmit(tracker);
        }
        #endregion


        #region PayloadSubmit(ClientHolder client, TransmissionPayload payload)
        /// <summary>
        /// This method processes an individual payload returned from a client.
        /// </summary>
        /// <param name="clientId">The originating client.</param>
        /// <param name="payload">The payload.</param>
        private void PayloadSubmit(Guid clientId, TransmissionPayload payload)
        {
            try
            {
                if (payload.Message.ChannelPriority < 0)
                    payload.Message.ChannelPriority = 0;

                mClientCollection.QueueTimeLog(clientId, payload.Message.EnqueuedTimeUTC);
                mClientCollection.ActiveIncrement(clientId);

                //Verify the incoming payload with the security container.
                PayloadIncomingSecurity(payload);

                //Do we need to redirect the payload based on the redirect/rewrite rules.
                PayloadIncomingRedirectCheck(payload);

                TaskTracker tracker = TaskManager.TrackerCreateFromPayload(payload, payload.Source);

                tracker.ExecuteComplete = (tr, failed, ex) =>
                {
                    try
                    {
                        var contextPayload = tr.Context as TransmissionPayload;

                        mClientCollection.ActiveDecrement(clientId, tr.TickCount);

                        if (failed)
                            mClientCollection.ErrorIncrement(clientId);

                        contextPayload.Signal(!failed);
                    }
                    catch (Exception exin)
                    {
                        Collector?.LogException($"Payload completion error-{payload} after {(tr.Context as TransmissionPayload)?.Message?.FabricDeliveryCount} delivery attempts", exin);
                    }
                };

                //Submit the tracker to the task manager.
                TaskSubmit(tracker);
            }
            catch (Exception ex)
            {
                Collector?.LogException($"ProcessClientPayload: unhandled error {payload.Source}/{payload.Message.CorrelationKey}-{payload} after {payload.Message?.FabricDeliveryCount} delivery attempts", ex);
                payload.SignalFail();
            }
        }
        #endregion
    }
}
