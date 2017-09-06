#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base abstract persistence provider class.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <typeparam name="S">The persistence statistics type.</typeparam>
    /// <typeparam name="P">The persistence command policy type.</typeparam>
    public abstract class PersistenceCommandBase<K, E, S, P> : CommandBase<S, P, PersistenceHandler>, IPersistenceMessageHandler
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
        where P : PersistenceCommandPolicy, new()
    {
        /// <summary>
        /// This event is raised when an entity is created, changes, or is removed.
        /// </summary>
        public event EventHandler<EntityChangeEventArgs> OnEntityChangeAction;

        #region Declarations
        /// <summary>
        /// This is the entity transform holder.
        /// </summary>
        protected readonly EntityTransformHolder<K, E> mTransform;
        /// <summary>
        /// This is the cache manager.
        /// </summary>
        protected readonly ICacheManager<K, E> mCacheManager;
        /// <summary>
        /// This is the set of in play requests currently being processed.
        /// </summary>
        protected readonly ConcurrentDictionary<Guid, IPersistenceRequestHolder> mRequestsCurrent;

        /// <summary>
        /// This class holds the expression class.
        /// </summary>
        protected SearchExpressionHelper<E> mExpressionHelper = null;
        #endregion
        #region Constructor
        /// <summary>
        /// This constructor specifies whether the service should be registered as a shared service
        /// that can be called directly by other message handler and Microservice components.
        /// </summary>
        /// <param name="persistenceRetryPolicy"></param>
        /// <param name="resourceProfile"></param>
        /// <param name="cacheManager"></param>
        /// <param name="defaultTimeout"></param>
        /// <param name="entityName"></param>
        /// <param name="versionPolicy"></param>
        /// <param name="keyMaker"></param>
        /// <param name="persistenceEntitySerializer"></param>
        /// <param name="cachingEntitySerializer"></param>
        /// <param name="keySerializer"></param>
        /// <param name="keyDeserializer"></param>
        /// <param name="referenceMaker"></param>
        /// <param name="referenceHashMaker"></param>
        protected PersistenceCommandBase(
              PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultTimeout = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , Func<E, K> keyMaker = null
            , EntitySerializer<E> persistenceEntitySerializer = null
            , EntitySerializer<E> cachingEntitySerializer = null
            , Func<K, string> keySerializer = null
            , Func<string, K> keyDeserializer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            )
        {
            mTransform = EntityTransformCreate(entityName, versionPolicy, keyMaker
                , persistenceEntitySerializer, cachingEntitySerializer
                , keySerializer, keyDeserializer, referenceMaker, referenceHashMaker);

            mRequestsCurrent = new ConcurrentDictionary<Guid, IPersistenceRequestHolder>();

            mPolicy.DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(10);
            mPolicy.PersistenceRetryPolicy = persistenceRetryPolicy ?? new PersistenceRetryPolicy();
            mPolicy.ResourceProfile = resourceProfile;

            mCacheManager = cacheManager ?? new NullCacheManager<K, E>();
        }

        /// <summary>
        /// This constructor specifies whether the service should be registered as a shared service
        /// that can be called directly by other message handler and Microservice components.
        /// </summary>
        protected PersistenceCommandBase(
              EntityTransformHolder<K, E> entityTransform
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , TimeSpan? defaultTimeout = null
            )
        {
            if (entityTransform == null)
                throw new ArgumentNullException("entityTransform cannot be null");

            mTransform = entityTransform;

            mPolicy.DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(10);
            mPolicy.PersistenceRetryPolicy = persistenceRetryPolicy ?? new PersistenceRetryPolicy();
            mPolicy.ResourceProfile = resourceProfile;

            mCacheManager = cacheManager ?? new NullCacheManager<K, E>();
        }
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method recalculates the statistics for the persistence command.
        /// </summary>
        protected override void StatisticsRecalculate(S stats)
        {
            base.StatisticsRecalculate(stats);

            StatisticsInternal.RequestsInPlay = mRequestsCurrent.Values.Select((v) => v?.Debug).ToArray();

        } 
        #endregion

        #region FriendlyName
        /// <summary>
        /// Update to friendly name to make it clear which entity is being used
        /// </summary>
        public override string FriendlyName
        {
            get
            {
                return $"{base.FriendlyName}-{typeof(E).Name}";
            }
        }
        #endregion

        #region EntityTransformCreate...
        /// <summary>
        /// The transform holder manages the serialization and deserialization of the entity and key 
        /// for the entity and key, and identifies the references for the entity.
        /// </summary>
        /// <param name="entityName">The entity name.</param>
        /// <param name="versionPolicy">The version policy for the entity.</param>
        /// <param name="keyMaker">The keymaker function that creates a key for the entity.</param>
        /// <param name="persistenceEntitySerializer">Used to serialize / deserialize for persistence</param>
        /// <param name="cachingEntitySerializer">Used to serialize / deserialize for caching</param>
        /// <param name="keySerializer">The serializer that converts the key in to a string.</param>
        /// <param name="keyDeserializer">The deserializer that converts a string in to a key.</param>
        /// <param name="referenceMaker">A function that returns references from the entity in a set of string Tuples.</param>
        /// <param name="referenceHashMaker">A function that creates a safe hash from the reference tuple</param>
        /// <returns>Returns the transform holder.</returns>
        protected virtual EntityTransformHolder<K, E> EntityTransformCreate(
              string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , Func<E, K> keyMaker = null
            , EntitySerializer<E> persistenceEntitySerializer = null
            , EntitySerializer<E> cachingEntitySerializer = null
            , Func<K, string> keySerializer = null
            , Func<string, K> keyDeserializer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null)
        {
            var transform = new EntityTransformHolder<K, E>
            {
                KeyMaker = keyMaker,
                KeySerializer = keySerializer ?? (i => i.ToString()),
                KeyDeserializer = keyDeserializer,
                ReferenceMaker = referenceMaker ?? (e => new Tuple<string, string>[] {}),
                ReferenceHashMaker = referenceHashMaker ?? (r => $"{r.Item1.ToLowerInvariant()}.{r.Item2.ToLowerInvariant()}"),
                Version = versionPolicy ?? new VersionPolicy<E>(),
                EntityName = entityName ?? typeof (E).Name.ToLowerInvariant(),
                PersistenceEntitySerializer = persistenceEntitySerializer,
                CacheEntitySerializer = cachingEntitySerializer,
            };

            return transform;
        }
        #endregion
        #region EntityType
        /// <summary>
        /// This is the entity type Name used for matching request and payloadRs messages.
        /// </summary>
        public virtual string EntityType
        {
            get
            {
                return mTransform.EntityName;
            }
        }
        #endregion

        #region Start/Stop Internal
        /// <summary>
        /// This method starts the persistence command.
        /// </summary>
        protected override void StartInternal()
        {
            var resourceTracker = SharedServices.GetService<IResourceTracker>();
            if (resourceTracker != null && mPolicy.ResourceProfile != null)
                mPolicy.ResourceConsumer = resourceTracker.RegisterConsumer(EntityType, mPolicy.ResourceProfile);

            base.StartInternal();
        }
        /// <summary>
        /// This method stops the persistence command.
        /// </summary>
        protected override void StopInternal()
        {
            base.StopInternal();
            mPolicy.ResourceConsumer = null;
            mExpressionHelper = null;
        }
        #endregion

        #region CommandsRegister()
        /// <summary>
        /// This method register the supported commands for a persistence handler.
        /// </summary>
        protected override void CommandsRegister()
        {
            PersistenceCommandRegister<K, E>(EntityActions.Create, ProcessCreate, true, timeoutcorrect: TimeoutCorrectCreateUpdate);

            PersistenceCommandRegister<K, E>(EntityActions.Read, ProcessRead);
            PersistenceCommandRegister<K, E>(EntityActions.ReadByRef, ProcessReadByRef);

            PersistenceCommandRegister<K, E>(EntityActions.Update, ProcessUpdate, true, timeoutcorrect: TimeoutCorrectCreateUpdate);

            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.Delete, ProcessDelete, true, timeoutcorrect: TimeoutCorrectDelete);
            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.DeleteByRef, ProcessDeleteByRef, true, timeoutcorrect: TimeoutCorrectDelete);

            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.Version, ProcessVersion);
            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.VersionByRef, ProcessVersionByRef);

            PersistenceCommandRegister<SearchRequest, SearchResponse>(EntityActions.Search, ProcessSearch);
            PersistenceCommandRegister<HistoryRequest<K>, HistoryResponse<K>>(EntityActions.History, ProcessHistory);
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

                    var rsPayload = new TransmissionPayload(rsMessage, traceEnabled:mPolicy.TransmissionPayloadTraceEnabled);
                    
                    bool hasTimedOut = false;

                    try
                    {
                        RepositoryHolder<KT, ET> rqTemp = incoming.MessageObject as RepositoryHolder<KT, ET>;

                        //Deserialize the incoming payloadRq request
                        if (rqTemp == null)
                            rqTemp = PayloadSerializer.PayloadDeserialize<RepositoryHolder<KT, ET>>(incoming);

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
                                    Collector?.LogMessage(LoggingLevel.Warning, $"Timeout occured for {EntityType} {actionType} for request:{profileHolder.Rq} with response:{profileHolder.Rs}", "DBTimeout");

                                profileHolder.Rq.IsRetry = true;
                                //These should not be counted against the limit.
                                if (!profileHolder.Rs.ShouldRetry)
                                    profileHolder.Rq.Retry++;

                                profileHolder.Rq.IsTimeout = false;

                                retryExceeded = incoming.Cancel.IsCancellationRequested
                                    || profileHolder.Rq.Retry > mPolicy.PersistenceRetryPolicy.GetMaximumRetries(incoming);
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
                                    , string.Format("Recovered timeout sucessfully for {0}-{1} for request:{2} - response:{3}", EntityType, actionType, profileHolder.Rq, profileHolder.Rs)
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

                        rsPayload.MessageObject = reposHolder;
                        rsPayload.Message.Blob = PayloadSerializer.PayloadSerialize(reposHolder);

                        rsPayload.Message.Status = "200";

                        if (!profileHolder.result.HasValue)
                            profileHolder.result = ResourceRequestResult.Success;
                    }
                    catch (PayloadSerializationException payex)
                    {
                        incoming.SignalSuccess(); //It's a success as there isn't anything that we can do with it except send an error.
                        rsPayload.Message.Status = "422"; //Unprocessable Entity (WebDAV) - we use this to show there is an error with the payload.
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
        /// <param name="originatorKey">This is the message orination id.</param>
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
        /// leak sensitive information aboout the application and persistence agent.
        /// This method logs the error and assigns it a trackable id and sends that instead to the 
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

                Collector.Write(new EventSourceEvent {Entry = data, OriginatorId = originatorKey, UtcTimeStamp = DateTime.UtcNow}, true);
            }
            catch (Exception ex)
            {
                Collector?.LogException($"Exception thrown for log to event source on {typeof (E).Name}-{actionType}-{originatorKey}", ex);
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
            if (mPolicy.ResourceConsumer == null)
                profileId = Guid.NewGuid();
            else
                profileId = mPolicy.ResourceConsumer.Start(prq.Message.ToKey(), prq.Id);

            var holder = new PersistenceRequestHolder<KT, ET>(profileId, prq, prs);

            mRequestsCurrent.TryAdd(holder.ProfileId, holder);

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
            mPolicy.ResourceConsumer?.End(holder.ProfileId, holder.Start, holder.result ?? ResourceRequestResult.Unknown);

            IPersistenceRequestHolder ok;
            mRequestsCurrent.TryRemove(holder.ProfileId, out ok);

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
            mPolicy.ResourceConsumer?.Retry(holder.ProfileId, retryStart, holder.Rs.ShouldRetry ? ResourceRetryReason.Other : ResourceRetryReason.Timeout);

            holder.Retry(retryStart);

            StatisticsInternal.RetryIncrement();
        }
        #endregion

        //Requests
        #region Create
        protected virtual async Task ProcessCreate(PersistenceRequestHolder<K, E> holder)
        {
            K key = mTransform.KeyMaker(holder.Rq.Entity);

            var result = await InternalCreate(key, holder);

            if (mCacheManager.IsActive && !mCacheManager.IsReadOnly && result.IsSuccess)
                mCacheManager.Write(mTransform, result.Entity);

            ProcessOutputEntity(key, holder.Rq, holder.Rs, result);
        }

        protected virtual async Task<IResponseHolder<E>> InternalCreate(K key, PersistenceRequestHolder<K, E> holder)
        {
            return new PersistenceResponseHolder<E>(PersistenceResponse.NotImplemented501);
        }

        #endregion

        #region Read
        protected virtual async Task ProcessRead(PersistenceRequestHolder<K, E> holder)
        {
            IResponseHolder<E> result = null;

            if (mCacheManager.IsActive && holder.Rq.Settings.UseCache)
                result = await mCacheManager.Read(mTransform, holder.Rq.Key);

            if (result == null || !result.IsSuccess)
            {
                result = await InternalRead(holder.Rq.Key, holder);

                if (mCacheManager.IsActive && !mCacheManager.IsReadOnly && result.IsSuccess)
                    mCacheManager.Write(mTransform, result.Entity);
            }
            else
                result.IsCacheHit = true;

            ProcessOutputEntity(holder.Rq.Key, holder.Rq, holder.Rs, result);
        }

        protected async virtual Task<IResponseHolder<E>> InternalRead(K key, PersistenceRequestHolder<K, E> holder)
        {
            return new PersistenceResponseHolder<E>(PersistenceResponse.NotImplemented501);
        }
        #endregion
        #region ReadByRef
        protected virtual async Task ProcessReadByRef(PersistenceRequestHolder<K, E> holder)
        {
            IResponseHolder<E> result = null;

            if (mCacheManager.IsActive && holder.Rq.Settings.UseCache)
                result = await mCacheManager.Read(mTransform, holder.Rq.KeyReference);

            if (result == null || !result.IsSuccess)
            {
                result = await InternalReadByRef(holder.Rq.KeyReference, holder);

                if (mCacheManager.IsActive && !mCacheManager.IsReadOnly && result.IsSuccess)
                    mCacheManager.Write(mTransform, result.Entity);
            }

            ProcessOutputEntity(holder.Rq.Key, holder.Rq, holder.Rs, result);
        }

        protected async virtual Task<IResponseHolder<E>> InternalReadByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, E> holder)
        {
            return new PersistenceResponseHolder<E>(PersistenceResponse.NotImplemented501);
        }
        #endregion

        #region Update
        protected virtual async Task ProcessUpdate(PersistenceRequestHolder<K, E> holder)
        {
            K key = mTransform.KeyMaker(holder.Rq.Entity);

            // Remove from the cache first to ensure no change of ending up with a stale cached item
            // if the write to cache fails for any reason
            if (mCacheManager.IsActive)
                mCacheManager.Delete(mTransform, key);

            var result = await InternalUpdate(key, holder);

            if (mCacheManager.IsActive && result.IsSuccess)
                mCacheManager.Write(mTransform, result.Entity);

            ProcessOutputEntity(key, holder.Rq, holder.Rs, result);
        }

        protected virtual async Task<IResponseHolder<E>> InternalUpdate(K key, PersistenceRequestHolder<K, E> holder)
        {
            return new PersistenceResponseHolder<E>(PersistenceResponse.NotImplemented501);
        }
        #endregion

        #region Delete
        protected virtual async Task ProcessDelete(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            //We presume that the delete will succeed and remove it from the cache before it is processed. 
            //Worse case this will result in a cache miss.
            if (mCacheManager.IsActive)
                await mCacheManager.Delete(mTransform, holder.Rq.Key);

            var result = await InternalDelete(holder.Rq.Key, holder);

            ProcessOutputKey(holder.Rq, holder.Rs, result);
        }

        protected virtual async Task<IResponseHolder> InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return new PersistenceResponseHolder(PersistenceResponse.NotImplemented501);
        }
        #endregion
        #region DeleteByRef
        protected virtual async Task ProcessDeleteByRef(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var result = await InternalDeleteByRef(holder.Rq.KeyReference, holder);

            if (mCacheManager.IsActive && result.IsSuccess)
                await mCacheManager.Delete(mTransform, mTransform.KeyDeserializer(result.Id));

            ProcessOutputKey(holder.Rq, holder.Rs, result);
        }
        protected virtual async Task<IResponseHolder> InternalDeleteByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return new PersistenceResponseHolder(PersistenceResponse.NotImplemented501);
        }
        #endregion

        #region Version
        protected virtual async Task ProcessVersion(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            IResponseHolder result = null;

            if (mCacheManager.IsActive)
                result = await mCacheManager.VersionRead(mTransform, holder.Rq.Key);

            if (result == null || !result.IsSuccess)
            {
                if (mTransform.Version == null)
                    //If we don't set a version maker then how can we return the version.
                    result = new PersistenceResponseHolder(PersistenceResponse.NotImplemented501) { IsSuccess = false };
                else
                    result = await InternalVersion(holder.Rq.Key, holder);

                if (mCacheManager.IsActive && !mCacheManager.IsReadOnly && result.IsSuccess)
                    mCacheManager.WriteVersion(mTransform, holder.Rq.Key, result.VersionId);
            }

            ProcessOutputKey(holder.Rq, holder.Rs, result);
        }

        protected virtual async Task<IResponseHolder> InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return new PersistenceResponseHolder(PersistenceResponse.NotImplemented501) { IsSuccess = false };
        }
        #endregion
        #region VersionByRef
        protected virtual async Task ProcessVersionByRef(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            IResponseHolder result = null;

            if (mCacheManager.IsActive)
                result = await mCacheManager.VersionRead(mTransform, holder.Rq.KeyReference);

            if (result == null || !result.IsSuccess)
            {
                result = await InternalVersionByRef(holder.Rq.KeyReference, holder);
                if (mCacheManager.IsActive && !mCacheManager.IsReadOnly && result.IsSuccess)
                    mCacheManager.WriteReference(mTransform, holder.Rq.KeyReference, holder.Rq.Key, result.VersionId);
            }
            else
                holder.Rq.Key = mTransform.KeyDeserializer(result.Id); // Pass back the entities actual id in the key field

            ProcessOutputKey(holder.Rq, holder.Rs, result);
        }

        protected virtual async Task<IResponseHolder> InternalVersionByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return new PersistenceResponseHolder(PersistenceResponse.NotImplemented501) { IsSuccess = false };
        }
        #endregion

        #region Search
        /// <summary>
        /// This is the entity search.
        /// </summary>
        /// <param name="holder">The is the entity search data.</param>
        /// <returns>This is an async task.</returns>
        protected virtual async Task ProcessSearch(PersistenceRequestHolder<SearchRequest, SearchResponse> holder)
        {
            IResponseHolder<SearchResponse> result = null;

            result = await InternalSearch(holder.Rq.Key, holder);

            holder.Rs.Entity = result.Entity;

            holder.Rs.Key = holder.Rq.Key;

            //rs.Settings.VersionId = mTransform.Version?.EntityVersionAsString(rs.Entity);

            //rs.KeyReference = new Tuple<string, string>(rs.Key.ToString(), rs.Settings.VersionId);

            holder.Rs.ResponseCode = (int)result.StatusCode;
            //holder.Rs.ResponseMessage = "Search is not implemented.";
        }


        protected async virtual Task<IResponseHolder<SearchResponse>> InternalSearch(SearchRequest key, PersistenceRequestHolder<SearchRequest, SearchResponse> holder)
        {
            return new PersistenceResponseHolder<SearchResponse>(PersistenceResponse.NotImplemented501);
        }
        #endregion
        #region History
        /// <summary>
        /// This is the entity history.
        /// </summary>
        /// <param name="holder">The is the entity history data.</param>
        /// <returns>This is an async task.</returns>
        protected virtual async Task ProcessHistory(PersistenceRequestHolder<HistoryRequest<K>, HistoryResponse<K>> holder)
        {
            holder.Rs.ResponseCode = (int)PersistenceResponse.NotImplemented501;
            holder.Rs.ResponseMessage = "History is not implemented.";
        }
        #endregion

        //Response Processing
        #region ProcessOutputEntity...
        /// <summary>
        /// This method sets the entity and any associated metadata in to the response.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="rq">The original request.</param>
        /// <param name="rs">The outgoing response.</param>
        /// <param name="holderResponse">The underlying storage response.</param>
        protected virtual void ProcessOutputEntity(E entity, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs)
        {
            rs.Entity = entity;
            rs.Key = mTransform.KeyMaker(rs.Entity);
            rs.Settings.VersionId = mTransform.Version?.EntityVersionAsString(rs.Entity);

            rs.KeyReference = new Tuple<string, string>(rs.Key.ToString(), rs.Settings.VersionId);
        }

        /// <summary>
        /// This method sets the entity and any associated metadata in to the response.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="rq">The original request.</param>
        /// <param name="rs">The outgoing response.</param>
        /// <param name="holderResponse">The underlying storage response.</param>
        protected virtual void ProcessOutputEntity(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, IResponseHolder<E> holderResponse)
        {
            rs.ResponseCode = holderResponse.StatusCode;

            if (holderResponse.IsSuccess)
                ProcessOutputEntity(holderResponse.Entity, rq, rs);
            else
                ProcessOutputError(key, holderResponse, rs);
        }
        /// <summary>
        /// This method sets the entity and any associated metadata in to the response.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="rq">The original request.</param>
        /// <param name="rs">The outgoing response.</param>
        /// <param name="holderResponse">The underlying storage response.</param>
        protected virtual void ProcessOutputEntity(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs
            , IResponseHolder holderResponse)
        {
            rs.ResponseCode = holderResponse.StatusCode;

            if (holderResponse.IsSuccess)
                ProcessOutputEntity(mTransform.PersistenceEntitySerializer.Deserializer(holderResponse.Content), rq, rs);
            else
                ProcessOutputError(key, holderResponse, rs);
        }
        #endregion
        #region ProcessOutputError(K key, IResponseHolder holderResponse, PersistenceRepositoryHolder<K, E> rs)
        /// <summary>
        /// This method is used to format the response when the request is not successful.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="holderResponse">The response.</param>
        /// <param name="rs">The repository holder.</param>
        protected virtual void ProcessOutputError(K key, IResponseHolder holderResponse, PersistenceRepositoryHolder<K, E> rs)
        {
            if (holderResponse.Ex != null && !rs.IsTimeout)
                Collector?.LogException($"Error in persistence {typeof (E).Name}-{key}", holderResponse.Ex);
            else if (rs.ResponseCode != 404)
                Collector?.LogMessage(
                    rs.IsTimeout ? LoggingLevel.Warning : LoggingLevel.Info,
                    $"Error in persistence {typeof (E).Name}-{rs.ResponseCode}-{key}-{holderResponse.Ex?.ToString() ?? rs.ResponseMessage}", typeof(E).Name);

            rs.IsTimeout = holderResponse.IsTimeout;
        }
        #endregion
        #region ProcessOutputKey...
        /// <summary>
        /// This method processes the common output method for key based operations such as delete and version.
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <param name="rs">The outgoing response.</param>
        /// <param name="holderResponse">The internal holder response.</param>
        protected virtual void ProcessOutputKey(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs
            , IResponseHolder holderResponse)
        {
            rs.Key = rq.Key;
            rs.KeyReference = rq.KeyReference;

            rs.ResponseCode = holderResponse.StatusCode;

            if (holderResponse.IsSuccess)
            {
                rs.Settings.VersionId = holderResponse.VersionId;
                rs.Entity = new Tuple<K, string>(rs.Key, holderResponse.VersionId);
                rs.KeyReference = new Tuple<string, string>(rs.Key == null ? null : rs.Key.ToString(), holderResponse.VersionId);
            }
            else
            {
                rs.IsTimeout = holderResponse.IsTimeout;
            }
        }
        #endregion
    }
}
