#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This command is the base implementation that allows commands to be sent from a client to a remote Microservice.
    /// The command will hold the incoming thread until the response is received, or the request times out.
    /// The initiator can also be used to send async messages and respond immediately.
    /// </summary>
    public abstract class MessageInitiatorBase<RT,S> : MessageHandlerBase<S>, IMessageInitiator
        where RT : MessageInitiatorRequestTracker, new()
        where S : MessageInitiatorStatistics, new()
    {
        #region Declarations
        /// <summary>
        /// This is the tracker used to record outgoing messages.
        /// </summary>
        protected RequestTrackerContainer<RT> mTracker;
        /// <summary>
        /// This is the inititator timeout schedule.
        /// </summary>
        protected TimeoutSchedule mScheduleTimeout;
        #endregion

        #region Events
        public event EventHandler<TransmissionPayload> OnTimedOutRequest;
        public event EventHandler<TransmissionPayload> OnTimedOutResponse;
        #endregion

        #region Constructor
        protected MessageInitiatorBase()
        {
            mTracker = new RequestTrackerContainer<RT>(
                (id, payload, holder) => Dispatcher(this, payload), keyMaker: (s, tp) => tp.Message.OriginatorKey);

            UseASPNETThreadModel = true;
        }
        #endregion

        protected override void StartInternal()
        {
            base.StartInternal();

            try
            {
                mScheduleTimeout = new TimeoutSchedule(OutgoingRequestsProcessTimeouts,
                    string.Format("{0} Initiator Timeout", GetType().Name))
                {
                    InitialWait = TimeSpan.FromSeconds(10),
                    Frequency = TimeSpan.FromSeconds(5)
                };

                Scheduler.Register(mScheduleTimeout);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void StopInternal()
        {
            Scheduler.Unregister(mScheduleTimeout);

            base.StopInternal();
        }

        #region Dispatcher
        /// <summary>
        /// This is the link to the Microservice dispatcher.
        /// </summary>
        public Action<IService, TransmissionPayload> Dispatcher
        {
            get;
            set;
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
                throw new ArgumentNullException(nameof(payloadRq));
            if (processPayload == null)
                throw new ArgumentNullException(nameof(processPayload));

            ValidateServiceStarted();

            //TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            var tracker = (RT)mTracker.ProcessMessage(this, payloadRq);

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
            catch (Exception ex)
            {
            }

            var response = processPayload(tracker.Tcs.Task.Status, payloadRs, false);

            return response;
        } 
        #endregion

        #region ResponseId
        /// <summary>
        /// This is the message filter that it used to tag the incoming payloadRs messages for the 
        /// initiator.
        /// </summary>
        protected abstract MessageFilterWrapper ResponseId { get; }
        #endregion
        #region Scheduler
        /// <summary>
        /// This is the scheduler. It is needed to process request timeouts.
        /// </summary>
        public virtual IScheduler Scheduler
        {
            get; set;
        } 
        #endregion
        #region CommandsRegister()
        /// <summary>
        /// This method registers the payloadRs channel used to receive the message 
        /// from the remote Microservice.
        /// </summary>
        public override void CommandsRegister()
        {
            //Check whether the ResponseId has been set, and if so then register the command.
            if (ResponseId != null)
                CommandRegister(ResponseId, ProcessResponse);
        }
        #endregion

        #region ProcessTimeout()
        /// <summary>
        /// This method is used to process any payloadRs timeouts.
        /// </summary>
        public virtual async Task OutgoingRequestsProcessTimeouts(Schedule schedule, CancellationToken token)
        {
            List<RT> timedOutRequests = mTracker.ProcessTimeout(h=>h.Tcs.SetCanceled());

            var timeoutSchedule = schedule as TimeoutSchedule;
            if (timeoutSchedule != null)
                timeoutSchedule.TimeoutIncrement(timedOutRequests.Count);

            if (OnTimedOutRequest != null && timedOutRequests.Count > 0)
                timedOutRequests.ForEach(tr => OnTimedOutRequest(this, tr.Payload));
        } 
        #endregion
        #region ProcessResponse(TransmissionPayload payload, List<TransmissionPayload> responses)
        /// <summary>
        /// This method processes the returning messages.
        /// </summary>
        /// <param name="payload">The incoming payload.</param>
        /// <param name="responses">The responses collection is not currently used.</param>
        protected virtual async Task ProcessResponse(TransmissionPayload payload, List<TransmissionPayload> responses)
        {
            string id = payload.Message.CorrelationKey;

            //If there is not a correlation key then quit.
            if (id == null)
                return;

            RT holder = null;
            if (mTracker.Remove(id, out holder))
            {
                //This next method signal to the waiting request that it has completed.
                if (holder.Tcs != null)
                    holder.Tcs.SetResult(payload);
            }
            else
            {
                if (OnTimedOutResponse != null)
                    OnTimedOutResponse(this, payload);
            }

            //Signal to the listener to release the message.
            payload.SignalSuccess();
        }
        #endregion
    }
}
