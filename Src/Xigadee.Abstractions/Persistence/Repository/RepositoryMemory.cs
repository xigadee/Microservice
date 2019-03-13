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
        public RepositoryMemory(Func<E, K> keyMaker
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , Func<IEnumerable<Tuple<string, Func<E, List<KeyValuePair<string, string>>, bool>>>> searchMaker = null
            , IEnumerable<E> prePopulate = null
            , VersionPolicy<E> versionPolicy = null
            , bool readOnly = false
            )
            : base(keyMaker, referenceMaker, propertiesMaker, versionPolicy)
        {
            _referenceModifyLock = new ReaderWriterLockSlim();

            _container = new Dictionary<K, EntityContainer>();
            _containerReference = new Dictionary<Tuple<string, string>, EntityContainer>(new ReferenceComparer());

            _filterMethods = new Dictionary<string, Func<E, List<KeyValuePair<string, string>>, bool>>();
            searchMaker?.Invoke().ForEach((t) => _filterMethods.Add(t.Item1, t.Item2));

            prePopulate?.ForEach(ke => Create(ke));
            IsReadOnly = readOnly;
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

            var result = Atomic(() =>
            {
                //Does the key already exist in the collection
                if (_container.ContainsKey(key))
                    return 409;

                //Are there any references? And do they already exist.
                if (ReferenceExistingMatch(references))
                    return 409;

                var newContainer = new EntityContainer(key, entity, references, properties, VersionPolicy?.EntityVersionAsString(entity));
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

            return ResultFormat(result, () => key, () => newEntity);
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

            bool result = Atomic(() => _container.TryGetValue(key, out container));

            var entity = container == null ? default(E) : container.Entity;

            container?.ReadHitIncrement();

            return ResultFormat(result ? 200 : 404
                , () => result ? container.Key : default(K)
                , () => result ? entity : default(E)
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

            bool result = Atomic(() => _containerReference.TryGetValue(reference, out container));

            E entity = container == null ?default(E):container.Entity;

            container?.ReadHitIncrement();

            return ResultFormat(result ? 200 : 404
                , () => result ? container.Key : default(K)
                , () => result ? entity : default(E)
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

            var result = Atomic(() =>
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

                newContainer = new EntityContainer(key, newEntity, newReferences, newProperties, newVersion);

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

            return ResultFormat(result, () => key, () => newEntity);
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

            var result = Atomic(() =>
            {
                if (_container.TryGetValue(key, out var container))
                    return DeleteInternal(container);

                return false;
            });

            return ResultFormat(result ? 200 : 404, () => key, () => new Tuple<K, string>(key, ""));
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

            var result = Atomic(() =>
            {
                if (!_containerReference.TryGetValue(reference, out container))
                    return false;

                return DeleteInternal(container);
            });

            return ResultFormat(result ? 200 : 404, () => container.Key, () => new Tuple<K, string>(container.Key, ""));
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

            var result = Atomic(() =>
            {
                return _container.TryGetValue(key, out container);
            });

            container?.ReadHitIncrement();

            return ResultFormat(result ? 200 : 404, () => container.Key, () => new Tuple<K, string>(container.Key, container.VersionId));
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

            var result = Atomic(() =>
            {
                return _containerReference.TryGetValue(reference, out container);
            });

            container?.ReadHitIncrement();

            return ResultFormat(result ? 200 : 404, () => container.Key, () => new Tuple<K, string>(container.Key, container.VersionId));

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
                return Atomic(() => _container.Count);
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
                return Atomic(() => _containerReference.Count);
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
            return Atomic(() => _container.ContainsKey(key));
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
            return Atomic(() => _containerReference.ContainsKey(reference));
        }
        #endregion

        #region Atomic...
        /// <summary>
        /// This wraps the requests the ensure that only one is processed at the same time.
        /// </summary>
        /// <param name="action">The action to process.</param>
        [DebuggerStepThrough]
        protected void Atomic(Action action)
        {
            try
            {
                _referenceModifyLock.EnterReadLock();

                action();
            }
            finally
            {
                _referenceModifyLock.ExitReadLock();
            }
        }

        /// <summary>
        /// This wraps the requests the ensure that only one is processed at the same time.
        /// </summary>
        /// <param name="action">The action to process.</param>
        /// <returns>Returns the value.</returns>
        [DebuggerStepThrough]
        protected T Atomic<T>(Func<T> action)
        {
            try
            {
                _referenceModifyLock.EnterReadLock();

                return action();
            }
            finally
            {
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
    }
}
