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

using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    public static class ConfigureMessagingHelper
    {
        /// <summary>
        /// This sets the standard settings for the messaging service.
        /// </summary>
        /// <typeparam name="P">The partition type.</typeparam>
        /// <param name="component">The component.</param>
        /// <param name="channelId">The channel id.</param>
        /// <param name="priorityPartitions">The partition collection.</param>
        /// <param name="resourceProfiles">The optional resource profile. This only applies to the listeners.</param>
        public static void ConfigureMessaging<P>(this IMessagingService<P> component
            , string channelId
            , IEnumerable<P> priorityPartitions
            , IEnumerable<ResourceProfile> resourceProfiles = null
            )
            where P : PartitionConfig
        {
            component.ChannelId = channelId;

            if (priorityPartitions != null)
                component.PriorityPartitions = priorityPartitions.ToList();

            if (component is IListener && resourceProfiles != null)
                ((IListener)component).ListenerResourceProfiles = resourceProfiles.ToList();
        }
    }
}
