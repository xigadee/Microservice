using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
namespace Xigadee
{
    /// <summary>
    /// This class allows you to connect to a persistence server command using the default actions with remote server capability and timeout support.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceClient<K, E> : PersistenceClientBase<K, E>, IPersistenceClientCommand 
        where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor. This set the default routing to external only.
        /// </summary>
        public PersistenceClient(ICacheManager<K, E> cacheManager = null, TimeSpan? defaultRequestTimespan = null)
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
            get { return new MessageFilterWrapper(new ServiceMessageHeader(ResponseChannelId, EntityType), OriginatorId.ExternalServiceId); }
        }
        #endregion

        #region RoutingDefault
        /// <summary>
        /// This is the default routing for outgoing messages. 
        /// By default messages will try external providers.
        /// </summary>
        public ProcessOptions? RoutingDefault { get; set; }
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
        protected override async Task<RepositoryHolder<KT, ET>> TransmitInternal<KT, ET>(
            string actionType, RepositoryHolder<KT, ET> rq, ProcessOptions? routing = null, IPrincipal principal = null)
        {
            TransmissionPayload payload;
            try
            {
                StatisticsInternal.ActiveIncrement();

                payload = TransmissionPayload.Create(Policy.TransmissionPayloadTraceEnabled);

                payload.SecurityPrincipal = TransmissionPayload.ConvertToClaimsPrincipal(principal ?? Thread.CurrentPrincipal);

                // Set the process correlation key to the correlation id if passed through the rq settings
                if (!string.IsNullOrEmpty(rq.Settings?.CorrelationId))
                    payload.Message.ProcessCorrelationKey = rq.Settings.CorrelationId;

                bool processAsync = rq.Settings?.ProcessAsync ?? false;

                payload.Message.ChannelPriority = processAsync ? 0 : 1;

                payload.Options = routing ?? RoutingDefault ?? ProcessOptions.RouteExternal;

                payload.Message.Holder = ServiceHandlerContext.CreateWithObject(rq);

                payload.Message.ResponseChannelId = ResponseChannelId;

                payload.Message.ResponseChannelId = ResponseId.Header.ChannelId;
                payload.Message.ResponseMessageType = ResponseId.Header.MessageType;
                payload.Message.ResponseActionType = ResponseId.Header.ActionType;

                payload.Message.ResponseChannelPriority = payload.Message.ChannelPriority;

                payload.Message.ChannelId = ChannelId;
                payload.Message.MessageType = EntityType;
                payload.Message.ActionType = actionType;

                payload.MaxProcessingTime = rq.Settings?.WaitTime ?? mDefaultRequestTimespan;

                payload.TraceWrite("Start Submit");

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
