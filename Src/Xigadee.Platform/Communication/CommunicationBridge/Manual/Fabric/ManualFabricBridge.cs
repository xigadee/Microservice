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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This is the communication bridge that simulates passing messages between Microservices and can be used for unit test based scenarios.
    /// </summary>
    public class ManualFabricBridge: FabricBase<FabricMessage, ManualFabricConnection>
    {
        private ConcurrentDictionary<string, ManualFabricChannel> mChannels;

        public ManualFabricBridge()
        {
            mChannels = new ConcurrentDictionary<string, ManualFabricChannel>();
        }

        public ManualFabricConnection CreateQueueClient(string channelId)
        {
            return GetChannel(channelId).CreateConnection(ManualFabricConnectionMode.Queue);
        }

        public ManualFabricConnection CreateSubscriptionClient(string channelId, string subscriptionId)
        {
            return GetChannel(channelId).CreateConnection(ManualFabricConnectionMode.Subscription, subscriptionId);
        }

        public ManualFabricConnection CreateTransmitClient(string channelId)
        {
            return GetChannel(channelId).CreateConnection(ManualFabricConnectionMode.Transmit);
        }

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
