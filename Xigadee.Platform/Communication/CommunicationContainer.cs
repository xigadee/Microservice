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
    public class CommunicationContainer: ServiceBase<CommunicationStatistics>
        , IServiceOriginator, IServiceLogger, IPayloadSerializerConsumer, IRequireSharedServices, IRequireScheduler
    {
        #region Declarations
        /// <summary>
        /// This collection holds the priority senders for each individual message.
        /// </summary>
        protected ConcurrentDictionary<string, List<ISender>> mMessageSenderMap;

        private Dictionary<Guid, ClientPriorityHolder> mListenerClients;

        private List<IListener> mListener, mDeadletterListener;

        private List<ISender> mSenders;

        private ISharedService mSharedServices;

        private ISupportedMessageTypes mSupportedMessageTypes;

        private List<MessageFilterWrapper> mSupportedMessages;

        private int mListenerActiveReservedSlots = 0;

        private int mListenerActiveAllowedOverage = 5;

        private long mPriorityCollectionIteration = 0;

        private ClientPriorityCollection mClientCollection = null;

        private IResourceTracker mResourceTracker;

        protected readonly CommunicationPolicy mPolicy;

        protected Schedule mClientRecalculateSchedule = null;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the container.
        /// </summary>
        /// <param name="clientPriorityAlgorithm">This is a algorithm used to calculate the 
        /// client poll order and the number of slots to release. You can use another algorithm when necessary by substituting this class for your own.</param>
        public CommunicationContainer(CommunicationPolicy policy = null)
        {
            mPolicy = policy ?? new CommunicationPolicy();

            mMessageSenderMap = new ConcurrentDictionary<string, List<ISender>>();
            mListenerClients = new Dictionary<Guid, ClientPriorityHolder>();
            mSupportedMessages = new List<MessageFilterWrapper>();
            mSenders = new List<ISender>();
            mListener = new List<IListener>();
            mDeadletterListener = new List<IListener>();
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

        #region SenderAdd(ISender sender)
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
        #region ListenerAdd(IListener listener, bool deadLetter)
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

                mListenerClients = ClientsPopulate();
                ListenersPriorityRecalculate().Wait();

                // Flush the accumulated telemetry 
                mClientRecalculateSchedule = Scheduler.Register(async (s, cancel) => await ListenersPriorityRecalculate()
                    , mPolicy.ClientPriorityRecalculateFrequency, "Communication Listeners Priority Recalculate", TimeSpan.FromMinutes(1), isInternal: true);

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
                Scheduler.Unregister(mClientRecalculateSchedule);

            if (mClientCollection != null)
                mClientCollection.Close();

            if (mDeadletterListener != null)
                mDeadletterListener.ForEach(l => ServiceStop(l));

            if (mListener != null)
                mListener.ForEach(l => ServiceStop(l));
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

        #region Logging
        public void QueueTimeLog(Guid clientId, DateTime? EnqueuedTimeUTC)
        {
            mListenerClients[clientId].QueueTimeLog(EnqueuedTimeUTC);
        }

        public void ActiveIncrement(Guid clientId)
        {
            mListenerClients[clientId].ActiveIncrement();
        }

        public void ActiveDecrement(Guid clientId, int TickCount)
        {
            mListenerClients[clientId].ActiveDecrement(TickCount);
        }

        public void ErrorIncrement(Guid clientId)
        {
            mListenerClients[clientId].ErrorIncrement();
        }
        #endregion

        #region --> ListenerClientNext(int slotsAvailable, out HolderSlot? holderId)
        /// <summary>
        /// This method is called by the Microservice to reserve a listener for processing.
        /// </summary>
        /// <param name="slotsAvailable">The number of slots currently available.</param>
        /// <param name="holderId">The output guid of the listener.</param>
        /// <returns></returns>
        public bool ListenerClientNext(int slotsAvailable, out HolderSlotContext holderId)
        {
            holderId = null;

            //Check that we have slots available after we have taken in to accounts the slots reserved for pending polls.
            int available = slotsAvailable - mListenerActiveReservedSlots + mListenerActiveAllowedOverage;
            if (Status != ServiceStatus.Running || available <= 0 || mClientCollection == null)
                return false;

            if (mClientCollection.TakeNext(available, TaskRecoverSlots, out holderId))
            {
                Interlocked.Add(ref mListenerActiveReservedSlots, holderId.Reserved);
                return true;
            }

            return false;
        }
        #endregion
        #region TaskRecoverSlots(int slots)
        /// <summary>
        /// This method is called after the poll has completed.
        /// </summary>
        /// <param name="slots">The number of reserved slots.</param>
        private void TaskRecoverSlots(int slots)
        {
            Interlocked.Add(ref mListenerActiveReservedSlots, slots * -1);
        }
        #endregion

        #region ClientsPopulate()
        /// <summary>
        /// This method will recalculate the clients list and return a new ClientPriorityHolder collection.
        /// </summary>
        /// <returns>Returns a list of active clients.</returns>
        private Dictionary<Guid, ClientPriorityHolder> ClientsPopulate()
        {
            var tempClients = new Dictionary<Guid, ClientPriorityHolder>();

            foreach (var listener in mListener.Union(mDeadletterListener))
                if (listener.Clients != null)
                    foreach (var client in listener.Clients)
                    {
                        tempClients.Add(client.Id, new ClientPriorityHolder(mResourceTracker, client, listener.MappingChannelId));
                    }

            return tempClients;
        }
        #endregion
        #region ListenersPriorityRecalculate()
        /// <summary>
        /// This method recalculate the poll chain for the listener and deadlistener collection 
        /// and swaps the new collection in.
        /// </summary>
        public async Task ListenersPriorityRecalculate()
        {
            if (Status != ServiceStatus.Running)
                return;

            //Check that the listener clients have been created.
            if (mListenerClients == null)
                return;

            try
            {
                //We do an atomic switch to add in a new priority list.
                var newColl = new ClientPriorityCollection(mListenerClients, mPolicy.PriorityAlgorithm, Interlocked.Increment(ref mPriorityCollectionIteration));
                var oldColl = Interlocked.Exchange(ref mClientCollection, newColl);
                //This may happen the first time.
                if (oldColl != null)
                    oldColl.Close();

                Logger.LogMessage(LoggingLevel.Info, "ListenersPriorityRecalculate completed.");
            }
            catch (Exception ex)
            {
                Logger.LogException("ListenersPriorityCalculate failed. Using the old collection.", ex);
            }
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

        #region RegisterSupportedMessages()
        /// <summary>
        /// This method registers and connects to the shared service for supported messages.
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
            mListenerClients = ClientsPopulate();

            ListenersPriorityRecalculate().Wait();
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
                Logger.LogException(string.Format("Unable to send message {0}", payload != null ? payload.Message : null), ex);
                return false;
            }

            return true;
        }
        #endregion
        #region MessageSenderResolve(ServiceMessage message)
        /// <summary>
        /// This message resolves the specific handler that can process the incoming message.
        /// </summary>
        /// <param name="message">The incoming message.</param>
        /// <returns>Returns the handler or null.</returns>
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
