using System.Collections.Generic;

namespace Xigadee
{
    public interface IMessagingService<P> 
        where P : PartitionConfig
    {
        IBoundaryLogger BoundaryLogger { get; set; }

        string ChannelId { get; set; }

        List<P> PriorityPartitions { get; set; }
    }
}