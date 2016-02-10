using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    [DebuggerDisplay("ResourceRateLimiter: {Name}={RateLimitAdjustmentPercentage} [{Debug}]")]
    public class ResourceRateLimiter: ResourceBase, IResourceRequestRateLimiter
    {
        #region Declarations
        List<ResourceStatistics> mProfiles;

        #endregion

        public ResourceRateLimiter(string name, List<ResourceStatistics> profiles)
        {
            mName = name;
            mProfiles = profiles;
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
                if (mProfiles == null || mProfiles.Count == 0)
                    return 1;//No adjustment = 100%

                return mProfiles.Select((e) => e.RateLimitAdjustmentPercentage).Min();
            }
        }
        #endregion

        public string Debug
        {
            get
            {
                string concatRates = string.Concat(mProfiles.Select((e) => string.Format("{0}-{1} ", e.Name, e.RateLimitAdjustmentPercentage)));
                return string.Format("{0} ({1}): {2}", mName, RateLimitAdjustmentPercentage, concatRates);
            }
        }     
    }
}
