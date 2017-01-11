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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to call a remote persistence manager,
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceMessageInitiator<K, E> : PersistenceInitiatorBase<K, E, PersistenceMessageInitiatorPolicy>
        , IPersistenceMessageInitiator 
        where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. This set the default routing to external only.
        /// </summary>
        public PersistenceMessageInitiator(ICacheManager<K, E> cacheManager = null, TimeSpan? defaultRequestTimespan = null) 
            : base(cacheManager, defaultRequestTimespan)
        {
            RoutingDefault = ProcessOptions.RouteExternal;
        }
        #endregion

        #region EntityType
        /// <summary>
        /// This is the entity type for the commands
        /// </summary>
        public virtual string EntityType
        {
            get
            {
                return typeof(E).Name;
            }
        } 
        #endregion
        #region ResponseId
        /// <summary>
        /// This is the MessageFilterWrapper for the payloadRs message channel. This will pick up all reponse messages of the specific type 
        /// for this instance and pipe them to be processed.
        /// </summary>
        protected override MessageFilterWrapper ResponseId
        {
            get { return new MessageFilterWrapper(new ServiceMessageHeader(ResponseChannelId, EntityType)) { ClientId = OriginatorId.ExternalServiceId }; }
        } 
        #endregion

        #region RoutingDefault
        /// <summary>
        /// This is the default routing for outgoing messages. 
        /// By default messages will try external providers.
        /// </summary>
        protected ProcessOptions? RoutingDefault { get; set; } 
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
            try
            {
                StatisticsInternal.ActiveIncrement();

                var payload = TransmissionPayload.Create();

                payload.SecurityPrincipal = TransmissionPayload.ConvertToClaimsPrincipal(rq.Settings?.SecurityPrincipal ?? Thread.CurrentPrincipal);

                // Set the process correlation key to the correlation id if passed through the rq settings
                if (!string.IsNullOrEmpty(rq.Settings?.CorrelationId))
                    payload.Message.ProcessCorrelationKey = rq.Settings.CorrelationId;

                bool processAsync = rq.Settings?.ProcessAsync ?? false;

                payload.Message.ChannelPriority = processAsync ? 0 : 1;

                payload.Options = routing ?? RoutingDefault ?? ProcessOptions.RouteExternal;

                payload.Message.Blob = PayloadSerializer.PayloadSerialize(rq);

                payload.Message.ResponseChannelId = ResponseChannelId;
                payload.Message.ResponseChannelPriority = payload.Message.ChannelPriority;

                payload.Message.ChannelId = ChannelId;
                payload.Message.MessageType = EntityType;
                payload.Message.ActionType = actionType;

                payload.MaxProcessingTime = rq.Settings?.WaitTime ?? mDefaultRequestTimespan;

                return await OutgoingRequestOut(payload, ProcessResponse<KT, ET>, processAsync);
            }
            catch (Exception ex)
            {
                string key = rq != null && rq.Key != null ? rq.Key.ToString() : string.Empty;
                Collector?.LogException($"Error transmitting {actionType}-{key} internally", ex);
                throw;
            }
        }
        #endregion

    }
}
