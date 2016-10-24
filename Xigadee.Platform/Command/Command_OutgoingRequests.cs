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
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {
        #region Declarations
        /// <summary>
        /// This is the tracker used to record outgoing messages.
        /// </summary>
        //protected RequestTrackerContainer<CommandOutgoingRequestTracker> mTracker;
        /// <summary>
        /// This is the inititator timeout schedule.
        /// </summary>
        protected CommandTimeoutSchedule mScheduleTimeout;
        /// <summary>
        /// This event can be subscribed for timed out requests.
        /// </summary>
        public event EventHandler<TransmissionPayload> OnTimedOutRequest;
        /// <summary>
        /// This event can be subscribed to detect unrecognised response messages.
        /// </summary>
        public event EventHandler<TransmissionPayload> OnTimedOutResponse;
        /// <summary>
        /// This is the collection of inplay messages.
        /// </summary>
        protected ConcurrentDictionary<string, OutgoingRequestTracker> mOutgoingRequests;

        public event EventHandler<OutgoingRequestTracker> DiagnosticsOnOutgoingRequest;
        public event EventHandler<OutgoingRequestTracker> DiagnosticsOnOutgoingRequestTimeout;
        public event EventHandler<OutgoingRequestTracker> DiagnosticsOnOutgoingRequestComplete;
        #endregion

        #region OutgoingRequestsStart()
        /// <summary>
        /// This method starts the outgoing request support.
        /// </summary>
        protected virtual void OutgoingRequestsInitialise()
        {
            mOutgoingRequests = new ConcurrentDictionary<string, OutgoingRequestTracker>();

            //Set a timer for aborted requests if the Task
            if (!TaskManagerTimeoutSupported)
            {
                mScheduleTimeout = new CommandTimeoutSchedule(OutgoingRequestsProcessTimeouts, mPolicy.OutgoingRequestsTimeoutPoll,
                    string.Format("{0} Command OutgoingRequests Timeout Poll", FriendlyName));

                Scheduler.Register(mScheduleTimeout);
            }

            //Check whether the ResponseId has been set, and if so then register the command.
            if (ResponseId == null)
                throw new CommandStartupException($"Command={GetType().Name}: Outgoing requests are enabled, but the ResponseId parameter has not been set");
            
            //This is the return message handler
            CommandRegister(ResponseId, OutgoingRequestResponseProcess);
        }
        #endregion
        #region OutgoingRequestsTimeoutStop()
        /// <summary>
        /// This method stops the outgoing request supports and marks any pending jobs as cancelled.
        /// </summary>
        protected virtual void OutgoingRequestsTimeoutStop()
        {
            if (mScheduleTimeout != null)
            {
                Scheduler.Unregister(mScheduleTimeout);
                mScheduleTimeout = null;
            }

            try
            {
                foreach (var key in mOutgoingRequests.Keys.ToList())
                    OutgoingRequestRemove(key, null);

                mOutgoingRequests.Clear();
                mOutgoingRequests = null;
            }
            catch (Exception ex)
            {
            }

        }
        #endregion
        #region --> OutgoingRequestResponseProcess(TransmissionPayload payload, List<TransmissionPayload> responses)
        /// <summary>
        /// This method processes the returning messages.
        /// </summary>
        /// <param name="payload">The incoming payload.</param>
        /// <param name="responses">The responses collection is not currently used.</param>
        protected virtual async Task OutgoingRequestResponseProcess(TransmissionPayload payload, List<TransmissionPayload> responses)
        {
            string id = payload?.Message?.CorrelationKey?.ToUpperInvariant(); ;

            //If there is not a correlation key then quit.
            if (id == null)
            {
                Logger?.LogMessage(LoggingLevel.Warning, "OutgoingRequestsProcessResponse - id is null");
                return;
            }

            //This method signals the request as completed or failed and sets the incoming payload.
            if (!OutgoingRequestRemove(id, payload))
            {
                try
                {
                    OnTimedOutResponse?.Invoke(this, payload);
                }
                catch (Exception ex)
                {
                    Logger?.LogException("OnTimedOutResponse", ex);
                    //We do not want to throw exceptions here.
                }
            }

            //Signal to the listener to release the message.
            payload.SignalSuccess();
        }
        #endregion

        #region ResponseId

        private MessageFilterWrapper mResponseId = null;

        /// <summary>
        /// This override will receive the incoming messages
        /// </summary>
        protected virtual MessageFilterWrapper ResponseId
        {
            get
            {
                if (ResponseChannelId == null)
                    return null;

                if (mResponseId == null)
                    mResponseId = new MessageFilterWrapper(new ServiceMessageHeader(ResponseChannelId, FriendlyName, ComponentId.ToString("N").ToUpperInvariant() )) { ClientId = OriginatorId.ExternalServiceId };

                return mResponseId;
            }
        }
        #endregion

        #region UseASPNETThreadModel
        /// <summary>
        /// This property should be set when hosted in an ASP.NET container.
        /// </summary>
        public virtual bool UseASPNETThreadModel { get; set; }
        #endregion

        #region TransmitAsync<K>...
        /// <summary>
        /// This method sends a message to the underlying dispatcher and tracks its progress.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="payloadRq">The payload to process.</param>
        /// <param name="processResponse"></param>
        /// <param name="processAsync"></param>
        /// <returns></returns>
        protected async Task<K> TransmitAsync<K>(TransmissionPayload payloadRq,
            Func<TaskStatus, TransmissionPayload, bool, K> processResponse,
            bool processAsync = false)
        {
            if (payloadRq == null)
                throw new ArgumentNullException("payloadRequest");
            if (processResponse == null)
                throw new ArgumentNullException("processPayload");

            ValidateServiceStarted();

            var tracker = OutgoingRequestTransmit(payloadRq);

            if (processAsync)
            {
                return processResponse(tracker.Tcs.Task.Status, payloadRq, true);
            }

            TransmissionPayload payloadRs = null;
            try
            {
                if (UseASPNETThreadModel)
                    payloadRs = Task.Run(async () => await tracker.Tcs.Task).Result;
                else
                    payloadRs = await tracker.Tcs.Task;
            }
            catch (Exception) { }

            var response = processResponse(tracker.Tcs.Task.Status, payloadRs, false);

            return response;
        }
        #endregion

        #region OutgoingRequestTransmit(TransmissionPayload requestPayload)
        /// <summary>
        /// This method marshalls the incoming requests from the Initiators.
        /// </summary>
        /// <param name="payload">The outgoing payload.</param>
        protected virtual OutgoingRequestTracker OutgoingRequestTransmit(TransmissionPayload payload)
        {
            //Create and register the request holder.
            string id = payload.Message.OriginatorKey.ToUpperInvariant();

            //Get the maximum processing time.
            TimeSpan processingTime = payload.MaxProcessingTime.HasValue ? 
                payload.MaxProcessingTime.Value : mPolicy.OutgoingRequestMaxProcessingTimeDefault;

            //Create and register the request holder.
            var holder = new OutgoingRequestTracker(id, payload, processingTime);

            //Add the outgoing holder to the collection
            if (!mOutgoingRequests.TryAdd(holder.Id, holder))
            {
                var errorStr = $"OutgoingRequestTransmit: Duplicate key {holder.Id}";
                Logger?.LogMessage(LoggingLevel.Error, errorStr);
                throw new OutgoingRequestTransmitException(errorStr);
            }

            //OK, add the message response type so that it will be picked up.


            //Submit the payload for processing
            TaskManager(this, holder.Payload);

            return holder;
        }
        #endregion

        #region --> OutgoingRequestsProcessTimeouts...
        /// <summary>
        /// This method is used to process any payloadRs timeouts.
        /// </summary>
        protected virtual async Task OutgoingRequestsProcessTimeouts(Schedule schedule, CancellationToken token)
        {
            if (mOutgoingRequests.IsEmpty)
                return;

            var timeoutSchedule = schedule as CommandTimeoutSchedule;

            var results = mOutgoingRequests.Where(i => i.Value.HasExpired()).ToList();
            if (results.Count == 0)
                return;

            timeoutSchedule?.TimeoutIncrement(results.Count);

            foreach (var key in results)
            {
                //Check that the object has not be removed by another process.
                TransmissionPayload payloadRq;
                if (!OutgoingRequestRemove(key.Key, null, out payloadRq))
                    continue;

                try
                {
                    OnTimedOutRequest?.Invoke(this, payloadRq);
                }
                catch (Exception ex)
                {
                    Logger?.LogException("OnTimedOutRequest", ex);
                    //We do not want to throw exceptions here.
                }
            }
        }
        #endregion

        #region OutgoingRequestRemove(string id, out CommandOutgoingRequestTracker holder)
        /// <summary>
        /// This method removes a request from the inplay collection.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <param name="holder">An output containing the holder object.</param>
        /// <returns>Returns true if successful.</returns>
        protected virtual bool OutgoingRequestRemove(string id, TransmissionPayload payloadIn)
        {
            TransmissionPayload payloadOut;
            return OutgoingRequestRemove(id, payloadIn, out payloadOut);
        }

        protected virtual bool OutgoingRequestRemove(string id, TransmissionPayload payloadIn, out TransmissionPayload payloadOut)
        {
            payloadOut = null;
            OutgoingRequestTracker holder;
            if (!mOutgoingRequests.TryRemove(id, out holder))
                return false;

            try
            {
                if (holder.Tcs != null)
                    if (payloadIn == null)
                    {
                        //This method signals that the request has been cancelled.
                        holder.Tcs.SetCanceled();
                    }
                    else
                    {
                        //This next method signal to the waiting request that it has completed.
                        holder.Tcs.SetResult(payloadIn);
                    }
            }
            catch (Exception ex)
            {
                Logger?.LogException("OutgoingRequestRemove TCS error", ex);
            }

            return true;
        }
        #endregion

        #region TaskManagerTimeoutSupported
        /// <summary>
        /// This boolean property indicates to the task manager that the command should be notified if a submitted task is cancelled.
        /// </summary>
        public virtual bool TaskManagerTimeoutSupported { get { return true; } }
        #endregion
        #region TaskManagerTimeoutNotification(string originatorKey)
        /// <summary>
        /// This method is called for processes that support direct notification from the Task Manager that a process has been
        /// cancelled.
        /// </summary>
        /// <param name="originatorKey">The is the originator tracking key.</param>
        public void TaskManagerTimeoutNotification(string originatorKey)
        {
            OutgoingRequestRemove(originatorKey, null);
            Logger?.LogMessage(LoggingLevel.Info, $"{FriendlyName} received abort notification for {originatorKey}");
        } 
        #endregion
        #region TaskManager
        /// <summary>
        /// This is the link to the Microservice dispatcher.
        /// </summary>
        public virtual Action<IService, TransmissionPayload> TaskManager
        {
            get;
            set;
        }
        #endregion

        #region Class -> OutgoingRequestTracker
        /// <summary>
        /// This class holds the parent task while it is being processed.
        /// </summary>
        public class OutgoingRequestTracker
        {
            public OutgoingRequestTracker(string id, TransmissionPayload payload, TimeSpan ttl, int? start = null)
            {
                Id = id;
                Payload = payload;
                Tcs = new TaskCompletionSource<TransmissionPayload>();
                Start = start??Environment.TickCount;
                MaxTTL = ttl;

                ResponseMessage = new ServiceMessageHeader(
                      payload.Message.ResponseChannelId
                    , payload.Message.ResponseMessageType
                    , payload.Message.ResponseActionType);
            }

            public string Id { get; }

            public TransmissionPayload Payload { get; }

            public ServiceMessageHeader ResponseMessage { get; }

            public int Start { get; }

            public TimeSpan MaxTTL { get; }

            public TimeSpan Extent { get { return ExtentNow(); } }

            private TimeSpan ExtentNow(int? now = null)
            {
                return ConversionHelper.DeltaAsTimeSpan(Start, now ?? Environment.TickCount).Value;
            }

            public bool HasExpired(int? now = null)
            {
                var extent = ExtentNow();
                return extent > MaxTTL;
            }

            /// <summary>
            /// This is the task holder for the command.
            /// </summary>
            public TaskCompletionSource<TransmissionPayload> Tcs { get; set; }

            public string Debug
            {
                get
                {
                    var debug =  $"{Id} TTL: {(MaxTTL - Extent).ToFriendlyString()} HasExpired: {(HasExpired()?"Yes":"No")}";
                    return debug;
                }
            }
        }
        #endregion

    }
}
