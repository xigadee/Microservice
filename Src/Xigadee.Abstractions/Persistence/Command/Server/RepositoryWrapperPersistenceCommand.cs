using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Xigadee
{
    #region RepositoryWrapperPersistenceCommand...
    /// <summary>
    /// This command provides the Xigadee plumbing to host a repository in a Microservice.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class RepositoryWrapperPersistenceCommand<K, E>
        : RepositoryWrapperPersistenceCommand<K, E, RepositoryPersistenceStatistics, RepositoryPersistenceCommandPolicy>
    where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryWrapperPersistenceCommand{K, E, S, P}"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="persistenceRetryPolicy">The persistence retry policy.</param>
        /// <param name="resourceProfile">The resource profile.</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="defaultTimeout">The default timeout.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="persistenceEntitySerializer">The persistence entity serializer.</param>
        /// <param name="cachingEntitySerializer">The caching entity serializer.</param>
        /// <param name="keySerializer">The key serializer.</param>
        /// <param name="keyDeserializer">The key deserializer.</param>
        /// <param name="referenceMaker">The reference maker.</param>
        /// <param name="referenceHashMaker">The reference hash maker.</param>
        /// <param name="policy">The policy.</param>
        public RepositoryWrapperPersistenceCommand(IRepositoryAsyncServer<K, E> repository,
              PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultTimeout = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , Func<E, K> keyMaker = null
            , EntitySerializer<E> persistenceEntitySerializer = null
            , EntitySerializer<E> cachingEntitySerializer = null
            , Func<K, string> keySerializer = null
            , Func<string, K> keyDeserializer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , RepositoryPersistenceCommandPolicy policy = null)
            : base(repository
                , persistenceRetryPolicy
                , resourceProfile
                , cacheManager
                , defaultTimeout
                , entityName
                , versionPolicy
                , keyMaker
                , persistenceEntitySerializer
                , cachingEntitySerializer
                , keySerializer
                , keyDeserializer
                , referenceMaker
                , referenceHashMaker
                , policy
                )
        {
        }
        #endregion
    } 
    #endregion
    /// <summary>
    /// This command provides the Xigadee plumbing to host a repository in a Microservice.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <typeparam name="S">The statistics type.</typeparam>
    /// <typeparam name="P">The command policy.</typeparam>
    public class RepositoryWrapperPersistenceCommand<K, E, S, P> : PersistenceCommandBase<K, E, S, P>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
        where P : PersistenceCommandPolicy, new()
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryWrapperPersistenceCommand{K, E, S, P}"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="persistenceRetryPolicy">The persistence retry policy.</param>
        /// <param name="resourceProfile">The resource profile.</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="defaultTimeout">The default timeout.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="persistenceEntitySerializer">The persistence entity serializer.</param>
        /// <param name="cachingEntitySerializer">The caching entity serializer.</param>
        /// <param name="keySerializer">The key serializer.</param>
        /// <param name="keyDeserializer">The key deserializer.</param>
        /// <param name="referenceMaker">The reference maker.</param>
        /// <param name="referenceHashMaker">The reference hash maker.</param>
        /// <param name="policy">The policy.</param>
        public RepositoryWrapperPersistenceCommand(IRepositoryAsyncServer<K, E> repository,
              PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultTimeout = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , Func<E, K> keyMaker = null
            , EntitySerializer<E> persistenceEntitySerializer = null
            , EntitySerializer<E> cachingEntitySerializer = null
            , Func<K, string> keySerializer = null
            , Func<string, K> keyDeserializer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , P policy = null) : base(persistenceRetryPolicy
                , resourceProfile
                , cacheManager
                , defaultTimeout
                , entityName
                , versionPolicy
                , keyMaker
                , persistenceEntitySerializer
                , cachingEntitySerializer
                , keySerializer
                , keyDeserializer
                , referenceMaker
                , referenceHashMaker
                , policy
                )
        {
            Repository = repository;

            Transform = RepositoryEntityTransformCreate(
                  repository ?? throw new ArgumentNullException("repository")
                , entityName, versionPolicy, keyMaker
                , persistenceEntitySerializer, cachingEntitySerializer
                , keySerializer, keyDeserializer, referenceMaker, referenceHashMaker);

        }
        #endregion

        /// <summary>
        /// Gets the internal repository.
        /// </summary>
        protected virtual IRepositoryAsyncServer<K, E> Repository { get; }


        #region RepositoryTransform
        /// <summary>
        /// Gets the repository transform that contains the repository.
        /// </summary>
        protected RepositoryEntityTransformHolder<K, E> RepositoryTransform => Transform as RepositoryEntityTransformHolder<K, E>;
        #endregion

        #region EntityTransform...
        protected virtual RepositoryEntityTransformHolder<K, E> RepositoryEntityTransformCreate(
            IRepositoryAsyncServer<K, E> repo
            , string entityName = null, VersionPolicy<E> versionPolicy = null, Func<E, K> keyMaker = null, EntitySerializer<E> persistenceEntitySerializer = null, EntitySerializer<E> cachingEntitySerializer = null, Func<K, string> keySerializer = null, Func<string, K> keyDeserializer = null, Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null, Func<Tuple<string, string>, string> referenceHashMaker = null)
        {
            var transform = new RepositoryEntityTransformHolder<K, E>(repo)
            {
                KeyMaker = keyMaker,
                KeySerializer = keySerializer ?? (i => i.ToString()),
                KeyDeserializer = keyDeserializer,
                ReferenceMaker = referenceMaker ?? (e => new Tuple<string, string>[] { }),
                ReferenceHashMaker = referenceHashMaker ?? (r => $"{r.Item1.ToLowerInvariant()}.{r.Item2.ToLowerInvariant()}"),
                Version = versionPolicy ?? new VersionPolicy<E>(),
                EntityName = entityName ?? typeof(E).Name.ToLowerInvariant(),
                PersistenceEntitySerializer = persistenceEntitySerializer,
                CacheEntitySerializer = cachingEntitySerializer,
            };

            return transform;
        }

        protected override EntityTransformHolder<K, E> EntityTransformCreate(string entityName = null, VersionPolicy<E> versionPolicy = null, Func<E, K> keyMaker = null, EntitySerializer<E> persistenceEntitySerializer = null, EntitySerializer<E> cachingEntitySerializer = null, Func<K, string> keySerializer = null, Func<string, K> keyDeserializer = null, Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null, Func<Tuple<string, string>, string> referenceHashMaker = null)
        {
            //We set this later in the main constructor with the repository.
            return null;
        } 
        #endregion

        protected override async Task<IResponseHolder<E>> InternalCreate(K key, PersistenceRequestHolder<K, E> holder)
        {
            var rs = await Repository.Create(holder.Rq.Entity);

            return new PersistenceResponseHolder<E>((PersistenceResponse)rs.ResponseCode, entity:rs.Entity);
        }

        protected override async Task<IResponseHolder<E>> InternalRead(K key, PersistenceRequestHolder<K, E> holder)
        {
            var rs = await Repository.Read(key);

            return new PersistenceResponseHolder<E>((PersistenceResponse)rs.ResponseCode, entity: rs.Entity);
        }

        protected override async Task<IResponseHolder<E>> InternalReadByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, E> holder)
        {
            var rs = await Repository.ReadByRef(reference.Item1, reference.Item2);

            return new PersistenceResponseHolder<E>((PersistenceResponse)rs.ResponseCode, entity: rs.Entity);
        }

        protected override async Task<IResponseHolder<E>> InternalUpdate(K key, PersistenceRequestHolder<K, E> holder)
        {
            var rs = await Repository.Update(holder.Rq.Entity);

            return new PersistenceResponseHolder<E>((PersistenceResponse)rs.ResponseCode, entity: rs.Entity);
        }

        protected override async Task<IResponseHolder> InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var rs = await Repository.Delete(key);

            return new PersistenceResponseHolder<E>((PersistenceResponse)rs.ResponseCode);
        }

        protected override async Task<IResponseHolder> InternalDeleteByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var rs = await Repository.DeleteByRef(reference.Item1, reference.Item2);

            //return new PersistenceResponseHolder<Tuple<K, string>> { StatusCode = 200, IsSuccess = true, Id = transform.KeySerializer(resolve.Item2), VersionId = resolve.Item3, Entity = new Tuple<K, string>(resolve.Item2, resolve.Item3) };

            return new PersistenceResponseHolder<E>((PersistenceResponse)rs.ResponseCode);
        }

        //protected override async Task<IResponseHolder> InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        //{
        //    var rs = await RepositoryTransform.Repository.Version(key);

        //    return new PersistenceResponseHolder<Tuple<K, string>> { StatusCode = 200, IsSuccess = true, Id = RepositoryTransform.KeySerializer(resolve.Item2), VersionId = resolve.Item3, Entity = new Tuple<K, string>(resolve.Item2, resolve.Item3) };
        //}

        protected override async Task<IResponseHolder> InternalVersionByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var rs = await Repository.VersionByRef(reference.Item1, reference.Item2);

            return await base.InternalVersionByRef(reference, holder);
        }

        protected override async Task<IResponseHolder<SearchResponse>> InternalSearch(SearchRequest key, PersistenceRequestHolder<SearchRequest, SearchResponse> holder)
        {
            var rs = await Repository.Search(key);

            return await base.InternalSearch(key, holder);
        }
    }

    public class RepositoryPersistenceCommandPolicy : PersistenceCommandPolicy { }

    public class RepositoryPersistenceStatistics : PersistenceStatistics { }

}
