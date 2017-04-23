#region using
using System;
using System.Collections.Generic;
using System.Linq; 
#endregion
namespace Xigadee
{
    public class EntityChangeReference
    {
        public string Version { get; set; }

        public string Source { get; set; }

        public Guid? CustomerId { get; set; }

        public List<EntityCacheReferenceTypeValueKey> References { get; set; }
    }

    public class EntityChangeReference<K> : EntityChangeReference
        where K : System.IEquatable<K>
    {
        public EntityChangeReference(K key, string version, string source = null, IEnumerable<EntityCacheReferenceTypeValueKey> references = null, Guid? customerId = null)
        {
            Key = key;
            Version = version;
            Source = source;
            References = references == null ? new List<EntityCacheReferenceTypeValueKey>() : references.ToList();
            CustomerId = customerId;
        }

        public K Key { get; set; }

    }
}
