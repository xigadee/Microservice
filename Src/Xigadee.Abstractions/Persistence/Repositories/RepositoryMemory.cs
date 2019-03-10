using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold an entity and its associated properties and references in memory.
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
        /// <summary>
        /// The collection is read only.
        /// </summary>
        protected readonly bool _isReadOnly;

        protected Dictionary<string, Func<E, List<KeyValuePair<string, string>>, bool>> _filterMethods;

        protected readonly Func<IEnumerable<Tuple<string, Func<E, List<KeyValuePair<string, string>>, bool>>>> _searchMaker;
        #endregion

        #region Constructor        
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryMemory{K, E}"/> class.
        /// </summary>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="keyManager">The key manager.</param>
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
            , IEnumerable<(K key, E entity)> prePopulate = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null
            , bool readOnly = false
            )
            : base(keyMaker, referenceMaker, propertiesMaker, versionPolicy, keyManager)
        {
            _referenceModifyLock = new ReaderWriterLockSlim();
            _container = new Dictionary<K, EntityContainer>();
            _containerReference = new Dictionary<Tuple<string, string>, EntityContainer>(new ReferenceComparer());
            _filterMethods = new Dictionary<string, Func<E, List<KeyValuePair<string, string>>, bool>>();
            _isReadOnly = readOnly;

            searchMaker?.Invoke().ForEach((t) => _filterMethods.Add(t.Item1, t.Item2));

            prePopulate?
                .ForEach(ke => _container.Add(ke.Item1
                , new EntityContainer(ke.Item1, ke.Item2, new List<Tuple<string, string>>(), null, versionPolicy)
                ));
        }
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

        protected Task<RepositoryHolder<KT, ET>> ResultFormat<KT, ET>(int result, Func<KT> key, Func<ET> entity)
        {
            switch (result)
            {
                case 200:
                case 201:
                    return Task.FromResult(new RepositoryHolder<KT, ET>(key(), null, entity(), result));
                case 404:
                    return Task.FromResult(new RepositoryHolder<KT, ET>(key(), null, default(ET), result));
                default:
                    return Task.FromResult(new RepositoryHolder<KT, ET>(key == null ? default(KT) : key(), null, default(ET), result));
            }
        }

        #region Create(E entity)
        /// <summary>
        /// Create
        /// </summary>
        public override Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings options = null)
        {
            var key = _keyMaker(entity);

            if (_isReadOnly)
                return ResultFormat(400, () => key, () => default(E));

            var references = _referenceMaker?.Invoke(entity).ToList();
            var properties = _propertiesMaker?.Invoke(entity).ToList();

            OnEntityEvent(EntityEventType.BeforeCreate, () => entity);

            EntityContainer newContainer = null;

            var result = Atomic(() =>
            {

                IncomingParameterChecks(key, entity);

                //Does the key already exist in the collection
                if (_container.ContainsKey(key))
                    return 409;

                //Are there any references?
                if (ReferenceExistingMatch(references))
                    return 409;

                newContainer = new EntityContainer(key, entity, references, properties, VersionPolicy);
                //OK, add the entity
                _container.Add(key, newContainer);
                //Add the entity references
                newContainer.References.ForEach((r) => _containerReference.Add(r, newContainer));

                OnEntityEvent(EntityEventType.AfterCreate, () => newContainer.Entity);

                return 201;
            });

            return ResultFormat(result, () => key, () => newContainer.Entity);
        }
        #endregion

        #region Read(K key)
        /// <summary>
        /// Read
        /// </summary>
        public override Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeRead, key);

            IncomingParameterChecks(key);

            EntityContainer newValue = null;

            bool result = Atomic(() => _container.TryGetValue(key, out newValue));

            var entity = newValue == null ? default(E) : newValue.Entity;

            return ResultFormat(result ? 200 : 404
                , () => result ? newValue.Key : default(K)
                , () => result ? entity : default(E)
                );
        }
        #endregion
        #region ReadByRef(string refKey, string refValue)
        /// <summary>
        /// Read by Reference
        /// </summary>
        public override Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeRead, refType: refKey, refValue: refValue);

            var reference = new Tuple<string, string>(refKey, refValue);

            EntityContainer newValue = null;

            bool result = Atomic(() => _containerReference.TryGetValue(reference, out newValue));

            E entity = default(E);
            if (newValue != null)
                entity = newValue.Entity;

            return ResultFormat(result ? 200 : 404
                , () => result ? newValue.Key : default(K)
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
            var key = _keyMaker(entity);

            if (_isReadOnly)
                return ResultFormat(400, () => key, () => default(E));

            OnEntityEvent(EntityEventType.BeforeUpdate, () => entity);

            var newReferences = _referenceMaker?.Invoke(entity).ToList();
            var properties = _propertiesMaker?.Invoke(entity).ToList();
            EntityContainer newContainer = null;

            var result = Atomic(() =>
            {
                if (_isReadOnly)
                    return 400;

                IncomingParameterChecks(key, entity);

                //If the doesn't already exist in the collection, throw a not-found error.
                if (!_container.TryGetValue(key, out var oldContainer))
                    return 404;

                if (VersionPolicy?.SupportsOptimisticLocking ?? false)
                {
                    var currentVersion = VersionPolicy.EntityVersionAsString(entity);
                    var oldVersion = VersionPolicy.EntityVersionAsString(oldContainer.Entity);

                    //The version id should match the current stored version. If not we reject it.
                    if (currentVersion != oldVersion)
                        return 409;
                    //OK, update the entity version parameters.
                    VersionPolicy.EntityVersionUpdate(entity);
                }

                //OK, get the new references, but check whether they are assigned to another entity and if so flag an error.
                if (ReferenceExistingMatch(newReferences, true, key))
                    return 409;

                newContainer = new EntityContainer(key, entity, newReferences, properties, VersionPolicy);

                //OK, update the entity
                _container[key] = newContainer;
                //Remove the old references, and add the new references.
                //Note we're being lazy we add/replace even if nothing has changed.
                oldContainer.References.ForEach((r) => _containerReference.Remove(r));
                newContainer.References.ForEach((r) => _containerReference.Add(r, newContainer));

                //All good.
                OnEntityEvent(EntityEventType.AfterUpdate, () => newContainer.Entity);

                return 200;
            });

            return ResultFormat(result, () => key, () => newContainer.Entity);
        }
        #endregion

        #region Delete(K key)
        /// <summary>
        /// Delete
        /// </summary>
        public override Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings options = null)
        {
            if (_isReadOnly)
                return ResultFormat(400, () => key, () => new Tuple<K, string>(key, ""));

            OnKeyEvent(KeyEventType.BeforeDelete, key);

            var result = Atomic(() =>
            {
                IncomingParameterChecks(key);
                if (_container.TryGetValue(key, out var container))
                    return RemoveInternal(container);

                return false;
            });

            return ResultFormat(result ? 200 : 404, () => key, () => new Tuple<K, string>(key, ""));
        }
        #endregion
        #region DeleteByRef(string refKey, string refValue)
        /// <summary>
        /// Delete by reference
        /// </summary>
        public override Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            if (_isReadOnly)
                return ResultFormat(400, () => default(K), () => new Tuple<K, string>(default(K), ""));

            OnKeyEvent(KeyEventType.BeforeDelete, refType: refKey, refValue: refValue);
            var reference = new Tuple<string, string>(refKey, refValue);
            EntityContainer container = null;

            var result = Atomic(() =>
            {
                if (!_containerReference.TryGetValue(reference, out container))
                    return false;

                return RemoveInternal(container);
            });

            return ResultFormat(result ? 200 : 404, () => container.Key, () => new Tuple<K, string>(container.Key, ""));
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

        private bool RemoveInternal(EntityContainer container)
        {
            bool result = _container.Remove(container.Key);
            if (result)
                container.References.ForEach((r) => _containerReference.Remove(r));

            return result;
        }

        #region Version(K key)
        /// <summary>
        /// Version
        /// </summary>
        public override Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeVersion, key);

            EntityContainer container = null;
            var result = Atomic(() =>
            {
                IncomingParameterChecks(key);
                return _container.TryGetValue(key, out container);
            });

            return ResultFormat(result ? 200 : 404, () => container.Key, () => new Tuple<K, string>(container.Key, ""));
        }
        #endregion
        #region VersionByRef(string refKey, string refValue)
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
            var result = Atomic(() =>
            {
                var reference = new Tuple<string, string>(refKey, refValue);
                return _containerReference.TryGetValue(reference, out container);
            });

            return ResultFormat(result ? 200 : 404, () => container.Key, () => new Tuple<K, string>(container.Key, ""));

        }
        #endregion

        #region Atomic...
        /// <summary>
        /// This wraps the requests the ensure that only one is processed at the same time.
        /// </summary>
        /// <param name="action">The action to process.</param>
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

        #region IncomingParameterChecks ...
        /// <summary>
        /// Checks the incoming key parameter has a value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="ArgumentOutOfRangeException">Key must be set to a value</exception>
        protected virtual void IncomingParameterChecks(K key)
        {
            if (key.Equals(default(K)))
                throw new ArgumentOutOfRangeException("key must be set to a value");
        }

        /// <summary>
        /// Checks the incoming key and entity value parameters have values.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The entity value.</param>
        /// <exception cref="ArgumentNullException">key or value must be set</exception>
        protected virtual void IncomingParameterChecks(K key, E value)
        {
            IncomingParameterChecks(key);
            if (value.Equals(default(E)))
                throw new ArgumentNullException("value must be set to a value");
        }
        #endregion

        #region Search(SearchRequest key)
        /// <summary>
        /// Searches the collection using the specified parameters.
        /// </summary>
        /// <param name="key">The search request.</param>
        /// <returns>Returns a collection of entities.</returns>
        public override Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest key, RepositorySettings options = null)
        {
            throw new NotImplementedException();
            //OnSearchEvent(key);

            //var response = new SearchResponse<E>() { Etag = Guid.NewGuid().ToString("N") };

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

            //var result = new RepositoryHolder<SearchRequest, SearchResponse<E>>(key, response, 200);

            //return Task.FromResult(result);
        }
        #endregion

        /// <summary>
        /// This is a private class it is used to ensure that we do not duplicate data.
        /// </summary>
        protected class EntityContainer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EntityContainer"/> class.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="entity">The entity.</param>
            /// <param name="references">The references.</param>
            /// <param name="properties">The entity properties.</param>
            /// <param name="vPol">The version policy, which can be null.</param>
            public EntityContainer(K key, E entity
                , IEnumerable<Tuple<string, string>> references
                , IEnumerable<Tuple<string, string>> properties
                , VersionPolicy<E> vPol
                )
            {
                Key = key;

                JsonBody = JsonConvert.SerializeObject(entity);

                References = references == null ? new List<Tuple<string, string>>() : references.ToList();
                Properties = properties == null ? new List<Tuple<string, string>>() : properties.ToList();

                if (vPol?.SupportsVersioning ?? false)
                    VersionId = vPol.EntityVersionAsString(entity);
            }
            /// <summary>
            /// Contains the key.
            /// </summary>
            public K Key { get; }
            /// <summary>
            /// Contains the entity.
            /// </summary>
            public E Entity => string.IsNullOrEmpty(JsonBody) ? default(E) : JsonConvert.DeserializeObject<E>(JsonBody);

            /// <summary>
            /// Gets or sets the json body of the entity. This is used to ensure that the entity is
            /// not modified in the main collection by other processes.
            /// </summary>
            public string JsonBody { get; }

            /// <summary>
            /// Contains the entity references.
            /// </summary>
            public List<Tuple<string, string>> References { get; }
            /// <summary>
            /// Contains the entity references.
            /// </summary>
            public List<Tuple<string, string>> Properties { get; }
            /// <summary>
            /// Gets or sets the version identifier.
            /// </summary>
            public string VersionId { get; }
        }

        /// <summary>
        /// This helper class is used to ensure that references are matched in a case insensitive manner.
        /// </summary>
        protected class ReferenceComparer : IEqualityComparer<Tuple<string, string>>
        {
            public bool Equals(Tuple<string, string> x, Tuple<string, string> y)
            {
                return string.Equals(x?.Item1, y?.Item1, StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(x?.Item2, y?.Item2, StringComparison.CurrentCultureIgnoreCase);
            }

            public int GetHashCode(Tuple<string, string> obj)
            {
                //We create a lower-case invariant tuple and return its hash code. This will make Equals fire for comparison.
                var invariantTuple = new Tuple<string, string>(obj?.Item1?.ToLowerInvariant(), obj?.Item2?.ToLowerInvariant());
                return invariantTuple.GetHashCode();
            }
        }
    }
}
