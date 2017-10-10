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

using Microsoft.Azure.ServiceBus;
using System;

namespace Xigadee
{
    /// <summary>
    /// This is the base abstract class for Service Bus Agents.
    /// </summary>
    /// <seealso cref="Xigadee.CommunicationBridgeAgent" />
    public abstract class AzureServiceBusBridgeAgentBase : CommunicationBridgeAgent, IAzureServiceBusFabricBridge
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="connectionString">The service bus connection string.</param>
        /// <param name="receiveMode">The default receive mode.</param>
        /// <param name="retryPolicy">The default retry policy.</param>
        protected AzureServiceBusBridgeAgentBase(ServiceBusConnectionStringBuilder connectionString
            , ReceiveMode receiveMode = ReceiveMode.PeekLock
            , RetryPolicy retryPolicy = null)
        {
            Connection = new AzureServiceBusConnection(connectionString, receiveMode, retryPolicy);
        }

        /// <summary>
        /// Gets the service bus connection.
        /// </summary>
        protected AzureServiceBusConnection Connection { get; }

        /// <summary>
        /// Gets a listener agent for the bridge.
        /// </summary>
        /// <param name="entityName">The Service Bus entity name.</param>
        public abstract IListener GetListener(string entityName);
        /// <summary>
        /// Gets a sender for the bridge.
        /// </summary>
        /// <param name="entityName">The Service Bus entity name.</param>
        public abstract ISender GetSender(string entityName);

    }
}
