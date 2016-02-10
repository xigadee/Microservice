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
    /// This class is used to call the remote persistence manager,
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class PersistenceMessageInitiator<K, E> : MessageInitiatorBase<MessageInitiatorRequestTracker, MessageInitiatorStatistics>
        , IRepositoryAsync<K, E>, IPersistenceMessageInitiator
        where K : IEquatable<K>
    {
        #region Declarations

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor. This set the default routing to external only.
        /// </summary>
        public PersistenceMessageInitiator()
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
            get { return new MessageFilterWrapper(new ServiceMessageHeader(ResponseChannelId, EntityType)) { ClientId = OriginatorId }; }
        } 
        #endregion

        #region RoutingDefault
        /// <summary>
        /// This is the default routing for outgoing messages. 
        /// By default messages will try external providers.
        /// </summary>
        protected ProcessOptions? RoutingDefault { get; set; } 
        #endregion

        #region Persistence shortcuts
        public async Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings settings = null)
        {
            return await TransmitInternal(EntityActions.Create, new RepositoryHolder<K, E> { Entity = entity, Settings = settings });
        }

        public async Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings settings = null)
        {
            return await TransmitInternal(EntityActions.Read, new RepositoryHolder<K, E> { Key = key, Settings = settings });
        }

        public async Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings settings = null)
        {
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
        protected async Task<RepositoryHolder<KT, ET>> TransmitInternal<KT, ET>(string actionType, RepositoryHolder<KT, ET> rq, ProcessOptions? routing = null)
        {
            try
            {
                mStatistics.ActiveIncrement();
                var payload = TransmissionPayload.Create();

                // Set the originator key to the correlation id if passed through the rq settings
                if (rq.Settings != null && !string.IsNullOrEmpty(rq.Settings.CorrelationId))
                    payload.Message.OriginatorKey = rq.Settings.CorrelationId;

                bool processAsync = rq.Settings == null ? false : rq.Settings.ProcessAsync;

                payload.Message.ChannelPriority = processAsync ? 0 : 1;

                payload.Options = routing ?? RoutingDefault ?? ProcessOptions.RouteExternal;

                payload.Message.Blob = PayloadSerializer.PayloadSerialize(rq);
                payload.Message.ResponseChannelId = ResponseChannelId;
                payload.Message.ResponseChannelPriority = payload.Message.ChannelPriority;

                payload.Message.ChannelId = ChannelId;
                payload.Message.MessageType = EntityType;
                payload.Message.ActionType = actionType;

                if (rq.Settings != null && rq.Settings.WaitTime.HasValue)
                    payload.MaxProcessingTime = rq.Settings.WaitTime;

                return await TransmitAsync(payload, ProcessResponse<KT, ET>, processAsync);
            }
            catch (Exception ex)
            {
                string key = rq != null && rq.Key != null ? rq.Key.ToString() : string.Empty;
                Logger.LogException(string.Format("Error transmitting {0}-{1} internally", actionType, key), ex);
                throw;
            }
        }
        #endregion
        #region ProcessResponse<KT, ET>(TaskStatus rType, TransmissionPayload payload, bool processAsync)

        /// <summary>
        /// This method process the response.
        /// </summary>
        /// <typeparam Name="KT">The key type.</typeparam>
        /// <typeparam Name="ET">The entity type.</typeparam>
        /// <param Name="rType">The response enumeration.</param>
        /// <param Name="payload">The message payload.</param>
        /// <param name="rType"></param>
        /// <returns>Returns a new repository holder.</returns>
        private RepositoryHolder<KT, ET> ProcessResponse<KT, ET>(TaskStatus rType, TransmissionPayload payload, bool processAsync)
        {
            mStatistics.ActiveDecrement(payload != null ? payload.Extent : TimeSpan.Zero);

            if (processAsync)
                return new RepositoryHolder<KT, ET> { ResponseCode = (int)PersistenceResponse.Accepted202, ResponseMessage = "Accepted" };

            try
            {
                switch (rType)
                {
                    case TaskStatus.RanToCompletion:
                        return PayloadSerializer.PayloadDeserialize<RepositoryHolder<KT, ET>>(payload);
                    case TaskStatus.Canceled:
                        //return new RepositoryHolder<KT, ET>() { ResponseCode = 408, ResponseMessage = "Response timeout." };
                    case TaskStatus.Faulted: //TODO:check that this works
                        return new RepositoryHolder<KT, ET>() { ResponseCode = (int)PersistenceResponse.GatewayTimeout504, ResponseMessage = "Response timeout." };
                    default:
                        Logger.LogMessage(LoggingLevel.Error, "Unknown task response of " + rType);
                        return new RepositoryHolder<KT, ET> {ResponseCode = (int)PersistenceResponse.UnknownError500, ResponseMessage = "Unknown error."};
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
