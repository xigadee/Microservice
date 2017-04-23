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
using System.Linq;
using System.Text;
using Microsoft.ServiceBus.Messaging;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the helper for Azure ServiceBus Topic and Subscription queues.
    /// </summary>
    public static class TopicHelper
    {
        #region TopicDescriptionGet(string cName)
        public static TopicDescription TopicDescriptionGet(string cName
            , TimeSpan? defaultMessageTTL = null
            )
        {
            return new TopicDescription(cName)
            {
                EnableBatchedOperations = true
                ,EnableFilteringMessagesBeforePublishing = false
                ,SupportOrdering = true
                ,DefaultMessageTimeToLive = defaultMessageTTL ??TimeSpan.FromDays(7)
                ,MaxSizeInMegabytes = 5120
                //,EnablePartitioning = true
            };
        } 
        #endregion

        #region SubscriptionDescriptionGet(string tPath, string sName)
        public static SubscriptionDescription SubscriptionDescriptionGet(string tPath, string sName
            , TimeSpan? autoDelete = null
            , TimeSpan? defaultMessageTTL = null
            , TimeSpan? lockDuration = null
            )
        {
            return new SubscriptionDescription(tPath, sName)
            {
                EnableDeadLetteringOnMessageExpiration = true,
                EnableDeadLetteringOnFilterEvaluationExceptions = false,
                LockDuration = lockDuration??TimeSpan.FromMinutes(5),
                EnableBatchedOperations = true,
                DefaultMessageTimeToLive = defaultMessageTTL ?? TimeSpan.FromDays(7),
                AutoDeleteOnIdle = autoDelete??TimeSpan.MaxValue
            };
        } 
        #endregion

        #region TopicFabricInitialize(this NamespaceManager mNamespaceManager, string mConnectionName)
        /// <summary>
        /// This method creates the topic if it does not exist.
        /// </summary>
        public static TopicDescription TopicFabricInitialize(this AzureConnection conn, string name
            , TimeSpan? defaultMessageTTL = null
            )
        {
            if (!conn.NamespaceManager.TopicExists(name))
            {
                try
                {
                    return conn.NamespaceManager.CreateTopic(TopicDescriptionGet(name, defaultMessageTTL));
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    // Another service created it before we did - just retrieve the one it created
                }
            }

            return conn.NamespaceManager.GetTopic(name);
        } 
        #endregion

        #region SubscriptionFabricInitialize(this NamespaceManager mNamespaceManager, string mConnectionName, string mSubscriptionId)
        /// <summary>
        /// This method creates the subscription if it does not exist.
        /// </summary>
        public static SubscriptionDescription SubscriptionFabricInitialize(this AzureConnection conn, string name, string subscriptionId
            , TimeSpan? autoDeleteSubscription = null
            , TimeSpan? defaultMessageTTL = null
            , TimeSpan? lockDuration = null)
        {
            if (!conn.NamespaceManager.SubscriptionExists(name, subscriptionId))
            {
                try
                {
                    return conn.NamespaceManager.CreateSubscription(
                        SubscriptionDescriptionGet(name, subscriptionId, autoDeleteSubscription, defaultMessageTTL, lockDuration)
                        );
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    // Another service created it before we did - just retrieve the one it created
                }
            }

            return conn.NamespaceManager.GetSubscription(name, subscriptionId);
        }
        #endregion


        public static List<SqlFilter> SqlFilter(List<MessageFilterWrapper> supportedMessageTypes, string channelId, string mappingChannelId = null)
        {
            if (channelId == null)
                throw new ArgumentNullException(nameof(channelId), "SqlFilter - channelId cannot be null.");

            var filters = new List<SqlFilter>();

            if (supportedMessageTypes == null || supportedMessageTypes.Count == 0)
                return filters;

            var channelToScan = mappingChannelId?? channelId;
            var messages = supportedMessageTypes.Where(mw => mw.Header.ChannelId == channelToScan).ToList();

            if (messages.Count > 0)
            {
                var types = messages.Select(m => m.Header.MessageType).Distinct().ToList();

                foreach (var type in types)
                {
                    var actionTypes = messages
                        .Where(m => m.Header.ChannelId == channelToScan && m.Header.MessageType == type)
                        .Select(m => m.Header.ActionType).ToArray();

                    filters.Add(SqlFilterQuery(channelId: channelId, messageType: type, actionType: actionTypes));
                }
            }

            return filters;
        }

        /// <summary>
        /// This method generated the SqlFilter for the message type.
        /// </summary>
        /// <param name="message">The message type to create the SqlFilter for.</param>
        /// <returns>Returns a SqlFilter.</returns>
        public static SqlFilter SqlFilterQuery(MessageFilterWrapper message)
        {
            StringBuilder sb = new StringBuilder();
            SqlFilterQueryStatement(sb, message);
            return new SqlFilter(sb.ToString());
        }

        public static SqlFilter SqlFilterQuery(string clientId = null, string channelId = null, string messageType = null, string[] actionType = null)
        {
            StringBuilder sb = new StringBuilder();
            SqlFilterQueryStatement(sb, clientId, channelId, messageType, actionType);
            return new SqlFilter(sb.ToString());
        }

        /// <summary>
        /// This method creates the appropriate SQL filter based on the MessageFilterWrapper
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="m">The message type to create the sql string for.</param>
        public static void SqlFilterQueryStatement(StringBuilder sb, MessageFilterWrapper m)
        {
            bool andFlag = false;

            Action<string, string> sqlParam = (id, val) =>
            {
                if (!string.IsNullOrEmpty(val))
                {
                    if (andFlag) sb.Append(" AND ");
                    andFlag = true;
                    //FIX: Case sensitive pattern matchin in ServiceBus.
                    sb.AppendFormat("{0}=\'{1}\'", id, val.ToLowerInvariant());
                }
            };

            if (!string.IsNullOrEmpty(m.ClientId))
                sqlParam("CorrelationServiceId", m.ClientId);

            if (!string.IsNullOrEmpty(m.Header.ChannelId))
                sqlParam("ChannelId", m.Header.ChannelId);

            if (!string.IsNullOrEmpty(m.Header.MessageType))
                sqlParam("MessageType", m.Header.MessageType);

            if (!string.IsNullOrEmpty(m.Header.ActionType))
                sqlParam("ActionType", m.Header.ActionType);

        }

        public static void SqlFilterQueryStatement(StringBuilder sb , string clientId = null, string channelId = null, string messageType = null, string[] actionType = null)
        {
            bool andFlag = false;

            Action<string, string> sqlParam = (id, val) =>
            {
                if (!string.IsNullOrEmpty(val))
                {
                    if (andFlag) sb.Append(" AND ");
                    andFlag = true;
                    //FIX: Case sensitive pattern matching in ServiceBus.
                    sb.AppendFormat("{0}=\'{1}\'", id, val.ToLowerInvariant());
                }
            };

            if (!string.IsNullOrEmpty(clientId))
                sqlParam("CorrelationServiceId", clientId);

            if (!string.IsNullOrEmpty(channelId))
                sqlParam("ChannelId", channelId);

            if (!string.IsNullOrEmpty(messageType))
                sqlParam("MessageType", messageType);

            if (actionType != null)
            {
                var list = actionType.Where(s => !string.IsNullOrEmpty(s)).ToList();

                if (list.Count == 0)
                    return;

                if (list.Count > 1)
                {
                    if (andFlag) sb.Append(" AND ");
                    var listVals = string.Join(",", list.Select(i => $"\'{i.ToLowerInvariant()}\'"));

                    sb.AppendFormat("{0} IN ({1})", "ActionType", listVals);
                }
                else
                    sqlParam("ActionType", list[0]);

            }
        }
    }
}
