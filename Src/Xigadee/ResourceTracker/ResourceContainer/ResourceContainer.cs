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
    /// Limiters are typically connected to listener clients and reduce the incoming traffic when the resource becomes stressed.
    /// </summary>
    public class ResourceContainer: ServiceContainerBase<ResourceContainerStatistics, ResourceContainerPolicy>
        , IRequireSharedServices, IResourceTracker, IRequireDataCollector
    {
        //AKA Dependency Monitor
        #region Declarations        
        /// <summary>
        /// The is the shared service container.
        /// </summary>
        private ISharedService mSharedServices;

        private readonly ConcurrentDictionary<Guid, ResourceConsumer> mResourceResourceConsumer;

        private readonly ConcurrentDictionary<string, ResourceStatistics> mResources;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="policy">The policy.</param>
        public ResourceContainer(ResourceContainerPolicy policy = null):base(policy)
        {
            mResources = new ConcurrentDictionary<string, ResourceStatistics>();

            mResourceResourceConsumer = new ConcurrentDictionary<Guid, ResourceConsumer>();
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
        /// <summary>
        /// There are no actions for the start of this container.
        /// </summary>
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

        #region StatisticsRecalculate(ResourceContainer.Statistics stats)
        /// <summary>
        /// This method recalculates the statistics summaries.
        /// </summary>
        /// <param name="stats">The statistics.</param>
        protected override void StatisticsRecalculate(ResourceContainerStatistics stats)
        {
            if (mResources != null)
                stats.Resources = mResources.Values.ToArray();

            //if (mResourceRateLimiters != null)
            //    stats.RateLimiters = mResourceRateLimiters.Values.Select((v) => v.Debug).ToArray();
        }
        #endregion

        #region ResourceStatisticsCreateOrGet(ResourceProfile profile)
        /// <summary>
        /// This method adds a new resource statistic unless is already exists, in which case it returns the existing one.
        /// </summary>
        /// <param name="profile">The resource profile to return.</param>
        /// <returns>Returns the associated Resource Statistic.</returns>
        protected ResourceStatistics ResourceStatisticsCreateOrGet(ResourceProfile profile)
        {
            ResourceStatistics stats = mResources.GetOrAdd(profile.Id, new ResourceStatistics(signal:ResourceStatisticsSignal) { Name = profile.Id });

            return stats;
        } 
        #endregion

        #region RegisterConsumer(string name, ResourceProfile profile)
        /// <summary>
        /// This method registers a consumer which can be used to track resource contention.
        /// </summary>
        /// <param name="name">The registration friendly name.</param>
        /// <param name="profile">The resource profile to connect to.</param>
        /// <returns>Returns the consumer used to track the resource.</returns>
        public IResourceConsumer RegisterConsumer(string name, ResourceProfile profile)
        {
            if (profile == null)
                return null;

            var stats = ResourceStatisticsCreateOrGet(profile);

            var consumer = new ResourceConsumer(stats, name);

            mResourceResourceConsumer.TryAdd(consumer.ResourceId, consumer);

            return consumer;
        } 
        #endregion
        #region RegisterRequestRateLimiter(string name, IEnumerable<ResourceProfile> profiles)
        /// <summary>
        /// This method registers a rate limiter and connects it to a set of resource profiles.
        /// </summary>
        /// <param name="name">The limiter friendly name.</param>
        /// <param name="profiles">The set of resource profiles.</param>
        /// <returns>Returns the resource limiter.</returns>
        public IResourceRequestRateLimiter RegisterRequestRateLimiter(string name, IEnumerable<ResourceProfile> profiles)
        {
            var list = profiles?.ToList();
            if (list == null || list.Count == 0)
                return null;

            var stats = list.Select((p) => ResourceStatisticsCreateOrGet(p)).ToList();

            var limiter = new ResourceRateLimiter(name, stats);

            //BUG: 200 in VSTS
            //mResourceRateLimiters.Add(limiter.ResourceId, limiter);

            return limiter;
        }
        /// <summary>
        /// This method registers a rate limiter and connects it to a set of resource profiles.
        /// </summary>
        /// <param name="name">The limiter friendly name.</param>
        /// <param name="profiles">The set of resource profiles.</param>
        /// <returns>Returns the resource limiter.</returns>
        public IResourceRequestRateLimiter RegisterRequestRateLimiter(string name, params ResourceProfile[] profiles)
        {
            return RegisterRequestRateLimiter(name, (IEnumerable<ResourceProfile>) profiles);
        }
        #endregion

        #region Collector
        /// <summary>
        /// This is the data collector that is used to log ResourceEvents.
        /// </summary>
        public IDataCollection Collector
        {
            get;
            set;
        }
        #endregion

        #region ResourceStatisticsSignal(ResourceStatisticsEventType type, ResourceStatistics stats)
        /// <summary>
        /// This method is used to signal a Resource state change and publish this to the DataCollector..
        /// </summary>
        /// <param name="type">The resource event type.</param>
        /// <param name="stats">The current statistics.</param>
        private void ResourceStatisticsSignal(ResourceStatisticsEventType type, ResourceStatistics stats)
        {
            switch (type)
            {
                case ResourceStatisticsEventType.Created:
                    Collector?.Write(new ResourceEvent() { Type = type, Name = stats.Name });
                    break;
                case ResourceStatisticsEventType.KeepAlive:
                    var re = new ResourceEvent() { Type = type, Name = stats.Name };
                    Collector?.Write(re);
                    break;
            }
        }
        #endregion
        #region ResourceStatusGet(string name)
        /// <summary>
        /// This method returns the status for a specific resource.
        /// </summary>
        /// <param name="name">The resource name.</param>
        /// <returns>Returns a ResourceStatus object, or null if the status is not found.</returns>
        public ResourceStatus ResourceStatusGet(string name)
        {
            ResourceStatistics stats;
            if (!mResources.TryGetValue(name, out stats))
                return null;

            return stats.ToResourceStatus();
        }
        #endregion

    }
}