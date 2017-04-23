using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface IResourceRequestRateLimiter: IResourceBase
    {
        /// <summary>
        /// This is the rate that the request process should adjust as a ratio 1 is the default without limiting.
        /// 0 is when the circuit breaker.
        /// </summary>
        double RateLimitAdjustmentPercentage { get; }

    }
}
