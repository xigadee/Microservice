using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ResourceTrackerStatistics: StatusBase
    {

        public ResourceStatistics[] Resources { get; set; }

        public string[] RateLimiters { get; set; }
    }
}
