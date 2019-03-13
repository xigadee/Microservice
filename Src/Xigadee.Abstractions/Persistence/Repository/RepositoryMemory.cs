using Newtonsoft.Json;
using System;
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
        /// This container holds the entities.
        /// </summary>
        protected readonly Dictionary<K, EntityContainer> _container;
        /// <summary>
        /// This container holds the key references.
        /// </summary>
        protected readonly Dictionary<Tuple<string, string>, EntityContainer> _containerReference;
        /// <summary>
        /// This lock is used when modifying references.
        /// </summary>
        protected readonly ReaderWriterLockSlim _referenceModifyLock;

        protected Dictionary<string, Func<E, List<KeyValuePair<string, string>>, bool>> _filterMethods;

        protected readonly Func<IEnumerable<Tuple<string, Func<E, List<KeyValuePair<string, string>>, bool>>>> _searchMaker;

        protected long _etagOrdinal = 0;

        #endregion
        #region Constructor        
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryMemory{K, E}"/> class.
        /// </summary>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="referenceMaker">The reference maker.</param>
        /// <param name="propertiesMaker">The properties maker.</param>
        /// <param name="searchMaker">The search maker.</param>
        /// <param name="prePopulate">The pre-populate function.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <param name="readOnly">This property specifies that the collection is read-only.</param>
        /// <param name="sContext">This context contains the serialization components.</param>
        public RepositoryMemory(Func<E, K> keyMaker
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , Func<IEnumerable<Tuple<string, Func<E, List<KeyValuePair<string, string>>, bool>>>> searchMaker = null
            , IEnumerable<E> prePopulate = null
            , VersionPolicy<E> versionPolicy = null
            , bool readOnly = false
            , ServiceHandlerCollectionContext sContext = null
            )
            : base(keyMaker, referenceMaker, propertiesMaker, versionPolicy)
        {
            _referenceModifyLock = new ReaderWriterLockSlim();

            _container = new Dictionary<K, EntityContainer>();
            _containerReference = new Dictionary<Tuple<string, string>, EntityContainer>(new ReferenceComparer());

            _filterMethods = new Dictionary<string, Func<E, List<KeyValuePair<string, string>>, bool>>();
            searchMaker?.Invoke().ForEach((t) => _filterMethods.Add(t.Item1, t.Item2));

            SerializationContext = sContext ?? DefaultSerializationContext();

            prePopulate?.ForEach(ke => Create(ke));
            IsReadOnly = readOnly;
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
        protected virtual EntityContainer CreateEntityContainer(K key, E newEntity
                , IEnumerable<Tuple<string, string>> newReferences
                , IEnumerable<Tuple<string, string>> newProperties
                , string newVersionId)
        {
            return new EntityContainer(key, newEntity, newReferences, newProperties, newVersionId, EntityDeserialize, EntitySerialize);
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

        #region Create(E entity)
        /// <summary>
        /// Create
        /// </summary>
        public override Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings options = null)
        {
            var key = KeyMaker(entity);

            if (IsReadOnly)
                return ResultFormat(400, () => key, () => default(E));

            IncomingParameterChecks(key, entity);

            OnEntityEvent(EntityEventType.BeforeCreate, () => entity);

            //We have to be careful as the caller still has a reference to the old entity and may change it.
            var references = _referenceMaker?.Invoke(entity).ToList();
            var properties = _propertiesMaker?.Invoke(entity).ToList();

            E newEntity = default(E);

            var result = Atomic(true,() =>
            {
                //Does the key already exist in the collection
                if (_container.ContainsKey(key))
                    return 409;

                //Are there any references? And do they already exist.
                if (ReferenceExistingMatch(references))
                    return 409;

                var newContainer = CreateEntityContainer(key, entity, references, properties, VersionPolicy?.EntityVersionAsString(entity));
                newEntity = newContainer.Entity;

                //OK, add the entity
                _container.Add(key, newContainer);

                //Add the entity references
                newContainer.References.ForEach((r) => _containerReference.Add(r, newContainer));

                ETagOrdinalIncrement();

                return 201;
            });

            if (result == 201)
                OnEntityEvent(EntityEventType.AfterCreate, () => newEntity);

            return ResultFormat(result, () => key, () => newEntity, options);
        }
        #endregion
        #region Read(K key)/ReadByRef(string refKey, string refValue)
        /// <summary>
        /// Read
        /// </summary>
        public override Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeRead, key);

            IncomingParameterChecks(key);

            EntityContainer container = null;

            bool result = Atomic(false, () => _container.TryGetValue(key, out container));

            var entity = container == null ? default(E) : container.Entity;

            container?.ReadHitIncrement();

            return ResultFormat(result ? 200 : 404
                , () => result ? container.Key : default(K)
                , () => result ? entity : default(E)
                , options
                );
        }
        /// <summary>
        /// Read by Reference
        /// </summary>
        public override Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeRead, refType: refKey, refValue: refValue);

            var reference = new Tuple<string, string>(refKey, refValue);

            EntityContainer container = null;

            bool result = Atomic(false,() => _containerReference.TryGetValue(reference, out container));

            E entity = container == null ?default(E):container.Entity;

            container?.ReadHitIncrement();

            return ResultFormat(result ? 200 : 404
                , () => result ? container.Key : default(K)
                , () => result ? entity : default(E)
                , options
                );
        }
        #endregion
        #region Update(E entity)
        /// <summary>
        /// Update
        /// </summary>
        public override Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings options = null)
        {
            var key = KeyMaker(entity);

            if (IsReadOnly)
                return ResultFormat(400, () => key, () => default(E));

            IncomingParameterChecks(key, entity);

            OnEntityEvent(EntityEventType.BeforeUpdate, () => entity);

            var newReferences = _referenceMaker?.Invoke(entity).ToList();
            var newProperties = _propertiesMaker?.Invoke(entity).ToList();

            EntityContainer newContainer = null;
            E newEntity = default(E);
            string newVersion = null;

            var result = Atomic(true,() =>
            {
                //If the doesn't already exist in the collection, throw a not-found error.
                if (!_container.TryGetValue(key, out var oldContainer))
                    return 404;

                //OK, get the new references, but check whether they are assigned to another entity and if so flag an error.
                if (ReferenceExistingMatch(newReferences, true, key))
                    return 409;

                //OK, we don't want to modify the incoming entity, so we first need to clone it.
                newEntity = entity.JsonClone();

                if (VersionPolicy?.SupportsOptimisticLocking ?? false)
                {
                    var incomingVersionId = VersionPolicy.EntityVersionAsString(entity);

                    //The version id should match the current stored version. If not we reject it.
                    if (incomingVersionId != oldContainer.VersionId)
                        return 409;

                    //OK, update the entity version parameters in the new entity.
                    newVersion = VersionPolicy.EntityVersionUpdate(newEntity);
                }

                newContainer = CreateEntityContainer(key, newEntity, newReferences, newProperties, newVersion);

                //OK, update the entity
                _container[key] = newContainer;
                //Remove the old references, and add the new references.
                //Note we're being lazy we add/replace even if nothing has changed.
                oldContainer.References.ForEach((r) => _containerReference.Remove(r));
                newContainer.References.ForEach((r) => _containerReference.Add(r, newContainer));

                ETagOrdinalIncrement();

                return 200;
            });

            //All good.
            if (result == 200)
                OnEntityEvent(EntityEventType.AfterUpdate, () => newEntity);

            return ResultFormat(result, () => key, () => newEntity, options);
        }
        #endregion

        #region Delete(K key)/DeleteByRef(string refKey, string refValue)
        /// <summary>
        /// Delete
        /// </summary>
        public override Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings options = null)
        {
            if (IsReadOnly)
                return ResultFormat(400, () => key, () => new Tuple<K, string>(key, ""));

            IncomingParameterChecks(key);

            OnKeyEvent(KeyEventType.BeforeDelete, key);

            var result = Atomic(true,() =>
            {
                if (_container.TryGetValue(key, out var container))
                    return DeleteInternal(container);

                return false;
            });

            return ResultFormat(result ? 200 : 404, () => key, () => new Tuple<K, string>(key, ""), options);
        }
        /// <summary>
        /// Delete by reference
        /// </summary>
        public override Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            if (IsReadOnly)
                return ResultFormat(400, () => default(K), () => new Tuple<K, string>(default(K), ""));

            OnKeyEvent(KeyEventType.BeforeDelete, refType: refKey, refValue: refValue);
            var reference = new Tuple<string, string>(refKey, refValue);
            EntityContainer container = null;

            var result = Atomic(true, () =>
            {
                if (!_containerReference.TryGetValue(reference, out container))
                    return false;

                return DeleteInternal(container);
            });

            var key = result ? container.Key : default(K);

            return ResultFormat(result ? 200 : 404
                , () => key
                , () => new Tuple<K, string>(key, "")
                , options);
        }

        private bool DeleteInternal(EntityContainer container)
        {
            bool result = _container.Remove(container.Key);

            if (result)
            {
                container.References.ForEach((r) => _containerReference.Remove(r));

                ETagOrdinalIncrement();
            }

            return result;
        }
        #endregion
        #region Version(K key)/VersionByRef(string refKey, string refValue)
        /// <summary>
        /// Version
        /// </summary>
        public override Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings options = null)
        {
            IncomingParameterChecks(key);

            OnKeyEvent(KeyEventType.BeforeVersion, key);

            EntityContainer container = null;

            var result = Atomic(false, () =>
            {
                return _container.TryGetValue(key, out container);
            });

            container?.ReadHitIncrement();

            return ResultFormat(result ? 200 : 404
                , () => key
                , () => new Tuple<K, string>(key, container?.VersionId ?? "")
                , options);
        }
        /// <summary>
        /// Returns the version by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <returns>Returns the entity key and version identifier.</returns>
        public override Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeVersion, refType: refKey, refValue: refValue);

            EntityContainer container = null;

            var reference = new Tuple<string, string>(refKey, refValue);

            var result = Atomic(false, () =>
            {
                return _containerReference.TryGetValue(reference, out container);
            });

            container?.ReadHitIncrement();

            var key = result ? container.Key : default(K);

            return ResultFormat(result ? 200 : 404, () => key
                , () => new Tuple<K, string>(key, container?.VersionId ?? ""));

        }
        #endregion

        #region Search(SearchRequest key)
        /// <summary>
        /// Searches the collection using the specified parameters.
        /// </summary>
        public override Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest rq, RepositorySettings options = null)
        {
            OnSearchEvent(rq);

            var response = new SearchResponse() { Etag = ETag };
            
            //Func<E, List<KeyValuePair<string, string>>, bool> filter;

            //if (string.IsNullOrEmpty(key.Query))//The default filter returns all records.
            //    filter = (e, p) => true;
            //else if (_filterMethods.ContainsKey(key.Query))
            //    filter = _filterMethods[key.Query];
            //else
            //    return Task.FromResult(new RepositoryHolder<SearchRequest, SearchResponse<E>>(key, response, 404));

            //response.Data = Atomic(() =>
            //{
            //    var res = _container.Values
            //    .Where((e) => filter(e.Entity, key.FilterParams))
            //    .Select((c) => c.Entity);

            //    if (key.Skip.HasValue)
            //        res = res.Skip(key.Top.Value);

            //    if (key.Top.HasValue)
            //        res = res.Take(key.Top.Value);

            //    return res.ToList();
            //});

            var result = new RepositoryHolder<SearchRequest, SearchResponse>(key:rq, entity:response, responseCode: 200);

            return Task.FromResult(result);
        }
        #endregion
        #region SearchEntity(SearchRequest key)
        /// <summary>
        /// Searches the collection using the specified parameters.
        /// </summary>
        public override Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest rq, RepositorySettings options = null)
        {
            OnSearchEvent(rq);

            var response = new SearchResponse<E>() { Etag = ETag };

            //Func<E, List<KeyValuePair<string, string>>, bool> filter;

            //if (string.IsNullOrEmpty(key.Query))//The default filter returns all records.
            //    filter = (e, p) => true;
            //else if (_filterMethods.ContainsKey(key.Query))
            //    filter = _filterMethods[key.Query];
            //else
            //    return Task.FromResult(new RepositoryHolder<SearchRequest, SearchResponse<E>>(key, response, 404));

            //response.Data = Atomic(() =>
            //{
            //    var res = _container.Values
            //    .Where((e) => filter(e.Entity, key.FilterParams))
            //    .Select((c) => c.Entity);

            //    if (key.Skip.HasValue)
            //        res = res.Skip(key.Top.Value);

            //    if (key.Top.HasValue)
            //        res = res.Take(key.Top.Value);

            //    return res.ToList();
            //});

            var result = new RepositoryHolder<SearchRequest, SearchResponse<E>>(key: rq, entity: response, responseCode: 200);

            return Task.FromResult(result);
        }
        #endregion

        #region IsReadOnly
        /// <summary>
        /// Specifies whether the collection is read only.
        /// </summary>
        protected bool IsReadOnly { get; } 
        #endregion

        #region Count
        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return Atomic(false,() => _container.Count);
            }
        }
        #endregion
        #region CountReference
        /// <summary>
        /// This is the number of entity references in the collection.
        /// </summary>
        public virtual int CountReference
        {
            get
            {
                return Atomic(false,() => _containerReference.Count);
            }
        }
        #endregion
        #region ContainsKey(K key)
        /// <summary>
        /// Determines whether the collection contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the collection contains the key; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool ContainsKey(K key)
        {
            return Atomic(false,() => _container.ContainsKey(key));
        }
        #endregion
        #region ContainsReference(Tuple<string, string> reference)
        /// <summary>
        /// Determines whether the collection contains the entity reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>
        ///   <c>true</c> if the collection contains reference; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool ContainsReference(Tuple<string, string> reference)
        {
            return Atomic(false, () => _containerReference.ContainsKey(reference));
        }
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
        #region ReferenceExistingMatch...
        /// <summary>
        /// This method checks for existing references, but also skips matches if the associated key is set to the value passed. 
        /// This allows the method to be used for both create and update requests.
        /// </summary>
        /// <param name="references">The references to check for matches.</param>
        /// <param name="skipOnKeyMatch">A boolean property that specifies a match should be skipped if it matches the key passed in the key parameter.</param>
        /// <param name="key">The key value to skip on a match.</param>
        /// <returns>Returns true if a references has been matched to an item in the collection.</returns>
        private bool ReferenceExistingMatch(IList<Tuple<string, string>> references, bool skipOnKeyMatch = false, K key = default(K))
        {
            if (references != null && references.Count > 0)
            {
                //And do any of the references already exist in the collection?
                foreach (var eRef in references)
                {
                    //OK, do we have a match?
                    if (_containerReference.TryGetValue(eRef, out var refKey))
                        if (!skipOnKeyMatch || !refKey.Key.Equals(key))
                            return true;
                }
            }

            return false;
        }
        #endregion

        #region Class -> ReferenceComparer
        /// <summary>
        /// This helper class is used to ensure that references are matched in a case insensitive manner.
        /// </summary>
        protected class ReferenceComparer : IEqualityComparer<Tuple<string, string>>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type T to compare.</param>
            /// <param name="y">The second object of type T to compare.</param>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            public bool Equals(Tuple<string, string> x, Tuple<string, string> y)
            {
                return string.Equals(x?.Item1, y?.Item1, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(x?.Item2, y?.Item2, StringComparison.CurrentCultureIgnoreCase);
            }
            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public int GetHashCode(Tuple<string, string> obj)
            {
                //We create a lower-case invariant tuple and return its hash code. This will make Equals fire for comparison.
                var invariantTuple = new Tuple<string, string>(obj?.Item1?.ToLowerInvariant(), obj?.Item2?.ToLowerInvariant());
                return invariantTuple.GetHashCode();
            }
        }
        #endregion

        #region ETag
        /// <summary>
        /// Gets the current collection ETag. This changes when an entity is created/updated or deleted.
        /// </summary>
        public string ETag => $"{typeof(E).Name}:{ETagCollectionId}:{_etagOrdinal}";
        #endregion
        #region ETagCollectionId
        /// <summary>
        /// Gets the collection identifier that is set when the collection is created.
        /// </summary>
        public string ETagCollectionId { get; } = Guid.NewGuid().ToString("N").ToUpperInvariant(); 
        #endregion
        #region ETagOrdinalIncrement()
        /// <summary>
        /// This method is called when the collection is changed.
        /// </summary>
        protected virtual void ETagOrdinalIncrement()
        {
            Interlocked.Increment(ref _etagOrdinal);
        }
        #endregion

        #region Class -> EntityContainer
        /// <summary>
        /// This is a private class it is used to ensure that we do not duplicate data.
        /// </summary>
        protected class EntityContainer
        {
            private long _hitCount = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="EntityContainer"/> class.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="entity">The entity.</param>
            /// <param name="references">The entity references.</param>
            /// <param name="properties">The entity properties.</param>
            /// <param name="versionId">The version id of the entity..</param>
            /// <param name="deserializer">The deserializer that converts the body to an entity.</param>
            /// <param name="serializer">The serializer that turns the entity in to a blob.</param>
            public EntityContainer(K key, E entity
                , IEnumerable<Tuple<string, string>> references
                , IEnumerable<Tuple<string, string>> properties
                , string versionId
                , Func<byte[],E> deserializer
                , Func<E, byte[]> serializer
                )
            {
                Key = key;

                Serializer = serializer ?? throw new ArgumentNullException("serializer");

                Deserializer = deserializer ?? throw new ArgumentNullException("deserializer");

                Body = Serializer(entity);

                References = references == null ? new List<Tuple<string, string>>() : references.ToList();
                Properties = properties == null ? new List<Tuple<string, string>>() : properties.ToList();

                VersionId = versionId;
            }

            /// <summary>
            /// Gets the serializer that turns the entity in to a blob.
            /// </summary>
            protected Func<E, byte[]> Serializer { get; }

            /// <summary>
            /// Gets the deserializer that converts the body to an entity.
            /// </summary>
            protected Func<byte[], E> Deserializer { get; }

            /// <summary>
            /// Contains the key.
            /// </summary>
            public K Key { get; }
            /// <summary>
            /// Gets or sets the version identifier.
            /// </summary>
            public string VersionId { get; }
            /// <summary>
            /// Contains the entity.
            /// </summary>
            public E Entity => (Body?.Length ?? 0)==0 ? default(E) : Deserializer(Body);

            /// <summary>
            /// Gets or sets the json body of the entity. This is used to ensure that the entity is
            /// not modified in the main collection by other processes.
            /// </summary>
            public byte[] Body { get; }

            /// <summary>
            /// Contains the entity references.
            /// </summary>
            public List<Tuple<string, string>> References { get; }
            /// <summary>
            /// Contains the entity references.
            /// </summary>
            public List<Tuple<string, string>> Properties { get; }

            /// <summary>
            /// The current 
            /// </summary>
            /// <returns>The previous count.</returns>
            public void ReadHitIncrement() => Interlocked.Increment(ref _hitCount);

            /// <summary>
            /// Gets the current hit count.
            /// </summary>
            public long ReadHitCount => _hitCount;

        }
        #endregion
    }
}
