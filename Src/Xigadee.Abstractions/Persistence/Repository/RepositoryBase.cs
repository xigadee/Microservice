using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
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
        /// <summary>
        /// Occurs before an entity is created.
        /// </summary>
        public event EventHandler<SearchRequest> OnBeforeSearch;

        /// <summary>
        /// Occurs before and entity is created.
        /// </summary>
        public event EventHandler<E> OnBeforeCreate;
        /// <summary>
        /// Occurs before an entity is updated.
        /// </summary>
        public event EventHandler<E> OnBeforeUpdate;
        /// <summary>
        /// Occurs after an entity is created.
        /// </summary>
        public event EventHandler<E> OnAfterCreate;
        /// <summary>
        /// Occurs after an entity is updated.
        /// </summary>
        public event EventHandler<E> OnAfterUpdate;
        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeRead;
        /// <summary>
        /// Occurs before an entity is deleted.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeDelete;
        /// <summary>
        /// Occurs before an entity is versioned.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeVersion;

        #region OnEntityEvent(EventType type, TEntity entity)        
        /// <summary>
        /// This is the entity event type.
        /// </summary>
        protected enum EntityEventType
        {
            /// <summary>
            /// Before the entity is created
            /// </summary>
            BeforeCreate,
            /// <summary>
            /// Before the entity is update
            /// </summary>
            BeforeUpdate,
            /// <summary>
            /// After the entity is created
            /// </summary>
            AfterCreate,
            /// <summary>
            /// After the entity is updated
            /// </summary>
            AfterUpdate
        }
        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="entity">The entity.</param>
        protected void OnEntityEvent(EntityEventType type, Func<E> entity)
        {
            switch (type)
            {
                case EntityEventType.BeforeCreate:
                    OnBeforeCreate?.Invoke(this, entity());
                    break;
                case EntityEventType.BeforeUpdate:
                    OnBeforeUpdate?.Invoke(this, entity());
                    break;
                case EntityEventType.AfterCreate:
                    OnAfterCreate?.Invoke(this, entity());
                    break;
                case EntityEventType.AfterUpdate:
                    OnAfterUpdate?.Invoke(this, entity());
                    break;
            }
        }
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
        #region OnSearchEvent(SearchRequest key)
        /// <summary>
        /// Called when a search event occurs.
        /// </summary>
        /// <param name="key">The search request.</param>
        protected void OnSearchEvent(SearchRequest key)
        {
            OnBeforeSearch?.Invoke(this, key);
        }
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
        /// <exception cref="ArgumentNullException">keyMaker</exception>
        protected RepositoryBase(Func<E, K> keyMaker
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            )
        {
            KeyMaker = keyMaker ?? throw new ArgumentNullException(nameof(keyMaker));

            _referenceMaker = referenceMaker ?? (e => new List<Tuple<string, string>>());
            _propertiesMaker = propertiesMaker ?? (e => new List<Tuple<string, string>>());
            VersionPolicy = versionPolicy;
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

        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings options = null);
        /// <summary>
        /// Deletes the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings options = null);
        /// <summary>
        /// Deletes the entity by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Reads the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings options = null);
        /// <summary>
        /// Reads the entity by a reference key-value pair.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings options = null);
        /// <summary>
        /// Validates the version by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings options = null);
        /// <summary>
        /// Validates the version by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings options = null);
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
        /// <returns>Returns the holder.</returns>
        public static Task<RepositoryHolder<KT, ET>> ResultFormat<KT, ET>(int result, Func<KT> key = null, Func<ET> entity = null
            , RepositorySettings options = null)
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
