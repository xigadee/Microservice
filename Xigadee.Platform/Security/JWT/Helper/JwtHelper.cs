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
    public static class JwtHelper
    {
        public static readonly DateTime EpochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Case insensitive mapping to the supported algorithm mappings.
        /// </summary>
        /// <param name="algorithm">The string algorithm.</param>
        /// <returns></returns>
        public static JwtHashAlgorithm ConvertToJwtHashAlgorithm(string algorithm)
        {
            JwtHashAlgorithm outAlg;
            if (Enum.TryParse(algorithm, true, out outAlg))
                return outAlg;

            throw new JwtAlgorithmNotSupportedException(algorithm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime? FromNumericDate(long? time)
        {
            if (!time.HasValue)
                return null;

            return EpochDate.AddSeconds(time.Value);
        }


        public static long? ToNumericDate(DateTime? time)
        {
            if (!time.HasValue)
                return null;

            TimeSpan span = time.Value - EpochDate;

            return (long)span.TotalSeconds;
        }

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
    }
}
