using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Xigadee
{
    #region PersistenceCommand<K, E>
    /// <summary>
    /// This command provides the Xigadee plumbing to host a repository in a Microservice.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceServer<K, E>: PersistenceServer<K, E, PersistenceServerStatistics, PersistenceServerPolicy>
        where K : IEquatable<K>
    {
        /// <summary>
        /// Initializes a new instance of the PersistenceServer class that hosts a repository in the Microservice.
        /// </summary>
        /// <param name="repository">The repository. This is mandatory.</param>
        /// <param name="persistenceRetryPolicy">The optional persistence retry policy.</param>
        /// <param name="resourceProfile">The optional resource profile.</param>
        /// <param name="cacheManager">The optional cache manager.</param>
        /// <param name="defaultTimeout">The default timeout.</param>
        /// <param name="policy">The optional policy.</param>
        public PersistenceServer(IRepositoryAsyncServer<K, E> repository
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultTimeout = null
            , PersistenceServerPolicy policy = null)
            : base(repository
                , persistenceRetryPolicy
                , resourceProfile
                , cacheManager
                , defaultTimeout
                , policy
                )
        {
        }
    }
    #endregion

    #region PersistenceServer<K, E, S, P>
    /// <summary>
    /// Initializes a new instance of the PersistenceServer class that hosts a repository in the Microservice.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <typeparam name="S">The persistence statistics type.</typeparam>
    /// <typeparam name="P">The persistence command policy type.</typeparam>
    public class PersistenceServer<K, E, S, P> : CommandBase<S, P, PersistenceHandler>, IPersistenceMessageHandler
        where K : IEquatable<K>
        where S : PersistenceServerStatistics, new()
        where P : PersistenceServerPolicy, new()
    {
        #region Declarations
        /// <summary>
        /// This event is raised when an entity is created, changes, or is removed.
        /// </summary>
        public event EventHandler<EntityChangeEventArgs> OnEntityChangeAction;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the PersistenceServer class that hosts a repository in the Microservice.
        /// </summary>
        /// <param name="repository">The repository. This is mandatory.</param>
        /// <param name="persistenceRetryPolicy">The optional persistence retry policy.</param>
        /// <param name="resourceProfile">The optional resource profile.</param>
        /// <param name="cacheManager">The optional cache manager.</param>
        /// <param name="defaultTimeout">The default timeout.</param>
        /// <param name="policy">The optional policy.</param>
        /// <exception cref="ArgumentNullException">entityTransform cannot be null</exception>
        public PersistenceServer(
              IRepositoryAsyncServer<K, E> repository
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultTimeout = null
            , P policy = null
            ) : base(policy)
        {
            Repository = repository ?? throw new ArgumentNullException("repository");
            Repository.Collector = Collector;

            Policy.DefaultTimeout = defaultTimeout ?? DefaultTimeout;
            Policy.PersistenceRetryPolicy = persistenceRetryPolicy ?? new PersistenceRetryPolicy();
            Policy.ResourceProfile = resourceProfile;

            CacheManager = cacheManager;
            CacheManagerActive = cacheManager != null;
        }
        #endregion

        #region Collector
        /// <summary>
        /// This is the data collector override.
        /// This override sets the collector on the repository.
        /// </summary>
        public override IDataCollection Collector
        {
            get => base.Collector;
            set
            {
                base.Collector = value;
                Repository.Collector = value;
            }
        } 
        #endregion

        #region Repository
        /// <summary>
        /// Gets the internal repository.
        /// </summary>
        public virtual IRepositoryAsyncServer<K, E> Repository { get; }
        #endregion

        #region CacheManager
        /// <summary>
        /// This is the cache manager.
        /// </summary>
        protected ICacheManager<K, E> CacheManager { get; }
        /// <summary>
        /// Specifies whether the cache manager is active.
        /// </summary>
        protected bool CacheManagerActive { get; }
        #endregion
        #region FriendlyName
        /// <summary>
        /// Update to friendly name to make it clear which entity is being used
        /// </summary>
        public override string FriendlyName => $"{base.FriendlyName}-{typeof(E).Name}";
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method recalculates the statistics for the persistence command.
        /// </summary>
        protected override void StatisticsRecalculate(S stats)
        {
            base.StatisticsRecalculate(stats);

            StatisticsInternal.RequestsInPlay = RequestsCurrent.Values.Select((v) => v?.Debug).ToArray();
        }
        #endregion

        #region EntityType
        /// <summary>
        /// This is the entity type Name used for matching request and payloadRs messages.
        /// </summary>
        public virtual string EntityType => Repository.EntityName;
        #endregion

        #region Start/Stop Internal
        /// <summary>
        /// This method starts the persistence command.
        /// </summary>
        protected override void StartInternal()
        {
            var resourceTracker = SharedServices.GetService<IResourceTracker>();
            if (resourceTracker != null && Policy.ResourceProfile != null)
                Policy.ResourceConsumer = resourceTracker.RegisterConsumer(EntityType, Policy.ResourceProfile);

            base.StartInternal();

            Collector?.LogMessage($"Repository {typeof(E).Name} started.");
        }
        /// <summary>
        /// This method stops the persistence command.
        /// </summary>
        protected override void StopInternal()
        {
            Collector?.LogMessage($"Repository {typeof(E).Name} stopping.");

            base.StopInternal();
            Policy.ResourceConsumer = null;
        }
        #endregion

        #region CommandsRegister()
        /// <summary>
        /// This method register the supported commands for a persistence handler.
        /// </summary>
        protected override void CommandsRegister()
        {
            //Create
            PersistenceCommandRegister<K, E>(EntityActions.Create, ProcessCreate, true, timeoutcorrect: TimeoutCorrectCreateUpdate);
            //Read
            PersistenceCommandRegister<K, E>(EntityActions.Read, ProcessRead);
            PersistenceCommandRegister<K, E>(EntityActions.ReadByRef, ProcessReadByRef);
            //Update
            PersistenceCommandRegister<K, E>(EntityActions.Update, ProcessUpdate, true, timeoutcorrect: TimeoutCorrectCreateUpdate);
            //Delete
            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.Delete, ProcessDelete, true, timeoutcorrect: TimeoutCorrectDelete);
            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.DeleteByRef, ProcessDeleteByRef, true, timeoutcorrect: TimeoutCorrectDelete);
            //Version
            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.Version, ProcessVersion);
            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.VersionByRef, ProcessVersionByRef);
            //Search
            PersistenceCommandRegister<SearchRequest, SearchResponse>(EntityActions.Search, ProcessSearch);
            PersistenceCommandRegister<SearchRequest, SearchResponse<E>>(EntityActions.SearchEntity, ProcessSearchEntity);
            //History
            PersistenceCommandRegister<HistoryRequest<K>, HistoryResponse<E>>(EntityActions.History, ProcessHistory);
        }
        #endregion
        #region PersistenceCommandRegister<KT,ET>...
        /// <summary>
        /// This method registers the specific persistence handler.
        /// </summary>
        /// <typeparam Name="KT">The key type.</typeparam>
        /// <typeparam Name="ET">The entity type.</typeparam>
        /// <param name="actionType">The action type identifier</param>
        /// <param name="action">The action to process the command.</param>
        /// <param name="logEventOnSuccess"></param>
        /// <param name="preaction"></param>
        /// <param name="postaction"></param>
        /// <param name="timeoutcorrect"></param>
        /// <param name="retryOnTimeout"></param>
        /// <param name="channelId"></param>
        /// <param name="entityType"></param>
        protected virtual void PersistenceCommandRegister<KT, ET>(string actionType
            , Func<PersistenceRequestHolder<KT, ET>, Task> action
            , bool logEventOnSuccess = false
            , Func<PersistenceRequestHolder<KT, ET>, Task<bool>> preaction = null
            , Func<PersistenceRequestHolder<KT, ET>, Task> postaction = null
            , Func<PersistenceRequestHolder<KT, ET>, Task<bool>> timeoutcorrect = null
            , int? retryOnTimeout = null
            , string channelId = null
            , string entityType = null
            )
        {
            Func<TransmissionPayload, List<TransmissionPayload>, Task> actionPayload = async (incoming, outgoing) =>
            {
                var profileHolder = ProfileStart<KT, ET>(incoming, outgoing);

                try
                {
                    var rsMessage = incoming.Message.ToResponse();

                    rsMessage.ChannelId = incoming.Message.ResponseChannelId;
                    rsMessage.ChannelPriority = incoming.Message.ResponseChannelPriority;
                    rsMessage.MessageType = incoming.Message.MessageType;
                    rsMessage.ActionType = "";

                    var rsPayload = new TransmissionPayload(rsMessage, traceEnabled: Policy.TransmissionPayloadTraceEnabled);

                    bool hasTimedOut = false;

                    try
                    {
                        RepositoryHolder<KT, ET> rqTemp = incoming.Message.Holder.Object as RepositoryHolder<KT, ET>;

                        profileHolder.Rq = new PersistenceRepositoryHolder<KT, ET>(rqTemp);

                        if (profileHolder.Rq.Timeout == null)
                            profileHolder.Rq.Timeout = TimeSpan.FromSeconds(10);

                        bool preactionFailed = false;

                        try
                        {
                            bool retryExceeded = false;

                            do
                            {
                                //Set the time stamp for the beginning of the resource request.
                                int attempt = Environment.TickCount;

                                //Create the payloadRs holder, and discard any previous version.
                                profileHolder.Rs = new PersistenceRepositoryHolder<KT, ET>();

                                if (preaction != null && !(await preaction(profileHolder)))
                                {
                                    preactionFailed = true;
                                    break;
                                }

                                //Call the specific command to process the action, i.e Create, Read, Update, Delete ... etc.
                                await action(profileHolder);

                                //Flag if the request times out at any point. 
                                //This may be required later when checking whether the action was actually successful.
                                hasTimedOut |= profileHolder.Rs.IsTimeout;

                                //OK, if this is not a time out then it is successful
                                if (!profileHolder.Rs.IsTimeout && !profileHolder.Rs.ShouldRetry)
                                    break;

                                ProfileRetry(profileHolder, attempt);

                                if (profileHolder.Rs.IsTimeout)
                                    Collector?.LogMessage(LoggingLevel.Warning, $"Timeout occurred for {EntityType} {actionType} for request:{profileHolder.Rq} with response:{profileHolder.Rs}", "DBTimeout");

                                profileHolder.Rq.IsRetry = true;
                                //These should not be counted against the limit.
                                if (!profileHolder.Rs.ShouldRetry)
                                    profileHolder.Rq.Retry++;

                                profileHolder.Rq.IsTimeout = false;

                                retryExceeded = incoming.Cancel.IsCancellationRequested
                                    || profileHolder.Rq.Retry > Policy.PersistenceRetryPolicy.GetMaximumRetries(incoming);
                            }
                            while (!retryExceeded);

                            //Signal to the underlying comms channel that the message has been processed successfully.
                            incoming.Signal(!retryExceeded);

                            // If we have exceeded the retry limit then Log error
                            if (retryExceeded)
                            {
                                Log(actionType
                                    , profileHolder
                                    , LoggingLevel.Error
                                    , $"Retry limit has been exceeded (cancelled ({incoming.Cancel.IsCancellationRequested})) for {EntityType} {actionType} for {profileHolder.Rq} after {incoming.Message?.FabricDeliveryCount} delivery attempts"
                                    , "DBRetry");

                                profileHolder.result = ResourceRequestResult.RetryExceeded;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException(actionType, profileHolder, ex);
                            incoming.SignalFail();
                            profileHolder.result = ResourceRequestResult.Exception;
                        }

                        bool logEventSource = !preactionFailed && logEventOnSuccess && profileHolder.Rs.IsSuccess;

                        if (!profileHolder.Rs.IsSuccess && hasTimedOut && timeoutcorrect != null && profileHolder.result != ResourceRequestResult.Exception)
                        {
                            if (await timeoutcorrect(profileHolder))
                            {
                                logEventSource = true;
                                Collector?.LogMessage(LoggingLevel.Info
                                    , string.Format("Recovered timeout successfully for {0}-{1} for request:{2} - response:{3}", EntityType, actionType, profileHolder.Rq, profileHolder.Rs)
                                    , "DBTimeout");
                            }
                            else
                            {
                                Collector?.LogMessage(LoggingLevel.Error
                                    , string.Format("Not recovered timeout for {0}-{1} for request:{2} - response:{3}", EntityType, actionType, profileHolder.Rq, profileHolder.Rs)
                                    , "DBTimeout");
                            }
                        }

                        if (logEventSource && profileHolder.Rs.ShouldLogEventSource)
                            await LogEventSource(actionType, profileHolder);

                        if (!preactionFailed && postaction != null)
                            await postaction(profileHolder);

                        //Serialize the payloadRs
                        var reposHolder = profileHolder.Rs.ToRepositoryHolder();

                        rsPayload.Message.Holder.SetObject(reposHolder);

                        rsPayload.Message.Status = "200";

                        if (!profileHolder.result.HasValue)
                            profileHolder.result = ResourceRequestResult.Success;
                    }
                    catch (PayloadSerializationException payex)
                    {
                        incoming.SignalSuccess(); //It's a success as there isn't anything that we can do with it except send an error.
                        rsPayload.Message.Status = "422"; //Un-processable Entity (WebDAV) - we use this to show there is an error with the payload.
                        rsPayload.Message.StatusDescription = $"Invalid payload: {payex.Message}/{payex.InnerException?.Message}";
                        Collector?.LogException($"Error processing message (was cancelled({incoming.Cancel.IsCancellationRequested}))-{EntityType}-{actionType}-{profileHolder.Rq}", payex);
                        profileHolder.result = ResourceRequestResult.Exception;
                    }
                    catch (Exception ex)
                    {
                        incoming.SignalFail();
                        rsPayload.Message.Status = "500";
                        rsPayload.Message.StatusDescription = ex.Message;
                        Collector?.LogException($"Error processing message (was cancelled({incoming.Cancel.IsCancellationRequested}))-{EntityType}-{actionType}-{profileHolder.Rq}", ex);
                        profileHolder.result = ResourceRequestResult.Exception;
                    }

                    // check whether we need to send a response message. If this is async and AsyncResponse is not set to true,
                    // then by default we do not send a response message to cut down on unnecessary traffic.
                    if (profileHolder.Rq == null || profileHolder.Rq.Settings == null || !profileHolder.Rq.Settings.ProcessAsync)
                        outgoing.Add(rsPayload);
                }
                finally
                {
                    ProfileEnd(profileHolder);
                }

                if (profileHolder.result == ResourceRequestResult.Success)
                    switch (actionType)
                    {
                        case EntityActions.Create:
                        case EntityActions.Update:
                        case EntityActions.Delete:
                        case EntityActions.DeleteByRef:
                            RaiseEntityChangeEvent(actionType, incoming.Message.MessageType, profileHolder.ProfileId
                                , profileHolder.Rs.KeyReference
                                , profileHolder.Prq?.Message?.ProcessCorrelationKey
                                , profileHolder.Prq?.Message?.OriginatorKey);
                            break;
                    }
            };

            if (channelId == null)
                channelId = ChannelId ?? string.Empty;

            if (entityType == null)
                entityType = EntityType;

            CommandRegister((channelId.ToLowerInvariant(), entityType.ToLowerInvariant(), actionType.ToLowerInvariant()), actionPayload);
        }
        #endregion

        #region RaiseEntityChangeEvent...
        /// <summary>
        /// This method raises an event when there is a change for an entity, specifically for Create, Update or Delete.
        /// </summary>
        /// <param name="actionType">The action type.</param>
        /// <param name="entityType">The entity type.</param>
        /// <param name="traceId">The trace event</param>
        /// <param name="idVersion">The id and version as string.</param>
        /// <param name="processCorrelationKey">This is the original initiating process id.</param>
        /// <param name="originatorKey">This is the message origination id.</param>
        protected virtual void RaiseEntityChangeEvent(string actionType, string entityType
            , Guid traceId, Tuple<string, string> idVersion
            , string processCorrelationKey, string originatorKey)
        {
            //Let's fire an event if something has changed.
            try
            {
                var args = new EntityChangeEventArgs();

                args.ActionType = actionType;
                args.KeyType = typeof(K).Name;
                args.EntityType = entityType;
                args.TraceId = traceId;
                args.Id = idVersion?.Item1;
                args.VersionId = idVersion?.Item2;

                args.ProcessCorrelationKey = processCorrelationKey;
                args.OriginatorKey = originatorKey;

                OnEntityChangeAction?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Collector?.LogException($"RaiseEntityChangeEvent {entityType}/{actionType} failed for {traceId.ToString("N")}", ex);
            }
        }
        #endregion

        //Timeout correction
        #region TimeoutCorrectCreateUpdate...

        /// <summary>
        /// This method can be overridden to correct timeouts for state changing requests and to log to the event source event on a timeout failure condition.
        /// This would be used when the underlying communication times out, but the action was ultimately successful.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <returns>Return true if the message should logged </returns>
        protected virtual async Task<bool> TimeoutCorrectCreateUpdate(PersistenceRequestHolder<K, E> holder)
        {
            return false;
        }
        #endregion
        #region TimeoutCorrectDelete...
        /// <summary>
        /// This method can be overridden to correct timeouts for state changing requests and to log to the event source event on a timeout failure condition.
        /// This would be used when the underlying communication times out, but the action was ultimately successful.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="rq"></param>
        /// <param name="rs"></param>
        /// <param name="m"></param>
        /// <param name="l"></param>
        /// <returns>Return true if the message should logged </returns>
        protected virtual async Task<bool> TimeoutCorrectDelete(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return false;
        }
        #endregion

        //Logging
        #region Log<KT, ET>...
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam Name="KT">The key type.</typeparam>
        /// <typeparam Name="ET">The entity type.</typeparam>
        /// <param name="action"></param>
        /// <param name="prq"></param>
        /// <param name="rq"></param>
        /// <param name="rs"></param>
        /// <param name="loggingLevel">Logging level</param>
        /// <param name="message"></param>
        protected void Log<KT, ET>(string action, PersistenceRequestHolder<KT, ET> holder
            , LoggingLevel loggingLevel = LoggingLevel.Info, string message = null, string category = null)
        {
            var logEvent = new PersistencePayloadLogEvent(holder.Prq, holder.Rq, holder.Rs, loggingLevel) { Message = message ?? string.Empty, Category = category };
            Collector?.Log(logEvent);
        }
        #endregion
        #region LogException<KT, ET>...
        /// <summary>
        /// We don't want to pass the exception details back to the calling party as this may
        /// leak sensitive information about the application and persistence agent.
        /// This method logs the error and assigns it a track-able id and sends that instead to the 
        /// front end.
        /// </summary>
        /// <typeparam Name="KT">The key type.</typeparam>
        /// <typeparam Name="ET">The entity type.</typeparam>
        /// <param Name="action">the entity action.</param>
        /// <param Name="prq">The payload request.</param>
        /// <param Name="ex">The exception.</param>
        /// <param name="rq">The request.</param>
        /// <param Name="rs">The response.</param>
        protected void LogException<KT, ET>(string action, PersistenceRequestHolder<KT, ET> holder, Exception ex)
        {

            try
            {
                var logEvent = new PersistencePayloadLogEvent(holder.Prq, holder.Rq, holder.Rs, LoggingLevel.Info, ex);
                Collector?.Log(logEvent);

                Guid errorId = Guid.NewGuid();
                string errMessage = string.Format("Exception tracker {0}/{1}/{2}", action, (holder.Prq != null && holder.Prq.Message != null ? holder.Prq.Message.OriginatorKey : string.Empty), errorId);
                holder.Rs.ResponseMessage = errMessage;

                if (holder.Rq != null)
                    errMessage += string.Format("/{0}-{1}", holder.Rq.Key, (holder.Rq.Entity == null) ? string.Empty : holder.Rq.Entity.ToString());

                Collector?.LogException(errMessage, ex);
            }
            catch (Exception)
            {
                // Do not fail due to an issue logging
            }

            holder.Rs.ResponseCode = 500;
            holder.Rs.ResponseMessage = ex.Message;
        }
        #endregion
        #region LogEventSource<KT, ET>...
        /// <summary>
        /// This method logs entities to the event source. This method will try indefinitely
        /// as we do not want the Event Source to fail.
        /// </summary>
        /// <typeparam Name="KT">The key type.</typeparam>
        /// <typeparam Name="ET">The entity type.</typeparam>
        /// <param name="actionType"></param>
        /// <param name="prq"></param>
        /// <param name="rq"></param>
        /// <param name="rs"></param>
        protected async virtual Task LogEventSource<KT, ET>(string actionType, PersistenceRequestHolder<KT, ET> holder)
        {
            // Only pass through the entity if is of the correct entity type. ET might be an entity or it might be a Tuple<K, string>> in which case pass a null  
            E entity = typeof(ET) == typeof(E) ? (E)Convert.ChangeType(holder.Rs.Entity, typeof(E)) : default(E);
            await LogEventSource(actionType, holder.Prq.Message.OriginatorKey, holder.Rs.Key, entity, holder.Rq.Settings);
        }
        /// <summary>
        /// This method logs entities to the event source. This method will try indefinitely
        /// as we do not want the Event Source to fail.
        /// </summary>
        /// <typeparam Name="KT">The key type.</typeparam>
        /// <param name="actionType"></param>
        /// <param name="originatorKey"></param>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        /// <param name="settings"></param>
        protected virtual Task LogEventSource<KT>(string actionType, string originatorKey, KT key, E entity, RepositorySettings settings)
        {
            try
            {
                var data = new EventSourceEntry<KT, E>
                {
                    EntityType = typeof(E).Name,
                    EventType = actionType,
                    Entity = entity,
                    EntityKey = key,
                    EntitySource = settings?.Source,
                    EntitySourceId = settings?.SourceId,
                    EntitySourceName = settings?.SourceName
                };

                if (settings != null)
                {
                    data.BatchId = settings.BatchId;
                    data.CorrelationId = settings.CorrelationId;
                    data.EntityVersionOld = settings.VersionId;

                    data.EntityVersion = settings.VersionId;
                }

                Collector.Write(new EventSourceEvent { Entry = data, OriginatorId = originatorKey, UtcTimeStamp = DateTime.UtcNow }, true);
            }
            catch (Exception ex)
            {
                Collector?.LogException($"Exception thrown for log to event source on {typeof(E).Name}-{actionType}-{originatorKey}", ex);
            }

            return Task.CompletedTask;
        }
        #endregion

        //Profiling
        #region ProfileStart/ProfileEnd/ProfileRetry
        /// <summary>
        /// This method starts the request profile and creates the request holder with the profile id.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="prq">The incoming request.</param>
        /// <param name="prs">The outgoing response.</param>
        /// <returns>Returns the new request holder.</returns>
        protected virtual PersistenceRequestHolder<KT, ET> ProfileStart<KT, ET>(TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            Guid profileId;
            if (Policy.ResourceConsumer == null)
                profileId = Guid.NewGuid();
            else
                profileId = Policy.ResourceConsumer.Start(prq.Message.ToKey(), prq.Id);

            var holder = new PersistenceRequestHolder<KT, ET>(profileId, prq, prs);

            RequestsCurrent.TryAdd(holder.ProfileId, holder);

            return holder;
        }
        /// <summary>
        /// This method marks the incoming request as complete.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="holder">The request holder.</param>
        protected virtual void ProfileEnd<KT, ET>(PersistenceRequestHolder<KT, ET> holder)
        {
            Policy.ResourceConsumer?.End(holder.ProfileId, holder.Start, holder.result ?? ResourceRequestResult.Unknown);

            IPersistenceRequestHolder ok;
            RequestsCurrent.TryRemove(holder.ProfileId, out ok);

            PersistenceHandler handler;
            if (SupportedResolve(holder.Prq.Message.ToServiceMessageHeader(), out handler))
                handler.Record(holder);
        }

        /// <summary>
        /// This is called when the request is retried due to an underlying storage issue.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="holder">The request holder.</param>
        /// <param name="retryStart">The tick count of the retry point.</param>
        protected virtual void ProfileRetry<KT, ET>(PersistenceRequestHolder<KT, ET> holder, int retryStart)
        {
            Policy.ResourceConsumer?.Retry(holder.ProfileId, retryStart, holder.Rs.ShouldRetry ? ResourceRetryReason.Other : ResourceRetryReason.Timeout);

            holder.Retry(retryStart);

            StatisticsInternal.RetryIncrement();
        }
        #endregion
        #region RequestsCurrent
        /// <summary>
        /// This is the set of in play requests currently being processed.
        /// </summary>
        protected ConcurrentDictionary<Guid, IPersistenceRequestHolder> RequestsCurrent { get; } = new ConcurrentDictionary<Guid, IPersistenceRequestHolder>();

        #endregion

        //Requests
        #region Create        
        /// <summary>
        /// Processes the create entity request.
        /// </summary>
        /// <param name="holder">The holder.</param>
        protected virtual async Task ProcessCreate(PersistenceRequestHolder<K, E> holder)
        {
            K key = Repository.KeyMaker(holder.Rq.Entity);

            var result = await InternalCreate(key, holder);

            //if (CacheManagerActive && !CacheManager.IsReadOnly && result.IsSuccess)
            //    CacheManager.Write(Transform, result.Entity);

            ProcessOutput(holder, result);
        }
        /// <summary>
        /// The internal create method that is called if the cache handler is not valid.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns the response holder.</returns>
        protected virtual Task<RepositoryHolder<K, E>> InternalCreate(K key, PersistenceRequestHolder<K, E> holder)
            => Repository.Create(holder.Rq.Entity);
        #endregion

        #region Read        
        /// <summary>
        /// Processes the read request.
        /// </summary>
        /// <param name="holder">The holder.</param>
        protected virtual async Task ProcessRead(PersistenceRequestHolder<K, E> holder)
        {
            //IResponseHolder<E> result = null;

            var key = holder.Rq.Key;
            //if ((CacheManager?.IsActive ?? false) && holder.Rq.Settings.UseCache)
            //    result = await CacheManager.Read(Transform, holder.Rq.Key);

            //if (result == null || !result.IsSuccess)
            //{
            var result = await InternalRead(key, holder);

            //if ((CacheManager?.IsActive ?? false) && !CacheManager.IsReadOnly && result.IsSuccess)
            //    CacheManager.Write(Transform, result.Entity);
            //}
            //else
            //    result.IsCacheHit = true;

            ProcessOutput(holder, result);
        }
        /// <summary>
        /// Processes the read if the cache manager is not hit.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns the response holder.</returns>
        protected virtual Task<RepositoryHolder<K, E>> InternalRead(K key, PersistenceRequestHolder<K, E> holder)
            => Repository.Read(key);

        #endregion
        #region ReadByRef
        protected virtual async Task ProcessReadByRef(PersistenceRequestHolder<K, E> holder)
        {
            //IResponseHolder<E> result = null;

            //if ((CacheManager?.IsActive ?? false) && holder.Rq.Settings.UseCache)
            //    result = await CacheManager.Read(Transform, holder.Rq.KeyReference);

            //if (result == null || !result.IsSuccess)
            //{
            var result = await InternalReadByRef(holder.Rq.KeyReference, holder);

            //    if ((CacheManager?.IsActive ?? false) && !CacheManager.IsReadOnly && result.IsSuccess)
            //        CacheManager.Write(Transform, result.Entity);
            //}

            ProcessOutput(holder, result);
        }

        protected virtual Task<RepositoryHolder<K, E>> InternalReadByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, E> holder)
            => Repository.ReadByRef(reference.Item1, reference.Item2);

        #endregion

        #region Update
        protected virtual async Task ProcessUpdate(PersistenceRequestHolder<K, E> holder)
        {
            K key = Repository.KeyMaker(holder.Rq.Entity);

            //// Remove from the cache first to ensure no change of ending up with a stale cached item
            //// if the write to cache fails for any reason
            //if ((CacheManager?.IsActive ?? false))
            //    CacheManager.Delete(Transform, key);

            var result = await InternalUpdate(key, holder);

            //if ((CacheManager?.IsActive ?? false) && result.IsSuccess)
            //    CacheManager.Write(Transform, result.Entity);

            ProcessOutput(holder, result);
        }

        protected virtual Task<RepositoryHolder<K, E>> InternalUpdate(K key, PersistenceRequestHolder<K, E> holder)
            => Repository.Update(holder.Rq.Entity);

        #endregion

        #region Delete
        protected virtual async Task ProcessDelete(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            ////We presume that the delete will succeed and remove it from the cache before it is processed. 
            ////Worse case this will result in a cache miss.
            //if ((CacheManager?.IsActive ?? false))
            //    await CacheManager.Delete(Transform, holder.Rq.Key);

            var result = await InternalDelete(holder.Rq.Key, holder);

            ProcessOutput(holder, result);
        }

        protected virtual Task<RepositoryHolder<K, Tuple<K, string>>> InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
            => Repository.Delete(key);
        #endregion
        #region DeleteByRef
        protected virtual async Task ProcessDeleteByRef(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var result = await InternalDeleteByRef(holder.Rq.KeyReference, holder);

            //if ((CacheManager?.IsActive ?? false) && result.IsSuccess)
            //    await CacheManager.Delete(Transform, Transform.KeyDeserializer(result.Id));

            ProcessOutput(holder, result);
        }
        protected virtual Task<RepositoryHolder<K, Tuple<K, string>>> InternalDeleteByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
            => Repository.DeleteByRef(reference.Item1, reference.Item2);

        #endregion

        #region Version
        protected virtual async Task ProcessVersion(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            //IResponseHolder result = null;

            //if ((CacheManager?.IsActive ?? false))
            //    result = await CacheManager.VersionRead(Transform, holder.Rq.Key);

            //if (result == null || !result.IsSuccess)
            //{
            //if (Transform.Version == null)
            //    //If we don't set a version maker then how can we return the version.
            //    result = new PersistenceResponseHolder(PersistenceResponse.NotImplemented501) { IsSuccess = false };
            //else
            var result = await InternalVersion(holder.Rq.Key, holder);

            //    if ((CacheManager?.IsActive ?? false) && !CacheManager.IsReadOnly && result.IsSuccess)
            //        CacheManager.WriteVersion(Transform, holder.Rq.Key, result.VersionId);
            //}

            ProcessOutput(holder, result);
        }

        protected virtual Task<RepositoryHolder<K, Tuple<K, string>>> InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
            => Repository.Version(key);

        #endregion
        #region VersionByRef
        protected virtual async Task ProcessVersionByRef(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            //RepositoryHolder<K,> result = null;

            //if ((CacheManager?.IsActive ?? false))
            //    result = await CacheManager.VersionRead(Transform, holder.Rq.KeyReference);

            //if (result == null || !result.IsSuccess)
            //{
            var result = await InternalVersionByRef(holder.Rq.KeyReference, holder);
            //    if ((CacheManager?.IsActive ?? false) && !CacheManager.IsReadOnly && result.IsSuccess)
            //        CacheManager.WriteReference(Transform, holder.Rq.KeyReference, holder.Rq.Key, result.VersionId);
            //}
            //else
            //    holder.Rq.Key = Transform.KeyDeserializer(result.Id); // Pass back the entities actual id in the key field

            ProcessOutput(holder, result);
        }

        protected virtual Task<RepositoryHolder<K, Tuple<K, string>>> InternalVersionByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
            => Repository.VersionByRef(reference.Item1, reference.Item2);
        #endregion

        #region Search
        /// <summary>
        /// This is the entity search.
        /// </summary>
        /// <param name="holder">The is the entity search data.</param>
        /// <returns>This is an async task.</returns>
        protected virtual async Task ProcessSearch(PersistenceRequestHolder<SearchRequest, SearchResponse> holder)
        {
            var result = await InternalSearch(holder.Rq.Key, holder);

            ProcessOutput(holder, result);
        }

        protected virtual Task<RepositoryHolder<SearchRequest, SearchResponse>> InternalSearch(SearchRequest key, PersistenceRequestHolder<SearchRequest, SearchResponse> holder)
            => Repository.Search(key);

        #endregion
        #region SearchEntity
        /// <summary>
        /// This is the entity search.
        /// </summary>
        /// <param name="holder">The is the entity search data.</param>
        /// <returns>This is an async task.</returns>
        protected virtual async Task ProcessSearchEntity(PersistenceRequestHolder<SearchRequest, SearchResponse<E>> holder)
        {
            var result = await InternalSearchEntity(holder.Rq.Key, holder);

            ProcessOutput(holder, result);
        }

        protected virtual Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> InternalSearchEntity(SearchRequest key, PersistenceRequestHolder<SearchRequest, SearchResponse<E>> holder)
            => Repository.SearchEntity(key);

        #endregion

        #region History
        /// <summary>
        /// This is the entity history.
        /// </summary>
        /// <param name="holder">The is the entity history data.</param>
        /// <returns>This is an async task.</returns>
        protected virtual async Task ProcessHistory(PersistenceRequestHolder<HistoryRequest<K>, HistoryResponse<E>> holder)
        {
            holder.Rs.ResponseCode = (int)PersistenceResponse.NotImplemented501;
            holder.Rs.ResponseMessage = "History is not implemented.";
        }

        protected virtual Task<RepositoryHolder<HistoryRequest<K>, HistoryResponse<E>>> InternalHistory(HistoryRequest<K> key, PersistenceRequestHolder<HistoryRequest<K>, HistoryResponse<E>> holder)
            => Repository.History(key);


        #endregion

        #region ProcessOutput<KT,ET>...
        /// <summary>
        /// Processes and formats the output.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="holder">The holder.</param>
        /// <param name="result">The result.</param>
        protected virtual void ProcessOutput<KT, ET>(PersistenceRequestHolder<KT, ET> holder
            , RepositoryHolder<KT, ET> result)
        {
            holder.Rs.ResponseCode = result.ResponseCode;
            holder.Rs.ResponseMessage = result.ResponseMessage;

            holder.Rs.Entity = result.Entity;
            holder.Rs.Key = result.Key;
            holder.Rs.KeyReference = result.KeyReference;

            //holder.Rs.IsTimeout = result.;

            //holder.Rs.Settings.VersionId = Transform.Version?.EntityVersionAsString(rs.Entity);
            //holder.Rs.KeyReference = new Tuple<string, string>(rs.Key.ToString(), rs.Settings.VersionId);

            //result.
        }
        #endregion

        //Response Processing
        //#region ProcessOutputEntity...
        ///// <summary>
        ///// This method sets the entity and any associated metadata in to the response.
        ///// </summary>
        ///// <param name="entity">The entity.</param>
        ///// <param name="rq">The original request.</param>
        ///// <param name="rs">The outgoing response.</param>
        //protected virtual void ProcessOutputEntity(E entity, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs)
        //{
        //    rs.Entity = entity;
        //    rs.Key = Repository.KeyMaker(rs.Entity);
        //    rs.Settings.VersionId = Repository.VersionPolicy?.EntityVersionAsString(rs.Entity);

        //    rs.KeyReference = new Tuple<string, string>(rs.Key.ToString(), rs.Settings.VersionId);
        //}

        ///// <summary>
        ///// This method sets the entity and any associated metadata in to the response.
        ///// </summary>
        ///// <param name="key">The entity key.</param>
        ///// <param name="rq">The original request.</param>
        ///// <param name="rs">The outgoing response.</param>
        ///// <param name="holderResponse">The underlying storage response.</param>
        //protected virtual void ProcessOutputEntity(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, IResponseHolder<E> holderResponse)
        //{
        //    rs.ResponseCode = holderResponse.StatusCode;

        //    if (holderResponse.IsSuccess)
        //        ProcessOutputEntity(holderResponse.Entity, rq, rs);
        //    else
        //        ProcessOutputError(key, holderResponse, rs);
        //}
        ///// <summary>
        ///// This method sets the entity and any associated metadata in to the response.
        ///// </summary>
        ///// <param name="key">The entity key.</param>
        ///// <param name="rq">The original request.</param>
        ///// <param name="rs">The outgoing response.</param>
        ///// <param name="holderResponse">The underlying storage response.</param>
        //protected virtual void ProcessOutputEntity(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs
        //    , IResponseHolder holderResponse)
        //{
        //    rs.ResponseCode = holderResponse.StatusCode;

        //    if (holderResponse.IsSuccess)
        //        ProcessOutputEntity(Transform.PersistenceEntitySerializer.Deserializer(holderResponse.Content), rq, rs);
        //    else
        //        ProcessOutputError(key, holderResponse, rs);
        //}
        //#endregion
        //#region ProcessOutputError(K key, IResponseHolder holderResponse, PersistenceRepositoryHolder<K, E> rs)
        ///// <summary>
        ///// This method is used to format the response when the request is not successful.
        ///// </summary>
        ///// <param name="key">The entity key.</param>
        ///// <param name="holderResponse">The response.</param>
        ///// <param name="rs">The repository holder.</param>
        //protected virtual void ProcessOutputError(K key, IResponseHolder holderResponse, PersistenceRepositoryHolder<K, E> rs)
        //{
        //    if (holderResponse.Ex != null && !rs.IsTimeout)
        //        Collector?.LogException($"Error in persistence {typeof (E).Name}-{key}", holderResponse.Ex);
        //    else if (rs.ResponseCode != 404)
        //        Collector?.LogMessage(
        //            rs.IsTimeout ? LoggingLevel.Warning : LoggingLevel.Info,
        //            $"Error in persistence {typeof (E).Name}-{rs.ResponseCode}-{key}-{holderResponse.Ex?.ToString() ?? rs.ResponseMessage}", typeof(E).Name);

        //    rs.IsTimeout = holderResponse.IsTimeout;
        //}
        //#endregion
        //#region ProcessOutputKey...
        ///// <summary>
        ///// This method processes the common output method for key based operations such as delete and version.
        ///// </summary>
        ///// <param name="rq">The incoming request.</param>
        ///// <param name="rs">The outgoing response.</param>
        ///// <param name="holderResponse">The internal holder response.</param>
        //protected virtual void ProcessOutputKey(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs
        //    , IResponseHolder holderResponse)
        //{
        //    rs.Key = rq.Key;
        //    rs.KeyReference = rq.KeyReference;

        //    rs.ResponseCode = holderResponse.StatusCode;

        //    if (holderResponse.IsSuccess)
        //    {
        //        rs.Settings.VersionId = holderResponse.VersionId;
        //        rs.Entity = new Tuple<K, string>(rs.Key, holderResponse.VersionId);
        //        rs.KeyReference = new Tuple<string, string>(rs.Key == null ? null : rs.Key.ToString(), holderResponse.VersionId);
        //    }
        //    else
        //    {
        //        rs.IsTimeout = holderResponse.IsTimeout;
        //    }
        //}
        //#endregion

        #region DefaultTimeout
        /// <summary>
        /// Gets the default timeout. You can override this property, or set it in the constructor or policy.
        /// </summary>
        public virtual TimeSpan DefaultTimeout { get; } = TimeSpan.FromSeconds(10);
        #endregion

    } 
    #endregion
}
