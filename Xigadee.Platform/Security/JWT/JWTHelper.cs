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

    public class JWTHolderBase: JWTHolderRaw
    {
        public JWTHolderBase(string encoding):base(encoding)
        {
            JWTPayload = JSONConvert(Raw[1]);

            if (Raw.Count > 2)
                JWSSignature = JSONConvert(Raw[2]);
        }


        /// <summary>
        /// This is the raw JSON string containing the claims set.
        /// </summary>
        public string JWTPayload { get; set; }
        /// <summary>
        /// This is the raw JSON string containing the claims set.
        /// </summary>
        public string JWSSignature { get; set; }
    }

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
            string b64joseHeader = SafeBase64UrlEncode(Encoding.UTF8.GetBytes(JoseHeader));
            string b64jwtClaimsSet = SafeBase64UrlEncode(Encoding.UTF8.GetBytes(JWTPayload));

            return $"{b64joseHeader}.{b64jwtClaimsSet}.{CalculateAuthSignature(algo, key, b64joseHeader, b64jwtClaimsSet)}";
        }

        #region CalculateAuthSignature(JWTHashAlgorithm algo, byte[] key, string joseHeader, string jwtClaimsSet)
        /// <summary>
        /// This method creates the necessary signature based on the header and claims passed.
        /// </summary>
        /// <param name="algo">The hash algorithm.</param>
        /// <param name="key">The hash key.</param>
        /// <param name="joseHeader">The base64 encoded header.</param>
        /// <param name="jwtClaimsSet">The base64 encoded claims set.</param>
        /// <returns>Returns the Base64 encoded header.</returns>
        public static string CalculateAuthSignature(JWTHashAlgorithm algo, byte[] key, string joseHeader, string jwtClaimsSet)
        {
            //Thanks to https://jwt.io/
            string sig = $"{joseHeader}.{jwtClaimsSet}";

            byte[] bySig = Encoding.UTF8.GetBytes(sig);

            string signature;
            using (var hashstring = GetAlgorithm(algo, key))
            {
                byte[] sha256Hash = hashstring.ComputeHash(bySig);

                signature = SafeBase64UrlEncode(sha256Hash);
            }

            return signature;
        }
        #endregion
        #region GetAlgorithm(JWTHashAlgorithm type, byte[] key)
        /// <summary>
        /// This method returns the relevant hash algorithm based on the enum type.
        /// </summary>
        /// <param name="type">The supported algorithm enum.</param>
        /// <param name="key">The hash key,</param>
        /// <returns>Returns the relevant algorithm.</returns>
        public static HMAC GetAlgorithm(JWTHashAlgorithm type, byte[] key)
        {
            switch (type)
            {
                case JWTHashAlgorithm.HS256:
                    return new HMACSHA256(key);
                case JWTHashAlgorithm.HS384:
                    return new HMACSHA384(key);
                case JWTHashAlgorithm.HS512:
                    return new HMACSHA512(key);
                default:
                    throw new AlgorithmNotSupportedException(type.ToString());
            }
        } 
        #endregion
    }
}
