using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class PersistenceInitiatorBase<K, E>
        : MessageInitiatorBase<MessageInitiatorRequestTracker, PersistenceInitiatorStatistics>, IRepositoryAsync<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This is the internal cache manager that can be set to redirect calls to the cache. 
        /// </summary>
        protected readonly ICacheManager<K, E> mCacheManager;
        #endregion

        /// <summary>
        /// This is the default constructor which sets the cache manager.
        /// </summary>
        /// <param name="cacheManager">THe cache manager.</param>
        protected PersistenceInitiatorBase(ICacheManager<K, E> cacheManager = null)
        {
            mCacheManager = cacheManager ?? new NullCacheManager<K, E>();
        }

        #region Persistence shortcuts

        public async Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings settings = null)
        {
            return await TransmitInternal(EntityActions.Create, new RepositoryHolder<K, E> { Entity = entity, Settings = settings });
        }

        public async Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings settings = null)
        {
            if (settings.UseCache && mCacheManager.IsActive)
            {
                var result = await mCacheManager.Read(key);
                if (result.IsSuccess)
                {
                    return new RepositoryHolder<K, E>(key, responseCode: 200, entity: result.Entity) { IsCached = true };
                }
            }

            return await TransmitInternal(EntityActions.Read, new RepositoryHolder<K, E> { Key = key, Settings = settings });
        }

        public async Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings settings = null)
        {
            //if (settings.UseCache && mCacheManager.IsActive)
            //{
            //    var result = await mCacheManager.Read(new Tuple<string,string>(refKey, refValue));
            //    if (result.IsSuccess)
            //    {
            //        return new RepositoryHolder<K, E>(result.k, responseCode: 200, entity: result.Entity) { IsCached = true };
            //    }
            //}

            return await TransmitInternal(EntityActions.ReadByRef, new RepositoryHolder<K, E> { KeyReference = new Tuple<string, string>(refKey, refValue), Settings = settings });
        }

        public async Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings settings = null)
        {
            return await TransmitInternal(EntityActions.Update, new RepositoryHolder<K, E> { Entity = entity, Settings = settings });
        }

        public async Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings settings = null)
        {
            return await TransmitInternal(EntityActions.Delete, new RepositoryHolder<K, Tuple<K, string>> { Key = key, Settings = settings });
        }

        public async Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings settings = null)
        {
            return await TransmitInternal(EntityActions.DeleteByRef, new RepositoryHolder<K, Tuple<K, string>> { KeyReference = new Tuple<string, string>(refKey, refValue), Settings = settings });
        }

        public async Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings settings = null)
        {
            return await TransmitInternal(EntityActions.Version, new RepositoryHolder<K, Tuple<K, string>> { Key = key, Settings = settings });
        }

        public async Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings settings = null)
        {
            return await TransmitInternal(EntityActions.VersionByRef, new RepositoryHolder<K, Tuple<K, string>> { KeyReference = new Tuple<string, string>(refKey, refValue), Settings = settings });
        }

        #endregion

        protected abstract Task<RepositoryHolder<KT, ET>> TransmitInternal<KT, ET>(string actionType, RepositoryHolder<KT, ET> rq, ProcessOptions? routing = null) 
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

            mStatistics.ActiveDecrement(payload != null ? payload.Extent : TimeSpan.Zero);

            if (processAsync)
                return new RepositoryHolder<KT, ET>(responseCode: 202, responseMessage: "Accepted");

            try
            {
                switch (rType)
                {
                    case TaskStatus.RanToCompletion:
                        if (payload.MessageObject != null)
                            return payload.MessageObject as RepositoryHolder<KT, ET>;

                        if (payload.Message.Blob == null)
                            return new RepositoryHolder<KT, ET>(responseCode: 500, responseMessage: "Unexpected response (no payload)");

                        try
                        {
                            var response = PayloadSerializer.PayloadDeserialize<RepositoryHolder<KT, ET>>(payload);
                            return response;
                        }
                        catch (Exception ex)
                        {
                            mStatistics.ErrorIncrement();
                            return new RepositoryHolder<KT, ET>(responseCode: 500, responseMessage: ex.Message);
                        }
                    case TaskStatus.Canceled:
                        mStatistics.ErrorIncrement();
                        return new RepositoryHolder<KT, ET>(responseCode: 408, responseMessage: "Time out");
                    case TaskStatus.Faulted:
                        mStatistics.ErrorIncrement();
                        return new RepositoryHolder<KT, ET>() { ResponseCode = (int)PersistenceResponse.GatewayTimeout504, ResponseMessage = "Response timeout." };
                    default:
                        mStatistics.ErrorIncrement();
                        return new RepositoryHolder<KT, ET>(responseCode: 500, responseMessage: rType.ToString());

                }
            }
            catch (Exception ex)
            {
                Logger.LogException("Error processing response for task status " + rType, ex);
                throw;
            }
        }
        #endregion

    }
}
