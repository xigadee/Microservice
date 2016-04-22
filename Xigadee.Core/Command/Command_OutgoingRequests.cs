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
    public abstract partial class CommandBase<S,P>
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
        #endregion

        #region OutgoingRequestsTimeoutStart()
        /// <summary>
        /// This method starts the outgoing request support.
        /// </summary>
        protected virtual void OutgoingRequestsTimeoutStart()
        {
            mOutgoingRequests = new ConcurrentDictionary<string, OutgoingRequestTracker>();

            mScheduleTimeout = new CommandTimeoutSchedule(OutgoingRequestsProcessTimeouts, mPolicy.OutgoingRequestsTimeoutPoll,
                string.Format("{0} Command OutgoingRequests Timeout Poll", FriendlyName));

            Scheduler.Register(mScheduleTimeout);
        }
        #endregion
        #region OutgoingRequestsTimeoutStop()
        /// <summary>
        /// This method stops the outgoing request supports and marks any pending jobs as cancelled.
        /// </summary>
        protected virtual void OutgoingRequestsTimeoutStop()
        {
            Scheduler.Unregister(mScheduleTimeout);
        }
        #endregion

        #region OutgoingRequestRegister(R holder)
        /// <summary>
        /// This method register a holder with the id specified in the object.
        /// </summary>
        /// <param name="holder">The holder to to add.</param>
        protected virtual void OutgoingRequestRegister(OutgoingRequestTracker holder)
        {
            if (!mOutgoingRequests.TryAdd(holder.Id, holder))
                throw new Exception("Duplicate key");
        }
        #endregion
        #region OutgoingRequestRemove(string id)
        /// <summary>
        /// This method removes a request from the inplay collection.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <returns>Returns true if successful.</returns>
        protected virtual bool OutgoingRequestRemove(string id)
        {
            OutgoingRequestTracker holder;
            return OutgoingRequestRemove(id, out holder);
        }
        #endregion
        #region OutgoingRequestRemove(string id, out CommandOutgoingRequestTracker holder)
        /// <summary>
        /// This method removes a request from the inplay collection.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <param name="holder">An output containing the holder object.</param>
        /// <returns>Returns true if successful.</returns>
        protected virtual bool OutgoingRequestRemove(string id, out OutgoingRequestTracker holder)
        {
            return mOutgoingRequests.TryRemove(id, out holder);
        }
        #endregion

        #region ChannelId
        /// <summary>
        /// The channel id.
        /// </summary>
        public virtual string ChannelId { get; set; }
        #endregion
        #region ResponseChannelId
        /// <summary>
        /// This is the channel used for the payloadRs message.
        /// </summary>
        public virtual string ResponseChannelId { get; set; }
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
        /// <param name="processPayload"></param>
        /// <param name="processAsync"></param>
        /// <returns></returns>
        protected async Task<K> TransmitAsync<K>(TransmissionPayload payloadRq,
            Func<TaskStatus, TransmissionPayload, bool, K> processPayload,
            bool processAsync = false)
        {
            if (payloadRq == null)
                throw new ArgumentNullException("payloadRequest");
            if (processPayload == null)
                throw new ArgumentNullException("processPayload");

            ValidateServiceStarted();

            //TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            var tracker = OutgoingRequestProcessMessage(this, payloadRq);

            if (processAsync)
            {
                return processPayload(tracker.Tcs.Task.Status, payloadRq, true);
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

            var response = processPayload(tracker.Tcs.Task.Status, payloadRs, false);

            return response;
        }
        #endregion

        #region --> OutgoingRequestsProcessTimeouts()
        /// <summary>
        /// This method is used to process any payloadRs timeouts.
        /// </summary>
        public virtual async Task OutgoingRequestsProcessTimeouts(Schedule schedule, CancellationToken token)
        {
            if (mOutgoingRequests.IsEmpty)
                return;

            var timeoutSchedule = schedule as TimeoutSchedule;

            var results = mOutgoingRequests.Where(i => i.Value.HasExpired()).ToList();
            if (results.Count == 0)
                return;

            timeoutSchedule?.TimeoutIncrement(results.Count);

            foreach (var key in results)
            {
                //Check that the object has not be removed by another process.
                OutgoingRequestTracker holder;
                if (!OutgoingRequestRemove(key.Key, out holder))
                    continue;

                try
                {
                    holder.Tcs.SetCanceled();
                }
                catch (Exception ex)
                {
                    Logger?.LogException("OutgoingRequestsProcessTimeouts TCS SetCancelled", ex);
                    //We do not want to throw exceptions here.
                }

                try
                {
                    OnTimedOutRequest?.Invoke(this, holder.Payload);
                }
                catch (Exception ex)
                {
                    Logger?.LogException("OnTimedOutRequest", ex);
                    //We do not want to throw exceptions here.
                }
            }
        }
        #endregion
        #region --> OutgoingRequestsProcessResponse(TransmissionPayload payload, List<TransmissionPayload> responses)
        /// <summary>
        /// This method processes the returning messages.
        /// </summary>
        /// <param name="payload">The incoming payload.</param>
        /// <param name="responses">The responses collection is not currently used.</param>
        protected virtual async Task OutgoingRequestsProcessResponse(TransmissionPayload payload, List<TransmissionPayload> responses)
        {
            string id = payload.Message.CorrelationKey;

            //If there is not a correlation key then quit.
            if (id == null)
                return;

            OutgoingRequestTracker holder = null;
            if (OutgoingRequestRemove(id, out holder))
            {
                try
                {
                    //This next method signal to the waiting request that it has completed.
                    if (holder.Tcs != null)
                        holder.Tcs.SetResult(payload);
                }
                catch (Exception ex)
                {
                    Logger?.LogException("OutgoingRequestsProcessResponse TCS SetResult", ex);
                }
            }
            else
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

        #region OutgoingRequestHolderCreate(IService caller, TransmissionPayload payload)
        /// <summary>
        /// This method creates the holder that contains the message currently being processed or queued for processing.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="payload">The payload to process.</param>
        /// <returns>Returns a new holder of the specific type.</returns>
        protected virtual OutgoingRequestTracker OutgoingRequestHolderCreate(IService caller, TransmissionPayload payload)
        {
            //Get a new id.
            string id = OutgoingRequestKeyMaker(caller, payload);

            TimeSpan processingTime = payload.MaxProcessingTime.HasValue ? payload.MaxProcessingTime.Value : mPolicy.OutgoingRequestMaxProcessingTimeDefault;

            //Create and register the request holder.
            var holder = new OutgoingRequestTracker(id, payload,processingTime);

            return holder;
        }
        #endregion
        #region OutgoingRequestKeyMaker(IService caller, TransmissionPayload payload)
        /// <summary>
        /// This method formats the key used to hold the priority processes.
        /// </summary>
        /// <param name="caller">The calling object.</param>
        /// <returns>Returns a formatted string containing both parts.</returns>
        protected virtual string OutgoingRequestKeyMaker(IService caller, TransmissionPayload payload)
        {
            string value = payload?.Message?.OriginatorKey;

            if (string.IsNullOrEmpty(value))
                value = string.Format("{0}|{1}", (caller == null ? "" : caller.GetType().Name), Guid.NewGuid().ToString("N")).ToLowerInvariant();

            return value;
        }
        #endregion

        #region OutgoingRequestProcessMessage(IService caller, TransmissionPayload requestPayload)
        /// <summary>
        /// This method marshalls the incoming requests from the Initiators.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="message">The message to process.</param>
        protected virtual OutgoingRequestTracker OutgoingRequestProcessMessage(IService caller, TransmissionPayload payload)
        {
            //Create and register the request holder.
            var holder = OutgoingRequestHolderCreate(caller, payload);

            OutgoingRequestRegister(holder);

            //Submit the payload for processing
            Dispatcher(this, holder.Payload);

            return holder;
        }
        #endregion

        #region Class -> OutgoingRequestTracker
        /// <summary>
        /// This class holds the parent task while it is being processed.
        /// </summary>
        protected class OutgoingRequestTracker
        {
            public OutgoingRequestTracker(string id, TransmissionPayload payload, TimeSpan ttl, int? start = null)
            {
                Id = id;
                Payload = payload;
                Tcs = new TaskCompletionSource<TransmissionPayload>();
                Start = start??Environment.TickCount;
                MaxTTL = ttl;
            }

            public string Id { get; }

            public TransmissionPayload Payload { get; }

            public int Start { get; }

            public TimeSpan MaxTTL { get; }

            public TimeSpan Extent { get { return ExtentNow(); } }

            private TimeSpan ExtentNow(int? now = null)
            {
                return ConversionHelper.DeltaAsTimeSpan(Start, now ?? Environment.TickCount).Value;
            }

            public bool HasExpired(int? now = null)
            {
                return ExtentNow(now) > MaxTTL;
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
