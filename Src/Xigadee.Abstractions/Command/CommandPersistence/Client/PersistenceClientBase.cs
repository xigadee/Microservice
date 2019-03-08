#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the abstract base class for persistence client/server communication functionality.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <typeparam name="P">The policy.</typeparam>
    public abstract class PersistenceClientBase<K, E, P> : CommandBase<PersistenceClientStatistics, P>
        , IRepositoryAsyncServer<K, E>
        where K : IEquatable<K>
        where P : CommandPolicy, new()
    {
        #region Declarations
        /// <summary>
        /// This is the internal cache manager that can be set to redirect calls to the cache. 
        /// </summary>
        protected readonly ICacheManager<K, E> mCacheManager;
        /// <summary>
        /// This is the default timespan that a message will wait if not set.
        /// </summary>
        protected readonly TimeSpan? mDefaultRequestTimespan;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor which sets the cache manager.
        /// </summary>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="defaultRequestTimespan">This is the default wait time for a response to arrive. This can be set to override the value stored in the policy.</param>
        protected PersistenceClientBase(ICacheManager<K, E> cacheManager = null, TimeSpan? defaultRequestTimespan = null)
        {
            mCacheManager = cacheManager ?? new NullCacheManager<K, E>();
            mDefaultRequestTimespan = defaultRequestTimespan;
        }
        #endregion

        #region FriendlyName
        /// <summary>
        /// Update to friendly name to make it clear which entity is being used
        /// </summary>
        public override string FriendlyName
        {
            get
            {
                return $"{base.FriendlyName}-{typeof(E).Name}";
            }
        }

        #endregion
        #region DefaultPrincipal
        /// <summary>
        /// This is the default principal that the inititor works under.
        /// </summary>
        public IPrincipal DefaultPrincipal
        {
            get; set;
        } 
        #endregion

        #region Persistence shortcuts

        #region Create(E entity, RepositorySettings settings = null)
        /// <summary>
        /// This method is used to create an entity in the persistence store.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <param name="settings">The persistence request settings.</param>
        /// <returns>Returns a response holder that indicates the status of the request and the entity or key and version id where appropriate.</returns>
        public virtual async Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings settings = null)
        {
            ValidateServiceStarted();

            return await TransmitInternal(EntityActions.Create, new RepositoryHolder<K, E> { Entity = entity, Settings = settings }, principal: DefaultPrincipal);
        }
        #endregion
        #region Read(K key, RepositorySettings settings = null)
        /// <summary>
        /// This method reads an entity.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="settings">The persistence request settings.</param>
        /// <returns>Returns a holder that indicates the status of the request and the entity where appropriate.</returns>
        public virtual async Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings settings = null)
        {
            ValidateServiceStarted();

            if ((settings?.UseCache ?? true) && mCacheManager.IsActive)
            {
                var result = await mCacheManager.Read(key);
                if (result.IsSuccess)
                {
                    return new RepositoryHolder<K, E>(key, new Tuple<string, string>(result.Id, result.VersionId), responseCode: 200, entity: result.Entity) { IsCached = true };
                }
            }

            return await TransmitInternal(EntityActions.Read, new RepositoryHolder<K, E> { Key = key, Settings = settings }, principal: DefaultPrincipal);
        }
        #endregion
        #region ReadByRef(string refKey, string refValue, RepositorySettings settings = null)
        /// <summary>
        /// This method reads and entity by the reference key/value pair.
        /// </summary>
        /// <param name="refKey">The reference key, i.e. email etc.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="settings">The persistence request settings.</param>
        /// <returns>Returns a response holder that indicates the status of the request and the entity or key and version id where appropriate.</returns>
        public virtual async Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings settings = null)
        {
            ValidateServiceStarted();

            if ((settings?.UseCache ?? true) && mCacheManager.IsActive)
            {
                // Do a version read initially to check it is there and get the key (not returned in a read by ref)
                var resultVersion = await mCacheManager.VersionRead(new Tuple<string, string>(refKey, refValue));
                if (resultVersion.IsSuccess)
                {
                    var resultRead = await mCacheManager.Read(new Tuple<string, string>(refKey, refValue));
                    if (resultRead.IsSuccess)
                        return new RepositoryHolder<K, E>(resultVersion.Entity.Item1, new Tuple<string, string>(resultVersion.Id, resultVersion.VersionId), responseCode: 200, entity: resultRead.Entity) { IsCached = true };
                }
            }

            return await TransmitInternal(EntityActions.ReadByRef, new RepositoryHolder<K, E> { KeyReference = new Tuple<string, string>(refKey, refValue), Settings = settings }, principal: DefaultPrincipal);
        }
        #endregion
        #region Update(E entity, RepositorySettings settings = null)
        /// <summary>
        /// This method updates an entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="settings">The persistence settings.</param>
        /// <returns>Returns a response holder that indicates the status of the request and the entity or key and version id where appropriate.</returns>
        public virtual async Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings settings = null)
        {
            ValidateServiceStarted();

            return await TransmitInternal(EntityActions.Update, new RepositoryHolder<K, E> { Entity = entity, Settings = settings }, principal: DefaultPrincipal);
        }
        #endregion
        #region Delete(K key, RepositorySettings settings = null)
        /// <summary>
        /// This method deletes an entity by its key.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="settings">The persistence request settings.</param>
        /// <returns>Returns a response holder that indicates the status of the request and the entity or key and version id where appropriate.</returns>
        public virtual async Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings settings = null)
        {
            ValidateServiceStarted();

            return await TransmitInternal(EntityActions.Delete, new RepositoryHolder<K, Tuple<K, string>> { Key = key, Settings = settings }, principal: DefaultPrincipal);
        }
        #endregion
        #region DeleteByRef(string refKey, string refValue, RepositorySettings settings = null)
        /// <summary>
        /// This method deletes an entity by its reference.
        /// </summary>
        /// <param name="refKey">The reference key, i.e. email etc.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="settings">The persistence request settings.</param>
        /// <returns>Returns a response holder that indicates the status of the request and the entity or key and version id where appropriate.</returns>
        public virtual async Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings settings = null)
        {
            ValidateServiceStarted();

            return await TransmitInternal(EntityActions.DeleteByRef, new RepositoryHolder<K, Tuple<K, string>> { KeyReference = new Tuple<string, string>(refKey, refValue), Settings = settings }, principal: DefaultPrincipal);
        }
        #endregion
        #region Version(K key, RepositorySettings settings = null)
        /// <summary>
        /// This method resolves an entity by its key.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="settings">The persistence request settings.</param>
        /// <returns>Returns a response holder that indicates the status of the request and the entity or key and version id where appropriate.</returns>
        public virtual async Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings settings = null)
        {
            ValidateServiceStarted();

            if ((settings?.UseCache ?? true) && mCacheManager.IsActive)
            {
                var result = await mCacheManager.VersionRead(key);
                if (result.IsSuccess)
                {
                    return new RepositoryHolder<K, Tuple<K, string>>(result.Entity.Item1, new Tuple<string, string>(result.Id, result.VersionId), responseCode: 200, entity: result.Entity) { IsCached = true };
                }
            }

            return await TransmitInternal(EntityActions.Version, new RepositoryHolder<K, Tuple<K, string>> { Key = key, Settings = settings }, principal: DefaultPrincipal);
        }
        #endregion
        #region VersionByRef(string refKey, string refValue, RepositorySettings settings = null)
        /// <summary>
        /// This method resolves an entity by its reference
        /// </summary>
        /// <param name="refKey">The reference key, i.e. email etc.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="settings">The persistence request settings.</param>
        /// <returns>Returns a response holder that indicates the status of the request and the entity or key and version id where appropriate.</returns>
        public virtual async Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings settings = null)
        {
            ValidateServiceStarted();

            if ((settings?.UseCache ?? true) && mCacheManager.IsActive)
            {
                var result = await mCacheManager.VersionRead(new Tuple<string, string>(refKey, refValue));
                if (result.IsSuccess)
                {
                    return new RepositoryHolder<K, Tuple<K, string>>(result.Entity.Item1, new Tuple<string, string>(result.Id, result.VersionId), responseCode: 200, entity: result.Entity) { IsCached = true };
                }
            }

            return await TransmitInternal(EntityActions.VersionByRef, new RepositoryHolder<K, Tuple<K, string>> { KeyReference = new Tuple<string, string>(refKey, refValue), Settings = settings }, principal: DefaultPrincipal);
        }
        #endregion

        #region Search(SearchRequest rq, RepositorySettings settings = null)
        /// <summary>
        /// This method issues a search request.
        /// </summary>
        /// <param name="rq">The search request.</param>
        /// <param name="settings">The persistence request settings.</param>
        /// <returns>The search response.</returns>
        public virtual async Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest rq, RepositorySettings settings = null)
        {
            ValidateServiceStarted();

            //if ((settings?.UseCache ?? true) && mCacheManager.IsActive)
            //{
            //    var result = await mCacheManager.VersionRead(new Tuple<string, string>(refKey, refValue));
            //    if (result.IsSuccess)
            //    {
            //        return new RepositoryHolder<K, Tuple<K, string>>(result.Entity.Item1, new Tuple<string, string>(result.Id, result.VersionId), responseCode: 200, entity: result.Entity) { IsCached = true };
            //    }
            //}

            return await TransmitInternal(EntityActions.Search, new RepositoryHolder<SearchRequest, SearchResponse> { Key = rq, Settings = settings }, principal: DefaultPrincipal);
        } 
        #endregion

        #endregion

        /// <summary>
        /// This abstract method is used to transmit the request to the appropriate party.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="actionType">The request action type, i.e. Create/Read etc.</param>
        /// <param name="rq">The request.</param>
        /// <param name="routing">The routing options.</param>
        /// <returns>Returns the request response.</returns>
        protected abstract Task<RepositoryHolder<KT, ET>> TransmitInternal<KT, ET>(string actionType, RepositoryHolder<KT, ET> rq, ProcessOptions? routing = null, IPrincipal principal = null) 
            where KT : IEquatable<KT>;

        #region ProcessResponse<KT, ET>(TaskStatus status, TransmissionPayload prs, bool async)
        /// <summary>
        /// This method is used to process the returning message response.
        /// </summary>
        /// <typeparam name="KT"></typeparam>
        /// <typeparam name="ET"></typeparam>
        /// <param name="rType"></param>
        /// <param name="payload"></param>
        /// <param name="processAsync"></param>
        /// <returns></returns>
        protected virtual RepositoryHolder<KT, ET> ProcessResponse<KT, ET>(TaskStatus rType, TransmissionPayload payload, bool processAsync)
        {
            StatisticsInternal.ActiveDecrement(payload != null ? payload.Extent : TimeSpan.Zero);

            if (processAsync)
                return new RepositoryHolder<KT, ET>(responseCode: 202, responseMessage: "Accepted");

            try
            {
                switch (rType)
                {
                    case TaskStatus.RanToCompletion:
                        if (payload.Message.Holder == null)
                        {
                            int rsCode = 500;
                            int.TryParse(payload.Message?.Status, out rsCode);

                            string rsMessage = payload.Message?.StatusDescription ?? "Unexpected response (no payload)";

                            return new RepositoryHolder<KT, ET>(responseCode: rsCode, responseMessage: rsMessage);
                        }

                        try
                        {
                            if (payload.Message.Holder.HasObject)
                                return (RepositoryHolder<KT, ET>)payload.Message.Holder.Object;
                            else
                            {
                                int rsCode = 500;
                                int.TryParse(payload.Message?.Status, out rsCode);

                                string rsMessage = payload.Message?.StatusDescription ?? "Unexpected response (no payload)";

                                return new RepositoryHolder<KT, ET>(responseCode: rsCode, responseMessage: rsMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            StatisticsInternal.ErrorIncrement();
                            return new RepositoryHolder<KT, ET>(responseCode: 500, responseMessage: $"Unexpected cast error: {payload.Message.Holder.Object.GetType().Name}-{ex.Message}");
                        }
                    case TaskStatus.Canceled:
                        StatisticsInternal.ErrorIncrement();
                        return new RepositoryHolder<KT, ET>(responseCode: 408, responseMessage: "Time out");
                    case TaskStatus.Faulted:
                        StatisticsInternal.ErrorIncrement();
                        return new RepositoryHolder<KT, ET>() { ResponseCode = (int)PersistenceResponse.GatewayTimeout504, ResponseMessage = "Response timeout." };
                    default:
                        StatisticsInternal.ErrorIncrement();
                        return new RepositoryHolder<KT, ET>(responseCode: 500, responseMessage: rType.ToString());

                }
            }
            catch (Exception ex)
            {
                Collector?.LogException("Error processing response for task status " + rType, ex);
                throw;
            }
        }
        #endregion
    }
}
