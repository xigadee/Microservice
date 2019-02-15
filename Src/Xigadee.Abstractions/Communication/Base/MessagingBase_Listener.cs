using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace Xigadee
{
    public abstract partial class MessagingBase<C, M, H> 
    {
        public string ListenerMappingChannelId { get; set; }

        public List<ResourceProfile> ListenerResourceProfiles { get; set; }

        public List<ListenerPartitionConfig> ListenerPriorityPartitions { get; set; }

        public virtual bool ListenerPollSupported => false;

        public virtual bool ListenerPollRequired => false;

        public virtual Task ListenerPoll()
        {
            return Task.CompletedTask;
        }
    }
}
