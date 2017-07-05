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

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to connect the internal persistence handler with other internal commands through the use of Shared Services.
    /// The primary difference between this class and PersistenceClient, is that this class is used by services to communicate directly with
    /// other services, and the requests generated are executed immediately, and do not wait for a priority slot. This stops deadlocks forming
    /// inside the Microservice.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceInternalService<K, E>: PersistenceClientBase<K, E, PersistenceInternalServicePolicy>
        , IPersistenceSharedService 
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
        /// <param name="defaultRequestTimespan"></param>
        public PersistenceInternalService(string responseChannel = "internalpersistence"
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
                        new Lazy<IRepositoryAsync<K, E>>(() => (IRepositoryAsync<K, E>)this), typeof(E).Name);
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
        protected override async Task<RepositoryHolder<KT, ET>> TransmitInternal<KT, ET>(string actionType, RepositoryHolder<KT, ET> rq, ProcessOptions? routing = null, IPrincipal principal = null)
        {
            StatisticsInternal.ActiveIncrement();

            var payloadRq = TransmissionPayload.Create();

            // Set the originator key to the correlation id if passed through the rq settings
            if (!string.IsNullOrEmpty(rq.Settings?.CorrelationId))
                payloadRq.Message.ProcessCorrelationKey = rq.Settings.CorrelationId;

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

            return await OutgoingRequestOut(payloadRq, ProcessResponse<KT, ET>, processAsync: processAsync);
        }
        #endregion
    }
}
