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
        private readonly MessageFilterWrapper mResponseId;
        private readonly string mMessageType;
        private readonly string mResponseChannel;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the shared service.
        /// </summary>
        /// <param name="responseChannel">This is the internal response channel that the message will listen on.</param>
        /// <param name="cacheManager"></param>
        public PersistenceSharedService(string responseChannel = "internalpersistence"
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultRequestTimespan = null) 
            :base(cacheManager, defaultRequestTimespan)
        {
            mMessageType = typeof(E).Name;
            mResponseId = new MessageFilterWrapper(new ServiceMessageHeader(responseChannel, mMessageType));
            mResponseChannel = responseChannel;
            UseASPNETThreadModel = false;
        }
        #endregion

        #region SharedServices
        /// <summary>
        /// This override registers the persistence shortcut interface.
        /// </summary>
        /// <param name="sharedServices">The shared service collecction.</param>
        protected override void SharedServicesChange(ISharedService sharedServices)
        {
            mSharedServices = sharedServices;

            if (!mShared 
                && mSharedServices != null 
                && !mSharedServices.HasService<IRepositoryAsync<K, E>>())
                    mShared = mSharedServices.RegisterService<IRepositoryAsync<K, E>>(
                        new Lazy<IRepositoryAsync<K, E>>(() => this), typeof(E).Name);
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This override removes the shared service registration.
        /// </summary>
        protected override void StopInternal()
        {
            base.StopInternal();

            if (mShared && mSharedServices != null)
                mSharedServices.RemoveService<IRepositoryAsync<K, E>>();
        }
        #endregion

        #region ResponseId
        /// <summary>
        /// This is the response message filter used to pick up the returning response.
        /// </summary>
        protected override MessageFilterWrapper ResponseId
        {
            get { return new MessageFilterWrapper(new ServiceMessageHeader(mResponseChannel, mMessageType)); }
        }
        #endregion

        #region TransmitInternal<KT, ET>(string actionType, RepositoryHolder<KT, ET> rq)
        /// <summary>
        /// This method marshals the RepositoryHolder and transmits it to the remote Microservice.
        /// </summary>
        /// <typeparam Name="KT">The key type.</typeparam>
        /// <typeparam Name="ET">The entity type.</typeparam>
        /// <param name="actionType">The action type.</param>
        /// <param name="rq">The repository holder request.</param>
        /// <param name="routing"></param>
        /// <returns>Returns an async task that will be signalled when the request completes or times out.</returns>
        protected override async Task<RepositoryHolder<KT, ET>> TransmitInternal<KT, ET>(string actionType, RepositoryHolder<KT, ET> rq, ProcessOptions? routing = null)
        {
            StatisticsInternal.ActiveIncrement();

            var payloadRq = TransmissionPayload.Create();

            bool processAsync = rq.Settings?.ProcessAsync ?? false;
            payloadRq.Options = ProcessOptions.RouteInternal;
            var message = payloadRq.Message;

            payloadRq.MaxProcessingTime = rq.Settings?.WaitTime ?? mDefaultRequestTimespan;
            payloadRq.MessageObject = rq;
            message.ChannelId = ChannelId;
            message.ChannelPriority = processAsync ? 0:-1;
            message.MessageType = mMessageType;
            message.ActionType = actionType;

            message.ResponseChannelId = mResponseChannel;
            message.ResponseChannelPriority = -1; //Always internal

            message.Blob = PayloadSerializer.PayloadSerialize(rq);

            return await TransmitAsync(payloadRq, ProcessResponse<KT, ET>, processAsync: processAsync);
        }
        #endregion

        #region TaskManagerTimeoutSupported
        /// <summary>
        /// Shared services operate as a bridge to the internal commands and use
        /// the Task Manager to support timeouts.
        /// </summary>
        public override bool TaskManagerTimeoutSupported
        {
            get
            {
                return true;
            }
        } 
        #endregion
    }
}
