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
using System.Diagnostics;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// The Rate limiter class is used to track the error rate from a managed resource 
    /// </summary>
    [DebuggerDisplay("ResourceRateLimiter: {Name}={RateLimitAdjustmentPercentage} [{Debug}]")]
    public class ResourceRateLimiter: ResourceBase, IResourceRequestRateLimiter
    {
        #region Declarations
        /// <summary>
        /// This is the list of registered profiles.
        /// </summary>
        protected readonly List<ResourceStatistics> mProfiles;
        /// <summary>
        /// This is the function used to calculate the rate.
        /// </summary>
        protected readonly Func<List<ResourceStatistics>, double> mFnCalculateRate;
        /// <summary>
        /// This is the function used to calculate the rate.
        /// </summary>
        protected readonly Func<List<ResourceStatistics>, CircuitBreakerState> mFnCalculateCircuitBreaker;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="name">The name of the rate limiter.</param>
        /// <param name="profiles">The prfiles that are tracker by this limiter.</param>
        /// <param name="calculateRate">This is the optional function used to calculate the overall rate.</param>
        /// <param name="calculateCircuitBreaker">This function is used to throw when the circuit breaker should be thrown.</param>
        public ResourceRateLimiter(string name, List<ResourceStatistics> profiles
            , Func<List<ResourceStatistics>, double> calculateRate = null
            , Func<List<ResourceStatistics>, CircuitBreakerState> calculateCircuitBreaker = null

            )
        {
            Name = name;
            mProfiles = profiles;

            mFnCalculateRate = calculateRate ?? (
                (p) =>
                {
                    if (p == null || p.Count == 0)
                        return 1D;//No adjustment = 100%

                    //Default to the minimum rate of all the registered resources.
                    return p.Select((e) => e.RateLimitAdjustmentPercentage).Min();
                }
            );

            mFnCalculateCircuitBreaker = calculateCircuitBreaker ?? (
                (List<ResourceStatistics> r) =>
                {
                    if (r == null || r.Count == 0)
                        return CircuitBreakerState.Closed;

                    //Default to the minimum rate of all the registered resources.
                    return r.FirstOrDefault((v) => v.RateLimitAdjustmentPercentage == 0) == null ? CircuitBreakerState.Closed : CircuitBreakerState.Open;
                }
            );

        }
        #endregion

        #region RateLimitAdjustmentPercentage
        /// <summary>
        /// This is the calculated rate limit summation across the active payload requests.
        /// If rate limiting is not supported the value will be 1.
        /// Otherwise, the profiles are scanned and the lowest percentage is returned.
        /// </summary>
        public virtual double RateLimitAdjustmentPercentage
        {
            get
            {
                return mFnCalculateRate(mProfiles);
            }
        }
        #endregion

        #region Debug
        /// <summary>
        /// This property is used for debug purposes.
        /// </summary>
        public virtual string Debug
        {
            get
            {
                string concatRates = string.Concat(mProfiles.Select((e) => string.Format("{0}-{1} ", e.Name, e.RateLimitAdjustmentPercentage)));
                return string.Format("{0} ({1}): {2}", Name, RateLimitAdjustmentPercentage, concatRates);
            }
        }   
        #endregion
    }
}
