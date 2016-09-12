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
        Ok200 = 200,
        Created201 = 201,
        Accepted202 = 202,

        BadRequest400 = 400,
        NotFound404 = 404,
        RequestTimeout408 = 408,
        Conflict409 = 409,
        PreconditionFailed412 = 412,
        TooManyRequests429 = 429,

        UnknownError500 = 500,
        NotImplemented501 = 501,
        ServiceUnavailable503 = 503,
        GatewayTimeout504 = 504
    }
}
