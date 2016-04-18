#region using
using System;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the individual configuration for each partition.
    /// </summary>
    public class ListenerPartitionConfig: PartitionConfig
    {
        #region Static methods

        static readonly ListenerPartitionConfig[] mDefault;

        static ListenerPartitionConfig()
        {
            mDefault = Init(1);
        }

        public static ListenerPartitionConfig[] Init(params int[] priority)
        {
            return Init<ListenerPartitionConfig>(priority, (p, e) =>
            {
                e.SupportsRateLimiting = p == 0;
                e.DefaultTimeout = TimeSpan.FromMinutes(p == 0 ? 10 : 1);
            });
        }

        public static ListenerPartitionConfig[] Default
        {
            get
            {
                return mDefault;
            }
        } 
        #endregion

        /// <summary>
        /// This is the default constructor that sets the weighting to 1 (100%).
        /// </summary>
        public ListenerPartitionConfig() 
        {
            Weighting = 1;
            SupportsRateLimiting = true;
        }

        /// <summary>
        /// Identifies whether the paritition client will implement rate limiting.
        /// </summary>
        public bool SupportsRateLimiting { get; set; }

        /// <summary>
        /// This is the default timeout - 1 minute by default. 10 minutes for the async channel.
        /// </summary>
        public TimeSpan? DefaultTimeout { get; set; }

        /// <summary>
        /// This is the percentage weighting for the channel used when calculating priority over the 
        /// other queues. 1 is the default value. A value of 1.1 will increase the overall priority score by 10%.
        /// </summary>
        public decimal Weighting { get; set; }

    }
}
