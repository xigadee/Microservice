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
    /// This abstract class is the base class to allocate polling slots to the listener collection.
    /// </summary>
    public abstract class ListenerClientPollAlgorithmBase: IListenerClientPollAlgorithm
    {
        /// <summary>
        /// This property specifies the number of additional slots the clients can poll for on top of the maximum allowed.
        /// </summary>
        public int AllowedOverage { get; set; } = 5;

        #region Name
        /// <summary>
        /// This is the algorithm name.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return GetType().Name;
            }
        } 
        #endregion

        /// <summary>
        /// This is the priority recalculate frequency. Leave this null if you do not wish it to recalculate.
        /// </summary>
        public TimeSpan? PriorityRecalculateFrequency { get; set; } = TimeSpan.FromMinutes(10);

        public TimeSpan MaxAllowedWaitBetweenPolls { get; set; } = TimeSpan.FromSeconds(1);

        public TimeSpan MinExpectedWaitBetweenPolls { get; set; } = TimeSpan.FromMilliseconds(100);

        public TimeSpan FabricPollWaitMin { get; set; } = TimeSpan.FromMilliseconds(100);

        public TimeSpan FabricPollWaitMax { get; set; } = TimeSpan.FromSeconds(1);

        public decimal PollTimeReduceRatio { get; set; } = 0.75M;

        public double CapacityPercentage { get; set; } = 0.75D;

        public abstract int CalculateSlots(int available, IClientPriorityHolderMetrics context);

        public abstract bool ShouldSkip(IClientPriorityHolderMetrics context);

        public abstract void CapacityPercentageRecalculate(IClientPriorityHolderMetrics context);

        public abstract void CapacityReset(IClientPriorityHolderMetrics context);

        public abstract long PriorityRecalculate(long? queueLength, IClientPriorityHolderMetrics context, int? timeStamp = null);

        public abstract void PollMetricsRecalculate(bool success, bool hasErrored, IClientPriorityHolderMetrics context);

        public abstract void InitialiseMetrics(IClientPriorityHolderMetrics context);

        public abstract bool PastDueCalculate(IClientPriorityHolderMetrics context, int? timeStamp = null);
        /// <summary>
        /// This method specifies that the algorithm supports a pass due client scan before the main scan.
        /// </summary>
        public virtual bool SupportPassDueScan { get { return false; } }
    }
}
