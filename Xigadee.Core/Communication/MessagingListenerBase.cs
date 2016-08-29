#region using
using System;
using System.Collections.Generic;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the generic listener class for messaging services.
    /// </summary>
    /// <typeparam name="C">The client type.</typeparam>
    /// <typeparam name="M">The client message type.</typeparam>
    /// <typeparam name="H">The clientholder type.</typeparam>
    public class MessagingListenerBase<C, M, H> : MessagingServiceBase<C, M, H, ListenerPartitionConfig>, IListener, IRequireSharedServices
        where H : ClientHolder<C, M>, new()
    {
        #region Declarations
        private object syncObject = new object();
        /// <summary>
        /// This is the supported list of message types for the client.
        /// </summary>
        protected List<MessageFilterWrapper> mSupportedMessageTypes;
        /// <summary>
        /// This id is for the specific client mappings.
        /// </summary>
        protected string mMappingChannelId;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="channelId">The string based channel id.</param>
        /// <param name="priorityPartitions">The number of priority channels. Null denotes a single channel of priority one.</param>
        /// <param name="defaultTimeout">This is the default timeout for incoming messages. By default this is set to 1 minute if left blank.</param>
        public MessagingListenerBase(string channelId
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , string mappingChannelId = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , IBoundaryLogger boundaryLogger = null)
            : base(channelId, priorityPartitions, boundaryLogger)
        {
            mSupportedMessageTypes = new List<MessageFilterWrapper>();
            mMappingChannelId = mappingChannelId;
            ResourceProfiles = resourceProfiles == null?new List<ResourceProfile>(): resourceProfiles.ToList();
        }
        #endregion

        #region ResourceProfiles
        /// <summary>
        /// This is the list of resource profiles that the Listener can be throttled on.
        /// </summary>
        public List<ResourceProfile> ResourceProfiles { get; set; } 
        #endregion

        #region SharedServices
        /// <summary>
        /// This is the shared service connector
        /// </summary>
        public virtual ISharedService SharedServices
        {
            get; set;
        }
        #endregion

        #region Update(List<MessageFilterWrapper> supported)
        /// <summary>
        /// This method is called with the list of currently supported message type supported by the Microservice.
        /// This can be used to filter the incoming messages where this is supported by the transport mechanism.
        /// The client should decide whether it should be active based on this list.
        /// </summary>
        public virtual void Update(List<MessageFilterWrapper> supported)
        {
            var newList = supported.Where((m) => m.Header.ChannelId == (mMappingChannelId ?? ChannelId)).ToList();
            var oldList = mSupportedMessageTypes;

            ClientsValidate(oldList, newList);
        }
        #endregion

        #region ClientCreate()
        /// <summary>
        /// This override sets the default processing time to the client for incoming messages.
        /// </summary>
        /// <returns>Returns the new client.</returns>
        protected override H ClientCreate(ListenerPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.MessageMaxProcessingTime = partition.PayloadMaxProcessingTime;
            client.FabricMaxMessageLock = partition.FabricMaxMessageLock;
            client.MappingChannelId = MappingChannelId;

            client.SupportsRateLimiting = partition.SupportsRateLimiting;
            client.Weighting = partition.PriorityWeighting;

            client.Start = () =>
            {
                client.Client = client.ClientCreate();

                client.IsActive = true;
            };

            client.Stop = () =>
            {
                client.IsActive = false;

                client.ClientClose();
            };
            
            return client;
        }
        #endregion

        #region PriorityPartitions
        /// <summary>
        /// This property contains the supported priority partitions for the Listener.
        /// </summary>
        public int[] PriorityPartitions
        {
            get { return null; }
        }
        #endregion
        #region MappingChannelId
        /// <summary>
        /// This is the mapping channel id.
        /// </summary>
        public string MappingChannelId
        {
            get
            {
                return mMappingChannelId;
            }
        } 
        #endregion

        #region ClientsValidate(List<MessageFilterWrapper> oldList, List<MessageFilterWrapper> newList)
        /// <summary>
        /// This method is used to revalidate the clients when a message type is enabled or disabled.
        /// </summary>
        protected virtual void ClientsValidate(List<MessageFilterWrapper> oldList, List<MessageFilterWrapper> newList)
        {
            if (Status != ServiceStatus.Running)
                return;

            //We only want to process a single thread at this point.
            lock (syncObject)
            {
                mSupportedMessageTypes = newList;

                if (newList == null || newList.Count == 0)
                {
                    ClientsStop();
                    return;
                }

                if ((oldList == null || oldList.Count == 0) && newList.Count > 0)
                {
                    ClientsStart();
                    return;
                }

                var deltaNew = newList.Except(oldList).ToList();
                var deltaOld = oldList.Except(newList).ToList();
                //Ok, there is a small change. Let the clients know there is additional messages to filter on
                //and let them work out how to do it themselves.
                if (deltaNew.Count > 0 || deltaOld.Count > 0)
                {
                    foreach (var client in mClients.Values)
                        ClientValidate(client, newList);
                }
            }
        }
        /// <summary>
        /// This method triggers a revalidates of the particular client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="newList">The new list of message filter wrappers.</param>
        protected virtual void ClientValidate(H client, List<MessageFilterWrapper> newList)
        {
            client.ClientRefresh();
        }
        #endregion

        #region ClientsStart()
        /// <summary>
        /// This method creates the clients.
        /// </summary>
        protected virtual void ClientsStart()
        {
            try
            {
                TearUp();

                //Start the client in either listener or sender mode.
                foreach (var partition in base.PriorityPartitions)
                {
                    var client = ClientCreate(partition);

                    client.ResourceProfiles = ResourceProfiles;

                    mClients.Add(partition.Priority, client);

                    if (client.CanStart)
                        base.ClientStart(client);
                    else
                        Logger.LogMessage(string.Format("Client not started: {0} :{1}/{2}", client.Type, client.Name, client.Priority));

                    if (partition.Priority == 1)
                        mDefaultPriority = 1;
                }

                //If the incoming priority cannot be reconciled we set it to the default
                //which is 1, unless 1 is not present and then we set it to the max value.
                if (!mDefaultPriority.HasValue && base.PriorityPartitions != null)
                    mDefaultPriority = base.PriorityPartitions.Select((p) => p.Priority).Max();
            }
            catch (Exception ex)
            {
                LogExceptionLocation("StartInternal", ex);
                throw;
            }
        } 
        #endregion
        #region ClientsStop()
        /// <summary>
        /// This method stops the clients.
        /// </summary>
        protected virtual void ClientsStop()
        {
            try
            {
                mClients.Values.ForEach((c) => ClientStop(c));
                mClients.Clear();
                TearDown();
            }
            catch (Exception ex)
            {
                LogExceptionLocation("StopInternal", ex);
            }
        } 
        #endregion

        #region StartInternal()
        /// <summary>
        /// This is the default start method for both listeners and senders.
        /// </summary>
        protected override void StartInternal()
        {
            //Do nothing - the start logic is overriden to make sure the clients only
            //start when there are active message filters for the connection.
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This is the default stop for all listeners and senders
        /// </summary>
        protected override void StopInternal()
        {
            ClientsStop();
        }
        #endregion
    }
}
