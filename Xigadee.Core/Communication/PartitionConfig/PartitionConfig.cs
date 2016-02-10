#region using
using System;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract class PartitionConfig
    {
        protected internal PartitionConfig()
        {
        }

        /// <summary>
        /// This is the numeric partition id.
        /// </summary>
        public int Id { get; internal set; }

        internal static P[] Init<P>(int[] priority, Action<int,P> initiate) where P : PartitionConfig, new()
        {
            return priority.Select((p) =>
            {
                var item = Create<P>(p);
                initiate(p,item);
                return item;
            }).ToArray();
        }

        public static P Create<P>(int priority, bool? supportsRateLimiting = null, TimeSpan? defaultTimeout = null) where P : PartitionConfig, new()
        {
            var item = new P();
            item.Id = priority;
            return item;
        }



    }
}
