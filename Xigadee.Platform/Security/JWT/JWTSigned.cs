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
    /// This is the signed JWT Token.
    /// </summary>
    public class JWTSigned:JWTHolderRaw
    {
        public JWTSigned(string encoding) : base(encoding)
        {
        }

        /// <summary>
        /// This is the raw JSON string containing the claims set.
        /// </summary>
        public string JWTPayload { get; set; }
        /// <summary>
        /// This is the raw JSON string containing the claims set.
        /// </summary>
        public string JWSSignature { get; set; }

        public bool Validate(byte[] key)
        {
            return false;
        }
    }
}
