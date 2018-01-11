using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    public class EntityContainerFile<K,E>: EntityContainerBase<K,E>
        where K : IEquatable<K>
    {
        public override int Count => throw new NotImplementedException();

        public override int CountReference => throw new NotImplementedException();

        public override ICollection<K> Keys => throw new NotImplementedException();

        public override ICollection<Tuple<string, string>> References => throw new NotImplementedException();

        public override ICollection<E> Values => throw new NotImplementedException();

        public override int Add(K key, E value, IEnumerable<Tuple<string, string>> references = null)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override bool ContainsKey(K key)
        {
            throw new NotImplementedException();
        }

        public override bool ContainsReference(Tuple<string, string> reference)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(K key)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(Tuple<string, string> reference)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetValue(K key, out E value)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetValue(Tuple<string, string> reference, out E value)
        {
            throw new NotImplementedException();
        }

        public override int Update(K key, E newEntity, IEnumerable<Tuple<string, string>> newReferences = null)
        {
            throw new NotImplementedException();
        }
    }
}
