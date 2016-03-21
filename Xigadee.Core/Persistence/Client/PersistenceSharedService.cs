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
    /// This class is used to connect the internal persistence handler with other internal commands through the use of 
    /// Shared Services.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceSharedService<K, E>: PersistenceInitiatorBase<K, E>
        , IRequireSharedServices, IPersistenceSharedService
        where K : IEquatable<K>
    {
        #region Declarations
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
        public PersistenceSharedService(string responseChannel = "internalpersistence", ICacheManager<K, E> cacheManager = null):base(cacheManager)
        {
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

        #region TransmitInternal<KT, ET>(string actionType, RepositoryHolder<KT, ET> rq)
        /// <summary>
        /// This method marshals the RepositoryHolder and transmits it to the remote Microservice.
        /// </summary>
        /// <typeparam Name="KT">The key type.</typeparam>
        /// <typeparam Name="ET">The entity type.</typeparam>
        /// <param Name="actionType">The action type.</param>
        /// <param Name="rq">The repository holder request.</param>
        /// <returns>Returns an async task that will be signalled when the request completes or times out.</returns>
        protected override async Task<RepositoryHolder<KT, ET>> TransmitInternal<KT, ET>(string actionType, RepositoryHolder<KT, ET> rq, ProcessOptions? routing = null)
        {
            mStatistics.ActiveIncrement();

            var payloadRq = TransmissionPayload.Create();
            var payloadsRs = new List<TransmissionPayload>();

            bool processAsync = rq.Settings == null ? false : rq.Settings.ProcessAsync;
            payloadRq.Options = ProcessOptions.RouteInternal;
            var message = payloadRq.Message;
            payloadRq.MaxProcessingTime = rq.Settings?.WaitTime;
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
    }
}
