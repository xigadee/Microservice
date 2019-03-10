namespace Xigadee
{
    public partial class CommunicationContainer
    {
        #region PayloadUnpack(TransmissionPayload payload)
        /// <summary>
        /// This method validates the payload with the security container.
        /// </summary>
        /// <param name="payload">The incoming payload.</param>
        protected virtual void PayloadUnpack(TransmissionPayload payload)
        { 
            //Try and resolve the channel.
            Channel channel = null;
            TryGet(payload.Message.ChannelId, ChannelDirection.Incoming, out channel);

            //Decrypt and verify the incoming message.
            Verify(channel, payload);
            payload.TraceWrite("Verified");
        }
        #endregion
        #region PayloadOutgoingSecurity(TransmissionPayload payload)
        /// <summary>
        /// This method validates the payload with the security container.
        /// </summary>
        /// <param name="payload">The incoming payload.</param>
        protected virtual void PayloadPack(TransmissionPayload payload)
        {
            //Try and resolve the channel.
            Channel channel = null;
            TryGet(payload.Message.ChannelId, ChannelDirection.Outgoing, out channel);

            //Secure the outgoing payload.
            Secure(channel, payload);
            payload.TraceWrite("Secured");
        }
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
                byte[] decrypt = ServiceHandlers.Encryption[channel.Encryption].Decrypt(payloadIn.Message.Holder.Blob);

                payloadIn.Message.Holder = decrypt;
            }

            //Now verify the signature
            if (channel.Authentication != null)
            {
                if (!ServiceHandlers.Authentication.Contains(channel.Authentication.Id))
                    throw new ChannelAuthenticationHandlerNotResolvedException(channel);

                ServiceHandlers.Authentication[channel.Authentication.Id].Verify(payloadIn);
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
                if (!ServiceHandlers.Authentication.Contains(channel.Authentication.Id))
                    throw new ChannelAuthenticationHandlerNotResolvedException(channel);

                var handler = ServiceHandlers.Authentication[channel.Authentication.Id];

                handler.Sign(payloadOut);
            }

            //Now encrypt the payload.
            if (channel.Encryption != null)
            {
                byte[] encrypt = ServiceHandlers.Encryption[channel.Encryption].Encrypt(payloadOut.Message.Holder.Blob);

                payloadOut.Message.Holder = encrypt;
            }
        }
        #endregion
    }
}
