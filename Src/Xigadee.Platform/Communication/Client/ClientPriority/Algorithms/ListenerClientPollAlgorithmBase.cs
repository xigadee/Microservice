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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This immutable abstract class is the base to allocate polling slots to the listener collection based on the 
    /// logic incorporated in here.
    /// </summary>
    public abstract class ListenerClientPollAlgorithmBase: IListenerClientPollAlgorithm
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="name">Defaults to the class name.</param>
        /// <param name="allowedOverage">Defaults to 5.</param>
        /// <param name="priorityRecalculateFrequency">Defaults to 10 minutes.</param>
        /// <param name="maxAllowedWaitBetweenPolls">Defaults to 1 second.</param>
        /// <param name="minExpectedWaitBetweenPolls">Defaults to 100ms.</param>
        /// <param name="fabricPollWaitMin">Defaults to 100ms.</param>
        /// <param name="fabricPollWaitMax">Defaults to 1s.</param>
        /// <param name="pollTimeReduceRatio">Defaults to 75%/0.75</param>
        /// <param name="capacityPercentage">Defaults to 75%/0.75</param>
        /// <param name="supportPassDueScan">Default is false.</param>
        public ListenerClientPollAlgorithmBase(
            string name = null
            , int? allowedOverage = null
            , TimeSpan? priorityRecalculateFrequency = null
            , TimeSpan? maxAllowedWaitBetweenPolls = null
            , TimeSpan? minExpectedWaitBetweenPolls = null
            , TimeSpan? fabricPollWaitMin = null
            , TimeSpan? fabricPollWaitMax = null
            , decimal? pollTimeReduceRatio = null
            , double? capacityPercentage = null
            , bool? supportPassDueScan = null
            )
        {
            Name = name ?? GetType().Name;
            AllowedOverage = allowedOverage ?? 5;
            PriorityRecalculateFrequency = priorityRecalculateFrequency ?? TimeSpan.FromMinutes(10);
            MaxAllowedWaitBetweenPolls = maxAllowedWaitBetweenPolls ?? TimeSpan.FromSeconds(1);
            MinExpectedWaitBetweenPolls = minExpectedWaitBetweenPolls ?? TimeSpan.FromMilliseconds(100);
            FabricPollWaitMin = fabricPollWaitMin ?? TimeSpan.FromMilliseconds(100);
            FabricPollWaitMax = fabricPollWaitMax ?? TimeSpan.FromSeconds(1);
            PollTimeReduceRatio = pollTimeReduceRatio ?? 0.75M;
            CapacityPercentage = capacityPercentage ?? 0.75D;
            SupportPassDueScan = supportPassDueScan ?? false;
        }

        /// <summary>
        /// This property specifies the number of additional slots the clients can poll for on top of the maximum allowed.
        /// </summary>
        public int AllowedOverage { get; }

        /// <summary>
        /// This is the priority recalculate frequency. Leave this null if you do not wish it to recalculate.
        /// </summary>
        public TimeSpan? PriorityRecalculateFrequency { get; } 
        /// <summary>
        /// This is the maximum time between polls for a client. The default is 1 second.
        /// </summary>
        public TimeSpan MaxAllowedWaitBetweenPolls { get; }
        /// <summary>
        /// This is the minimum time between polls. The default is 100ms. This ensures we do not constantly poll a conneection.
        /// </summary>
        public TimeSpan MinExpectedWaitBetweenPolls { get; }
        /// <summary>
        /// This is the minimum wait time before polling the fabric.
        /// </summary>
        public TimeSpan FabricPollWaitMin { get; } 
        /// <summary>
        /// This is the maximum 
        /// </summary>
        public TimeSpan FabricPollWaitMax { get; }

        public decimal PollTimeReduceRatio { get; }

        public double CapacityPercentage { get; }

        /// <summary>
        /// This is the algorithm name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// This method specifies that the algorithm supports a pass due client scan before the main scan.
        /// </summary>
        public bool SupportPassDueScan { get; }


        public abstract int CalculateSlots(int available, IClientPriorityHolderMetrics context);

        public abstract bool ShouldSkip(IClientPriorityHolderMetrics context);

        public abstract void CapacityPercentageRecalculate(IClientPriorityHolderMetrics context);

        public abstract void CapacityReset(IClientPriorityHolderMetrics context);

        public abstract long PriorityRecalculate(long? queueLength, IClientPriorityHolderMetrics context, int? timeStamp = null);

        public abstract void PollMetricsRecalculate(bool success, bool hasErrored, IClientPriorityHolderMetrics context);

        public abstract void InitialiseMetrics(IClientPriorityHolderMetrics context);

        public abstract bool PastDueCalculate(IClientPriorityHolderMetrics context, int? timeStamp = null);


    }
}
