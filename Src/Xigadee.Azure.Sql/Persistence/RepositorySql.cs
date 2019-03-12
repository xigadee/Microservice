using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public abstract class RepositorySqlBase<K, E> : RepositoryBase<K, E>
        where K : IEquatable<K>
    {
        protected RepositorySqlBase(Func<E, K> keyMaker
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null) 
            : base(keyMaker, referenceMaker, propertiesMaker, versionPolicy)
        {
        }
    }
}
