#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S,P>
    {
        #region Declarations
        /// <summary>
        /// This is the tracker used to record outgoing messages.
        /// </summary>
        protected RequestTrackerContainer<CommandOutgoingRequestTracker> mTracker;
        /// <summary>
        /// This is the inititator timeout schedule.
        /// </summary>
        protected CommandTimeoutSchedule mScheduleTimeout;

        public event EventHandler<TransmissionPayload> OnTimedOutRequest;

        public event EventHandler<TransmissionPayload> OnTimedOutResponse;
        #endregion

        protected virtual void OutgoingRequestsTimeoutStart()
        {
            mScheduleTimeout = new CommandTimeoutSchedule(OutgoingRequestsProcessTimeouts, mPolicy.OutgoingRequestsTimeoutPoll,
                string.Format("{0} Command Timeout", GetType().Name));

            Scheduler.Register(mScheduleTimeout);
        }

        protected virtual void OutgoingRequestsTimeoutStop()
        {
            Scheduler.Unregister(mScheduleTimeout);
        }

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

            var tracker = (CommandOutgoingRequestTracker)mTracker.ProcessMessage(this, payloadRq);

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
            List<CommandOutgoingRequestTracker> timedOutRequests = mTracker.ProcessTimeout(h => h.Tcs.SetCanceled());

            var timeoutSchedule = schedule as TimeoutSchedule;
            if (timeoutSchedule != null)
                timeoutSchedule.TimeoutIncrement(timedOutRequests.Count);

            if (OnTimedOutRequest != null && timedOutRequests.Count > 0)
                timedOutRequests.ForEach(tr => OnTimedOutRequest(this, tr.Payload));
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

            CommandOutgoingRequestTracker holder = null;
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
