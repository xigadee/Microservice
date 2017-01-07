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

namespace Xigadee
{
    /// <summary>
    /// This clas holds the raw Jwt token.
    /// </summary>
    public class JwtRoot
    {
        /// <summary>
        /// This constructor parses the incoming JSON token in to its main parts.
        /// </summary>
        /// <param name="token">The string token.</param>
        /// <exception cref="Xigadee.InvalidJwtTokenStructureException">This exception is thrown if the token does not have at least one period (.) character.</exception>
        public JwtRoot(string token)
        {
            if (token == null)
                return;

            var split = token.Split('.');
            if (split.Length < 2)
                throw new InvalidJwtTokenStructureException();

            var dict = new Dictionary<int, byte[]>();

            for (int i = 0; i < split.Length; i++)
            {
                dict.Add(i, SafeBase64UrlDecode(split[i]));
            }

            Raw = dict;

            JoseHeader = JSONConvert(Raw[0]);
        }

        /// <summary>
        /// This is the list of raw binary token parameters.
        /// </summary>
        public Dictionary<int,byte[]> Raw { get; }

        #region RawOrdered()
        /// <summary>
        /// This method can be used to return the serializable pieces in their correct order.
        /// </summary>
        /// <returns>This is a selection of values in the correct order.</returns>
        protected virtual IEnumerable<byte[]> RawOrdered()
        {
            return Raw
                .OrderBy((k) => k.Key)
                .Select((v) => v.Value);
        } 
        #endregion

        /// <summary>
        /// This is the raw JSON string containing the JOSE Header.
        /// </summary>
        public string JoseHeader { get; set; }

        #region SafeBase64...
        /// <summary>
        /// This method encodes the byte array in to a url safe Base64 string.
        /// </summary>
        /// <param name="arg">The byte array to convert.</param>
        /// <returns></returns>
        public static string SafeBase64UrlEncode(byte[] arg)
        {
            string s = Convert.ToBase64String(arg); // Standard base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }
        /// <summary>
        /// This method recodes the Base64 string in to the correct format.
        /// </summary>
        /// <param name="arg">The incoming string.</param>
        /// <returns>Returns the byte array</returns>
        public static byte[] SafeBase64UrlDecode(string arg)
        {
            string s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default:
                    throw new System.Exception("Illegal base64url string!");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        } 
        #endregion
        #region JSONConvert(byte[] raw)
        /// <summary>
        /// This method converts the binary UTF8 data to a string.
        /// </summary>
        /// <param name="raw">The UTF8 byte array.</param>
        /// <returns>returns the string formatted data.</returns>
        public static string JSONConvert(byte[] raw)
        {
            try
            {
                return Encoding.UTF8.GetString(raw);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion

        #region ToJWSCompactSerialization()
        /// <summary>
        /// This method outputs the necessary pieces in the correct order.
        /// </summary>
        /// <returns></returns>
        public string ToJWSCompactSerialization()
        {
            StringBuilder sb = new StringBuilder();

            RawOrdered()
                .ForIndex((i, s) =>
            {
                if (i > 0)
                    sb.Append('.');
                sb.Append(SafeBase64UrlEncode(s));
            });

            return sb.ToString();
        }
        #endregion

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

                signature = JwtRoot.SafeBase64UrlEncode(sha256Hash);
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

    //https://medium.facilelogin.com/jwt-jws-and-jwe-for-not-so-dummies-b63310d201a3#.fmgvwdaz9
}
