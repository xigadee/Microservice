#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class contains a set of entities, and keeps them in memory.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public abstract class EntityCacheAsyncBase<K, E>: CommandBase<EntityCacheStatistics, EntityCacheAsyncPolicy>, 
        IRequireSharedServices, IEntityCacheAsync<K, E>
        where K : IEquatable<K>
        where E : class
    {
        #region Declarations
        /// <summary>
        /// The resource consumer 
        /// </summary>
        protected IResourceConsumer mResourceConsumer;

        protected readonly ResourceProfile mResourceProfile;

        protected ISharedService mSharedServices;


        protected readonly ConcurrentDictionary<K, EntityCacheHolder<K, E>> mEntities;

        protected long mAdded = 0;

        protected long mRemoved = 0;

        protected long mWaitCycles = 0;

        protected DateTime? mLastScheduleTime = null;

        #endregion
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="policy">This is the default configuration policy.</param>
        /// <param name="resourceProfile">This is the optional resource profile that can be used to teack and limit resources.</param>
        public EntityCacheAsyncBase(EntityCacheAsyncPolicy policy, ResourceProfile resourceProfile = null) :base(policy)
        {
            if (policy == null)
                throw new ArgumentNullException("EntityCacheAsyncPolicy cannot be null");

            mResourceProfile = resourceProfile ?? new ResourceProfile(string.Format("Cache_{0}", typeof(E).Name));
            mEntities = new ConcurrentDictionary<K, EntityCacheHolder<K, E>>();
        }
        ///// <summary>
        ///// This is the default constructor for the cache handler.
        ///// </summary>
        ///// <param name="interval"></param>
        ///// <param name="initialWait"></param>
        ///// <param name="initialTime"></param>
        ///// <param name="trackEvents"></param>
        //protected EntityCacheAsyncBase(TimeSpan? interval = null, TimeSpan? initialWait = null, DateTime? initialTime = null
        //    , bool trackEvents = false, int maxCount = 200000, ResourceProfile resourceProfile = null, TimeSpan? defaultTTL = null)
        //    : base(JobConfiguration.ToJob(interval ?? TimeSpan.FromMinutes(5), initialWait ?? TimeSpan.FromSeconds(5), initialTime))
        //{
        //    mResourceProfile = resourceProfile ?? new ResourceProfile(string.Format("Cache_{0}", typeof(E).Name));
        //    mEntities = new ConcurrentDictionary<K, EntityCacheHolder<K, E>>();
        //}
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method recalculates the statistics for the async cache
        /// </summary>
        protected override void StatisticsRecalculate(EntityCacheStatistics stats)
        {
            base.StatisticsRecalculate(stats);

            stats.CurrentCached = mEntities.Count;
            stats.CurrentCachedEntities = mEntities.Values.Where((e) => e.Entity != null).LongCount();
            stats.CurrentCacheLimit = mPolicy.EntityCacheLimit;
            stats.TrackEvents = mPolicy.EntityChangeTrackEvents;
            stats.WaitCycles = mWaitCycles;
            stats.Removed = mRemoved;
            stats.Added = mAdded;
            stats.LastScheduleTime = mLastScheduleTime;

            stats.Current = mEntities.Values.Select((e) => e.Debug).ToList();

            //mStatistics.DefaultExpiry = mDefaultExpiry;
        }
        #endregion
        #region TimerPollSchedulesRegister(JobConfiguration config)
        /// <summary>
        /// This method register the job expiry task.
        /// </summary>
        /// <param name="config">The job configuration</param>
        protected override void TimerPollSchedulesRegister()
        {
            var job = new CommandSchedule(ScheduleExpireEntities
                , mPolicy.JobPollSchedule
                , $"EntityCacheHandlerBase: {typeof(E).Name} Expire Entities"
                , mPolicy.JobPollIsLongRunning);

            mSchedules.Add(job);
        } 
        #endregion
        #region --> ScheduleExpireEntities(Schedule schedule, CancellationToken cancel)
        /// <summary>
        /// Remove the expired references.
        /// </summary>
        /// <param name="schedule">The schedule to poll the handler.</param>
        /// <returns></returns>
        protected async Task ScheduleExpireEntities(Schedule schedule, CancellationToken cancel)
        {
            try
            {
                var now = DateTime.UtcNow;
                mLastScheduleTime = now;
                var expired = mEntities.Values.Where(e => e.Expiry < now).ToList();
                expired.ForEach(v => Remove(v.Key));
                int reduce = mEntities.Count - mPolicy.EntityCacheLimit;
                if (reduce > 0)
                {
                    //We're still over capacity, so remove records based on their low hitcount and take the ones
                    //that will expire the soonest.
                    var toRemove = mEntities.Values
                        .OrderBy((e) => e.HitCount)
                        .OrderBy((e) => e.Expiry)
                        .Take(reduce).ToList();

                    toRemove.ForEach(v => Remove(v.Key));
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("Unexpected error in ScheduleExpireEntities", ex);
            }

        }
        #endregion
        #region StartInternal()/StopInternal()
        /// <summary>
        /// This method starts the service and retrieves the services required for connectivity.
        /// </summary>
        protected override void StartInternal()
        {
            base.StartInternal();

            var resourceTracker = SharedServices.GetService<IResourceTracker>();
            if (resourceTracker != null && mResourceProfile != null)
                mResourceConsumer = resourceTracker.RegisterConsumer("CacheV2" + mResourceProfile.Id, mResourceProfile);
        }
        /// <summary>
        /// This is the class that removes references to service and removes shared services.
        /// </summary>
        protected override void StopInternal()
        {
            mResourceConsumer = null;

            base.StopInternal();
        }
        #endregion
        #region SharedServices
        /// <summary>
        /// This is the shared service collection.
        /// </summary>
        public virtual ISharedService SharedServices
        {
            get
            {
                return mSharedServices;
            }
            set
            {
                mSharedServices = value;
                if (value != null)
                    mSharedServices.RegisterService<IEntityCacheAsync<K, E>>(this);
            }
        }
        #endregion

        #region CommandsRegister()
        /// <summary>
        /// This override connects the cache handler to the interservice message handling.
        /// </summary>
        public override void CommandsRegister()
        {
            if (mPolicy.EntityChangeTrackEvents)
                EntityChangeEventCommandsRegister();
        }
        #endregion
        #region EntityChangeEventCommandsRegister()
        /// <summary>
        /// This method registers the listener commands for system wide entity changes.
        /// </summary>
        public virtual void EntityChangeEventCommandsRegister()
        {
            if (string.IsNullOrWhiteSpace(mPolicy.EntityChangeEventsChannel))
                throw new NotSupportedException("EntityChangeTrackEvents is set as active, but the EntityChangeEventsChannel is not defined.");

            CommandRegister(mPolicy.EntityChangeEventsChannel, typeof(E).Name, EntityActions.Create, EntityChangeNotification);
            CommandRegister(mPolicy.EntityChangeEventsChannel, typeof(E).Name, EntityActions.Update, EntityChangeNotification);
            CommandRegister(mPolicy.EntityChangeEventsChannel, typeof(E).Name, EntityActions.Delete, EntityChangeNotification);
        } 
        #endregion

        #region Remove(K key)
        /// <summary>
        /// This method removes an item from the cache collection.
        /// </summary>
        /// <param name="key">The item key.</param>
        protected virtual void Remove(K key)
        {
            EntityCacheHolder<K, E> holder;
            if (mEntities.TryRemove(key, out holder))
            {
                holder.Cancel();
                Interlocked.Increment(ref mRemoved);
            }
        }
        #endregion

        #region EntityChangeNotification(TransmissionPayload rq, List<TransmissionPayload> rs)
        /// <summary>
        /// This method is called when an entity of the cache type is updated or deleted.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <returns></returns>
        protected virtual async Task EntityChangeNotification(TransmissionPayload rq, List<TransmissionPayload> rs)
        {
            try
            {
                EntityChangeReference<K> entityChangeReference = PayloadSerializer.PayloadDeserialize<EntityChangeReference<K>>(rq);
                var key = entityChangeReference.Key;
                Remove(key);
            }
            catch (Exception ex)
            {
                Logger.LogException("Unable to retrieve the entity change reference", ex);
            }
        }
        #endregion
        #region ResponseId
        /// <summary>
        /// This property is not used.
        /// </summary>
        protected override MessageFilterWrapper ResponseId
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region ContainsKey(K key)
        /// <summary>
        /// This method returns true if the key is valid for the collection.
        /// </summary>
        /// <param name="key">The key to validate.</param>
        /// <returns>Returns true if the key exists in the collection.</returns>
        public async Task<bool> ContainsKey(K key)
        {
            var result = await TryGetEntity(key);
            return result.Exists;
        }
        #endregion
        #region TryGetEntity(K key)
        /// <summary>
        /// This method returns the entity along with a value indicating whether the entity exists in the collection.
        /// </summary>
        /// <param name="key">The reference key.</param>
        /// <returns>Returns a key value pair indicating whether the retrieval was successful.</returns>
        public async Task<EntityCacheResult<E>> TryGetEntity(K key)
        {
            var result = await TryGetCacheHolder(key);
            return result.ToCacheResult();
        }
        #endregion

        #region Gatekeeper(CacheHolderV2<K,E> cacheHolder, Guid traceId, out ResourceRequestResult status, int attempts = 5)
        /// <summary>
        /// This method is used to keep incoming threads cycling while the first thread retrieves the entity from
        /// the persistence store.
        /// </summary>
        /// <param name="cacheHolder">The current cache holder.</param>
        /// <param name="traceId">The resource traceid</param>
        /// <param name="status">The final status.</param>
        /// <returns></returns>
        private void Gatekeeper(EntityCacheHolder<K, E> cacheHolder, Guid traceId, out ResourceRequestResult status, int attempts = 5)
        {
            int retryStart = Environment.TickCount;

            while (cacheHolder.State != EntityCacheHolderState.Completed && --attempts > 0)
            {
                cacheHolder.Wait();
                mResourceConsumer.Retry(traceId, retryStart, ResourceRetryReason.Timeout);
                retryStart = Environment.TickCount;
            }

            if (cacheHolder.State == EntityCacheHolderState.Completed)
                status = ResourceRequestResult.Success;
            else if (attempts <= 0)
                status = ResourceRequestResult.RetryExceeded;
            else
                status = ResourceRequestResult.TaskCancelled;
        }
        #endregion

        #region TryGetCacheHolder(K key)
        /// <summary>
        /// This method retrieves the cache holder from the store. If the cache holder is not present then it 
        /// tries to retrieve it from the underlying store.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The cache holder.</returns>
        protected async Task<EntityCacheHolder<K, E>> TryGetCacheHolder(K key)
        {
            var traceId = mResourceConsumer.Start("TryGetCacheHolder", Guid.NewGuid());
            ResourceRequestResult status = ResourceRequestResult.Unknown;
            EntityCacheHolder<K, E> cacheHolder;
            int start = StatisticsInternal.ActiveIncrement();

            try
            {
                while (true)
                {
                    int retryStart = Environment.TickCount;

                    //Get the cache holder, and if it already exists, see if you need to wait.
                    if (!mEntities.TryGetValue(key, out cacheHolder))
                    {
                        var temp = new EntityCacheHolder<K, E>(key, mPolicy.EntityDefaultTTL);
                        if (mEntities.TryAdd(key, temp))
                        {
                            //Ok, the entity is not in the cache, so let's go and get the entity.
                            cacheHolder = temp;
                            break;
                        }
                    }
                    else
                    {
                        Gatekeeper(cacheHolder, traceId, out status);
                        return cacheHolder;
                    }

                    mResourceConsumer.Retry(traceId, retryStart, ResourceRetryReason.Timeout);
                }

                try
                {
                    //Ok, read the entity
                    var result = await Read(key);

                    if (result != null && !result.IsFaulted && (result.ResponseCode == 200 || result.ResponseCode == 404))
                    {
                        cacheHolder.Load(result.Entity, result.ResponseCode);
                        status = ResourceRequestResult.Success;
                    }
                    else
                    {
                        cacheHolder.Cancel();
                        status = ResourceRequestResult.TaskCancelled;
                    }
                }
                catch (Exception ex)
                {
                    cacheHolder.Cancel();
                    status = ResourceRequestResult.TaskCancelled;
                    Logger.LogException(string.Format("Error requesting {0} - {1} ", typeof(E).Name, key.ToString()), ex);
                }
                finally
                {
                    if (cacheHolder.State == EntityCacheHolderState.Cancelled)
                        mEntities.TryRemove(key, out cacheHolder);

                    //Make sure that any waiting requests are released.
                    cacheHolder.Release();
                }
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(start);
                if (status != ResourceRequestResult.Success)
                    StatisticsInternal.ErrorIncrement();

                mResourceConsumer.End(traceId, start, status);
            }

            return cacheHolder;
        }
        #endregion

        /// <summary>
        /// This abstract method is used to read from the underlying store.
        /// </summary>
        /// <param name="key">The key to read using.</param>
        /// <returns>Returns a repository holder with the correct status and supported entity.</returns>
        protected abstract Task<RepositoryHolder<K, E>> Read(K key);
    }
}
