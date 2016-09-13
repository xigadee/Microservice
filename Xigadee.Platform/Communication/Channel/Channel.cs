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
    /// The channel class is used to simplify the connection between communication and command components.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="id">The channel Id.</param>
        /// <param name="direction">The direction of the channel - Incoming or outgoing</param>
        /// <param name="description">The optional description</param>
        /// <param name="internalOnly">This property specifies that the channel should only be used for internal messaging.</param>
        public Channel(string id, ChannelDirection direction, string description = null, IBoundaryLogger boundaryLogger = null, bool internalOnly = false)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id cannot be null or empty.");

            Id = id;
            Direction = direction;
            Description = description;
            InternalOnly = internalOnly;
            BoundaryLogger = boundaryLogger;
        }

        /// <summary>
        /// The channel Id.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// An optional description of the channel
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The direction of the channel - Incoming or outgoing
        /// </summary>
        public ChannelDirection Direction { get; }

        /// <summary>
        /// This property specifies that the channel should only be used for internal messaging.
        /// </summary>
        public bool InternalOnly { get; }
        /// <summary>
        /// This is the boundary logger set for the channel
        /// </summary>
        public IBoundaryLogger BoundaryLogger {get;set;}

        /// <summary>
        /// This is a set of resource profiles attached to the channel.
        /// </summary>
        public List<ResourceProfile> ResourceProfiles { get; set; } = new List<ResourceProfile>();

        /// <summary>
        /// This property contains the partitions set for the channel. This is IEnumerable to enable covariant support.
        /// </summary>
        public IEnumerable<PartitionConfig> Partitions { get; set; }  = null;
    }
}
