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
        int AllowedOverage { get; set; }
        /// <summary>
        /// This is the priority recalculate frequency. Leave this null if you do not wish it to recalculate.
        /// </summary>
        TimeSpan? PriorityRecalculateFrequency { get; set; }

        string Name { get; }

        TimeSpan FabricPollWaitMin { get; set; }

        TimeSpan FabricPollWaitMax { get; set; }

        TimeSpan MaxAllowedWaitBetweenPolls { get; set; }

        TimeSpan MinExpectedWaitBetweenPolls { get; set; }

        decimal PollTimeReduceRatio { get; set; }

        double CapacityPercentage { get; set; }

        int CalculateSlots(int available, ClientPriorityHolderMetrics context);

        bool ShouldSkip(ClientPriorityHolderMetrics context);

        void CapacityPercentageRecalculate(ClientPriorityHolderMetrics context);

        void CapacityReset(ClientPriorityHolderMetrics context);

        long PriorityRecalculate(long? queueLength, ClientPriorityHolderMetrics context, int? timeStamp = null);

        void PollMetricsRecalculate(bool success, bool hasErrored, ClientPriorityHolderMetrics context);

        void InitialiseMetrics(ClientPriorityHolderMetrics context);

        bool PastDueCalculate(ClientPriorityHolderMetrics context, int? timeStamp = null);
    }


}
