#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the statistics class used to log persistence shared service requests.
    /// </summary>
    public class PersistenceSharedServiceStatistics:MessageInitiatorStatistics
    {

    }

    /// <summary>
    /// This class is used to connect the internal persistence handler with other internal commands through the use of 
    /// Shared Services.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceSharedService<K, E>: MessageInitiatorBase<MessageInitiatorRequestTracker, PersistenceSharedServiceStatistics>
        , IRepositoryAsync<K, E>, IRequireSharedServices, IPersistenceSharedService
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This is the internal cache manager that can be set to redirect calls to the cache. 
        /// </summary>
        private ICacheManager<K, E> mCacheManager;
        /// <summary>
        /// This boolean property indicates whether the service is shared.
        /// </summary>
        private bool mShared = false;
        /// <summary>
        /// This is the shared services collection
        /// </summary>
        private ISharedService mSharedServices = null;
        private readonly MessageFilterWrapper mResponseId;
        private readonly string mMessageType;
        private readonly string mResponseChannel;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the shared service.
        /// </summary>
        /// <param name="responseChannel">This is the internal response channel that the message will listen on.</param>
        public PersistenceSharedService(string responseChannel = "internalpersistence", ICacheManager<K, E> cacheManager = null)
        {
            mCacheManager = cacheManager?? new NullCacheManager<K,E>();
            mMessageType = typeof(E).Name;
            mResponseId = new MessageFilterWrapper(new ServiceMessageHeader(responseChannel, mMessageType));
            mResponseChannel = responseChannel;
        }
        #endregion

        #region SharedServices
        /// <summary>
        /// This implementation registers the shared service with a lazy constructor.
        /// </summary>
        public ISharedService SharedServices
        {
            get { return mSharedServices; }
            set
            {
                mSharedServices = value;
                if (!mShared && !mSharedServices.HasService<IRepositoryAsync<K, E>>())
                    mSharedServices.RegisterService<IRepositoryAsync<K, E>>(
                        new Lazy<IRepositoryAsync<K, E>>(() => this), typeof(E).Name);
            }
        }
        #endregion

        #region ResponseId
        /// <summary>
        /// THis is the response message filter used to pick up the returning response.
        /// </summary>
        protected override MessageFilterWrapper ResponseId
        {
            get { return mResponseId; }
        }
        #endregion

        #region StopInternal()
        /// <summary>
        /// This override removes the shared service registration.
        /// </summary>
        protected override void StopInternal()
        {
            if (mShared)
                mSharedServices.RemoveService<IRepositoryAsync<K, E>>();

            base.StopInternal();
        }
        #endregion

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
            //    var result = await mCacheManager.Read(refKey, refValue);
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

        #region TransmitInternal<KT, ET>(string actionType, RepositoryHolder<KT, ET> rq)
        /// <summary>
        /// This method marshals the RepositoryHolder and transmits it to the remote Microservice.
        /// </summary>
        /// <typeparam Name="KT">The key type.</typeparam>
        /// <typeparam Name="ET">The entity type.</typeparam>
        /// <param Name="actionType">The action type.</param>
        /// <param Name="rq">The repository holder request.</param>
        /// <returns>Returns an async task that will be signalled when the request completes or times out.</returns>
        protected async Task<RepositoryHolder<KT, ET>> TransmitInternal<KT, ET>(string actionType, RepositoryHolder<KT, ET> rq)
            where KT : IEquatable<K>
        {
            mStatistics.ActiveIncrement();
            var payloadRq = TransmissionPayload.Create();
            var payloadsRs = new List<TransmissionPayload>();

            bool processAsync = rq.Settings == null ? false : rq.Settings.ProcessAsync;
            payloadRq.Options = ProcessOptions.RouteInternal;
            var message = payloadRq.Message;

            payloadRq.MessageObject = rq;
            message.ChannelId = ChannelId;
            message.ChannelPriority = processAsync ? 0:-1;
            message.MessageType = mMessageType;
            message.ActionType = actionType;

            message.ResponseChannelId = mResponseChannel;

            message.Blob = PayloadSerializer.PayloadSerialize(rq);

            return await TransmitAsync<RepositoryHolder<KT, ET>>(payloadRq, ProcessResponse<KT, ET>, processAsync: processAsync);
        }
        #endregion

        #region UseASPNETThreadModel
        /// <summary>
        /// This will return false as we are only used for internal Microservice use.
        /// </summary>
        public override bool UseASPNETThreadModel
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        #endregion

        #region ProcessResponse<KT, ET>(TaskStatus status, TransmissionPayload prs, bool async)
        /// <summary>
        /// This method is used to process the returning message response.
        /// </summary>
        /// <typeparam name="KT"></typeparam>
        /// <typeparam name="ET"></typeparam>
        /// <param name="status"></param>
        /// <param name="prs"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        protected virtual RepositoryHolder<KT, ET> ProcessResponse<KT, ET>(TaskStatus status, TransmissionPayload prs, bool async)
        {
            if (prs == null)
            {
                mStatistics.ActiveDecrement(0);
                mStatistics.ErrorIncrement();
                Logger.LogMessage(LoggingLevel.Fatal, "RepositoryHolder - unexpected error: prs is null");
                return new RepositoryHolder<KT, ET>(responseCode: 520, responseMessage: "RepositoryHolder - unexpected error (prs)");
            }

            mStatistics.ActiveDecrement(prs.Extent);

            if (async)
                return new RepositoryHolder<KT, ET>(responseCode: 202, responseMessage: "Accepted");

            switch (status)
            {
                case TaskStatus.RanToCompletion:
                    if (prs.MessageObject != null)
                        return prs.MessageObject as RepositoryHolder<KT, ET>;

                    if (prs.Message.Blob == null)
                        return new RepositoryHolder<KT, ET>(responseCode: 500, responseMessage: "Unexpected response (no payload)");

                    try
                    {
                        var response = PayloadSerializer.PayloadDeserialize<RepositoryHolder<KT, ET>>(prs);
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
                default:
                    mStatistics.ErrorIncrement();
                    return new RepositoryHolder<KT, ET>(responseCode: 500, responseMessage: status.ToString());

            }
        } 
        #endregion
    }
}
