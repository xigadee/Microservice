using System;
using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This abstract class is used to implement data collectors.
    /// </summary>
    /// <typeparam name="S">The statistics type.</typeparam>
    /// <typeparam name="P">The policy type.</typeparam>
    public abstract class DataCollectorBase<S, P>: ServiceContainerBase<S, P>, IDataCollectorComponent, IRequireSecurityService, IRequireSharedServices
        where S : DataCollectorStatistics, new()
        where P : DataCollectorPolicy, new()
    {
        #region Declarations
        /// <summary>
        /// This is the azure storage wrapper.
        /// </summary>
        protected readonly ResourceProfile mResourceProfile;
        /// <summary>
        /// This is the resource consumer for the collector.
        /// </summary>
        protected IResourceConsumer mResourceConsumer;
        /// <summary>
        /// This is the encryption handler id.
        /// </summary>
        protected readonly EncryptionHandlerId mEncryption;

        /// <summary>
        /// This dictionary object holds the action mapping for the logging type.
        /// </summary>
        protected Dictionary<DataCollectionSupport, Action<EventHolder>> mSupported;
        /// <summary>
        /// This is the support map which indicates which type of logging is supported by the collector.
        /// </summary>
        protected readonly DataCollectionSupport? mSupportMapSubmitted;
        /// <summary>
        /// This is the actual calculated support map based on the mappings available.
        /// </summary>
        protected DataCollectionSupport mSupportMapActual;
        #endregion
        #region Constructor
        /// <summary>
        /// This constructor passes in the support types for the collector.
        /// </summary>
        protected DataCollectorBase(EncryptionHandlerId encryptionId = null
            , ResourceProfile resourceProfile = null
            , DataCollectionSupport? supportMap = null
            , P policy = null) :base(policy)
        {
            mResourceProfile = resourceProfile;
            mSupportMapSubmitted = supportMap;
            mEncryption = encryptionId;
        }
        #endregion

        #region Start/Stop ...
        /// <summary>
        /// This method configures the mapping.
        /// </summary>
        protected override void StartInternal()
        {
            var resourceTracker = SharedServices?.GetService<IResourceTracker>();
            if (resourceTracker != null && mResourceProfile != null)
                mResourceConsumer = resourceTracker.RegisterConsumer(GetType().Name, mResourceProfile);

            mSupported = new Dictionary<DataCollectionSupport, Action<EventHolder>>();

            SupportLoadDefault();

            var support = mSupported.Select((k) => k.Key).Aggregate((a, b) => a | b);

            if (mSupportMapSubmitted.HasValue)
                mSupportMapActual = support & mSupportMapSubmitted.Value;
            else
                mSupportMapActual = support;
        }
        /// <summary>
        /// This method stops the collection.
        /// </summary>
        protected override void StopInternal()
        {
            mSupported.Clear();
        } 
        #endregion

        #region SupportLoadDefault()
        /// <summary>
        /// This method loads the support.
        /// </summary>
        protected virtual void SupportLoadDefault()
        {
            throw new NotImplementedException("DataCollectorBase/SupportLoadDefault must be implemented to enable support.");
        }
        #endregion
        #region SupportAdd(DataCollectionSupport eventType, Action<EventBase> eventData)
        /// <summary>
        /// This method adds support for the log event.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventData">The event data.</param>
        protected virtual void SupportAdd(DataCollectionSupport eventType, Action<EventHolder> eventData)
        {
            mSupported[eventType] = eventData;
        } 
        #endregion

        #region IsSupported(DataCollectionSupport support)
        /// <summary>
        /// Returns true if the requested type is supported.
        /// </summary>
        /// <param name="support">The data collection type</param>
        /// <returns></returns>
        public virtual bool IsSupported(DataCollectionSupport support)
        {
            return (mSupportMapActual & support)>0;
        }
        #endregion
        #region OriginatorId
        /// <summary>
        /// This is is the Microservice originator information.
        /// </summary>
        public virtual MicroserviceId OriginatorId
        {
            get; set;
        }
        #endregion

        #region Write...
        /// <summary>
        /// This is the extended write method for collectors that require the additional data.
        /// </summary>
        /// <param name="holder"></param>
        public virtual void Write(EventHolder holder)
        {
            if (IsSupported(holder.DataType))
                mSupported[holder.DataType](holder);
        }
        #endregion

        #region Flush ...
        /// <summary>
        /// This method is called to flush the collector, if CanFlush is set to true.
        /// </summary>
        public virtual void Flush()
        {
            //This does nothing by default.
        }

        /// <summary>
        /// This method specifies whether the collector supports flushing. The default is false.
        /// </summary>
        public bool CanFlush { get; set; }
        #endregion

        #region Security
        /// <summary>
        /// This is a reference to the security service used for encryption.
        /// </summary>
        public ISecurityService Security
        {
            get; set;
        }
        #endregion

        #region SharedServices
        /// <summary>
        /// This is the shared service collection.
        /// </summary>
        public ISharedService SharedServices { get; set; } 
        #endregion

        #region Profiling ...
        /// <summary>
        /// This method starts the profile session and returns the profile trace id.
        /// </summary>
        /// <param name="id">The trace id.</param>
        /// <returns>Returns a profile Guid.</returns>
        protected Guid ProfileStart(string id)
        {
            return mResourceConsumer?.Start(id, Guid.NewGuid()) ?? Guid.NewGuid();
        }
        /// <summary>
        /// This method ends the profile session
        /// </summary>
        /// <param name="profileId">The trace id.</param>
        /// <param name="start">The start tick count.</param>
        /// <param name="result">The session result.</param>
        protected void ProfileEnd(Guid profileId, int start, ResourceRequestResult result)
        {
            mResourceConsumer?.End(profileId, start, result);
        }
        /// <summary>
        /// This method is called if a session request needs to be retried.
        /// </summary>
        /// <param name="profileId">The trace id.</param>
        /// <param name="retryStart">The retry tick count.</param>
        /// <param name="reason">The retry reason.</param>
        protected void ProfileRetry(Guid profileId, int retryStart, ResourceRetryReason reason)
        {
            mResourceConsumer?.Retry(profileId, retryStart, reason);
        }
        #endregion
    }

    #region DataCollectorBase ...
    /// <summary>
    /// This abstract class allows data collectors to be create without the need for a policy.
    /// </summary>
    public abstract class DataCollectorBase : DataCollectorBase<DataCollectorStatistics, DataCollectorPolicy>
    {
        #region Constructor
        /// <summary>
        /// This constructor passes in the support types for the collector.
        /// </summary>
        protected DataCollectorBase(EncryptionHandlerId encryptionId = null, ResourceProfile resourceProfile = null, DataCollectionSupport? supportMap = null, DataCollectorPolicy policy = null)
            : base(encryptionId, resourceProfile, supportMap, policy)
        {
        }
        #endregion
    }

    /// <summary>
    /// This abstract class allows data collectors to be create without the need for a policy.
    /// </summary>
    /// <typeparam name="S">The statistics type.</typeparam>
    public abstract class DataCollectorBase<S> : DataCollectorBase<S, DataCollectorPolicy>
        where S : DataCollectorStatistics, new()
    {
        #region Constructor
        /// <summary>
        /// This constructor passes in the support types for the collector.
        /// </summary>
        protected DataCollectorBase(EncryptionHandlerId encryptionId = null, ResourceProfile resourceProfile = null, DataCollectionSupport? supportMap = null, DataCollectorPolicy policy = null)
            : base(encryptionId, resourceProfile, supportMap, policy)
        {
        }
        #endregion
    } 
    #endregion
}
