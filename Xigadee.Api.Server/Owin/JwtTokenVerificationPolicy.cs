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
    public class JwtTokenVerificationPolicy
    {
        /// <summary>
        /// The token secret
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

        public bool ValidateAudience { get; set; } = true;

        public bool ValidateExpiry { get; set; } = true;

        public bool ValidateNotBefore { get; set; } = true;

    }
}
