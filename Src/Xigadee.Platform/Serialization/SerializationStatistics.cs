namespace Xigadee
{
    /// <summary>
    /// This is the statistics collection for the serialization container.
    /// </summary>
    public class SerializationStatistics: CollectionStatistics
    {
        /// <summary>
        /// The cache count.
        /// </summary>
        public virtual int CacheCount { get; set; }
        /// <summary>
        /// The supported serialization classes.
        /// </summary>
        public string[] Serialization { get; set; }
        /// <summary>
        /// The supported serialization classes.
        /// </summary>
        public string[] Compression { get; set; }
        /// <summary>
        /// The serialization cache.
        /// </summary>
        public string[] Cache { get; set; }
    }
}
