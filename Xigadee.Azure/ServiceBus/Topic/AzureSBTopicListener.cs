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
        protected string mSubscriptionId;
        /// <summary>
        /// This property indicates to delete the subscription when the listener stops./
        /// </summary>
        protected bool mDeleteOnStop;

        protected bool mListenOnOriginatorId = false;

        protected TimeSpan? mDeleteOnIdleTime;
        #endregion
        #region Constructor

        public AzureSBTopicListener(string channelId
            , string connectionString
            , string connectionName
            , IEnumerable<ListenerPartitionConfig> priorityPartitions
            , string subscriptionId = null
            , bool isDeadLetterListener = false
            , bool deleteOnStop = true
            , bool listenOnOriginatorId = false
            , string mappingChannelId = null
            , TimeSpan? deleteOnIdleTime = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , IBoundaryLogger boundaryLogger = null)
            : base(channelId, connectionString, connectionName, priorityPartitions, isDeadLetterListener, mappingChannelId
                  , resourceProfiles:resourceProfiles, boundaryLogger:boundaryLogger)
        {
            mSubscriptionId = subscriptionId ?? mappingChannelId;
            mDeleteOnStop = deleteOnStop;
            mListenOnOriginatorId = listenOnOriginatorId;
            mDeleteOnIdleTime = deleteOnIdleTime??(deleteOnStop?TimeSpan.FromHours(4):default(TimeSpan?));
        }
        #endregion
        #region OriginatorId
        /// <summary>
        /// This override sets the SubscriptionId if it is null. This is set from the first 50 characters of the originator id
        /// </summary>
        public override string OriginatorId
        {
            get
            {
                return base.OriginatorId;
            }

            set
            {
                base.OriginatorId = value;

                SubscriptionIdSet(value);
            }
        }
        #endregion

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

        #region GetFilters()
        /// <summary>
        /// This method returns a list of SqlFilter object for the listener channel.
        /// </summary>
        /// <returns>The list of SqlFilter object</returns>
        protected virtual List<SqlFilter> GetFilters()
        {
            List<SqlFilter> list;

            string id = OriginatorId;
            if (id.Length > 50)
                id = id.Substring(0, 50);

            if (mListenOnOriginatorId)
            {
                list = new List<SqlFilter>();
                var servMess = new ServiceMessageHeader(ChannelId);
                list.Add(TopicHelper.SqlFilterQuery(new MessageFilterWrapper(servMess) { ClientId = OriginatorId }));
            }
            else
                list = TopicHelper.SqlFilter(mSupportedMessageTypes, ChannelId, MappingChannelId);

            return list;
        }

        #endregion

        public override void Update(List<MessageFilterWrapper> supported)
        {
            base.Update(supported);
        }

        #region SubscriptionId
        /// <summary>
        /// This helper method shows the current subscription id.
        /// </summary>
        public string SubscriptionId { get { return mSubscriptionId; } } 
        #endregion

        #region ClientCreate()
        /// <summary>
        /// This method creates the specific client.
        /// </summary>
        /// <returns>Returns the client.</returns>
        protected override AzureClientHolder<SubscriptionClient, BrokeredMessage> ClientCreate(ListenerPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.Filters = GetFilters().Select((f) => f.SqlExpression).ToList();

            client.Type = "Subscription Listener";

            client.Name = mPriorityClientNamer(mAzureSB.ConnectionName, partition.Priority);

            client.AssignMessageHelpers();

            client.FabricInitialize = () =>
            {
                mAzureSB.TopicFabricInitialize(client.Name);
                var subDesc = mAzureSB.SubscriptionFabricInitialize(client.Name, mSubscriptionId
                    , autoDeleteSubscription: mDeleteOnIdleTime
                    , lockDuration: partition.FabricMaxMessageLock);
            };

            client.SupportsQueueLength = true;

            client.QueueLength = () =>
            {
                try
                {
                    var desc = mAzureSB.NamespaceManager.GetSubscription(client.Name, mSubscriptionId);

                    client.QueueLengthLastPoll = DateTime.UtcNow;

                    if (client.IsDeadLetter)
                        return desc.MessageCountDetails.DeadLetterMessageCount;
                    else
                        return desc.MessageCountDetails.ActiveMessageCount;
                }
                catch (Exception)
                {
                    return null;
                }
            };

            client.CanStart = GetFilters().Count > 0;

            client.ClientCreate = () =>
            {
                var messagingFactory = MessagingFactory.CreateFromConnectionString(mAzureSB.ConnectionString);
                var subClient = messagingFactory.CreateSubscriptionClient(client.Name, mSubscriptionId);

                subClient.PrefetchCount = 50;

                subClient.RemoveRule("$default");
                SetFilters(subClient, client.Name, mSubscriptionId);

                return subClient;
            };

            client.ClientRefresh = () =>
            {
                SetFilters(client.Client, client.Name, mSubscriptionId);
            };

            client.MessageReceive = async (c,t) =>
            {
                var messages = await client.Client.ReceiveBatchAsync(c??10, TimeSpan.FromMilliseconds(t??500));
                return messages;
            };

            return client;
        }
        #endregion

        protected void SetFilters(SubscriptionClient client, string name, string subscriptionId)
        {
            try
            {
                //Get the list of current rules.
                var rules = mAzureSB.NamespaceManager.GetRules(name, subscriptionId);

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
                Logger.LogException(string.Format("SetFilters failed for {0}/{1}", name, subscriptionId), ex);
                throw;
            }

        }

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

        #region TearDown()
        /// <summary>
        /// This override removes the subscription if the deleteOnStop flag is set to true.
        /// </summary>
        protected override void TearDown()
        {
            base.TearDown();

            if (mDeleteOnStop && mSubscriptionId != null)
            {
                if (mAzureSB.NamespaceManager.SubscriptionExists(mAzureSB.ConnectionName, mSubscriptionId))
                {
                    //Listen just for the selected channels and message types
                    mAzureSB.NamespaceManager.DeleteSubscription(mAzureSB.ConnectionName, mSubscriptionId);
                }
            }
        } 
        #endregion
    }
}
