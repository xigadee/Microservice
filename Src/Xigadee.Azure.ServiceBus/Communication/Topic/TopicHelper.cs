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
using Microsoft.Azure.ServiceBus;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the helper for Azure ServiceBus Topic and Subscription queues.
    /// </summary>
    public static class TopicHelper
    {
        /// <summary>
        /// Returns a list of SqlFilters for a Service Bus Subscription based on a list of supported message types.
        /// </summary>
        /// <param name="supportedMessageTypes">The list of supported message types.</param>
        /// <param name="channelId">The channel Id to construct the queries from.</param>
        /// <param name="mappingChannelId">The optional mapping channel Id used to scan the supported message types, but repalced by channelId in the SqlFilter objects.</param>
        /// <returns>Returns a list of SqlFilter objects</returns>
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

        /// <summary>
        /// This method generatea a SqlFilter for the supplied parameters.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="channelId">The channel id.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
        /// <returns>Returns the SqlFilter object.</returns>
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
        /// <summary>
        /// This method builds up the SQL statement from the supplied parameters.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="channelId">The channel id.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
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
