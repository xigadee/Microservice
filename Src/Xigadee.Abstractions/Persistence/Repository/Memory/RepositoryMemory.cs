using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold an entity in memory with its associated properties and references.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public partial class RepositoryMemory<K, E> : RepositoryBase<K, E>
        where K : IEquatable<K>
    {
        #region Declarations        
        /// <summary>
        /// This is the entity container.
        /// </summary>
        protected readonly RepositoryMemoryContainer<K, E> _container;
        /// <summary>
        /// This is the
        /// </summary>
        protected readonly ConcurrentDictionary<K, E> _searchCache;
        /// <summary>
        /// This lock is used when modifying references.
        /// </summary>
        protected readonly ReaderWriterLockSlim _referenceModifyLock;
        /// <summary>
        /// The supported search collection.
        /// </summary>
        protected readonly Dictionary<string, RepositoryMemorySearch<K,E>> _supportedSearches;
        #endregion
        #region Constructor        
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryMemory{K, E}"/> class.
        /// </summary>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="referenceMaker">The reference maker function.</param>
        /// <param name="propertiesMaker">The properties maker function.</param>
        /// <param name="searches">The supported searches.</param>
        /// <param name="prePopulate">The pre-populate function.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <param name="readOnly">This property specifies that the collection is read-only.</param>
        /// <param name="sContext">This context contains the serialization components for storing the entities.</param>
        /// <param name="keyManager">The key serialization manager. if this is not passed, then a default serializer will be passed using the component model.</param>
        public RepositoryMemory(Func<E, K> keyMaker
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , IEnumerable<RepositoryMemorySearch<K,E>> searches = null
            , IEnumerable<E> prePopulate = null
            , bool readOnly = false
            , ServiceHandlerCollectionContext sContext = null
            , RepositoryKeyManager<K> keyManager = null
            )
            : base(keyMaker, referenceMaker, propertiesMaker, versionPolicy, keyManager)
        {
            _referenceModifyLock = new ReaderWriterLockSlim();

            _container = new RepositoryMemoryContainer<K, E>();
            _searchCache = new ConcurrentDictionary<K, E>();

            _supportedSearches = searches?.ToDictionary(s => s.Id.ToLowerInvariant(), s => s) ?? new Dictionary<string, RepositoryMemorySearch<K, E>>();

            SerializationContext = sContext ?? DefaultSerializationContext();

            prePopulate?.ForEach(ke => Create(ke));
            IsReadOnly = readOnly;
        }
        #endregion

        #region SearchAdd(RepositoryMemorySearch<K, E> algo, bool setAsDefault = false)
        /// <summary>
        /// This method can be used to add or replace a search algorithm.
        /// </summary>
        /// <param name="algo">The search algorithm.</param>
        /// <param name="setAsDefault">Set this algorithm as the default algorithm. If an Id is not set, this one will be used.</param>
        public void SearchAdd(RepositoryMemorySearch<K, E> algo, bool setAsDefault = false)
        {
            var id = algo?.Id;

            if (string.IsNullOrEmpty(id))
                throw new ArgumentOutOfRangeException($"search id must be a valid string.");

            _supportedSearches[id.ToLowerInvariant()] = algo;

            if (setAsDefault)
                SearchDefaultId = id;
        }
        #endregion
        #region SearchDefaultId
        /// <summary>
        /// This is the default search algorithm.
        /// </summary>
        public string SearchDefaultId { get; set; } 
        #endregion

        #region IsReadOnly
        /// <summary>
        /// Specifies whether the collection is read only.
        /// </summary>
        protected bool IsReadOnly { get; }
        #endregion


        #region Create(E entity)
        /// <summary>
        /// Implements the internal create logic.
        /// </summary>
        protected override Task<RepositoryHolder<K, E>> CreateInternal(K key, E entity, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction)
        {
            if (IsReadOnly)
                return ResultFormat(400, () => key, () => default(E), options, holderAction);

            //We have to be careful as the caller still has a reference to the old entity and may change it.
            var references = _referenceMaker?.Invoke(entity).ToList();
            var properties = _propertiesMaker?.Invoke(entity).ToList();

            E newEntity = default(E);

            var result = Atomic(true, () =>
            {
                var newContainer = CreateEntityContainer(
                    key, entity, references, properties, VersionPolicy?.EntityVersionAsString(entity), KeyManager.Serialize(key));

                //OK, add the entity
                if (!_container.Add(newContainer))
                    return 409;

                newEntity = newContainer.Entity;

                return 201;
            });

            return ResultFormat(result, () => key, () => newEntity, options, holderAction);
        }

        #endregion
        #region Read(K key)/ReadByRef(string refKey, string refValue)
        /// <summary>
        /// Read the entity
        /// </summary>
        protected override Task<RepositoryHolder<K, E>> ReadInternal(K key, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction)
        {
            EntityContainer<K,E> container = null;

            bool result = Atomic(false, () => _container.TryGetValue(key, out container));

            var entity = container == null ? default(E) : container.Entity;

            container?.ReadHitIncrement();

            return ResultFormat(result ? 200 : 404
                , () => result ? container.Key : default(K)
                , () => result ? entity : default(E)
                , options
                , holderAction
                );
        }
        /// <summary>
        /// Read by Reference
        /// </summary>
        protected override Task<RepositoryHolder<K, E>> ReadByRefInternal(string refKey, string refValue, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction)
        {
            var reference = new Tuple<string, string>(refKey, refValue);

            EntityContainer<K,E> container = null;

            bool result = Atomic(false, () => _container.TryGetValue(reference, out container));

            E entity = container == null ? default(E) : container.Entity;

            container?.ReadHitIncrement();

            return ResultFormat(result ? 200 : 404
                , () => result ? container.Key : default(K)
                , () => result ? entity : default(E)
                , options
                , holderAction
                );
        }
        #endregion
        #region Update(E entity)
        /// <summary>
        /// Updates the entity.
        /// </summary>
        protected override Task<RepositoryHolder<K, E>> UpdateInternal(K key, E entity, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction)
        {
            if (IsReadOnly)
                return ResultFormat(400, () => key, () => default(E), options, holderAction);

            var newReferences = _referenceMaker?.Invoke(entity).ToList();
            var newProperties = _propertiesMaker?.Invoke(entity).ToList();

            EntityContainer<K,E> newContainer = CreateEntityContainer(key, entity, newReferences, newProperties, null, KeyManager.Serialize(key));

            var newEntity = default(E);

            var result = Atomic(true, () =>
             {
                 //If the doesn't already exist in the collection, throw a not-found error.
                 if (!_container.TryGetValue(key, out var oldContainer))
                     return 404;

                 //OK, get the new references, but check whether they are assigned to another entity and if so flag an error.
                 if (_container.ReferenceExistingMatch(newReferences, true, key))
                     return 409;

                 //OK, do we have to update the version id?
                 if (VersionPolicy?.SupportsOptimisticLocking ?? false)
                 {
                     var incomingVersionId = VersionPolicy.EntityVersionAsString(entity);

                     //The version id should match the current stored version. If not we reject it.
                     if (incomingVersionId != oldContainer.VersionId)
                         return 409;

                     //OK, we don't want to modify the incoming entity, so we first need to clone it.
                     newEntity = newContainer.Entity;
                     //OK, update the entity version parameters in the new entity.
                     string newVersion = VersionPolicy.EntityVersionUpdate(newEntity);

                     //We need to update the container as the version has changed.
                     newContainer = CreateEntityContainer(key, newEntity, newReferences, newProperties, newVersion, KeyManager.Serialize(key));
                 }
                 else
                     newEntity = newContainer.Entity;

                 _container.Replace(oldContainer, newContainer);

                 return 200;
             });

            return ResultFormat(result, () => key, () => newEntity, options, holderAction);
        }
        #endregion
        #region Delete(K key)/DeleteByRef(string refKey, string refValue)
        /// <summary>
        /// Deletes the entity.
        /// </summary>
        protected override Task<RepositoryHolder<K, Tuple<K, string>>> DeleteInternal(K key, RepositorySettings options
            , Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            if (IsReadOnly)
                return ResultFormat(400, () => key, () => new Tuple<K, string>(key, ""), options, holderAction);

            EntityContainer<K, E> container;
            var result = Atomic(true, () => _container.Delete(key, out container));

            return ResultFormat(result ? 200 : 404, () => key, () => new Tuple<K, string>(key, ""), options, holderAction);
        }
        /// <summary>
        /// Delete the entity by reference
        /// </summary>
        protected override Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRefInternal(string refKey, string refValue, RepositorySettings options
            , Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            if (IsReadOnly)
                return ResultFormat(400, () => default(K), () => new Tuple<K, string>(default(K), ""), options, holderAction);

            OnKeyEvent(KeyEventType.BeforeDelete, refType: refKey, refValue: refValue);
            var reference = new Tuple<string, string>(refKey, refValue);

            EntityContainer<K, E> container = null;

            var result = Atomic(true, () => _container.Delete(reference, out container));

            var key = result ? container.Key : default(K);

            return ResultFormat(result ? 200 : 404
                , () => key
                , () => new Tuple<K, string>(key, "")
                , options, holderAction);
        }
        #endregion
        #region Version(K key)/VersionByRef(string refKey, string refValue)
        /// <summary>
        /// Retrieves the entity version.
        /// </summary>
        protected override Task<RepositoryHolder<K, Tuple<K, string>>> VersionInternal(K key, RepositorySettings options
            , Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            EntityContainer<K,E> container = null;

            var result = Atomic(false, () =>_container.TryGetValue(key, out container));

            container?.ReadHitIncrement();

            return ResultFormat(result ? 200 : 404
                , () => key
                , () => new Tuple<K, string>(key, container?.VersionId ?? "")
                , options
                , holderAction);
        }
        /// <summary>
        /// Returns the entity version by reference.
        /// </summary>
        protected override Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRefInternal(string refKey, string refValue
            , RepositorySettings options, Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            EntityContainer<K,E> container = null;

            var reference = new Tuple<string, string>(refKey, refValue);

            var result = Atomic(false, () =>_container.TryGetValue(reference, out container));

            container?.ReadHitIncrement();

            var key = result ? container.Key : default(K);

            return ResultFormat(result ? 200 : 404, () => key
                , () => new Tuple<K, string>(key, container?.VersionId ?? ""), options, holderAction);

        }
        #endregion

        #region Search(SearchRequest key)
        /// <summary>
        /// Searches the collection using the specified parameters.
        /// </summary>
        public override async Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest rq, RepositorySettings options = null)
        {
            var result = await SearchInternal(rq, options, (rs) =>
            {
                var output = new SearchResponse();

                output.Fields.Add(0, new FieldMetadata { Name = "_" });
                rq.Select().ForIndex((i, s) => output.Fields[i+1] = new FieldMetadata { Name = s });

                rs.ForEach(r => output.Data.Add(CollateRecord(rq, r).ToArray()));
                
                return output;
            });

            OnAfterSearchEvent(result);

            return result;
        }
        /// <summary>
        /// This method collates the search request.
        /// </summary>
        /// <param name="rq">The search request.</param>
        /// <param name="wrapper">The entity wrapper.</param>
        /// <returns></returns>
        protected virtual IEnumerable<string> CollateRecord(SearchRequest rq, EntityContainerWrapper<K, E> wrapper)
        {
            yield return wrapper.Container.Id;

            foreach (var id in rq.Select())
                yield return wrapper.PropertyGet(id);
        }
        #endregion
        #region SearchEntity(SearchRequest key)
        /// <summary>
        /// Searches the collection using the specified parameters.
        /// </summary>
        public override async Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest rq
            , RepositorySettings options = null)
        {
            var result = await SearchInternal(rq, options, (rs) =>
            {
                var output = new SearchResponse<E>();
                output.Data = rs.Select(i => i.Entity).ToList();
                return output;
            });

            OnAfterSearchEntityEvent(result);

            return result;
        }
        #endregion

        #region SearchInternal<S> ...
        /// <summary>
        /// This method provides the generic search method.
        /// </summary>
        /// <typeparam name="S">The response type.</typeparam>
        /// <param name="rq">The search request</param>
        /// <param name="options">The options.</param>
        /// <param name="loader">The loader method.</param>
        /// <returns>Returns the holder.</returns>
        protected virtual async Task<RepositoryHolder<SearchRequest, S>> SearchInternal<S>(SearchRequest rq, RepositorySettings options
            , Func<IEnumerable<EntityContainerWrapper<K, E>>, S> loader)
            where S : SearchResponseBase, new()
        {
            OnBeforeSearchEvent(rq);

            var result = new RepositoryHolder<SearchRequest, S>();

            result.Key = rq;
            result.Entity = new S();

            //Get the search id
            var searchId = rq.Id?.ToLowerInvariant() ?? SearchDefaultId?.ToLowerInvariant();

            try
            {
                if (_supportedSearches.ContainsKey(searchId))
                {
                    result.ResponseCode = 200;
                    var output = await Atomic(false, async () => await _supportedSearches[searchId].SearchEntity(_container, rq, options));
                    result.Entity = loader(output);

                    result.Entity.Etag = ETag;
                    result.Entity.Skip = rq.SkipValue;
                    result.Entity.Top = rq.TopValue;

                }
                else
                {
                    result.ResponseCode = 404;
                    result.ResponseMessage = $"Search algorithm '{searchId}' cannot be found.";
                }
            }
            catch (Exception ex)
            {
                result.ResponseCode = 500;
                result.ResponseMessage = $"Search algorithm '{searchId}' unexpected exception.";
                result.Ex = ex;
            }

            return result;
        }
        #endregion

        #region SerializationContext
        /// <summary>
        /// Gets the serialization context that is used to serialize and deserialize the container entity.
        /// </summary>
        protected virtual ServiceHandlerCollectionContext SerializationContext { get; }
        #endregion
        #region DefaultSerializationContext()
        /// <summary>
        /// Creates the default serialization context. Json serialization with gzip compression.
        /// </summary>
        protected virtual ServiceHandlerCollectionContext DefaultSerializationContext()
        {
            var context = new ServiceHandlerCollectionContext();

            context.Set(new JsonRawSerializer());
            context.Set(new CompressorGzip());

            return context;
        }
        #endregion

        #region CreateEntityContainer
        /// <summary>
        /// Creates the entity container.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="newEntity">The new entity.</param>
        /// <param name="newReferences">The new references.</param>
        /// <param name="newProperties">The new properties.</param>
        /// <param name="newVersionId">The new version identifier.</param>
        /// <returns>Returns the new container with the serialized entity.</returns>
        protected virtual EntityContainer<K, E> CreateEntityContainer(K key, E newEntity
                , IEnumerable<Tuple<string, string>> newReferences
                , IEnumerable<Tuple<string, string>> newProperties
                , string newVersionId
                , string keyAsString)
        {
            return new EntityContainer<K, E>(
                key, newEntity, newReferences, newProperties, newVersionId, EntityDeserialize, EntitySerialize, keyAsString);
        }

        protected virtual byte[] EntitySerialize(E entity)
        {
            if (!SerializationContext.HasSerialization)
                throw new ArgumentOutOfRangeException("SerializationContext.Serializer is not set.");

            var ctx = ServiceHandlerContext.CreateWithObject(entity);

            if (entity.Equals(default(E)))
                return null;

            SerializationContext.Serializer.TrySerialize(ctx);

            return ctx.Blob;
        }

        protected virtual E EntityDeserialize(byte[] blob)
        {
            if (!SerializationContext.HasSerialization)
                throw new ArgumentOutOfRangeException("SerializationContext.Serializer is not set.");

            if ((blob?.Length ?? 0) == 0)
                return default(E);

            var ctx = ServiceHandlerContext.CreateWithBlob(
                blob, SerializationContext.Serialization, SerializationContext.Compression, typeof(E).FullName);

            SerializationContext.Serializer.TryDeserialize(ctx);

            return (E)ctx.Object;
        }
        #endregion

        #region Count
        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        public virtual int Count => Atomic(false, () => _container.Count);
        #endregion
        #region CountReference
        /// <summary>
        /// This is the number of entity references in the collection.
        /// </summary>
        public virtual int CountReference => Atomic(false, () => _container.CountReference);
        #endregion

        #region ContainsKey(K key)
        /// <summary>
        /// Determines whether the collection contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the collection contains the key; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool ContainsKey(K key) => Atomic(false, () => _container.Contains(key));
        #endregion
        #region ContainsReference(Tuple<string, string> reference)
        /// <summary>
        /// Determines whether the collection contains the entity reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>
        ///   <c>true</c> if the collection contains reference; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool ContainsReference(Tuple<string, string> reference) => Atomic(false, () => _container.Contains(reference));
        #endregion

        #region Atomic...
        /// <summary>
        /// This wraps the requests the ensure that only one is processed at the same time.
        /// </summary>
        /// <param name="write">Specifies whether this is a write action. This will block read actions.</param>
        /// <param name="action">The action to process.</param>
        [DebuggerStepThrough]
        protected void Atomic(bool write, Action action)
        {
            try
            {
                if (write)
                    _referenceModifyLock.EnterWriteLock();
                else
                    _referenceModifyLock.EnterReadLock();

                action();
            }
            finally
            {
                if (write)
                    _referenceModifyLock.ExitWriteLock();
                else
                    _referenceModifyLock.ExitReadLock();
            }
        }

        /// <summary>
        /// This wraps the requests the ensure that only one is processed at the same time.
        /// </summary>
        /// <param name="write">Specifies whether this is a write action. This will block read actions.</param>
        /// <param name="action">The action to process.</param>
        /// <returns>Returns the value.</returns>
        [DebuggerStepThrough]
        protected T Atomic<T>(bool write, Func<T> action)
        {
            try
            {
                if (write)
                    _referenceModifyLock.EnterWriteLock();
                else
                    _referenceModifyLock.EnterReadLock();

                return action();
            }
            finally
            {
                if (write)
                    _referenceModifyLock.ExitWriteLock();
                else
                    _referenceModifyLock.ExitReadLock();
            }
        }
        #endregion

        #region ETag
        /// <summary>
        /// Gets the current collection ETag. This changes when an entity is created/updated or deleted.
        /// </summary>
        public string ETag => $"{ETagCollectionId}:{_container.ETagOrdinal}";
        #endregion
        #region ETagCollectionId
        /// <summary>
        /// Gets the collection identifier that is set when the collection is created.
        /// </summary>
        public string ETagCollectionId { get; } = $"{typeof(E).Name}:{Guid.NewGuid().ToString("N").ToUpperInvariant()}";
        #endregion
    }
}
