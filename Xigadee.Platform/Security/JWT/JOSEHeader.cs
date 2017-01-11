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
using System.Threading.Tasks;

namespace Xigadee
{
    public class JOSEHeader:ClaimsSet
    {
        public const string HeaderType = "typ";
        public const string HeaderAlgorithm = "alg";
        public const string HeaderContentType = "cty";
        public const string HeaderJSONWebKey = "jwk";
        public const string HeaderKeyID = "kid";
        public const string HeaderJWKSetURL = "jku";
        public const string HeaderCritical = "crit";

        public JOSEHeader():base()
        {
        }

        public JOSEHeader(string json) : base(json)
        {
            
        }

        public JwtHashAlgorithm SupportedAlgorithm
        {
            get
            {
                return JwtHelper.ConvertToJwtHashAlgorithm((string)this[HeaderAlgorithm]);
            }
            set
            {
                this[HeaderAlgorithm] = Enum.GetName(typeof(JwtHashAlgorithm), value);
            }
        }

        public string Type { get { return GetClaim<string>(HeaderType); } set { base[HeaderType] = value; } }
        public string Algorithm { get { return GetClaim<string>(HeaderAlgorithm); } set { base[HeaderAlgorithm] = value; } }
        public string ContentType { get { return GetClaim<string>(HeaderContentType); } set { base[HeaderContentType] = value; } }
        public string JSONWebKey { get { return GetClaim<string>(HeaderJSONWebKey); } set { base[HeaderJSONWebKey] = value; } }
        public string KeyID { get { return GetClaim<string>(HeaderKeyID); } set { base[HeaderKeyID] = value; } }
        public string JWKSetURL { get { return GetClaim<string>(HeaderJWKSetURL); } set { base[HeaderJWKSetURL] = value; } }
        public string Critical { get { return GetClaim<string>(HeaderCritical); } set { base[HeaderCritical] = value; } }

    }
}
