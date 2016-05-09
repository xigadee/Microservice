using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ResourceTrackerStatistics: StatusBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ResourceStatistics[] Resources { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] RateLimiters { get; set; }
    }
}
