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
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class handles the token handshake between Microservices. It takes the submitted ClaimsPrincipal and passes
    /// it through to the remote Microservice along with the current service's metadata.
    /// </summary>
    public class JwtTokenAuthenticationHandler: AuthenticationHandlerBase
    {
        #region Static claims definitions
        public static string ClaimDestination = "http://claims.xigadee.com/dest";
        public static string ClaimProcessCorrelationKey = "http://claims.xigadee.com/paypck";
        public static string ClaimPayloadId = "http://claims.xigadee.com/payid";
        public static string ClaimServiceVersion = "http://claims.xigadee.com/ver";
        public static string ClaimServiceEngineVersion = "http://claims.xigadee.com/engver";
        #endregion
        #region Declarations
        readonly byte[] mSecret;
        readonly JwtHashAlgorithm mAlgorithm;
        readonly string mAudience;
        #endregion

        #region Constructor
        /// <summary>
        /// This constructor sets the secret and the auidence for the handler.
        /// </summary>
        /// <param name="algo">The hash algorithm to be used.</param>
        /// <param name="base64Secret">The secret byte array as a base64 string.</param>
        /// <param name="audience">The audience. This is compared when the token is received and validated. 
        /// By default it is set to 'comms'</param>
        public JwtTokenAuthenticationHandler(JwtHashAlgorithm algo, string base64Secret, string audience = "comms"):base(nameof(JwtTokenAuthenticationHandler))
        {
            mAlgorithm = algo;
            mSecret = Convert.FromBase64String(base64Secret);
            mAudience = audience;
        }
        /// <summary>
        /// This constructor sets the secret and the auidence for the handler.
        /// </summary>
        /// <param name="algo">The hash algorithm to be used.</param>
        /// <param name="secret">The secret byte array.</param>
        /// <param name="audience">The audience. This is compared when the token is received and validated. 
        /// By default it is set to 'comms'</param>
        public JwtTokenAuthenticationHandler(JwtHashAlgorithm algo, byte[] secret, string audience = "comms") : base(nameof(JwtTokenAuthenticationHandler))
        {
            mAlgorithm = algo;
            mSecret = secret;
            mAudience = audience;
        }
        #endregion

        #region Sign(TransmissionPayload payload)
        /// <summary>
        /// This method adds the Jwt signature to the outgoing message.
        /// </summary>
        /// <param name="payload"></param>
        public override void Sign(TransmissionPayload payload)
        {
            try
            {
                var token = TokenGenerate(payload);

                payload.Message.SecuritySignature = token.ToString(mSecret);

            }
            catch (Exception ex)
            {
                Collector?.SecurityEvent(SecurityEventDirection.Signing, ex);
                throw;
            }
        }
        #endregion
        #region Verify(TransmissionPayload payload)
        /// <summary>
        /// This method validates the incoming payload and Jwt token and sets the SecurityPrincipal 
        /// from teh token.
        /// </summary>
        /// <param name="payload"></param>
        public override void Verify(TransmissionPayload payload)
        {
            try
            {
                var token = TokenValidate(payload);
                //Does the token audience match.
                if (token.Claims.Audience != mAudience)
                    throw new TokenInvalidAudienceException(token.Claims.Audience, mAudience);
                //Does the payload destination match the claims.
                if (token.Claims.JWTId != payload.Message.OriginatorKey)
                    throw new TokenInvalidInformationException(token.Claims.JWTId, "twtid"
                        , token.Claims.JWTId, payload.Message.OriginatorKey);
                //Has the token expired.
                if (token.Claims.ExpirationTime.HasValue && token.Claims.ExpirationTime.Value < DateTime.UtcNow)
                    throw new TokenExpiredException(token.Claims.JWTId);
                //Does the destiantion match the token.
                if (!token.Claims.Exists(ClaimDestination)
                    || (string)token.Claims[ClaimDestination] != payload.Message.ToKey())
                    throw new TokenInvalidInformationException(token.Claims.JWTId, ClaimDestination
                        , token.Claims.Exists(ClaimDestination) ? (string)token.Claims[ClaimDestination] : "", payload.Message.ToKey());

                //Create the principal, which will be passed through the Microservice.
                payload.SecurityPrincipal = new MicroserviceSecurityPrincipal(token);
            }
            catch (Exception ex)
            {
                Collector?.SecurityEvent(SecurityEventDirection.Verification, ex);
                throw;
            }
        }
        #endregion

        #region TokenGenerate(TransmissionPayload payload)
        /// <summary>
        /// This method generates a Jwt token from the payload and it associated security principal and Microservice metadata.
        /// </summary>
        /// <param name="payload">The payload to sign.</param>
        /// <returns>The corresponsing token</returns>
        protected virtual JwtToken TokenGenerate(TransmissionPayload payload)
        {
            JwtToken token = new JwtToken(mAlgorithm);

            token.Claims.Audience = mAudience;
            token.Claims.Issuer = OriginatorId.ExternalServiceId;
            token.Claims.IssuedAt = DateTime.UtcNow;
            token.Claims.JWTId = payload.Message.OriginatorKey;

            token.Claims.Add(ClaimPayloadId, payload.Id.ToString("N").ToUpperInvariant());
            token.Claims.Add(ClaimServiceVersion, OriginatorId.ServiceVersionId);
            token.Claims.Add(ClaimServiceEngineVersion, OriginatorId.ServiceEngineVersionId);

            var correl = payload.Message.ProcessCorrelationKey;
            if (correl != null)
                token.Claims.Add(ClaimProcessCorrelationKey, correl);

            token.Claims.Add(ClaimDestination, payload.Message.ToKey());

            IIdentity identity = payload.SecurityPrincipal?.Identity;
            if (identity != null)
            {
                token.Claims.Add(ClaimTypes.Authentication, identity.IsAuthenticated ? "true" : "false");
                token.Claims.Add(ClaimTypes.Role, "Default");

                if (identity.Name != null)
                    token.Claims.Add(ClaimTypes.Name, identity.Name);

                if (identity.IsAuthenticated && identity.AuthenticationType != null)
                    token.Claims.Add(ClaimTypes.AuthenticationMethod, identity.AuthenticationType);
            }

            return token;
        } 
        #endregion
        #region TokenValidate(TransmissionPayload payload)
        /// <summary>
        /// This method validated the incoming signature.
        /// </summary>
        /// <param name="payload">The payload containing the signature.</param>
        /// <returns></returns>
        protected virtual JwtToken TokenValidate(TransmissionPayload payload)
        {
            var tokensig = payload.Message.SecuritySignature;
            if (string.IsNullOrEmpty(tokensig))
                throw new IncomingPayloadTokenSignatureNotPresentException();

            JwtToken token = new JwtToken(tokensig, mSecret, true, false);

            return token;
        } 
        #endregion
    }
}
