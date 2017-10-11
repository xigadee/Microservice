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
    /// This is the root class for Azure Service Bus communication.
    /// </summary>
    public class AzureServiceBusFabricBridge : FabricBridgeBase<IAzureServiceBusFabricBridge>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="sasToken">The service bus sas token based connection string.</param>
        /// <param name="receiveMode">The default receive mode.</param>
        /// <param name="retryPolicy">The default retry policy.</param>
        public AzureServiceBusFabricBridge(string sasToken
            , ReceiveMode receiveMode = ReceiveMode.PeekLock
            , RetryPolicy retryPolicy = null)
            : this(new ServiceBusConnectionStringBuilder(sasToken), receiveMode, retryPolicy)
        {
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="connection">The service bus connection.</param>
        /// <param name="receiveMode">The default receive mode.</param>
        /// <param name="retryPolicy">The default retry policy.</param>
        public AzureServiceBusFabricBridge(ServiceBusConnectionStringBuilder connection
            , ReceiveMode receiveMode = ReceiveMode.PeekLock
            , RetryPolicy retryPolicy = null
            )
        {
            Connection = connection;
            DefaultReceiveMode = receiveMode;
            DefaultRetryPolicy = retryPolicy;
        } 
        #endregion

        /// <summary>
        /// This is the Service Bus connection.
        /// </summary>
        public ServiceBusConnectionStringBuilder Connection { get; set; }

        /// <summary>
        /// The default receive mode.
        /// </summary>
        public ReceiveMode DefaultReceiveMode { get; set; }

        /// <summary>
        /// The default retry policy.
        /// </summary>
        public RetryPolicy DefaultRetryPolicy { get; set; }

        /// <summary>
        /// Returns a communication bridge of the required type.
        /// </summary>
        /// <param name="mode">The communication mode.</param>
        /// <returns>The topic or queue bridge.</returns>
        public override IAzureServiceBusFabricBridge this[FabricMode mode]
        {
            get
            {
                switch (mode)
                {
                    case FabricMode.Queue:
                        return new AzureServiceBusQueueBridgeAgent(Connection, DefaultReceiveMode, DefaultRetryPolicy);
                    case FabricMode.Broadcast:
                        return new AzureServiceBusTopicBridgeAgent(Connection, DefaultReceiveMode, DefaultRetryPolicy);
                    case FabricMode.NotSet:
                        throw new BridgeAgentModeNotSetException();
                    default:
                        throw new NotSupportedException($"{mode.ToString()} is not supported.");
                }
            }
        }
    }
}
