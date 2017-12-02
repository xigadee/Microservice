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

#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This enumeration contains the standard HTTP response codes that is provided to 
    /// keep consistency between the various persistence implementations. 
    /// </summary>
    public enum PersistenceResponse:int
    {
        /// <summary>
        /// The OK 200 response.
        /// </summary>
        Ok200 = 200,
        /// <summary>
        /// The created 201 response.
        /// </summary>
        Created201 = 201,
        /// <summary>
        /// The accepted 202 response.
        /// </summary>
        Accepted202 = 202,
        /// <summary>
        /// The bad request 400 response.
        /// </summary>
        BadRequest400 = 400,
        /// <summary>
        /// The not found 404 response.
        /// </summary>
        NotFound404 = 404,
        /// <summary>
        /// The request timeout 408 response.
        /// </summary>
        RequestTimeout408 = 408,
        /// <summary>
        /// The conflict 409 response.
        /// </summary>
        Conflict409 = 409,
        /// <summary>
        /// The precondition failed 412 response.
        /// </summary>
        PreconditionFailed412 = 412,
        /// <summary>
        /// The too many requests 429 response.
        /// </summary>
        TooManyRequests429 = 429,

        /// <summary>
        /// The unknown error 500 response.
        /// </summary>
        UnknownError500 = 500,
        /// <summary>
        /// The not implemented 501 response.
        /// </summary>
        NotImplemented501 = 501,
        /// <summary>
        /// The service unavailable 503 response.
        /// </summary>
        ServiceUnavailable503 = 503,
        /// <summary>
        /// The gateway timeout 504 response.
        /// </summary>
        GatewayTimeout504 = 504
    }
}
