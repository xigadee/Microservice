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
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is the base class for Azure Service Bus messaging listeners.
    /// </summary>
    /// <typeparam name="C">The client entity type.</typeparam>
    /// <typeparam name="M">The messaging type.</typeparam>
    public abstract class AzureSBSenderBase<C, M> : MessagingSenderBase<C, M, AzureClientHolder<C, M>>
        , IAzureServiceBusMessagingService<SenderPartitionConfig>
        where C : ClientEntity
    {
        #region Connection
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        public AzureServiceBusConnection Connection { get; set; }
        #endregion

        #region EntityName
        /// <summary>
        /// This is the Azure Service Bus entity name. Usually this will map the Xigadee channel identifier, but can be overridden in specific circumstances.
        /// </summary>
        public string EntityName { get; set; } 
        #endregion

        #region ClientCreate(SenderPartitionConfig partition)
        /// <summary>
        /// This is the default client create logic.
        /// </summary>
        /// <param name="partition">The specific partition to create the client.</param>
        /// <returns>
        /// Returns the client.
        /// </returns>
        protected override AzureClientHolder<C, M> ClientCreate(SenderPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);

            client.Start = () =>
            {
                client.Client = client.ClientCreate();
                client.IsActive = true;
            };

            client.ClientClose = () => client.Client.CloseAsync().Wait();

            return client;
        } 
        #endregion
    }
}
