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
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class holds the statistics for a particular resource.
    /// </summary>
    [DebuggerDisplay("ResourceConsumer: {Name}={RateLimitAdjustmentPercentage}")]
    public class ResourceConsumer: ResourceBase, IResourceConsumer
    {
        protected readonly ResourceStatistics mResource;

        public ResourceConsumer(ResourceStatistics resource, string name)
        {
            Name = name;
            mResource = resource;
        }

        #region RateLimitAdjustmentPercentage
        /// <summary>
        /// This is the current rate limit summation across the active payload requests.
        /// If rate limiting is not supported the value will be null.
        /// </summary>
        public double RateLimitAdjustmentPercentage
        {
            get
            {
                //Adjust the percentage so that as the ratio reduces the adjustment decreases.
                return mResource.RateLimitAdjustmentPercentage;
            }
        }
        #endregion

        /// <summary>
        /// This method tracks the start of a request and returns a profileid.
        /// </summary>
        /// <param name="group">The resource group.</param>
        /// <param name="requestId">The request id.</param>
        /// <returns>Returns the profile id.</returns>
        public Guid Start(string group, Guid requestId)
        {
            return mResource.Start(Name, group, requestId);
        }
        /// <summary>
        /// This method should be called when a request ends.
        /// </summary>
        /// <param name="profileId">The profileid.</param>
        /// <param name="start">The tick count start time.</param>
        /// <param name="result">The result.</param>
        public void End(Guid profileId, int start, ResourceRequestResult result)
        {
            mResource.End(Name, profileId, start, result);
        }
        /// <summary>
        /// This method should be called when a request to a resource needs to be retried.
        /// </summary>
        /// <param name="profileId">The profile id.</param>
        /// <param name="retryStart">The tick count for the retry start time.</param>
        /// <param name="reason">The retry reason.</param>
        public void Retry(Guid profileId, int retryStart, ResourceRetryReason reason)
        {
            mResource.Retry(Name, profileId, retryStart, reason);
        }
        /// <summary>
        /// This method is called when an request cannot be completed due to an exception.
        /// </summary>
        /// <param name="profileId">The profile id.</param>
        /// <param name="retryStart">The retry start time.</param>
        public void Exception(Guid profileId, int retryStart)
        {
            Retry(profileId, retryStart, ResourceRetryReason.Exception);
        }
    }
}
