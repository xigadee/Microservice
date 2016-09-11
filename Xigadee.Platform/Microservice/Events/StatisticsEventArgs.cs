using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to signal statistics events.
    /// </summary>
    public class StatisticsEventArgs: MicroserviceEventArgs
    {
        /// <summary>
        /// The new statistics.
        /// </summary>
        public MicroserviceStatistics Statistics { get; set; }
    }
}
