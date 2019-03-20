using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Xigadee
{
    /// <summary>
    /// This is the base repository holder.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public abstract class RepositoryBase<K, E> : RepositoryBase, IRepositoryAsyncServer<K, E>
        where K : IEquatable<K>
    {
        #region Events

        #region OnBeforeCreate...
        /// <summary>
        /// Called before and entity is created.
        /// </summary>
        /// <param name="entity">The incoming entity.</param>
        protected void OnBeforeCreateEvent(E entity) => OnBeforeCreate?.Invoke(this, entity);
        /// <summary>
        /// Occurs before and entity is created.
        /// </summary>
        public event EventHandler<E> OnBeforeCreate;
        #endregion
        #region OnAfterCreate
        /// <summary>
        /// Occurs after an entity is created/attempted.
        /// </summary>
        protected void OnAfterCreateEvent(RepositoryHolder<K, E> holder) => OnAfterCreate?.Invoke(this, holder);
        /// <summary>
        /// Occurs after an entity is created.
        /// </summary>
        public event EventHandler<RepositoryHolder<K, E>> OnAfterCreate; 
        #endregion

        #region OnBeforeUpdate ...
        /// <summary>
        /// Called before and entity is created.
        /// </summary>
        /// <param name="entity">The incoming entity.</param>
        protected void OnBeforeUpdateEvent(E entity) => OnBeforeUpdate?.Invoke(this, entity);
        /// <summary>
        /// Occurs before an entity is updated.
        /// </summary>
        public event EventHandler<E> OnBeforeUpdate;
        #endregion

        #region OnAfterUpdate ...
        /// <summary>
        /// Occurs after an entity is created/attempted.
        /// </summary>
        protected void OnAfterUpdateEvent(RepositoryHolder<K, E> holder) => OnAfterUpdate?.Invoke(this, holder);
        /// <summary>
        /// Occurs after an entity is updated.
        /// </summary>
        public event EventHandler<RepositoryHolder<K, E>> OnAfterUpdate; 
        #endregion

        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeRead;

        #region OnAfterRead
        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        protected void OnAfterReadEvent(RepositoryHolder<K, E> holder) => OnAfterRead?.Invoke(this, holder);

        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<RepositoryHolder<K, E>> OnAfterRead; 
        #endregion

        /// <summary>
        /// Occurs before an entity is deleted.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeDelete;

        #region OnAfterDelete
        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        protected void OnAfterDeleteEvent(RepositoryHolder<K, Tuple<K, string>> holder) => OnAfterDelete?.Invoke(this, holder);

        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<RepositoryHolder<K, Tuple<K, string>>> OnAfterDelete;
        #endregion

        /// <summary>
        /// Occurs before an entity is versioned.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeVersion;

        #region OnAfterVersion
        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        protected void OnAfterVersionEvent(RepositoryHolder<K, Tuple<K, string>> holder) => OnAfterVersion?.Invoke(this, holder);

        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<RepositoryHolder<K, Tuple<K, string>>> OnAfterVersion;
        #endregion

        #region OnKeyEvent(KeyEventType type, TKey key = default(TKey), string refType = null, string refValue = null)
        protected enum KeyEventType
        {
            BeforeRead,
            BeforeDelete,
            BeforeVersion,
        }
        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key"></param>
        /// <param name="refType"></param>
        /// <param name="refValue"></param>
        protected void OnKeyEvent(KeyEventType type, K key = default(K), string refType = null, string refValue = null)
        {
            var holder = new ReferenceHolder<K>() { Key = key, RefType = refType, RefValue = refValue };

            switch (type)
            {
                case KeyEventType.BeforeRead:
                    OnBeforeRead?.Invoke(this, holder);
                    break;
                case KeyEventType.BeforeDelete:
                    OnBeforeDelete?.Invoke(this, holder);
                    break;
                case KeyEventType.BeforeVersion:
                    OnBeforeVersion?.Invoke(this, holder);
                    break;
            }
        }
        #endregion

        #region OnBeforeSearch...
        /// <summary>
        /// Occurs before an entity is created.
        /// </summary>
        public event EventHandler<SearchRequest> OnBeforeSearch;
        /// <summary>
        /// Called when a search event occurs.
        /// </summary>
        /// <param name="key">The search request.</param>
        protected void OnBeforeSearchEvent(SearchRequest key) => OnBeforeSearch?.Invoke(this, key);
        #endregion
        #region OnAfterSearchEvent(RepositoryHolder<SearchRequest, SearchResponse> rs)
        /// <summary>
        /// Occurs before an entity is created.
        /// </summary>
        public event EventHandler<RepositoryHolder<SearchRequest, SearchResponse>> OnAfterSearch;
        /// <summary>
        /// Called after a search event occurs.
        /// </summary>
        /// <param name="rs">The search response.</param>
        protected void OnAfterSearchEvent(RepositoryHolder<SearchRequest, SearchResponse> rs) => OnAfterSearch?.Invoke(this, rs);

        #endregion
        #region OnAfterSearchEntityEvent(RepositoryHolder<SearchRequest, SearchResponse<E>> rs)
        /// <summary>
        /// Occurs before an entity is created.
        /// </summary>
        public event EventHandler<RepositoryHolder<SearchRequest, SearchResponse<E>>> OnAfterSearchEntity;
        /// <summary>
        /// Called after a search event occurs.
        /// </summary>
        /// <param name="rs">The search response.</param>
        protected void OnAfterSearchEntityEvent(RepositoryHolder<SearchRequest, SearchResponse<E>> rs) => OnAfterSearchEntity?.Invoke(this, rs);
        #endregion

        #endregion

        #region Declarations        
        /// <summary>
        /// The reference maker used to make the reference values from an entity.
        /// </summary>
        protected readonly Func<E, IEnumerable<Tuple<string, string>>> _referenceMaker;
        /// <summary>
        /// The properties maker is used to extract the common entity properties.
        /// </summary>
        protected readonly Func<E, IEnumerable<Tuple<string, string>>> _propertiesMaker;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TKey, TEntity}"/> class.
        /// </summary>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="referenceMaker">The reference maker.</param>
        /// <param name="propertiesMaker">The properties maker.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <param name="keyManager">The key serialization manager. if this is not passed, then a default serializer will be passed using the component model.</param>
        /// <exception cref="ArgumentNullException">keyMaker</exception>
        protected RepositoryBase(Func<E, K> keyMaker
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null
            )
        {
            KeyMaker = keyMaker ?? throw new ArgumentNullException(nameof(keyMaker));

            _referenceMaker = referenceMaker ?? (e => new List<Tuple<string, string>>());
            _propertiesMaker = propertiesMaker ?? (e => new List<Tuple<string, string>>());
            VersionPolicy = versionPolicy;
            KeyManager = keyManager ?? RepositoryKeyManager.Resolve<K>();
        }
        #endregion

        #region VersionPolicy
        /// <summary>
        /// Holds the policy concerning whether the repository implements optimistic locking 
        /// and or history of entities.
        /// </summary>
        public VersionPolicy<E> VersionPolicy { get; }
        #endregion
        #region KeyMaker
        /// <summary>
        /// The key maker used to extract the key from an incoming entity.
        /// </summary>
        public Func<E, K> KeyMaker { get; protected set; }
        #endregion
        #region EntityName
        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        public string EntityName { get; protected set; } = typeof(E).Name;
        #endregion
        #region KeyManager
        /// <summary>
        /// Gets the key manager which is used for managing the serialization of a key to and from a string.
        /// </summary>
        public RepositoryKeyManager<K> KeyManager { get; }
        #endregion

        #region Create
        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public virtual Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings options = null)
        {
            var key = KeyMaker(entity);

            IncomingParameterChecks(key, entity);

            OnBeforeCreateEvent(entity);

            return CreateInternal(key, entity, options, r => OnAfterCreateEvent(r));
        }

        /// <summary>
        /// This override has the repository specific logic to create an entity.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options.</param>
        /// <param name="holderAction">The post completion action.</param>
        /// <returns></returns>
        protected abstract Task<RepositoryHolder<K, E>> CreateInternal(K key, E entity, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction);
        #endregion
        #region Read/ReadByRef
        /// <summary>
        /// Reads the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public virtual Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings options = null)
        {
            IncomingParameterChecks(key);

            OnKeyEvent(KeyEventType.BeforeRead, key);

            return ReadInternal(key, options, r => OnAfterReadEvent(r));
        }
        /// <summary>
        /// Reads the internal.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <param name="holderAction">The holder action.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        protected abstract Task<RepositoryHolder<K, E>> ReadInternal(K key, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction);
        /// <summary>
        /// Reads the entity by a reference key-value pair.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>Returns the holder with the response and data.</returns>
        public virtual Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeRead, refType: refKey, refValue: refValue);

            return ReadByRefInternal(refKey, refValue, options, r => OnAfterReadEvent(r));
        }
        /// <summary>
        /// Reads the entity by a reference key-value pair.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <param name="holderAction">The holder action.</param>
        /// <returns>Returns the holder with the response and data.</returns>
        protected abstract Task<RepositoryHolder<K, E>> ReadByRefInternal(string refKey, string refValue, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction); 
        #endregion
        #region Update
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public virtual Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings options = null)
        {
            var key = KeyMaker(entity);

            IncomingParameterChecks(key, entity);

            OnBeforeUpdateEvent(entity);

            return UpdateInternal(key, entity, options, r => OnAfterUpdateEvent(r));
        }

        /// <summary>
        /// This override has the repository specific logic to create an entity.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options.</param>
        /// <param name="holderAction">The post completion action.</param>
        /// <returns></returns>
        protected abstract Task<RepositoryHolder<K, E>> UpdateInternal(K key, E entity, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction);
        #endregion
        #region Delete/DeleteByRef
        /// <summary>
        /// Deletes the entity based on the key specified.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>Returns the holder with the response and data.</returns>
        public virtual Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings options = null)
        {
            IncomingParameterChecks(key);

            OnKeyEvent(KeyEventType.BeforeDelete, key);

            return DeleteInternal(key, options, r => OnAfterDeleteEvent(r));
        }

        /// <summary>
        /// Deletes the entity based on the key specified.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <param name="holderAction">The holder action.</param>
        /// <returns>Returns the holder with the response and data.</returns>
        protected abstract Task<RepositoryHolder<K, Tuple<K, string>>> DeleteInternal(K key, RepositorySettings options 
            , Action<RepositoryHolder<K, Tuple<K, string>>> holderAction);

        /// <summary>
        /// Deletes the entity by the reference specified.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public virtual Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeDelete, refType: refKey, refValue: refValue);

            return DeleteByRefInternal(refKey, refValue, options, r => OnAfterDeleteEvent(r));
        }
        /// <summary>
        /// Deletes the entity by reference specified.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The options.</param>
        /// <param name="holderAction">The holder action.</param>
        /// <returns>Returns the holder with the response and data.</returns>
        protected abstract Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRefInternal(string refKey, string refValue, RepositorySettings options = null
            , Action<RepositoryHolder<K, Tuple<K, string>>> holderAction = null);
        #endregion
        #region Version/VersionByRef
        /// <summary>
        /// Validates the version by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public virtual Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings options = null)
        {
            IncomingParameterChecks(key);

            OnKeyEvent(KeyEventType.BeforeVersion, key);

            return VersionInternal(key, options, r => OnAfterVersionEvent(r));
        }
        /// <summary>
        /// Versions the internal.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <param name="holderAction">The holder action.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        protected abstract Task<RepositoryHolder<K, Tuple<K, string>>> VersionInternal(K key, RepositorySettings options
            , Action<RepositoryHolder<K, Tuple<K, string>>> holderAction);

        /// <summary>
        /// Validates the version by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public virtual Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeVersion, refType: refKey, refValue: refValue);

            return VersionByRefInternal(refKey, refValue, options, r => OnAfterVersionEvent(r));
        }
        /// <summary>
        /// Versions the by reference internal.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The options.</param>
        /// <param name="holderAction">The holder action.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        protected abstract Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRefInternal(string refKey, string refValue, RepositorySettings options
            , Action<RepositoryHolder<K, Tuple<K, string>>> holderAction); 
        #endregion

        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest rq, RepositorySettings options = null);

        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and entities.
        /// </returns>
        public abstract Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest rq, RepositorySettings options = null);
    }

    /// <summary>
    /// This is the base holding class.
    /// </summary>
    public abstract class RepositoryBase
    {
        #region ResultFormat...
        /// <summary>
        /// Formats the outgoing result.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type..</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="key">The key.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The incoming options.</param>
        /// <param name="holderAction">The action to execute when the holder is created.</param>
        /// <returns>Returns the holder.</returns>
        public static Task<RepositoryHolder<KT, ET>> ResultFormat<KT, ET>(int result, Func<KT> key = null, Func<ET> entity = null
            , RepositorySettings options = null, Action<RepositoryHolder<KT, ET>> holderAction = null)
            where KT : IEquatable<KT>
        {
            var k = key != null ? key() : default(KT);
            var e = entity != null ? entity() : default(ET);

            RepositoryHolder<KT, ET> holder;
            switch (result)
            {
                case 200:
                case 201:
                    holder = new RepositoryHolder<KT, ET>(k, null, e, result);
                    break;
                //case 404:
                //    return Task.FromResult(new RepositoryHolder<KT, ET>(k, null, default(ET), result));
                default:
                    holder = new RepositoryHolder<KT, ET>(k, null, default(ET), result);
                    break;
            }

            holderAction?.Invoke(holder);

            return Task.FromResult(holder);
        }
        #endregion

        #region IncomingParameterChecks ...
        /// <summary>
        /// Checks the incoming key parameter has a value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="ArgumentOutOfRangeException">Key must be set to a value</exception>
        public static void IncomingParameterChecks<KT>(KT key) where KT : IEquatable<KT>
        {
            if (key.Equals(default(KT)))
                throw new ArgumentOutOfRangeException("key must be set to a value");
        }

        /// <summary>
        /// Checks the incoming key and entity value parameters have values.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The entity value.</param>
        /// <exception cref="ArgumentNullException">key or value must be set</exception>
        public static void IncomingParameterChecks<KT, ET>(KT key, ET value) where KT : IEquatable<KT>
        {
            IncomingParameterChecks(key);
            if (value.Equals(default(ET)))
                throw new ArgumentNullException("value must be set to a value");
        }
        #endregion
    }
}
