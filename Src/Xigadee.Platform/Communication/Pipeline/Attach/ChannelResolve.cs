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
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This helper resolves a channel from the direction specified.
        /// </summary>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="direction"></param>
        /// <param name="throwIfChannelIsNull"></param>
        /// <returns>The resolved channel or null if not resolved and throwIfChannelIsNull is false. Otherwise an exception will be raised.</returns>
        public static Channel ToChannel(this IPipelineChannel cpipe, ChannelDirection direction, bool throwIfChannelIsNull = true)
        {
            Channel channel = null;

            if (cpipe is IPipelineChannelBroadcast)
                switch (direction)
                {
                    case ChannelDirection.Incoming:
                        channel = ((IPipelineChannelBroadcast)cpipe).ChannelListener;
                        break;
                    case ChannelDirection.Outgoing:
                        channel = ((IPipelineChannelBroadcast)cpipe).ChannelSender;
                        break;
                    default:
                        throw new NotSupportedException($"ChannelDirection {direction} not supported in {nameof(CorePipelineExtensions)}/{nameof(ToChannel)}");
                }
            else
                channel = cpipe.Channel;

            if (channel == null && throwIfChannelIsNull)
                throw new ArgumentNullException($"The pipe channel is null -> {direction}");

            return channel;
        }
    }
}
