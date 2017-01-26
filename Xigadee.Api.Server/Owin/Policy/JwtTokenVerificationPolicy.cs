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
using System.Threading.Tasks;
using System.IdentityModel.Claims;
using System.Security.Claims;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace Xigadee
{
    /// <summary>
    /// This is the token verification policy.
    /// </summary>
    public class JwtTokenVerificationPolicy: IJwtTokenVerificationPolicy
    {
        /// <summary>
        /// The token secret used to hash the token.
        /// </summary>
        public byte[] Secret { get; set; }
        /// <summary>
        /// The supported algorithm type
        /// </summary>
        public JwtHashAlgorithm Algorithm { get; set; }
        /// <summary>
        /// The token audience. Set this to null if you do not require it to check the audience value.
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// This property indicates whether the token audience should be validated. The default is true.
        /// </summary>
        public bool ValidateAudience { get; set; } = true;
        /// <summary>
        /// This property indicates whether the token expiry date should be checked. The default is true.
        /// </summary>
        public bool ValidateExpiry { get; set; } = true;
        /// <summary>
        /// This property indicates whether the token valid from date should be checked. The default is true.
        /// </summary>
        public bool ValidateNotBefore { get; set; } = true;
        /// <summary>
        /// This property specifies whether the filter should deny access by default.
        /// </summary>
        public bool DenyByDefault { get; set; } = true;
        /// <summary>
        /// This method is used to validate the token.
        /// </summary>
        /// <param name="tokenParameter">The token auth string parameter.</param>
        /// <returns>Returns a Jwt token if the validation is passed.</returns>
        public virtual JwtToken Validate(string tokenParameter)
        {
            var token = new JwtToken(tokenParameter, Secret);

            if (ValidateAudience &&
                !token.Claims.Audience.Equals(Audience, StringComparison.InvariantCultureIgnoreCase))
                throw new WebApiJwtFilterValidationException(JwtClaims.HeaderAudience);

            if (ValidateExpiry &&
                (!token.Claims.ExpirationTime.HasValue || token.Claims.ExpirationTime.Value < DateTime.UtcNow))
                throw new WebApiJwtFilterValidationException(JwtClaims.HeaderExpirationTime);

            if (ValidateNotBefore &&
                (!token.Claims.NotBefore.HasValue || token.Claims.NotBefore.Value >= DateTime.UtcNow))
                throw new WebApiJwtFilterValidationException(JwtClaims.HeaderNotBefore);

            return token;
        }
    }
}
