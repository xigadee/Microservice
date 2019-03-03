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

using System;

namespace Xigadee
{
    /// <summary>
    /// This is the Farbric Bridge for the Azure Event Hubs.
    /// </summary>
    public class AzureEventHubsFabricBridge : CommunicationFabricBase<ICommunicationFabricBridge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureEventHubsFabricBridge"/> class.
        /// </summary>
        public AzureEventHubsFabricBridge()
        {

        }
        /// <summary>
        /// Gets the <see cref="ICommunicationFabricBridge"/> with the specified mode.
        /// </summary>
        /// <value>
        /// The <see cref="ICommunicationFabricBridge"/>.
        /// </value>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        /// <exception cref="BridgeAgentModeNotSetException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public override ICommunicationFabricBridge this[CommunicationFabricMode mode]
        {
            get
            {
                switch (mode)
                {
                    case CommunicationFabricMode.Broadcast:
                    case CommunicationFabricMode.Queue:
                        return new AzureEventHubsBridgeAgent();
                    case CommunicationFabricMode.NotSet:
                        throw new BridgeAgentModeNotSetException();
                    default:
                        throw new NotSupportedException($"{mode.ToString()} is not supported.");
                }
            }
        }
    }
}
