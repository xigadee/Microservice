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
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class JwtTokenAuthenticationHandler: AuthenticationHandlerBase
    {
        readonly byte[] mSecret;
        readonly JwtHashAlgorithm mAlgorithm;
        readonly string mAudience;

        public JwtTokenAuthenticationHandler(JwtHashAlgorithm algo, string base64Secret, string audience = "comms")
        {
            mAlgorithm = algo;
            mSecret = Convert.FromBase64String(base64Secret);
            mAudience = audience;
        }

        public JwtTokenAuthenticationHandler(JwtHashAlgorithm algo, byte[] secret, string audience = "comms")
        {
            mAlgorithm = algo;
            mSecret = secret;
            mAudience = audience;
        }

        public override void Sign(TransmissionPayload payload)
        {
            var token = TokenGenerate(payload);

            payload.Message.SecuritySignature = token.ToString(mSecret);
        }

        public override void Verify(TransmissionPayload payload)
        {
            var token = TokenValidate(payload);

        }

        protected virtual JwtToken TokenGenerate(TransmissionPayload payload)
        {
            JwtToken token = new JwtToken(mAlgorithm);

            token.Claims.Issuer = OriginatorId.ExternalServiceId;
            token.Claims.Audience = mAudience;
            token.Claims.IssuedAt = DateTime.UtcNow;
            token.Claims.JWTId = payload.Message.OriginatorKey;


            return token;
        }


        protected virtual JwtToken TokenValidate(TransmissionPayload payload)
        {
            JwtToken token = new JwtToken(payload.Message.SecuritySignature, mSecret);

            payload.SecurityPrincipal = GenerateClaimsPrincipal(token);

            return token;
        }

        protected virtual ClaimsPrincipal GenerateClaimsPrincipal(JwtToken token)
        {
            var principal = new ClaimsPrincipal();

            return principal;
        }
    }
}
