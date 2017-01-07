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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Xigadee
{

    //Returns a numeric value representing the number of seconds from 1970-01-01T0:0:0Z UTC until the given UTC date/time


    public class JWTHolder: JWTHolderBase
    {
        public JWTHolder(JWTHashAlgorithm algo):base(null)
        {
            HashAlgorithm = algo;

            var obj = new JObject();
            obj["typ"] = "JWT";
            obj["alg"] = Enum.GetName(typeof(JWTHashAlgorithm), algo);
            JoseHeader = obj.ToString();
        }
    

        public JWTHolder(string incoming):base(incoming)
        {

        }

        /// <summary>
        /// The algorithm type used, by default HS256.
        /// </summary>
        public JWTHashAlgorithm HashAlgorithm { get; } = JWTHashAlgorithm.HS256;

        public static JWTHashAlgorithm ConvertToJWTHashAlgorithm(string algorithm)
        {
            JWTHashAlgorithm outAlg;
            if (Enum.TryParse(algorithm, false, out outAlg))
                return outAlg;

            throw new NotSupportedException();
        }


        public string ToString(byte[] key)
        {
            return ToString(HashAlgorithm, key);
        }

        public string ToString(JWTHashAlgorithm algo, byte[] key)
        {
            string b64joseHeader = JwtRoot.SafeBase64UrlEncode(Encoding.UTF8.GetBytes(JoseHeader));
            string b64jwtClaimsSet = JwtRoot.SafeBase64UrlEncode(Encoding.UTF8.GetBytes(JWTPayload));

            return $"{b64joseHeader}.{b64jwtClaimsSet}.{JwtRoot.CalculateAuthSignature(algo, key, b64joseHeader, b64jwtClaimsSet)}";
        }

    }
}
