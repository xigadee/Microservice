#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public class ServiceHolderStatistics: StatusBase
    {
        public long References;

        #region Increment(int delta)
        /// <summary>
        /// This method increments the batchn with the message delta.
        /// </summary>
        public void Increment()
        {
            Interlocked.Increment(ref References);
        }
        #endregion
    }
}
