using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class measures the statistics for the resource container.
    /// </summary>
    public class ResourceContainerStatistics : StatusBase
    {
        /// <summary>
        /// This is the resource collections.
        /// </summary>
        public ResourceStatistics[] Resources { get; set; }
        /// <summary>
        /// This is the collection of rate limiters
        /// </summary>
        public string[] RateLimiters { get; set; }
    }
}
