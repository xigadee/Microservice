#region using

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the abstract base class for persistence services that use JSON and the serialization mechanism.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="E"></typeparam>
    public abstract class PersistenceManagerHandlerJsonBase<K, E, S, P> : PersistenceCommandBase<K, E, S, P>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
        where P : PersistenceCommandPolicy, new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="keyDeserializer"></param>
        /// <param name="entityName">The entity name, derived from E if left null.</param>
        /// <param name="versionPolicy">The optional version and locking policy.</param>
        /// <param name="defaultTimeout">The default timeout when making requests.</param>
        /// <param name="keyMaker"></param>
        /// <param name="persistenceRetryPolicy"></param>
        /// <param name="resourceProfile"></param>
        /// <param name="cacheManager"></param>
        /// <param name="referenceMaker"></param>
        /// <param name="referenceHashMaker"></param>
        /// <param name="keySerializer"></param>
        protected PersistenceManagerHandlerJsonBase(
              Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , Func<K, string> keySerializer = null
            ) :
            base(persistenceRetryPolicy: persistenceRetryPolicy
                , resourceProfile: resourceProfile
                , cacheManager: cacheManager
                , entityName: entityName
                , versionPolicy: versionPolicy
                , defaultTimeout: defaultTimeout
                , keyMaker: keyMaker
                , referenceMaker: referenceMaker
                , referenceHashMaker: referenceHashMaker
                , keySerializer: keySerializer
                , keyDeserializer: keyDeserializer
                )
        {
        }
        #endregion

        #region EntityTransformCreate...
        /// <summary>
        /// This method sets the Json serializer as the primary transform mechanism.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="versionPolicy"></param>
        /// <param name="keyMaker"></param>
        /// <param name="persistenceEntitySerializer"></param>
        /// <param name="cachingEntitySerializer"></param>
        /// <param name="keySerializer"></param>
        /// <param name="keyDeserializer"></param>
        /// <param name="referenceMaker"></param>
        /// <param name="referenceHashMaker"></param>
        /// <returns></returns>
        protected override EntityTransformHolder<K, E> EntityTransformCreate(
              string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , Func<E, K> keyMaker = null
            , EntitySerializer<E> persistenceEntitySerializer = null
            , EntitySerializer<E> cachingEntitySerializer = null
            , Func<K, string> keySerializer = null
            , Func<string, K> keyDeserializer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null)
        {
            var transform = base.EntityTransformCreate(
                  entityName, versionPolicy, keyMaker
                , persistenceEntitySerializer, cachingEntitySerializer
                , keySerializer, keyDeserializer, referenceMaker, referenceHashMaker);

            // Use Json for both persistence and caching serialization
            transform.PersistenceEntitySerializer = transform.CacheEntitySerializer = new EntitySerializer<E>(transform.JsonSerialize, transform.JsonDeserialize);

            return transform;
        }
        #endregion

    }
}
