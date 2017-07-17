using System.Collections.Generic;

namespace Xigadee
{
    public interface IMessagingService<P> : IRequireDataCollector
        where P : PartitionConfig
    {
        string ChannelId { get; set; }

        List<P> PriorityPartitions { get; set; }
    }
}