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

#region using
using System;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base class for channel partition.
    /// </summary>
    public abstract class PartitionConfig
    {
        /// <summary>
        /// This is the default abstract constructor for the PartitionConfig class
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="fabricMaxMessageLock">The preferred fabric lock time. If this is not set, then the default of 4min30s is used.</param>
        protected internal PartitionConfig(int priority, TimeSpan? fabricMaxMessageLock = null)
        {
            Priority = priority;
            FabricMaxMessageLock = fabricMaxMessageLock ?? TimeSpan.FromMinutes(4.5d);
        }

        /// <summary>
        /// This is the numeric partition id.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// This is the message lock duration for the underlying fabric
        /// </summary>
        public TimeSpan? FabricMaxMessageLock { get; set; }
    }
}
