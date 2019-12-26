using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public class BlobStorageEntityContext : EntityContext
    {
        public BlobStorageEntityContext(RepositorySettings options) : base(options)
        {

        }
    }

    public class BlobStorageEntityContext<E> : BlobStorageEntityContext, IEntityContext<E>
    {
        public BlobStorageEntityContext(RepositorySettings options, E entity) : base(options)
        {
            EntityIncoming = entity;
        }

        public List<E> ResponseEntities { get; } = new List<E>();

        public E EntityIncoming { get; }

        public E EntityOutgoing { get; set; }
    }

    public class BlobStorageEntityContext<K,E> : BlobStorageEntityContext<E>, IEntityContext<K,E>
        where K: IEquatable<K>
    {
        public BlobStorageEntityContext(RepositorySettings options, K key, E entity = default(E)) : base(options, entity)
        {
            Key = key;
        }

        public K Key { get; }
    }
}
