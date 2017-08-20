using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is the root interface for services that provide messaging services.
    /// </summary>
    /// <typeparam name="P">The partition type.</typeparam>
    /// <seealso cref="Xigadee.IRequireDataCollector" />
    public interface IMessagingService<P> : IRequireDataCollector
        where P : PartitionConfig
    {
        /// <summary>
        /// Gets or sets the channel identifier.
        /// </summary>
        string ChannelId { get; set; }
        /// <summary>
        /// Gets or sets a list of priority partitions.
        /// </summary>
        List<P> PriorityPartitions { get; set; }
    }
}