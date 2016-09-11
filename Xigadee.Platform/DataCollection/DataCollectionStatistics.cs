using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base data collection statistics.
    /// </summary>
    public class DataCollectionStatistics: StatusBase, ICollectionStatistics
    {
        public int ItemCount
        {
            get;set;
        }
    }
}
