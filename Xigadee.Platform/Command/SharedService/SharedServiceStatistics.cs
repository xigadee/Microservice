#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the statist
    /// </summary>
    public class SharedServiceStatistics: StatusBase
    {
        public List<ServiceHolderStatistics> Services { get; set; }
    }
}
