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
namespace Xigadee
{
    /// <summary>
    /// This is the base interface implemented by listeners.
    /// </summary>
    public interface IListener : IMessaging
    {
        /// <summary>
        /// This method is used to change the supported filters. This happens when a command starts or stops in the microservice.
        /// </summary>
        /// <param name="supported"></param>
        void Update(List<MessageFilterWrapper> supported);

        /// <summary>
        /// This is the channel id that incoming messages will be mapped to.
        /// </summary>
        string MappingChannelId { get; set;}

        /// <summary>
        /// This is the list of resource profiles attached to the listener.
        /// Resource profiles are use to implement rate limiting for incoming requests.
        /// </summary>
        List<ResourceProfile> ResourceProfiles {get;set;}

        /// <summary>
        /// This contains the listener priority partitions.
        /// </summary>
        List<ListenerPartitionConfig> PriorityPartitions { get; set; }
    }
}
