using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class EntityCacheAsyncPolicy:CommandPolicy
    {
        public EntityCacheAsyncPolicy()
        {
        }

        public bool EntityChangeTrackEvents { get; set; }

        public string EntityChangeEventsChannel { get; set; }

        public int EntityCacheLimit { get; set; } = 200000;

        public TimeSpan EntityDefaultTTL { get; set; } = TimeSpan.FromDays(2);
    }
}
