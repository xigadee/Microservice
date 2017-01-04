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
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// Ti
    /// </summary>
    public class JWTHolderRaw
    {
        /// <summary>
        /// This constructor parses the incoming JSON token in to its main parts.
        /// </summary>
        /// <param name="token">The string token.</param>
        /// <exception cref="Xigadee.InvalidJWTTokenStructureException">This exception is thrown if the token does not have at least one period (.) character.</exception>
        public JWTHolderRaw(string token)
        {
            if (token == null)
                return;

            var split = token.Split('.');
            if (split.Length < 2)
                throw new InvalidJWTTokenStructureException();

            Raw = split.Select((s) => Safeb64UrlDecode(s)).ToList();

            JoseHeader = JSONConvert(Raw[0]);
        }

        /// <summary>
        /// This is the list of raw binary parameters.
        /// </summary>
        public List<byte[]> Raw { get; }

        /// <summary>
        /// This is the raw JSON string containing the JOSE Header.
        /// </summary>
        public string JoseHeader { get; set; }

        public static string Safeb64UrlEncode(byte[] arg)
        {
            string s = Convert.ToBase64String(arg); // Standard base64 encoder
            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding
            return s;
        }

        static byte[] Safeb64UrlDecode(string arg)
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

        protected string JSONConvert(byte[] raw)
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

        public string ToJWSCompactSerialization()
        {
            StringBuilder sb = new StringBuilder();

            Raw.ForIndex((i,s) =>
            {
                if (i>0)
                    sb.Append('.');
                sb.Append(Safeb64UrlEncode(s));
                });

            return sb.ToString();
        }
    }
}
