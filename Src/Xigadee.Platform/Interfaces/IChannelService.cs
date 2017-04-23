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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by the Communication service and allows
    /// components to query the system channels.
    /// </summary>
    public interface IChannelService
    {
        IEnumerable<Channel> Channels { get; }

        bool Add(Channel item);

        bool Remove(Channel item);

        bool Exists(string channelId, ChannelDirection direction);

        bool TryGet(string channelId, ChannelDirection direction, out Channel channel);

    }
    /// <summary>
    /// This interface is implemented by components that require a direct access to the 
    /// system channels.
    /// </summary>
    public interface IRequireChannelService
    {
        /// <summary>
        /// The service reference.
        /// </summary>
        IChannelService ChannelService { get; set; }
    }
}
