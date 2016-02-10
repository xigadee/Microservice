using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public class PersistenceStatistics: MessageInitiatorStatistics
    {
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }

        #region ErrorIncrement()
        /// <summary>
        /// This method is used to increment the active and total record count.
        /// </summary>
        public virtual void RetryIncrement()
        {
            Interlocked.Increment(ref mRetries);
        }
        #endregion

        public long Retries {get { return mRetries; } }

        private long mRetries;
    }
}
