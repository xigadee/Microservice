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

namespace Xigadee
{
    public class JwtClaims: ClaimsSet
    {
        #region Registered claims
        public const string HeaderIssuer = "iss";
        public const string HeaderSubject = "sub";
        public const string HeaderAudience = "aud";
        public const string HeaderJWTID = "jti";

        public const string HeaderExpirationTime = "exp";
        public const string HeaderNotBefore = "nbf";
        public const string HeaderIssuedAt = "iat";
        #endregion

        public JwtClaims():base()
        {

        }
        /// <summary>
        /// This override provides specific claims for the JWTClaims.
        /// </summary>
        /// <param name="json"></param>
        public JwtClaims(string json) : base(json)
        {
        }



        public string JWTId { get { return GetClaim<string>(HeaderJWTID);} set { base[HeaderJWTID] = value; } }

        public string Issuer { get { return GetClaim<string>(HeaderIssuer); } set { base[HeaderIssuer] = value; } }

        public string Subject { get { return GetClaim<string>(HeaderSubject); } set { base[HeaderSubject] = value; } }

        public string Audience { get { return GetClaim<string>(HeaderAudience); } set { base[HeaderAudience] = value; } }

        public DateTime? ExpirationTime { get { return JwtHelper.FromNumericDate(GetClaim<long?>(HeaderExpirationTime)); } set { base[HeaderExpirationTime] = JwtHelper.ToNumericDate(value); } }
        public DateTime? NotBefore { get { return JwtHelper.FromNumericDate(GetClaim<long?>(HeaderNotBefore)); } set { base[HeaderNotBefore] = JwtHelper.ToNumericDate(value); } }
        public DateTime? IssuedAt { get { return JwtHelper.FromNumericDate(GetClaim<long?>(HeaderIssuedAt)); } set { base[HeaderIssuedAt] = JwtHelper.ToNumericDate(value); } }

    }
}
