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

using System.Security.Claims;

namespace Xigadee
{
    /// <summary>
    /// The security container class contains all the components to secure the incoming messaging for a Microservice, 
    /// and to ensure that incoming message requests have the correct permissions necessary to be processed.
    /// </summary>
    public partial class SecurityContainer
    {


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
                byte[] decrypt = Decrypt(channel.Encryption, payloadIn.Message.Blob);

                payloadIn.Message.Blob = decrypt;
            }

            //Now verify the signature
            if (channel.Authentication != null)
            {
                if (!mAuthenticationHandlers.ContainsKey(channel.Authentication.Id))
                    throw new ChannelAuthenticationHandlerNotResolvedException(channel);

                mAuthenticationHandlers[channel.Authentication.Id].Verify(payloadIn);
            }
            else
                payloadIn.SecurityPrincipal = new ClaimsPrincipal();

        }

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
                if (!mAuthenticationHandlers.ContainsKey(channel.Authentication.Id))
                    throw new ChannelAuthenticationHandlerNotResolvedException(channel);

                mAuthenticationHandlers[channel.Authentication.Id].Sign(payloadOut);
            }

            //Now encrpyt the payload.
            if (channel.Encryption != null)
            {
                byte[] encrypt = Encrypt(channel.Encryption, payloadOut.Message.Blob);

                payloadOut.Message.Blob = encrypt;
            }

        }
    }
}
