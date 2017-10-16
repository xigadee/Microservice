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
    /// This is the interface used by poll algorithms.
    /// </summary>
    public interface IListenerClientPollAlgorithm
    {
        /// <summary>
        /// This method specifies that the algorithm supports a pass due client scan before the main scan.
        /// </summary>
        bool SupportPassDueScan { get; }
        /// <summary>
        /// This is the number of allowed additional slots over the available slots.
        /// </summary>
        int AllowedOverage { get; }
        /// <summary>
        /// This is the priority recalculate frequency. Leave this null if you do not wish it to recalculate.
        /// </summary>
        TimeSpan? PriorityRecalculateFrequency { get; }
        /// <summary>
        /// The is the frequency that the client collection should be occasionally recalculated. The default is every 30 minutes.
        /// </summary>
        TimeSpan? PriorityRebuildFrequency { get; }

        /// <summary>
        /// Gets the name of the algorithm.
        /// </summary>
        string Name { get; }

        TimeSpan FabricPollWaitMin { get; }

        TimeSpan FabricPollWaitMax { get; }

        TimeSpan MaxAllowedWaitBetweenPolls { get; }

        TimeSpan MinExpectedWaitBetweenPolls { get; }

        decimal PollTimeReduceRatio { get; }

        double CapacityPercentage { get; }

        int CalculateSlots(int available, IClientPriorityHolderMetrics context);

        bool ShouldSkip(IClientPriorityHolderMetrics context);

        void CapacityPercentageRecalculate(IClientPriorityHolderMetrics context);

        void CapacityReset(IClientPriorityHolderMetrics context);

        long PriorityRecalculate(long? queueLength, IClientPriorityHolderMetrics context, int? timeStamp = null);

        void PollMetricsRecalculate(bool success, bool hasErrored, IClientPriorityHolderMetrics context);

        void InitialiseMetrics(IClientPriorityHolderMetrics context);

        bool PastDueCalculate(IClientPriorityHolderMetrics context, int? timeStamp = null);
    }
}
