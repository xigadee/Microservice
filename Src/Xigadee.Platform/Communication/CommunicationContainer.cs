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
    /// <summary>
    /// This container holds all the communication components (sender/listener/bidirectional) for the Microservice.
    /// </summary>
    public partial class CommunicationContainer: ServiceContainerBase<CommunicationStatistics, CommunicationPolicy>
        , IRequireServiceOriginator, IRequireDataCollector, IRequirePayloadSerializer, IRequireSharedServices
        , IRequireScheduler, ITaskManagerProcess, IRequireSecurityService
    {
        #region Declarations
        /// <summary>
        /// This collection holds the priority senders for each individual message.
        /// </summary>
        protected ConcurrentDictionary<string, List<ISender>> mMessageSenderMap;
        /// <summary>
        /// This is the list of communication listeners.
        /// </summary>
        private List<IListener> mListener;
        /// <summary>
        /// This is the list of listeners that require polling support.
        /// </summary>
        private List<IListenerPoll> mListenerPoll = null;
        /// <summary>
        /// This is the list of communication senders.
        /// </summary>
        private List<ISender> mSenders;
        /// <summary>
        /// This is the shared service container.
        /// </summary>
        private ISharedService mSharedServices;
        /// <summary>
        /// This is the collection of supported message types.
        /// </summary>
        private ISupportedMessageTypes mSupportedMessageTypes;

        /// <summary>
        /// This is the collection of supported messages that the commands expect to receive from the listeners.
        /// </summary>
        protected List<MessageFilterWrapper> mSupportedMessages;

        /// <summary>
        /// This is a counter to the current listener iteration.
        /// </summary>
        private long mListenersPriorityIteration = 0;

        /// <summary>
        /// This is a list of active clients ordered by their priority.
        /// </summary>
        private ClientPriorityCollection mClientCollection = null;

        /// <summary>
        /// This is the resource tracker service.
        /// </summary>
        private IResourceTracker mResourceTracker;

        /// <summary>
        /// This is the schedule used to call the clients that require polling support.
        /// </summary>
        protected Schedule mClientRecalculateSchedule = null;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the container.
        /// </summary>
        /// <param name="policy">This is a algorithm used to calculate the 
        /// client poll order and the number of slots to release. You can use another algorithm when necessary by substituting this class for your own.</param>
        public CommunicationContainer(CommunicationPolicy policy = null):base(policy)
        {
            mMessageSenderMap = new ConcurrentDictionary<string, List<ISender>>();
            mSupportedMessages = new List<MessageFilterWrapper>();
            mSenders = new List<ISender>();
            mListener = new List<IListener>();
            mContainerIncoming = new Dictionary<string, Channel>();
            mContainerOutgoing = new Dictionary<string, Channel>();
        }
        #endregion
        #region StatisticsRecalculate()
        /// <summary>
        /// This method recalculates the statistics for the communication holder.
        /// </summary>
        protected override void StatisticsRecalculate(CommunicationStatistics stats)
        {
            base.StatisticsRecalculate(stats);

            if (mSenders != null)
                stats.Senders = mSenders.SelectMany((c) => c.Clients).Select((l) => l.Statistics).ToList();

            if (mListener != null)
                stats.Listeners = mListener.SelectMany((c) => c.Clients).Select((l) => l.Statistics).ToList();

            if (mClientCollection != null)
                stats.Active = mClientCollection.Statistics;
        }
        #endregion

        #region StartInternal()
        /// <summary>
        /// This method gets the list of clients for the relevant listeners.
        /// </summary>
        protected override void StartInternal()
        {
            //Get the resource tracker that will be used to reduce message in
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
            mContainerIncoming.Clear();
            mContainerOutgoing.Clear();
        }
        #endregion

        #region --> CanProcess()
        /// <summary>
        /// This returns true if the service is running.
        /// </summary>
        /// <returns></returns>
        public bool CanProcess()
        {
            return Status == ServiceStatus.Running && TaskSubmit != null;
        }
        #endregion
        #region --> Process(TaskManagerAvailability availability)
        /// <summary>
        /// This method processes any outstanding schedules and created a new task.
        /// This call is always single threaded.
        /// </summary>
        public void Process()
        {
            //Set the current collection for this iteration.
            //This could change during execution of this process.
            var collection = mClientCollection;

            //Check that we have something to do. If this is null, closed, or without any clients, then we skip polling.
            if (collection == null || collection.IsClosed || collection.Count == 0)
                return;

            if ((mListenerPoll?.Count ?? 0) > 0)
                ProcessListeners();

            //Do the past due scan to process the lower priority clients that have overdue polls first.
            if (mPolicy.ListenerClientPollAlgorithm.SupportPassDueScan)
                ProcessClients(true);

            //Process the standard client logic poll.
            ProcessClients(false);
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
        /// This method provides a dynamic notification to the communication listener when the supported messages change.
        /// Clients can use this information to decide whether they should start or stop listening.
        /// </summary>
        /// <param name="sender">The sender that initiated the call.</param>
        /// <param name="e">The change parameter.</param>
        private void SupportedMessageTypes_OnCommandChange(object sender, SupportedMessagesChange e)
        {
            mSupportedMessages = e.Messages;

            mListener.ForEach((c) => c.Update(e.Messages));

            //Update the listeners as there may be new active listeners or other may have shutdown.
            ListenersPriorityRecalculate(true).Wait();
        }
        #endregion

        //Helpers
        #region TaskSubmit
        /// <summary>
        /// This is the action path back to the TaskManager to submit a request..
        /// </summary>
        public Action<TaskTracker> TaskSubmit
        {
            get; set;
        }
        #endregion
        #region TaskAvailability
        /// <summary>
        /// This is the task availability collection
        /// </summary>
        public ITaskAvailability TaskAvailability { get; set; }
        #endregion

        #region Collector
        /// <summary>
        /// This is the system wide data collector
        /// </summary>
        public IDataCollection Collector
        {
            get; set;
        }
        #endregion
        #region OriginatorId
        /// <summary>
        /// The system originatorId which is appended to outgoing messages.
        /// </summary>
        public MicroserviceId OriginatorId
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
                SharedServicesChange(value);
            }
        }
        /// <summary>
        /// This method is called to set or remove the shared service reference.
        /// You can override your logic to safely set the shared service collection here.
        /// </summary>
        /// <param name="sharedServices">The shared service reference or null if this is not set.</param>
        protected virtual void SharedServicesChange(ISharedService sharedServices)
        {
            mSharedServices = sharedServices;
            if (mSharedServices != null)
            {
                RegisterSupportedMessages();
                mSharedServices.RegisterService<IChannelService>(this, "Channel");
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
                if (service is IRequireDataCollector)
                    ((IRequireDataCollector)service).Collector = Collector;

                if (service is IRequireServiceOriginator)
                    ((IRequireServiceOriginator)service).OriginatorId = OriginatorId;

                if (service is IRequirePayloadSerializer)
                    ((IRequirePayloadSerializer)service).PayloadSerializer = PayloadSerializer;

                if (service is IContainerService)
                    ((IContainerService)service).Services.ForEach(s => ServiceStart(s));

                if (service is IRequireSharedServices)
                    ((IRequireSharedServices)service).SharedServices = SharedServices;

                base.ServiceStart(service);

                if (service is IListener)
                    ((IListener)service).Update(mSupportedMessages);
            }
            catch (Exception ex)
            {
                Collector?.LogException("Communication/ServiceStart" + service.ToString(), ex);
                throw;
            }
        }
        #endregion
    }
}