using System;
using System.Linq;
namespace Xigadee
{
    /// <summary>
    /// This is the abstract class used by the primary Microservice containers.
    /// </summary>
    /// <typeparam name="S">The status class.</typeparam>
    /// <typeparam name="P">The policy class.</typeparam>
    public partial class ServiceHandlerContainer: ServiceContainerBase<ServiceHandlerContainerStatistics, ServiceHandlerContainerPolicy>, IServiceHandlers
        , IRequireDataCollector, IRequireServiceOriginator, IRequireSharedServices
    {
        #region Constructor
        /// <summary>
        /// This is the default construct that sets or creates the policy object depending on whether it is passed in to the constructor. 
        /// </summary>
        /// <param name="policy">The optional policy class.</param>
        /// <param name="name">The optional name for the component.</param>
        public ServiceHandlerContainer(ServiceHandlerContainerPolicy policy = null, string name = null) : base(policy, name)
        {
            Authentication = new ServiceHandlerCollection<IServiceHandlerAuthentication>(OnAuthenticationAdd);
            Compression = new ServiceHandlerCollection<IServiceHandlerCompression>();
            Encryption = new ServiceHandlerCollection<IServiceHandlerEncryption>();
            Serialization = new ServiceHandlerCollection<IServiceHandlerSerialization>();
        }
        #endregion

        #region StatisticsRecalculate(ServiceHandlerContainerStatistics statistics)
        /// <summary>
        /// This method updates the security statistics.
        /// </summary>
        /// <param name="statistics">The statistics.</param>
        protected override void StatisticsRecalculate(ServiceHandlerContainerStatistics statistics)
        {
            base.StatisticsRecalculate(statistics);

            try
            {
                statistics.Authentication = Authentication?.Select((h) => HandlerDebug(h)).ToArray();
                statistics.Encryption = Encryption?.Select((h) => HandlerDebug(h)).ToArray(); ;
                statistics.Serialization = Serialization?.Select((h) => HandlerDebug(h)).ToArray(); ;
                statistics.Compression = Compression?.Select((h) => HandlerDebug(h)).ToArray(); ;
            }
            catch (Exception)
            {

            }
        }

        private string HandlerDebug(IServiceHandler h)
        {
            return $"{h.Id}: {h.Name}";
        }
        #endregion

        #region Start/Stop
        /// <summary>
        /// Starts the service handler service..
        /// </summary>
        /// <exception cref="PayloadSerializerCollectionIsEmptyException"></exception>
        protected override void StartInternal()
        {
            if (Serialization.Count == 0)
                throw new PayloadSerializerCollectionIsEmptyException();

        }
        /// <summary>
        /// Stops the service handler service.
        /// </summary>
        protected override void StopInternal()
        {
        } 
        #endregion

        #region Collector
        /// <summary>
        /// This is the data collector used for logging.
        /// </summary>
        public IDataCollection Collector { get; set; }
        #endregion
        #region OriginatorId
        /// <summary>
        /// This is the system information.
        /// </summary>
        public MicroserviceId OriginatorId { get; set; }
        #endregion
        #region SharedServices
        /// <summary>
        /// Gets or sets the shared services.
        /// </summary>
        public ISharedService SharedServices { get; set; } 
        #endregion

        #region Verify(Channel channel, TransmissionPayload payloadIn)
        /// <summary>
        /// This method verifies the incoming payload, and decrypts the channel payload if this has been specified 
        /// for the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="payloadIn">The incoming payload.</param>
        public void Verify(Channel channel, TransmissionPayload payloadIn)
        {
            //First decrypt the payload.
            if (channel.Encryption != null)
            {
                byte[] decrypt = Decrypt(channel.Encryption, payloadIn.Message.Holder);

                payloadIn.Message.Holder = decrypt;
            }

            //Now verify the signature
            if (channel.Authentication != null)
            {
                IServiceHandlerAuthentication handler;

                if (!Authentication.TryGet(channel.Authentication.Id, out handler))
                    throw new ChannelAuthenticationHandlerNotResolvedException(channel);

                handler.Verify(payloadIn);
            }
            else
                payloadIn.SecurityPrincipal = new MicroserviceSecurityPrincipal();

        }
        #endregion
        #region Secure(Channel channel, TransmissionPayload payloadOut)
        /// <summary>
        /// This method encrypts the outgoing payload if this has been set.
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <param name="payloadOut">The outgoing payload.</param>
        public void Secure(Channel channel, TransmissionPayload payloadOut)
        {
            //First sign the message, if set.
            if (channel.Authentication != null)
            {
                IServiceHandlerAuthentication handler;

                if (!Authentication.TryGet(channel.Authentication.Id, out handler))
                    throw new ChannelAuthenticationHandlerNotResolvedException(channel);

                handler.Sign(payloadOut);
            }

            //Now encrypt the payload.
            if (channel.Encryption != null)
            {
                byte[] encrypt = Encrypt(channel.Encryption, payloadOut.Message.Holder);

                payloadOut.Message.Holder = encrypt;
            }
        }
        #endregion

    }
}
