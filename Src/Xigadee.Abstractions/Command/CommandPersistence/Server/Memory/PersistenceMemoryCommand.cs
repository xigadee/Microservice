using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public class PersistenceMemoryStatistics : PersistenceStatistics
    {

    }

    public class PersistenceMemoryCommand<K, E> : RepositoryWrapperPersistenceCommand<K, E, PersistenceMemoryStatistics, PersistenceCommandPolicy>
        where K : IEquatable<K>
    {
        public PersistenceMemoryCommand(IRepositoryAsyncServer<K, E> repository
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultTimeout = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , Func<E, K> keyMaker = null
            , EntitySerializer<E> persistenceEntitySerializer = null
            , EntitySerializer<E> cachingEntitySerializer = null
            , Func<K, string> keySerializer = null, Func<string, K> keyDeserializer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , PersistenceCommandPolicy policy = null) 
            : base(repository, persistenceRetryPolicy, resourceProfile, cacheManager, defaultTimeout, entityName
                  , versionPolicy ?? repository.VersionPolicy
                  , keyMaker
                  , persistenceEntitySerializer, cachingEntitySerializer
                  , keySerializer, keyDeserializer
                  , referenceMaker
                  , referenceHashMaker
                  , policy)
        {            
        }
    }
}
