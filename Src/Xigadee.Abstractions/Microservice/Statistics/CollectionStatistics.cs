namespace Xigadee
{
    /// <summary>
    /// This is the base collection statistics.
    /// </summary>
    public class CollectionStatistics: StatusBase, ICollectionStatistics
    {
        /// <summary>
        /// The item count.
        /// </summary>
        public virtual int ItemCount { get; set; }
    }
}
