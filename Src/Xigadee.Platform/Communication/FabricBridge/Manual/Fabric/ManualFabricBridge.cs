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
using System.Collections.Concurrent;

namespace Xigadee
{
    /// <summary>
    /// This is the communication bridge that simulates passing messages between Microservices and can be used for unit test based scenarios.
    /// </summary>
    public class ManualFabricBridge: FabricBridgeBase
    {
        private ConcurrentDictionary<string, ManualFabricChannel> mChannels;

        /// <summary>
        /// Gets the <see cref="ICommunicationBridge"/> with the specified mode.
        /// </summary>
        /// <value>
        /// The <see cref="ICommunicationBridge"/>.
        /// </value>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">The communication bridge mode is not supported</exception>
        public override ICommunicationBridge this[CommunicationBridgeMode mode]
        {
            get
            {
                switch (mode)
                {
                    case CommunicationBridgeMode.NotSet:
                        throw new NotSupportedException("The communication bridge mode is not supported");
                }

                return new ManualCommunicationBridgeAgent(mode);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ManualFabricBridge"/> class.
        /// </summary>
        public ManualFabricBridge()
        {
            mChannels = new ConcurrentDictionary<string, ManualFabricChannel>();
        }
        /// <summary>
        /// Creates the queue client.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns>The manual fabric connection.</returns>
        public ManualFabricConnection CreateQueueClient(string channelId)
        {
            return GetChannel(channelId).CreateConnection(ManualFabricConnectionMode.Queue);
        }
        /// <summary>
        /// Creates the subscription client.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>The manual fabric connection.</returns>
        public ManualFabricConnection CreateSubscriptionClient(string channelId, string subscriptionId)
        {
            return GetChannel(channelId).CreateConnection(ManualFabricConnectionMode.Subscription, subscriptionId);
        }
        /// <summary>
        /// Creates the transmit client.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns>The manual fabric connection.</returns>
        public ManualFabricConnection CreateTransmitClient(string channelId)
        {
            return GetChannel(channelId).CreateConnection(ManualFabricConnectionMode.Transmit);
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns>The manual fabric connection.</returns>
        protected ManualFabricChannel GetChannel(string channelId)
        {
            ManualFabricChannel channel;
            if (!mChannels.TryGetValue(channelId, out channel))
            {
                channel = new ManualFabricChannel(channelId);
                //This might happen in a multi-threaded world.
                if (!mChannels.TryAdd(channelId, channel))
                {
                    channel = mChannels[channelId];
                }
            }
            return channel;
        }

    }
}
