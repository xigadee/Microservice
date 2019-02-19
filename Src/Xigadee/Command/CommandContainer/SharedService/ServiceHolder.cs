using System;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold a service by the Shared Service Container.
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <seealso cref="Xigadee.ServiceHolder" />
    public class ServiceHolder<I> : ServiceHolder
        where I: class
    {
        private I mInstance;

        private Lazy<I> mInstanceCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHolder{I}"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="lazyInstance">The lazy service reference.</param>
        /// <param name="serviceName">Name of the service.</param>
        public ServiceHolder(I instance, Lazy<I> lazyInstance, string serviceName = null)
        {
            mInstance = instance;
            mInstanceCreator = lazyInstance;
            StatisticsInternal.Name = serviceName ?? typeof(I).Name;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        public I Service 
        { 
            get 
            {
                //Increment the access statistics.
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
        /// <summary>
        /// Statistics to recalculate.
        /// </summary>
        /// <param name="stats">The stats.</param>
        protected override void StatisticsRecalculate(ServiceHolderStatistics stats)
        {
        }
    }
}
