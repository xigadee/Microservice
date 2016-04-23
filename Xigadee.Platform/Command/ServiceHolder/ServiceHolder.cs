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

    public class ServiceHolder<I> : ServiceHolder
        where I: class
    {
        private I mInstance;

        private Lazy<I> mInstanceCreator;

        public ServiceHolder(I instance, Lazy<I> lazyInstance, string serviceName = null)
        {
            mInstance = instance;
            mInstanceCreator = lazyInstance;
            StatisticsInternal.Name = serviceName ?? typeof(I).Name;
        }

        public I Service 
        { 
            get 
            {
                StatisticsInternal.Increment();

                if (mInstance == default(I))
                {
                    mInstance = mInstanceCreator.Value;
                }
                return mInstance;
            } 
        }
    }

    /// <summary>
    /// This class holds the specific reference for a Shared Service.
    /// </summary>
    public class ServiceHolder: StatisticsBase<ServiceHolderStatistics>
    {
        protected override void StatisticsRecalculate(ServiceHolderStatistics stats)
        {
        }
    }
}
