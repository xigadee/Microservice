using System.Collections.Generic;

namespace Xigadee
{
    public interface IMessagingService<P> : IRequireBoundaryLogger
        where P : PartitionConfig
    {
        string ChannelId { get; set; }

        List<P> PriorityPartitions { get; set; }
    }
}