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
    /// <summary>
    /// This container holds all the communication components (sender/listener/bidirectional) for the Microservice.
    /// </summary>
    public class CommunicationContainer: ServiceBase<CommunicationStatistics>, 
        IServiceOriginator, IServiceLogger, IPayloadSerializerConsumer, IRequireSharedServices, IRequireScheduler, ITaskManagerProcess
    {
        /// <summary>
        /// This class is used to record the number of slots currently assigned.
        /// </summary>
        protected class ListenerSlotReservations
        {
            private int mActiveSlots;

            private readonly int mAllowedOverage;

            public ListenerSlotReservations(int allowedOverage)
            {
                mAllowedOverage = allowedOverage;
                mActiveSlots = 0;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="slots"></param>
            public void SlotsReserve(int available, int taken)
            {
                Interlocked.Add(ref mActiveSlots, taken-available);
            }

            /// <summary>
            /// This method recovers the reserved slots when the poll task completes.
            /// </summary>
            /// <param name="taken"></param>
            public void SlotsRecover(int taken)
            {
                Interlocked.Add(ref mActiveSlots, taken * -1);
            }

            public int GetAvailability(int slotsAvailable)
            {
                return slotsAvailable;// - mActiveSlots + mAllowedOverage;
            }

            /// <summary>
            /// This is the debug string used for statistics.
            /// </summary>
            public string[] Debug
            {
                get
                {
                    return new[] { "" };
                }
            }
        }


        #region Declarations
        /// <summary>
        /// This is the listener reservation collection
        /// </summary>
        protected ListenerSlotReservations mReservations { get; }
        /// <summary>
        /// This is the configuration policy.
        /// </summary>
        protected readonly CommunicationPolicy mPolicy;
        /// <summary>
        /// This collection holds the priority senders for each individual message.
        /// </summary>
        protected ConcurrentDictionary<string, List<ISender>> mMessageSenderMap;

        private List<IListener> mListener, mDeadletterListener;

        private List<ISender> mSenders;

        private ISharedService mSharedServices;

        private ISupportedMessageTypes mSupportedMessageTypes;

        /// <summary>
        /// This is the collection of supported messages that the commands expect to receive from the listeners.
        /// </summary>
        protected List<MessageFilterWrapper> mSupportedMessages;

        private int mListenerActiveReservedSlots = 0;

        private int mListenerActiveAllowedOverage = 5;

        /// <summary>
        /// This is a counter to the current listener iteration.
        /// </summary>
        private long mListenersPriorityIteration = 0;

        private ListenerClientPriorityCollection mClientCollection = null;

        private IResourceTracker mResourceTracker;

        /// <summary>
        /// This is the schedule used to recalculate the client schedule.
        /// </summary>
        protected Schedule mClientRecalculateSchedule = null;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the container.
        /// </summary>
        /// <param name="policy">This is a algorithm used to calculate the 
        /// client poll order and the number of slots to release. You can use another algorithm when necessary by substituting this class for your own.</param>
        public CommunicationContainer(CommunicationPolicy policy = null)
        {
            mPolicy = policy ?? new CommunicationPolicy();

            mReservations = new ListenerSlotReservations(policy.ListenerClientPollAlgorithm.AllowedOverage);

            mMessageSenderMap = new ConcurrentDictionary<string, List<ISender>>();
            mSupportedMessages = new List<MessageFilterWrapper>();
            mSenders = new List<ISender>();
            mListener = new List<IListener>();
            mDeadletterListener = new List<IListener>();
        }
        #endregion
        #region StatisticsRecalculate()
        /// <summary>
        /// This method recalculates the statistics for the communication holder.
        /// </summary>
        protected override void StatisticsRecalculate()
        {
            base.StatisticsRecalculate();
            try
            {
                mStatistics.ActiveReservedSlots = mListenerActiveReservedSlots;

                mStatistics.ActiveAllowedOverage = mListenerActiveAllowedOverage;

                if (mSenders != null)
                    mStatistics.Senders = mSenders.SelectMany((c) => c.Clients).Select((l) => l.Statistics).ToList();
                if (mListener != null)
                    mStatistics.Listeners = mListener.SelectMany((c) => c.Clients).Select((l) => l.Statistics).ToList();
                if (mDeadletterListener != null)
                    mStatistics.DeadLetterListeners = mDeadletterListener.SelectMany((c) => c.Clients).Select((l) => l.Statistics).ToList();

                if (mClientCollection != null)
                    mStatistics.ActiveListeners = mClientCollection.Statistics;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region StartInternal()
        /// <summary>
        /// This method gets the list of clients for the relevant listeners.
        /// </summary>
        protected override void StartInternal()
        {
            mResourceTracker = SharedServices.GetService<IResourceTracker>();

            if (mResourceTracker == null)
                throw new ArgumentNullException("ResourceTracker cannot be retrieved.");
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This method stops the collection.
        /// </summary>
        protected override void StopInternal()
        {
        }
        #endregion

        #region --> SenderAdd(ISender sender)
        /// <summary>
        /// This method adds a registered sender to the communication collection.
        /// </summary>
        /// <param name="sender">The sender to add.</param>
        public void SenderAdd(ISender sender)
        {
            mSenders.Add(sender);
            mMessageSenderMap.Clear();
        }
        #endregion
        #region --> ListenerAdd(IListener listener, bool deadLetter)
        /// <summary>
        /// This method adds a listener or a deadletter listener to the collection.
        /// </summary>
        /// <param name="listener">The listener.</param>
        /// <param name="deadLetter">True indicates that this is a deadletter listener.</param>
        public void ListenerAdd(IListener listener, bool deadLetter)
        {
            if (deadLetter)
                mDeadletterListener.Add(listener);
            else
                mListener.Add(listener);
        }
        #endregion

        //Listener
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
                mDeadletterListener.ForEach(l => ServiceStart(l));

                //Set the client priority collection.
                ListenersPriorityRecalculate().Wait();

                if (mPolicy.ListenerClientPollAlgorithm.PriorityRecalculateFrequency.HasValue)
                    //Set the reschedule priority.
                    mClientRecalculateSchedule = Scheduler.Register(async (s, cancel) => await ListenersPriorityRecalculate()
                        , mPolicy.ListenerClientPollAlgorithm.PriorityRecalculateFrequency.Value
                        , "Communication: Listeners Priority Recalculate"
                        , TimeSpan.FromMinutes(1)
                        , isInternal: true);

            }
            catch (Exception ex)
            {
                Logger.LogException("Communication/ListenersStart", ex);
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

            mDeadletterListener?.ForEach(l => ServiceStop(l));

            mListener?.ForEach(l => ServiceStop(l));
        }
        #endregion

        #region ListenersPriorityRecalculate()
        /// <summary>
        /// This method recalculate the new poll chain for the listener and deadletter listener collection 
        /// and swaps the new collection in atomically. This is done on a schedule to ensure that the collection priority
        /// does not become stale, and that the most active clients receive the most amount of attention.
        /// </summary>
        private async Task ListenersPriorityRecalculate()
        {
            if (Status != ServiceStatus.Running)
                return;

            try
            {
                //We do an atomic switch to add in a new priority list.
                var newColl = new ListenerClientPriorityCollection(mListener, mDeadletterListener
                    , mResourceTracker
                    , mPolicy.ListenerClientPollAlgorithm
                    , Interlocked.Increment(ref mListenersPriorityIteration));

                //Switch out the old collection for the new collection atomically
                var oldColl = Interlocked.Exchange(ref mClientCollection, newColl);

                //Close the old collection, note that it will be null the first time.
                oldColl?.Close();

                Logger?.LogMessage(LoggingLevel.Info, "ListenersPriorityRecalculate completed.");
            }
            catch (Exception ex)
            {
                Logger?.LogException("ListenersPriorityCalculate failed. Using the old collection.", ex);
            }
        }
        #endregion

        #region --> CanProcess()
        /// <summary>
        /// This returns true if the service is running.
        /// </summary>
        /// <returns></returns>
        public bool CanProcess()
        {
            return Status == ServiceStatus.Running && Submit != null;
        }
        #endregion
        #region --> Process(TaskManagerAvailability availability)
        /// <summary>
        /// This method processes any outstanding schedules and created a new task.
        /// This call is always single threaded.
        /// </summary>
        public void Process(ITaskAvailability availability)
        {
            //Set the current collection for this iteration.
            var collection = mClientCollection;
            //Check that we have something to do.
            if (collection == null || collection.IsClosed || collection.Count == 0)
                return;

            try
            {
                //Process each priority level in decending priority. The Levels property is already ordered correctly.
                foreach (int priorityLevel in collection.Levels)
                {
                    //Do we have slots for this level?
                    int slotsAvailable = availability.Level(priorityLevel);

                    while (CanProcess() && !collection.IsClosed)
                    {
                        int available = mReservations.GetAvailability(slotsAvailable);
                        if (available <= 0)
                            break;

                        ClientPriorityHolder context;
                        if (!mClientCollection.TakeNext(priorityLevel, available, out context))
                            break;

                        mReservations.SlotsReserve(available, context.Reserved);

                        TaskTracker tracker = TrackerCreateFromListenerContext(context);

                        Submit(tracker);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #region TrackerCreateFromListenerContext(HolderSlotContext context)
        /// <summary>
        /// This method builds the task tracker for the listener poll.
        /// </summary>
        /// <param name="context">The client holder context.</param>
        /// <returns>Returns a tracker of type listener poll.</returns>
        private TaskTracker TrackerCreateFromListenerContext(ClientPriorityHolder context)
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
                        PayloadSubmit(currentContext.Id, payload);
            };

            tracker.ExecuteComplete = (tr, failed, ex) =>
            {
                ((ClientPriorityHolder)tr.Context).Release(failed);
            };

            return tracker;
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
                        Logger?.LogException($"Payload completion error-{payload} after {(tr.Context as TransmissionPayload)?.Message?.FabricDeliveryCount} delivery attempts", exin);
                    }
                };
                //Submit the tracker to the task manager.
                Submit(tracker);
            }
            catch (Exception ex)
            {
                Logger?.LogException($"ProcessClientPayload: unhandled error {payload.Source}/{payload.Message.CorrelationKey}-{payload} after {payload.Message?.FabricDeliveryCount} delivery attempts", ex);
                payload.SignalFail();
            }
        }
        #endregion

        //Command message event handling
        #region RegisterSupportedMessages()
        /// <summary>
        /// This method registers and connects to the shared service for supported messages
        /// This event will be called whenever a command changes its status and becomes active or inactive.
        /// This is used to start or stop particular listeners.
        /// </summary>
        protected virtual void RegisterSupportedMessages()
        {
            mSupportedMessageTypes = mSharedServices.GetService<ISupportedMessageTypes>();
            if (mSupportedMessageTypes != null)
                mSupportedMessageTypes.OnCommandChange += SupportedMessageTypes_OnCommandChange;
        }
        #endregion
        #region SupportedMessageTypes_OnCommandChange(object sender, SupportedMessagesChange e)
        /// <summary>
        /// This method provides a dynamic notification to the client when the supported messages change.
        /// Clients can use this information to decide whether they should start or stop listening.
        /// </summary>
        /// <param name="sender">The sender that initiated the call.</param>
        /// <param name="e">The change parameter.</param>
        private void SupportedMessageTypes_OnCommandChange(object sender, SupportedMessagesChange e)
        {
            mSupportedMessages = e.Messages;
            mListener.ForEach((c) => c.Update(e.Messages));
            //Update the listeners as there may be new active listeners or other may have shutdown.
            ListenersPriorityRecalculate().Wait();
        }
        #endregion

        //Senders
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
                Logger.LogException("Communication/SendersStart", ex);
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
            try
            {
                //No, we want to send the message externally.
                List<ISender> messageSenders = null;
                //Get the supported message handler
                if (payload.Message.ChannelId != null && !mMessageSenderMap.TryGetValue(payload.Message.ChannelId, out messageSenders))
                    messageSenders = MessageSenderResolve(payload);

                //If there are no supported senders for the particular channelId then throw an exception
                if (messageSenders == null || messageSenders.Count == 0)
                {
                    Logger.LogMessage(LoggingLevel.Info, string.Format("Unable to resolve sender for message {0}", payload != null ? payload.Message : null), "Communication");
                    return false;
                }

                //Set the outgoing originator if not set.
                if (string.IsNullOrEmpty(payload.Message.OriginatorServiceId))
                    payload.Message.OriginatorServiceId = OriginatorId;

                //Send the message to the supported senders.
                await Task.WhenAll(messageSenders.Select(s => s.ProcessMessage(payload)));
            }
            catch (Exception ex)
            {
                Logger?.LogException(string.Format("Unable to send message {0}", payload != null ? payload.Message : null), ex);
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

        //Helpers
        #region Submit
        /// <summary>
        /// This is the action path back to the TaskManager.
        /// </summary>
        public Action<TaskTracker> Submit
        {
            get; set;
        } 
        #endregion
        #region Logger
        /// <summary>
        /// The system logger.
        /// </summary>
        public ILoggerExtended Logger
        {
            get; set;
        }
        #endregion
        #region OriginatorId
        /// <summary>
        /// The system originatorId which is appended to outgoing messages.
        /// </summary>
        public string OriginatorId
        {
            get; set;
        }
        #endregion
        #region PayloadSerializer
        /// <summary>
        /// This is the serializer used by the clients to decode the incoming payload.
        /// </summary>
        public IPayloadSerializationContainer PayloadSerializer
        {
            get; set;
        }
        #endregion
        #region SharedServices
        /// <summary>
        /// This is the shared service collection.
        /// This override extracts the supported message types function when the shared services are set.
        /// </summary>
        public ISharedService SharedServices
        {
            get
            {
                return mSharedServices;
            }
            set
            {
                mSharedServices = value;
                if (mSharedServices != null)
                    RegisterSupportedMessages();
            }
        }
        #endregion
        #region Scheduler
        /// <summary>
        /// This is the system wide scheduler. It is used to execute the client priority recalculate task.
        /// </summary>
        public IScheduler Scheduler
        {
            get; set;
        }
        #endregion

        #region ServiceStart(object service)
        /// <summary>
        /// This override sets the listener and sender specific service references.
        /// </summary>
        /// <param name="service">The service to start</param>
        protected override void ServiceStart(object service)
        {
            try
            {
                if (service is IContainerService)
                    ((IContainerService)service).Services.ForEach(s => ServiceStart(s));

                if (service is IRequireSharedServices)
                    ((IRequireSharedServices)service).SharedServices = SharedServices;

                if (service is IServiceOriginator)
                    ((IServiceOriginator)service).OriginatorId = OriginatorId;

                if (service is IServiceLogger)
                    ((IServiceLogger)service).Logger = Logger;

                if (service is IPayloadSerializerConsumer)
                    ((IPayloadSerializerConsumer)service).PayloadSerializer = PayloadSerializer;

                base.ServiceStart(service);

                if (service is IListener)
                    ((IListener)service).Update(mSupportedMessages);
            }
            catch (Exception ex)
            {
                Logger.LogException("Communication/ServiceStart" + service.ToString(), ex);
                throw;
            }
        }
        #endregion
    }
}