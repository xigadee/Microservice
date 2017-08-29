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
using System.Linq;
using System.Security.Claims;

namespace Xigadee
{
    /// <summary>
    /// This method holds the security information for the current request with the Microservice.
    /// These claims can be set from the remote party.
    /// </summary>
    public class MicroserviceSecurityPrincipal:ClaimsPrincipal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceSecurityPrincipal"/> class.
        /// </summary>
        public MicroserviceSecurityPrincipal()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceSecurityPrincipal"/> class.
        /// </summary>
        /// <param name="incoming">The incoming token.</param>
        public MicroserviceSecurityPrincipal(JwtToken incoming):base(new MicroserviceIdentity(incoming))
        {

        }
    }

    /// <summary>
    /// This class holds the identity of the party specified in the token.
    /// </summary>
    /// <seealso cref="System.Security.Claims.ClaimsIdentity" />
    public class MicroserviceIdentity: ClaimsIdentity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceIdentity"/> class.
        /// </summary>
        /// <param name="incoming">The incoming token that holds the claims.</param>
        public MicroserviceIdentity(JwtToken incoming)
        {
            incoming.Claims
                .Where((c) => c.Value is string)
                .ForEach((c) => AddClaim(new Claim(c.Key, c.Value as string)));
        }
    }
}
