using System;
using System.Security.Claims;

namespace Xigadee
{
    internal class DataCollectionWrapper: WrapperBase, IMicroserviceDataCollection
    {
        /// <summary>
        /// This collection holds the loggers for the Microservice.
        /// </summary>
        private DataCollectionContainer mDataCollection;

        internal DataCollectionWrapper(DataCollectionContainer dataCollection, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mDataCollection = dataCollection;
        }

        //Data Collector
        #region Register(IDataCollectorComponent collector)
        /// <summary>
        /// This method is used to register a collector.
        /// </summary>
        /// <param name="collector">The collector component.</param>
        /// <returns>Returns the collector passed through the registration.</returns>
        public IDataCollectorComponent Register(IDataCollectorComponent collector)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(collector);
            return collector;
        }
        #endregion

        /// <summary>
        /// This method is used by the extension method to write data to the collector.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="support">The support type.</param>
        /// <param name="sync">The sync identifier. </param>
        /// <param name="claims">Any specific claims.</param>
        public void Write(EventBase eventData, DataCollectionSupport support, bool sync = false, ClaimsPrincipal claims = null)
        {
            ValidateServiceStarted();
            mDataCollection.Write(eventData, support, sync, claims);
        }
    }
}
