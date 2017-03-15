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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// The Rate limiter class is used to track the error rate from a managed resource and 
    /// </summary>
    [DebuggerDisplay("ResourceRateLimiter: {Name}={RateLimitAdjustmentPercentage} [{Debug}]")]
    public class ResourceRateLimiter: ResourceBase, IResourceRequestRateLimiter
    {
        #region Declarations
        List<ResourceStatistics> mProfiles;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="name">The name of the rate limiter.</param>
        /// <param name="profiles">The prfiles that are tracker by this limiter.</param>
        public ResourceRateLimiter(string name, List<ResourceStatistics> profiles)
        {
            Name = name;
            mProfiles = profiles;
        } 
        #endregion

        #region RateLimitAdjustmentPercentage
        /// <summary>
        /// This is the current rate limit summation across the active payload requests.
        /// If rate limiting is not supported the value will be 1.
        /// Otherwise, the profiles are scanned and the lowest percentage is returned.
        /// </summary>
        public double RateLimitAdjustmentPercentage
        {
            get
            {
                if (mProfiles == null || mProfiles.Count == 0)
                    return 1;//No adjustment = 100%

                return mProfiles.Select((e) => e.RateLimitAdjustmentPercentage).Min();
            }
        }
        #endregion

        #region Debug
        /// <summary>
        /// This property is used for debug purposes.
        /// </summary>
        public string Debug
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
