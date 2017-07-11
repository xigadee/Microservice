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

namespace Xigadee
{
    /// <summary>
    /// This enumeration contains the state of a resource currently being managed.
    /// </summary>
    [Flags()]
    public enum CircuitBreakerState: int
    {
        /// <summary>
        /// Any requests fail immediately as the service has reached the threshold failure.
        /// </summary>
        Open = 1,
        /// <summary>
        /// All is good. The service is operating as normal.
        /// </summary>
        Closed = 2,
        /// <summary>
        /// The service is tentatively accepting requests on a ratio. If the calls succeed the circuit breaker will close, 
        /// otherwise the breaker will reopen. This is used during a retry state after the breaker has opened. It is used to ensure 
        /// we do not inundate the underlying service with too many calls when we restart.
        /// </summary>
        HalfOpen = 3
    }
}
