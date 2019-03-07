using System;
using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    public partial class ServiceHandlerContainer
    {
        /// <summary>
        /// Gets the authentication collection.
        /// </summary>
        public ServiceHandlerCollection<IServiceHandlerAuthentication> Authentication { get; }

        private void OnAuthenticationAdd(IServiceHandlerAuthentication handler)
        {
            handler.Collector = Collector;
            handler.OriginatorId = OriginatorId;
        }


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
