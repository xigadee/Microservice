namespace Xigadee
{
    /// <summary>
    /// This is the base data collection statistics.
    /// </summary>
    public class DataCollectionContainerStatistics : MessagingStatistics
    {
        public int ItemCount { get; set; }

        public int QueueLength { get; set; }

        public bool Overloaded { get; set; }

        public int OverloadProcessCount { get; set; }

        public int? OverloadThreshold { get; set; }

        public long OverloadProcessHits { get; set; }
    }
}
