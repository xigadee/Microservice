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
    /// This is the Azure Service Bus Queue agent.
    /// </summary>
    /// <seealso cref="Xigadee.AzureServiceBusBridgeAgentBase" />
    public class AzureServiceBusQueueBridgeAgent : AzureServiceBusBridgeAgentBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceBusQueueBridgeAgent"/> class.
        /// </summary>
        /// <param name="connectionString">The service bus connection string.</param>
        /// <param name="receiveMode">The default receive mode.</param>
        /// <param name="retryPolicy">The default retry policy.</param>
        public AzureServiceBusQueueBridgeAgent(ServiceBusConnectionStringBuilder connectionString, ReceiveMode receiveMode = ReceiveMode.PeekLock, RetryPolicy retryPolicy = null) 
            : base(connectionString, receiveMode, retryPolicy)
        {
        }

        public override IListener GetListener()
        {
            throw new NotImplementedException();
        }

        public override ISender GetSender()
        {
            throw new NotImplementedException();
        }
    }
}
