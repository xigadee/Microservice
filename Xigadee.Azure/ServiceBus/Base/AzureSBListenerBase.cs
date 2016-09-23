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
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the abstract listener base.
    /// </summary>
    public abstract class AzureSBListenerBase<C, M> : MessagingListenerBase<C, M, AzureClientHolder<C, M>>
        where C : ClientEntity
    {
        #region Declarations
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        protected AzureConnection mAzureSB;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="channelId">The channelId used to identify internally the comms layer.</param>
        /// <param name="connectionString">The Azure Service bus connection string.</param>
        /// <param name="connectionName"></param>
        /// <param name="defaultTimeout"></param>
        /// <param name="isDeadLetterListener"></param>
        public AzureSBListenerBase(string channelId
            , string connectionString
            , string connectionName
            , IEnumerable<ListenerPartitionConfig> priorityPartitions
            , bool isDeadLetterListener = false
            , string mappingChannelId = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            )
            : base(channelId, priorityPartitions, mappingChannelId, resourceProfiles:resourceProfiles)
        {
            mAzureSB = new AzureConnection() { ConnectionName = connectionName, ConnectionString = connectionString };
            IsDeadLetterListener = isDeadLetterListener;
        } 
        #endregion

        #region IsDeadLetterListener
        /// <summary>
        /// This property identifies whether the listener is a deadletter listener.
        /// </summary>
        public bool IsDeadLetterListener
        {
            get;
            private set;
        } 
        #endregion

        #region ClientCreate()
        /// <summary>
        /// This method sets the start and stop listener methods.
        /// </summary>
        /// <returns>The client.</returns>
        protected override AzureClientHolder<C, M> ClientCreate(ListenerPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.IsDeadLetter = IsDeadLetterListener;

            client.ClientClose = () => 
            {
                if (client.Client != null)
                    client.Client.Close();
            };
          
            return client;
        } 
        #endregion
    }
}
