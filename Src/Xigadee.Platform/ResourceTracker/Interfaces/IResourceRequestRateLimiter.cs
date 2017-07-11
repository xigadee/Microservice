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
    /// <summary>
    /// This interface is implemented by Rate Limiters.
    /// </summary>
    public interface IResourceRequestRateLimiter: IResourceBase
    {
        /// <summary>
        /// This is the rate that the request process should adjust as a ratio 1 is the default without limiting.
        /// 0 is when the circuit breaker is active.
        /// </summary>
        double RateLimitAdjustmentPercentage { get; }
    }
}
