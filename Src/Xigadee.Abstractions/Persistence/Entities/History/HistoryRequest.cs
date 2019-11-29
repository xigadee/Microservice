using System;

namespace Xigadee
{
    public class HistoryRequest<K>: SearchRequest,IEquatable<HistoryRequest<K>>
        where K : IEquatable<K>
    {
        public K EntityKey { get; set; }

        public bool Equals(HistoryRequest<K> other)
        {
            return EntityKey.Equals(other.EntityKey);
        }
    }
}
