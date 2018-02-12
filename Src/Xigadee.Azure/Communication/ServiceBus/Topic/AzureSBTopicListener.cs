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
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Security.Cryptography;
#endregion
namespace Xigadee
{
    /// <summary>
    /// The Azure ServiceBus topic listener. This listener will dynamically register itself based on the filter passed.
    /// </summary>
    [DebuggerDisplay("AzureSBTopicListener: Subscription={SubscriptionId}|{MappingChannelId} {ChannelId}")]
    public class AzureSBTopicListener : AzureSBListenerBase<SubscriptionClient,BrokeredMessage>
    {
        #region Declarations
        /// <summary>
        /// This is the name of the subscription.
        /// </summary>
        public string mSubscriptionId { get; set; }
        /// <summary>
        /// This property indicates to delete the subscription when the listener stops./
        /// </summary>
        public bool DeleteOnStop { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the topic listener should filter out messages intended for this
        /// Microservice.
        /// </summary>
        public bool ListenOnOriginatorId { get; set; }
        /// <summary>
        /// Gets or sets the DeleteOnIdleTime. Set this to null if you do not want the subscription to be removed.
        /// </summary>
        public TimeSpan? DeleteOnIdleTime { get; set; }
        #endregion
        #region Constructor

        /// <summary>
        /// This is the default constructor used by the pipeline.
        /// </summary>
        public AzureSBTopicListener() { }

        //public AzureSBTopicListener(string channelId
        //    , string connectionString
        //    , string connectionName
        //    , IEnumerable<ListenerPartitionConfig> priorityPartitions
        //    , string subscriptionId = null
        //    , bool isDeadLetterListener = false
        //    , bool deleteOnStop = true
        //    , bool listenOnOriginatorId = false
        //    , string mappingChannelId = null
        //    , TimeSpan? deleteOnIdleTime = null
        //    , IEnumerable<ResourceProfile> resourceProfiles = null):base()

        //{
        //    mSubscriptionId = subscriptionId ?? mappingChannelId;
        //    DeleteOnStop = deleteOnStop;
        //    ListenOnOriginatorId = listenOnOriginatorId;
        //    DeleteOnIdleTime = deleteOnIdleTime??(deleteOnStop?TimeSpan.FromHours(4):default(TimeSpan?));
        //}
        #endregion

        #region SettingsValidate()
        /// <summary>
        /// This override is used to validate the listener configuration settings on start-up.
        /// </summary>
        protected override void SettingsValidate()
        {
            base.SettingsValidate();

            DeleteOnIdleTime = DeleteOnIdleTime ?? (DeleteOnStop ? TimeSpan.FromHours(4) : default(TimeSpan?));
        } 
        #endregion

        #region OriginatorId
        /// <summary>
        /// This override sets the SubscriptionId if it is null. This is set from the first 50 characters of the originator id
        /// </summary>
        public override MicroserviceId OriginatorId
        {
            get
            {
                return base.OriginatorId;
            }

            set
            {
                base.OriginatorId = value;

                SubscriptionIdSet(value.ExternalServiceId);
            }
        }
        #endregion

        #region SubscriptionIdSet(string originatorId)
        /// <summary>
        /// This method sets the subscription id from the first 50 characters of the incoming originator id.
        /// </summary>
        /// <param name="originatorId">The originator id.</param>
        protected virtual void SubscriptionIdSet(string originatorId)
        {
            if (mSubscriptionId == null)
            {
                mSubscriptionId = originatorId;
                //Azure has a limit of 50 characters for a subscription name, so we may need to truncate.
                if (mSubscriptionId.Length > 50)
                    mSubscriptionId = mSubscriptionId.Substring(0, 50);
            }
        } 
        #endregion

        #region GetFilters()
        /// <summary>
        /// This method returns a list of SqlFilter object for the listener channel.
        /// </summary>
        /// <returns>The list of SqlFilter object</returns>
        protected virtual List<SqlFilter> GetFilters()
        {
            List<SqlFilter> list;

            string id = OriginatorId.ExternalServiceId;
            if (id.Length > 50)
                id = id.Substring(0, 50);

            if (ListenOnOriginatorId)
            {
                list = new List<SqlFilter>();
                var servMess = new ServiceMessageHeader(ChannelId);
                list.Add(TopicHelper.SqlFilterQuery(new MessageFilterWrapper(servMess, OriginatorId.ExternalServiceId)));
            }
            else
                list = TopicHelper.SqlFilter(mSupportedMessageTypes, ChannelId, ListenerMappingChannelId);

            return list;
        }

        #endregion

        #region SubscriptionId
        /// <summary>
        /// This helper method shows the current subscription id.
        /// </summary>
        public string SubscriptionId { get { return mSubscriptionId; } } 
        #endregion

        #region ClientCreate()
        ///// <summary>
        ///// This method creates the specific client.
        ///// </summary>
        ///// <returns>Returns the client.</returns>
        //protected override AzureClientHolder<SubscriptionClient, BrokeredMessage> ClientCreate(ListenerPartitionConfig partition)
        //{
        //    var client = base.ClientCreate(partition);

        //    client.Filters = GetFilters().Select((f) => f.SqlExpression).ToList();

        //    client.Type = "Subscription Listener";

        //    client.Name = mPriorityClientNamer(AzureConn.ConnectionName, partition.Priority);

        //    client.AssignMessageHelpers();

        //    client.FabricInitialize = () =>
        //    {
        //        AzureConn.TopicFabricInitialize(client.Name);
        //        var subDesc = AzureConn.SubscriptionFabricInitialize(client.Name, mSubscriptionId
        //            , autoDeleteSubscription: DeleteOnIdleTime
        //            , lockDuration: partition.FabricMaxMessageLock);
        //    };

        //    client.SupportsQueueLength = true;

        //    client.QueueLength = () =>
        //    {
        //        try
        //        {
        //            var desc = AzureConn.NamespaceManager.GetSubscription(client.Name, mSubscriptionId);

        //            client.QueueLengthLastPoll = DateTime.UtcNow;

        //            return desc.MessageCountDetails.ActiveMessageCount;
        //        }
        //        catch (Exception)
        //        {
        //            return null;
        //        }
        //    };

        //    client.CanStart = GetFilters().Count > 0;

        //    client.ClientCreate = () =>
        //    {
        //        var messagingFactory = MessagingFactory.CreateFromConnectionString(AzureConn.ConnectionString);
        //        var subClient = messagingFactory.CreateSubscriptionClient(client.Name, mSubscriptionId);

        //        subClient.PrefetchCount = 50;

        //        subClient.RemoveRule("$default");
        //        SetFilters(subClient, client.Name, mSubscriptionId);

        //        return subClient;
        //    };

        //    client.ClientRefresh = () =>
        //    {
        //        SetFilters(client.Client, client.Name, mSubscriptionId);
        //    };

        //    client.MessageReceive = async (c,t) =>
        //    {
        //        var messages = await client.Client.ReceiveBatchAsync(c??10, TimeSpan.FromMilliseconds(t??500));
        //        return messages;
        //    };

        //    return client;
        //}
        #endregion
        #region SetFilters(SubscriptionClient client, string name, string subscriptionId)
        /// <summary>
        /// Adds and/or removes subscription filter rules based on the current status.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="name">The name.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        protected void SetFilters(SubscriptionClient client, string name, string subscriptionId)
        {
            try
            {
                //Get the list of current rules.
                var rules = AzureConn.NamespaceManager.GetRules(name, subscriptionId);

                var newFilters = GetFilters()
                    .ToDictionary((f) => FilterToId(f), (f) => f);

                var existingFilters = rules
                    .Where((r) => r.Filter is SqlFilter)
                    .Select((f) => f.Name);


                var ruleToAdd = newFilters.Keys.Except(existingFilters);
                var ruleToRemove = existingFilters.Except(newFilters.Keys);

                //Add new rules
                ruleToAdd.ForEach((f) => client.AddRule(f, newFilters[f]));

                //Remove unmatched rules.
                ruleToRemove.ForEach((r) => client.RemoveRule(r));
            }
            catch (Exception ex)
            {
                Collector?.LogException(string.Format("SetFilters failed for {0}/{1}", name, subscriptionId), ex);
                throw;
            }
        }
        #endregion
        #region FilterToId(SqlFilter filter)
        /// <summary>
        /// Converts a SQL filter to an MD5 Hash. This is to enable easy deletion when filters change.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">FilterToId - filter cannot be null.</exception>
        protected string FilterToId(SqlFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("FilterToId - filter cannot be null.");

            // get the byte representation
            var bytes = Encoding.UTF8.GetBytes(filter.SqlExpression);

            // create the md5 hash
            using (MD5 md5Hasher = MD5.Create())
            {
                byte[] data = md5Hasher.ComputeHash(bytes);

                // convert the hash to a Guid
                var id = new Guid(data);

                return string.Format("hash{0}", id.ToString("N").ToUpperInvariant());
            }
        } 
        #endregion

        #region TearDown()
        /// <summary>
        /// This override removes the subscription if the deleteOnStop flag is set to true.
        /// </summary>
        protected override void TearDown()
        {
            base.TearDown();

            if (DeleteOnStop && mSubscriptionId != null)
            {
                if (AzureConn.NamespaceManager.SubscriptionExists(AzureConn.ConnectionName, mSubscriptionId))
                {
                    //Listen just for the selected channels and message types
                    AzureConn.NamespaceManager.DeleteSubscription(AzureConn.ConnectionName, mSubscriptionId);
                }
            }
        }
        #endregion
    }
}
