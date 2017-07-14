using System;

namespace Xigadee
{
    /// <summary>
    /// This interface contains the key metrics for the priority algorithm calculations.
    /// </summary>
    public interface IClientPriorityHolderMetrics
    {
        /// <summary>
        /// 
        /// </summary>
        double CapacityPercentage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int? FabricPollWaitTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int? LastPollTickCount { get; }
        /// <summary>
        /// 
        /// </summary>
        int? LastReserved { get; set; }
        /// <summary>
        /// 
        /// </summary>
        TimeSpan MaxAllowedPollWait { get; set; }
        /// <summary>
        /// 
        /// </summary>
        TimeSpan MinExpectedPollWait { get; set; }
        /// <summary>
        /// 
        /// </summary>
        long PollAchievedBatch { get; set; }
        /// <summary>
        /// 
        /// </summary>
        long PollAttemptedBatch { get; set; }
        /// <summary>
        /// 
        /// </summary>
        decimal PollSuccessRate { get; }
        /// <summary>
        /// 
        /// </summary>
        decimal? PollTimeReduceRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        long? PriorityCalculated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        long? PriorityQueueLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int? PriorityTickCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        decimal PriorityWeighting { get; }
        /// <summary>
        /// 
        /// </summary>
        IResourceRequestRateLimiter RateLimiter { get; }
        /// <summary>
        /// 
        /// </summary>
        int SkipCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool SkipCountDecrement();
    }
}