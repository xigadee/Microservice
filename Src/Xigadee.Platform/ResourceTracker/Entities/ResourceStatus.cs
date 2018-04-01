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

namespace Xigadee
{
    /// <summary>
    /// This class contains a brief summary the resource status
    /// </summary>
    public class ResourceStatus
    {
        /// <summary>
        /// This is the name of the resource.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// This is the current circuit breaker state.
        /// </summary>
        public CircuitBreakerState State { get; set; } = CircuitBreakerState.Closed;
        /// <summary>
        /// This is the time to the next pool retry if the circuit breaker is open.
        /// </summary>
        public int? RetryInSeconds { get; set; }
        /// <summary>
        /// This is the percentage of requests that should pass through, if the circuit breaker is in a half-open state.
        /// </summary>
        public int FilterPercentage { get; set; } = 100;
    }
}
