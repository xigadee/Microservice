#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to connect resource consumers with resource limiters.
    /// Limiters are typically connected to listener clients and reduce the imcoming traffic when the resource becomes stressed.
    /// </summary>
    public class ResourceTracker: ServiceBase<ResourceTrackerStatistics>, IRequireSharedServices, IResourceTracker
    {
        #region Declarations
        private ISharedService mSharedServices;

        private Dictionary<Guid, ResourceRateLimiter> mResourceRateLimiters;
        private Dictionary<Guid, ResourceConsumer> mResourceResourceConsumer;

        private ConcurrentDictionary<string, ResourceStatistics> mResources; ///
        #endregion

        #region Constructor
        public ResourceTracker()
        {
            mResources = new ConcurrentDictionary<string, ResourceStatistics>();

            mResourceRateLimiters = new Dictionary<Guid, ResourceRateLimiter>();
            mResourceResourceConsumer = new Dictionary<Guid, ResourceConsumer>();
        }
        #endregion

        #region SharedServices
        /// <summary>
        /// This method registers the IResourceTracker reference when the shared services reference is set.
        /// </summary>
        public ISharedService SharedServices
        {
            get
            {
                return mSharedServices;
            }
            set
            {
                mSharedServices = value;
                value?.RegisterService<IResourceTracker>(this);
            }
        } 
        #endregion
        #region Start/Stop
        protected override void StartInternal()
        {

        }
        /// <summary>
        /// This method removes the shared service reference.
        /// </summary>
        protected override void StopInternal()
        {
            try
            {
                SharedServices.RemoveService<IResourceTracker>();
            }
            catch
            {

            }
        } 
        #endregion

        protected override void StatisticsRecalculate(ResourceTrackerStatistics stats)
        {
            if (mResources!=null)
                stats.Resources = mResources.Values.ToArray();

            if (mResourceRateLimiters != null)
                stats.RateLimiters = mResourceRateLimiters.Values.Select((v) => v.Debug).ToArray();
        }

        protected ResourceStatistics ResourceCreate(ResourceProfile profile)
        {
            ResourceStatistics stats = mResources.GetOrAdd(profile.Id, new ResourceStatistics() { Name = profile.Id });
       
            return stats;
        }

        public IResourceConsumer RegisterConsumer(string name, ResourceProfile profile)
        {
            if (profile == null)
                return null;

            var stats = ResourceCreate(profile);

            var consumer = new ResourceConsumer(stats, name);

            mResourceResourceConsumer.Add(consumer.ResourceId, consumer);

            return consumer;
        }

        /// <summary>
        /// This method registers a rate limiter and connects it to a set of resource profiles.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="profiles"></param>
        /// <returns></returns>
        public IResourceRequestRateLimiter RegisterRequestRateLimiter(string name, IEnumerable<ResourceProfile> profiles)
        {
            if (profiles == null)
                return null;

            var list = profiles.ToList();
            if (list.Count == 0)
                return null;

            var stats = list.Select((p) => ResourceCreate(p)).ToList();

            var limiter = new ResourceRateLimiter(name, stats);

            mResourceRateLimiters.Add(limiter.ResourceId, limiter);

            return limiter;
        }

    }
}
