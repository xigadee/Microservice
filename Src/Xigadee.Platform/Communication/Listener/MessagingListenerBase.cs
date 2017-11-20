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
using System.Diagnostics;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the generic listener class for messaging services.
    /// </summary>
    /// <typeparam name="C">The client type.</typeparam>
    /// <typeparam name="M">The client message type.</typeparam>
    /// <typeparam name="H">The client-holder type.</typeparam>
    [DebuggerDisplay("{GetType().Name}: {ChannelId}|{MappingChannelId}@{Status} [{ComponentId}]")]
    public class MessagingListenerBase<C, M, H> : MessagingServiceBase<C, M, H, ListenerPartitionConfig>
        , IListener, IRequireSharedServices
        where H : ClientHolder<C, M>, new()
    {
        #region Declarations
        private object syncObject = new object();
        /// <summary>
        /// This is the supported list of message types for the client.
        /// </summary>
        protected List<MessageFilterWrapper> mSupportedMessageTypes= new List<MessageFilterWrapper>();
        #endregion

        #region ResourceProfiles
        /// <summary>
        /// This is the list of resource profiles that the Listener can be throttled on.
        /// </summary>
        public List<ResourceProfile> ResourceProfiles { get; set; } = new List<ResourceProfile>();
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
            var newList = supported.Where((m) => string.Equals(m.Header.ChannelId, (MappingChannelId ?? ChannelId), StringComparison.InvariantCultureIgnoreCase)).ToList();
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

        #region MappingChannelId
        /// <summary>
        /// This is the mapping channel id. It is used to map the incoming channel to a new channel when required.
        /// </summary>
        public string MappingChannelId
        {
            get;set;
        } 
        #endregion

        #region ClientsValidate(List<MessageFilterWrapper> oldList, List<MessageFilterWrapper> newList)
        /// <summary>
        /// This method is used to revalidate the clients when a message type is enabled or disabled, and stop or start the appropriate clients.
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
                //OK, there is a small change. Let the clients know there is additional messages to filter on
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
                foreach (var partition in PriorityPartitions)
                {
                    var client = ClientCreate(partition);

                    client.ResourceProfiles = ResourceProfiles;

                    mClients.AddOrUpdate(partition.Priority, client, (i,h) => h);

                    if (client.CanStart)
                        base.ClientStart(client);
                    else
                        Collector?.LogMessage(string.Format("Client not started: {0} :{1}/{2}", client.Type, client.Name, client.Priority));

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
            SettingsValidate();
            //Do nothing - the start logic is overridden to make sure the clients only
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
