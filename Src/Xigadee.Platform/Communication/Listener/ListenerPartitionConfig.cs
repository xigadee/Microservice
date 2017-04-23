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
using System.Collections.Generic;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the individual configuration for each listener partition.
    /// </summary>
    public class ListenerPartitionConfig: PartitionConfig
    {
        #region Static methods
        /// <summary>
        /// This static method sets the priorty channels for the integers passed at their default settings.
        /// By default, only channel 0 will support rate limiting. The default processing time is set to 4 minutes.
        /// </summary>
        /// <param name="priority">The priority list, i.e. 0,1</param>
        /// <returns>Returns a list of configs.</returns>
        public static IEnumerable<ListenerPartitionConfig> Init(params int[] priority)
        {
            foreach(int p in priority)
                yield return new ListenerPartitionConfig(p);
        }
        #endregion

        /// <summary>
        /// This is the default constructor that sets the weighting to 1 (100%).
        /// </summary>
        public ListenerPartitionConfig(int priority
            , decimal priortyWeighting = 1m
            , bool? supportsRateLimiting = null
            , TimeSpan? payloadMaxProcessingTime = null
            , TimeSpan? fabricMaxMessageLock = null
            )
            : base(priority, fabricMaxMessageLock)
        {
            PriorityWeighting = priortyWeighting;
            SupportsRateLimiting = supportsRateLimiting ?? priority == 0;
            PayloadMaxProcessingTime = payloadMaxProcessingTime ?? TimeSpan.FromMinutes(4d);
        }

        /// <summary>
        /// Identifies whether the paritition client will implement rate limiting.
        /// </summary>
        public bool SupportsRateLimiting { get; set; }

        /// <summary>
        /// This is the default timeout - 4 minutes by default.
        /// </summary>
        public TimeSpan? PayloadMaxProcessingTime { get; set; }

        /// <summary>
        /// This is the percentage weighting for the channel used when calculating priority over the 
        /// other queues. 1 is the default value. A value of 1.1 will increase the overall priority score by 10%.
        /// </summary>
        public decimal PriorityWeighting { get; set; }

    }
}
