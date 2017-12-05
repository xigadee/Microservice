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
    /// <summary>
    /// Statistics holder for the default service holder.
    /// </summary>
    /// <seealso cref="Xigadee.StatusBase" />
    public class ServiceHolderStatistics: StatusBase
    {
        /// <summary>
        /// The reference count;
        /// </summary>
        public long References;

        #region Increment(int delta)
        /// <summary>
        /// This method increments the batch with the message delta.
        /// </summary>
        public void Increment()
        {
            Interlocked.Increment(ref References);
        }
        #endregion
    }
}
