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

        void SkipCountRecalculate(bool success, ClientPriorityHolderMetrics context);

        void InitialiseMetrics(ClientPriorityHolderMetrics context);

        bool PastDueCalculate(ClientPriorityHolderMetrics context, int? timeStamp = null);
    }


}
