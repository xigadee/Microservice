using System;
using System.Collections.Generic;
using System.Linq;
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

        #region OnBeforeHistory...
        /// <summary>
        /// Occurs before an entity history is retrieved.
        /// </summary>
        public event EventHandler<HistoryRequest<K>> OnBeforeHistory;
        /// <summary>
        /// Called when a history event occurs.
        /// </summary>
        /// <param name="key">The search request.</param>
        protected void OnBeforeHistoryEvent(HistoryRequest<K> key) => OnBeforeHistory?.Invoke(this, key);
        #endregion
        #region OnAfterHistory...
        /// <summary>
        /// Occurs after a history is created.
        /// </summary>
        public event EventHandler<RepositoryHolder<HistoryRequest<K>, HistoryResponse<E>>> OnAfterHistory;
        /// <summary>
        /// Called after a history event occurs.
        /// </summary>
        /// <param name="rs">The search response.</param>
        protected void OnAfterHistoryEvent(RepositoryHolder<HistoryRequest<K>, HistoryResponse<E>> rs) => OnAfterHistory?.Invoke(this, rs);
        #endregion

        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TKey, TEntity}"/> class.
        /// </summary>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="referenceMaker">The reference maker.</param>
        /// <param name="propertiesMaker">The properties maker.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <param name="keyManager">The key serialization manager. if this is not passed, then a default serializer 
        /// will be passed using the component model.</param>
        /// <param name="signaturePolicy">This is the manual signature policy for the entity. 
        /// If this is null, the repository attempts to set the policy using the EntitySignaturePolicyAttribute</param>
        /// <exception cref="ArgumentNullException">keyMaker</exception>
        protected RepositoryBase(Func<E, K> keyMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null
            , ISignaturePolicy signaturePolicy = null
           ) : base(typeof(IRepositoryAsyncServer<K, E>))
        {
            var res = EntityHintHelper.Resolve<E>();

            //Key maker
            KeyMaker = keyMaker;
            if (KeyMaker == null)
                if (res?.SupportsId ?? false)
                    KeyMaker = ((e) => res.Id<K>(e));
                else
                    throw new ArgumentNullException($"{nameof(keyMaker)} cannot be null.");

            //References
            ReferencesMaker = referenceMaker;
            if (ReferencesMaker == null)
                if (res?.SupportsReferences ?? false)
                    ReferencesMaker = (e) => res.References(e);
                else
                    ReferencesMaker = e => new List<Tuple<string, string>>();

            //Properties
            PropertiesMaker = propertiesMaker;
            if (PropertiesMaker == null)
                if (res?.SupportsProperties ?? false)
                    PropertiesMaker = (e) => res.Properties(e);
                else
                    PropertiesMaker = e => new List<Tuple<string, string>>();

            //Version maker
            VersionPolicy = versionPolicy;
            if (VersionPolicy == null && (res?.SupportsVersion ?? false))
                    VersionPolicy = res.VersionPolicyGet<E>();

            //Signature policy
            SignaturePolicy = SignaturePolicyCalculate(res, signaturePolicy);
           
            //Key Manager
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

        #region ReferencesMaker
        /// <summary>
        /// The reference maker used to make the reference values from an entity.
        /// </summary>
        public Func<E, IEnumerable<Tuple<string, string>>> ReferencesMaker { get; protected set; }
        #endregion
        #region PropertiesMaker        
        /// <summary>
        /// The properties maker is used to extract the common entity properties.
        /// </summary>
        public Func<E, IEnumerable<Tuple<string, string>>> PropertiesMaker { get; protected set; }
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

        #region Search
        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest rq, RepositorySettings options = null);
        #endregion
        #region SearchEntity
        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and entities.
        /// </returns>
        public abstract Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest rq, RepositorySettings options = null);
        #endregion

        #region History
        /// <summary>
        /// This method is used to return a selection of previous versions of an entity. 
        /// </summary>
        /// <param name="key">The search key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and entities.
        /// </returns>
        public abstract Task<RepositoryHolder<HistoryRequest<K>, HistoryResponse<E>>> History(HistoryRequest<K> key, RepositorySettings options = null);
        #endregion

        #region SignaturePolicy
        /// <summary>
        /// Holds the policy concerning whether the repository implements an entity signature when creating and reading an entity.
        /// </summary>
        public ISignaturePolicy SignaturePolicy { get; }
        #endregion
        #region SignaturePolicyCalculate(EntityHintResolver res, ISignaturePolicy signaturePolicy)
        /// <summary>
        /// This method calculates the resulting signature policy from the parameters passed.
        /// </summary>
        /// <param name="res">The entity hint resolver.</param>
        /// <param name="signaturePolicy">The policy passed in the constructor.</param>
        /// <returns>Returns the ammended policy.</returns>
        protected virtual ISignaturePolicy SignaturePolicyCalculate(EntityHintResolver res, ISignaturePolicy signaturePolicy)
        {
            try
            {
                //OK, firstly do we have a root signature policy wrapper. If we do, we just set that and move on.
                if (signaturePolicy != null && !(signaturePolicy is ISignaturePolicyWrapper))
                    return signaturePolicy;

                bool resSupportsSignature = res?.SupportsSignature ?? false;

                ISignaturePolicy leafPolicy = null;
                if (resSupportsSignature)
                    leafPolicy = res.SignaturePolicyGet();
                else
                    leafPolicy = new SignaturePolicyNull();

                if (signaturePolicy == null)
                    return leafPolicy;

                //OK, we now need to register the leaf policy with the wrapper.
                var wrapperPolicy = signaturePolicy as ISignaturePolicyWrapper;

                //Signature policy
                wrapperPolicy.RegisterChildPolicy(leafPolicy);

                return wrapperPolicy;
            }
            catch (Exception ex)
            {
                throw new SignaturePolicyCalculateException($"Unexpected exception {GetType().Name} - {ex.Message}",ex);
            }
        } 
        #endregion
        #region SignatureCreate(E entity)
        /// <summary>
        /// TODO: Creates the signature hash for the entity. 
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the string signature.</returns>
        protected virtual string SignatureCreate(E entity)
        {
            if (!(SignaturePolicy?.Supports(entity.GetType()) ?? false))
                return null;

            var signature = SignaturePolicy?.Calculate(entity);

            return signature;
        }
        #endregion
        #region SignatureValidate(E entity, string signature)
        /// <summary>
        /// Validates the signature hash and confirms that the key fields have not been altered in the database. 
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="signature">The signature.</param>
        /// <returns>Returns the string signature.</returns>
        protected virtual bool SignatureValidate(E entity, string signature)
        {
            if (!(SignaturePolicy?.Supports(entity.GetType()) ?? false))
                return true;

            var success = SignaturePolicy?.Verify(entity, signature) ?? true;

            return success;
        }
        #endregion

        #region VersionPolicySet(IEntityContext<K, E> ctx, bool isUpdate)
        /// <summary>
        /// This method sets the version policy for the specific entity.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="isUpdate">Specifies whether this is an update/</param>
        protected virtual void VersionPolicySet(IEntityContext<E> ctx, bool isUpdate)
        {
            var entity = ctx.EntityIncoming;

            ctx.EntityOutgoing = JsonHelper.Clone(entity);

            //OK, do we have to update the version id?
            if (isUpdate && (VersionPolicy?.SupportsOptimisticLocking ?? false))
            {
                var incomingVersionId = VersionPolicy.EntityVersionAsString(entity);
                string newVersion = VersionPolicy.EntityVersionUpdate(ctx.EntityOutgoing);
            }
        }
        #endregion
        #region ContextLogger
        /// <summary>
        /// This method logs the response to the request.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="action">The repository action.</param>
        /// <param name="key">The entity key.</param>
        protected void ContextLogger<X>(IEntityContext ctx, string action, X key)
            => ContextLoggerInternal(ctx, action, key.ToString());
        /// <summary>
        /// This method logs the response to the request.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="action">The repository action.</param>
        /// <param name="keyValue">The key type.</param>
        /// <param name="keyReference">The key value.</param>
        protected virtual void ContextLogger(IEntityContext ctx, string action, string keyValue, string keyReference)
            => ContextLoggerInternal(ctx, action, $"{keyValue}|{keyReference}");

        /// <summary>
        /// This method logs the response to the request.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="action">The repository action.</param>
        /// <param name="data">The data.</param>
        protected virtual void ContextLoggerInternal(IEntityContext ctx, string action, string data)
        {
            if (ctx.IsSuccessResponse)
                Collector?.LogMessage($"{action}@{typeof(E).Name}/{data} success: {ctx.ResponseCode}");
            else if (ctx.IsNotFoundResponse)
                Collector?.LogMessage($"{action}@{typeof(E).Name}/{data} not found: {ctx.ResponseCode}: {ctx.ResponseMessage}");
            else
                Collector?.LogWarning($"{action}@{typeof(E).Name}/{data} failed: {ctx.ResponseCode}: {ctx.ResponseMessage}");
        }
        #endregion

        #region ProcessOutputEntity/ProcessOutputVersion
        /// <summary>
        /// Converts the SQL output to a repository holder format..
        /// </summary>
        /// <param name="context">The context response.</param>
        /// <param name="onEvent">The event to fire.</param>
        /// <returns>The repository response.</returns>
        protected virtual RepositoryHolder<K, E> ProcessOutputEntity(IEntityContext<E> context
            , Action<RepositoryHolder<K, E>> onEvent = null)
        {
            E entity = context.ResponseEntities.FirstOrDefault();
            K key = entity != null ? KeyMaker(entity) : default(K);

            var rs = new RepositoryHolder<K, E>(key, null, entity, context.ResponseCode, context.ResponseMessage);

            onEvent?.Invoke(rs);

            return rs;
        }

        /// <summary>
        /// Converts the SQL output to a repository holder format..
        /// </summary>
        /// <param name="context">The SQL response.</param>
        /// <param name="key">The optional key.</param>
        /// <param name="onEvent">The event to fire.</param>
        /// <returns>The repository response.</returns>
        protected virtual RepositoryHolder<K, Tuple<K, string>> ProcessOutputVersion(
            IEntityContext<Tuple<K, string>> context, K key = default(K)
            , Action<RepositoryHolder<K, Tuple<K, string>>> onEvent = null)

        {
            var entity = context.ResponseEntities.FirstOrDefault();
            var rs = (entity == null) ? new RepositoryHolder<K, Tuple<K, string>>(key, null, null, context.ResponseCode, context.ResponseMessage)
                : new RepositoryHolder<K, Tuple<K, string>>(entity.Item1, null, new Tuple<K, string>(entity.Item1, entity.Item2), context.ResponseCode, context.ResponseMessage);

            onEvent?.Invoke(rs);

            return rs;
        }
        #endregion

        #region Collector
        /// <summary>
        /// This is the data collector used to collect logging and data information.
        /// </summary>
        public virtual IDataCollection Collector { get; set; } 
        #endregion
    }

    /// <summary>
    /// This is the base holding class.
    /// </summary>
    public abstract class RepositoryBase
    {
        #region Constructor
        /// <summary>
        /// This is the base repository shared class.
        /// </summary>
        /// <param name="repositoryType">The type of the inherited repository.</param>
        protected RepositoryBase(Type repositoryType)
        {
            RepositoryTypeValidate(repositoryType, out var keyType, out var entityType);

            RepositoryServerType = repositoryType;
            TypeKey = keyType;
            TypeEntity = entityType;

            RepositoryType = typeof(IRepositoryAsync<,>).MakeGenericType(keyType, entityType);
        } 
        #endregion

        #region RepositoryTypeValidate(Type repositoryType, out Type keyType, out Type entityType)
        /// <summary>
        /// This method checks the type declaration for the overridden entity.
        /// </summary>
        /// <param name="repositoryType">The repository type.</param>
        /// <param name="keyType">The outgoing key type.</param>
        /// <param name="entityType">The outgoing entity type.</param>
        protected virtual void RepositoryTypeValidate(Type repositoryType, out Type keyType, out Type entityType)
        {
            //Let's just check that the return type is a repository.
            if (!repositoryType.IsSubclassOfRawGeneric(typeof(IRepositoryAsyncServer<,>)))
                throw new ArgumentOutOfRangeException($"{nameof(RepositoryBase)}: '{repositoryType.Name}' does not implement IRepositoryAsyncServer<,>");

            keyType = repositoryType.GenericTypeArguments[0];
            entityType = repositoryType.GenericTypeArguments[1];
        } 
        #endregion

        #region ResultFormat...
        /// <summary>
        /// Formats the outgoing result.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type..</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="key">The key.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="tup">The tuple function.</param>
        /// <param name="options">The incoming options.</param>
        /// <param name="holderAction">The action to execute when the holder is created.</param>
        /// <returns>Returns the holder.</returns>
        public static Task<RepositoryHolder<KT, ET>> ResultFormat<KT, ET>(int result, Func<KT> key = null, Func<ET> entity = null
            , Func<Tuple<string,string>> tup = null
            , RepositorySettings options = null, Action<RepositoryHolder<KT, ET>> holderAction = null)
            where KT : IEquatable<KT>
        {
            var k = key != null ? key() : default(KT);
            var e = entity != null ? entity() : default(ET);
            var t = tup != null ? tup() : null;

            RepositoryHolder<KT, ET> holder;
            switch (result)
            {
                case 200:
                case 201:
                    holder = new RepositoryHolder<KT, ET>(k, t, e, result);
                    break;
                default:
                    holder = new RepositoryHolder<KT, ET>(k, t, default(ET), result);
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
            if (object.Equals(key,default(KT)))
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
            if (object.Equals(value,default(ET)))
                throw new ArgumentNullException("value must be set to a value");
        }
        #endregion

        /// <summary>
        /// This is the generic repository type, i.e. IRepositoryAsyncServer
        /// </summary>
        public Type RepositoryServerType { get; }

        /// <summary>
        /// This is the generic repository type, i.e. IRepositoryAsyncServer
        /// </summary>
        public Type RepositoryType { get; }

        /// <summary>
        /// This is the key type,
        /// </summary>
        public Type TypeKey { get; }
        /// <summary>
        /// This is the entity type.
        /// </summary>
        public Type TypeEntity { get; }
    }
}
