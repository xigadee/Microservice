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
    [DebuggerDisplay("ResourceConsumer: {Name}={RateLimitAdjustmentPercentage}")]
    public class ResourceConsumer: ResourceBase, IResourceConsumer
    {
        protected readonly ResourceStatistics mResource;

        public ResourceConsumer(ResourceStatistics resource, string name)
        {
            mName = name;
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

        public Guid Start(string group, Guid requestId)
        {
            return mResource.Start(mName, group, requestId);
        }

        public void End(Guid profileId, int start, ResourceRequestResult result)
        {
            mResource.End(mName, profileId, start, result);
        }

        public void Retry(Guid profileId, int retryStart, ResourceRetryReason reason)
        {
            mResource.Retry(mName, profileId, retryStart, reason);
        }

        public void Exception(Guid profileId, int retryStart)
        {
            Retry(profileId, retryStart, ResourceRetryReason.Exception);
        }
    }
}
