#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    public abstract class PersistenceMessageHandlerBase<K, E>: PersistenceMessageHandlerBase<K,E, PersistenceStatistics>
    where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// This constructor specifies whether the service should be registered as a shared service
        /// that can be called directly by other message handler and Microservice components.
        /// </summary>
        /// <param name="persistenceRetryPolicy"></param>
        protected PersistenceMessageHandlerBase(PersistenceRetryPolicy persistenceRetryPolicy = null, ResourceProfile resourceProfile = null)
            :base(persistenceRetryPolicy, resourceProfile)
        {
        }
        #endregion
    }

    /// <summary>
    /// This is the base entity provider class.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public abstract class PersistenceMessageHandlerBase<K,E,S> : MessageHandlerBase<S>, IPersistenceMessageHandler, IRequireSharedServices
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
    {
        #region Declarations
        /// <summary>
        /// This function is used by optimistic locking, it is used to define the version id for the entity.
        /// </summary>
        protected readonly VersionPolicy<E> mVersion;

        /// <summary>
        /// This is the default time allowed when making a call to the underlying persistence layer.
        /// </summary>
        protected readonly TimeSpan? mDefaultTimeout;
        /// <summary>
        /// This is the entity name.
        /// </summary>
        protected readonly string mEntityName;

        /// <summary>
        /// This function can be set to make the key from the entity.
        /// </summary>
        protected readonly Func<E, K> mKeyMaker;
        /// <summary>
        /// This function can be used to extract the references from an incoming entity to allow for caching.
        /// </summary>
        protected readonly Func<E, IEnumerable<KeyValuePair<string,string>>> mReferenceMaker;

        /// <summary>
        /// The resource consumer 
        /// </summary>
        protected IResourceConsumer mResourceConsumer;

        /// <summary>
        /// Specifies the persistence retry policy
        /// </summary>
        protected readonly PersistenceRetryPolicy mPersistenceRetryPolicy;

        protected readonly ResourceProfile mResourceProfile;

        protected readonly ICacheManager<K, E> mCacheManager;
        #endregion
        #region Constructor
        /// <summary>
        /// This constructor specifies whether the service should be registered as a shared service
        /// that can be called directly by other message handler and Microservice components.
        /// </summary>
        /// <param name="persistenceRetryPolicy"></param>
        protected PersistenceMessageHandlerBase(
              PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , Func<E, K> keyMaker = null
            , Func<E, IEnumerable<KeyValuePair<string, string>>> referenceMaker = null
            )
        {
            mKeyMaker = keyMaker;
            mReferenceMaker = referenceMaker;
            mVersion = versionPolicy ?? new VersionPolicy<E>();
            mDefaultTimeout = defaultTimeout;

            mPersistenceRetryPolicy = persistenceRetryPolicy ?? new PersistenceRetryPolicy();
            mResourceProfile = resourceProfile;
            mCacheManager = cacheManager ?? new NullCacheManager<K, E>();
            mEntityName = entityName ?? typeof(E).Name.ToLowerInvariant();
        }
        #endregion

        #region ChannelId
        /// <summary>
        /// This is the channelId that the message will be received on.
        /// </summary>
        public virtual string ChannelId { get; set; } 
        #endregion
        #region EntityType
        /// <summary>
        /// This is the entity type Name used for matching request and payloadRs messages.
        /// </summary>
        public virtual string EntityType
        {
            get
            {
                return typeof(E).Name;
            }
        }
        #endregion

        #region Start/Stop Internal
        protected override void StartInternal()
        {
            base.StartInternal();

            var resourceTracker = SharedServices.GetService<IResourceTracker>();
            if (resourceTracker != null && mResourceProfile != null)
                mResourceConsumer = resourceTracker.RegisterConsumer(EntityType, mResourceProfile);
        }
        protected override void StopInternal()
        {
            mResourceConsumer = null;

            base.StopInternal();
        }
        #endregion

        #region SharedServices
        /// <summary>
        /// This is the shared service connector
        /// </summary>
        public virtual ISharedService SharedServices
        {
            get; set;
        } 
        #endregion

        #region CommandsRegister()
        /// <summary>
        /// This method register the supported commands for a persistence handler.
        /// </summary>
        public override void CommandsRegister()
        {
            PersistenceCommandRegister<K, E>(EntityActions.Create, ProcessCreate, true, timeoutcorrect: TimeoutCorrectCreateUpdate);

            PersistenceCommandRegister<K, E>(EntityActions.Read, ProcessRead);
            PersistenceCommandRegister<K, E>(EntityActions.ReadByRef, ProcessReadByRef);

            PersistenceCommandRegister<K, E>(EntityActions.Update, ProcessUpdate, true, timeoutcorrect: TimeoutCorrectCreateUpdate);

            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.Delete, ProcessDelete, true, timeoutcorrect: TimeoutCorrectDelete);
            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.DeleteByRef, ProcessDeleteByRef, true, timeoutcorrect: TimeoutCorrectDelete);

            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.Version, ProcessVersion);
            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.VersionByRef, ProcessVersionByRef);

            PersistenceCommandRegister<K, Tuple<K, string>>(EntityActions.Search, ProcessSearch);
        }
        #endregion

        #region TimeoutCorrectCreateUpdate...
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
        protected virtual async Task<bool> TimeoutCorrectCreateUpdate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload m, List<TransmissionPayload> l)
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
        protected virtual async Task<bool> TimeoutCorrectDelete(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload m, List<TransmissionPayload> l)
        {
            return false;
        } 
        #endregion

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
        protected void Log<KT, ET>(string action, TransmissionPayload pr,
            PersistenceRepositoryHolder<KT, ET> rq, PersistenceRepositoryHolder<KT, ET> rs, LoggingLevel loggingLevel = LoggingLevel.Info, string message = null, string category = null)
        {
            var logEvent = new PersistencePayloadLogEvent(pr, rq, rs, loggingLevel) {Message = message ?? string.Empty, Category = category};
            Logger.Log(logEvent);
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
        protected void LogException<KT, ET>(string action,
            TransmissionPayload prq, Exception ex, PersistenceRepositoryHolder<KT, ET> rq, PersistenceRepositoryHolder<KT, ET> rs)
        {

            try
            {
                var logEvent = new PersistencePayloadLogEvent(prq, rq, rs, LoggingLevel.Info, ex);
                Logger.Log(logEvent);

                Guid errorId = Guid.NewGuid();
                string errMessage = string.Format("Exception tracker {0}/{1}/{2}", action, (prq != null && prq.Message != null ? prq.Message.OriginatorKey : string.Empty), errorId);
                rs.ResponseMessage = errMessage;

                if (rq != null)
                    errMessage += string.Format("/{0}-{1}", rq.Key, (rq.Entity == null) ? string.Empty : rq.Entity.ToString());

                Logger.LogException(errMessage, ex);
            }
            catch (Exception)
            {
                // Do not fail due to an issue logging
            }

            rs.ResponseCode = 500;
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
        protected async virtual Task LogEventSource<KT, ET>(string actionType, TransmissionPayload prq, PersistenceRepositoryHolder<KT, ET> rq, PersistenceRepositoryHolder<KT, ET> rs)
        {
            // Only pass through the entity if is of the correct entity type. ET might be an entity or it might be a Tuple<K, string>> in which case pass a null  
            E entity = typeof (ET) == typeof (E) ? (E)Convert.ChangeType(rs.Entity, typeof(E)) : default(E);
            await LogEventSource(actionType, prq.Message.OriginatorKey, rs.Key, entity, rq.Settings);
        }
        #endregion
        #region LogEventSource<KT, ET>...

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
        protected async virtual Task LogEventSource<KT>(string actionType, string originatorKey, KT key, E entity, RepositorySettings settings)
        {
            try
            {
                var data = new EventSourceEntry<KT, E>
                {
                    EntityType = typeof(E).Name,
                    EventType = actionType,
                    Entity = entity,
                    EntityKey = key
                };

                if (settings != null)
                {
                    data.BatchId = settings.BatchId;
                    data.CorrelationId = settings.CorrelationId;
                    data.EntityVersionOld = settings.VersionId;

                    data.EntityVersion = settings.VersionId;
                }

                await EventSource.Write(originatorKey, data, sync: true);
            }
            catch (Exception ex)
            {
                Logger.LogException(string.Format("Exception thrown for log to event source on {0}-{1}-{2}", typeof(E).Name, actionType, originatorKey), ex);
            }
        }
        #endregion

        #region Profile
        protected virtual Guid ProfileStart(TransmissionPayload payload)
        {
            if (mResourceConsumer == null)
                return Guid.NewGuid();

            return mResourceConsumer.Start(payload.Message.ToKey(), payload.Id);
        }

        protected virtual void ProfileEnd(Guid profileId, int start, ResourceRequestResult result)
        {
            if (mResourceConsumer == null)
                return;

            mResourceConsumer.End(profileId, start, result);
        }

        protected virtual void ProfileRetry(Guid profileId, int retryStart, ResourceRetryReason reason)
        {
            if (mResourceConsumer == null)
                return;

            mResourceConsumer.Retry(profileId, retryStart, reason);
        } 
        #endregion

        #region PersistenceCommandRegister<KT,ET>...
        /// <summary>
        /// This method registers the specific persistence handler.
        /// </summary>
        /// <typeparam Name="KT">The key type.</typeparam>
        /// <typeparam Name="ET">The entity type.</typeparam>
        /// <param Name="actionType">The action type identifier</param>
        /// <param Name="action">The action to process the command.</param>
        protected virtual void PersistenceCommandRegister<KT, ET>(string actionType,
              Func<PersistenceRepositoryHolder<KT, ET>, PersistenceRepositoryHolder<KT, ET>, TransmissionPayload, List<TransmissionPayload>, Task> action
            , bool logEventOnSuccess = false
            , Func<PersistenceRepositoryHolder<KT, ET>, PersistenceRepositoryHolder<KT, ET>, TransmissionPayload, List<TransmissionPayload>, Task<bool>> preaction = null
            , Func<PersistenceRepositoryHolder<KT, ET>, PersistenceRepositoryHolder<KT, ET>, TransmissionPayload, List<TransmissionPayload>, Task> postaction = null
            , Func<PersistenceRepositoryHolder<KT, ET>, PersistenceRepositoryHolder<KT, ET>, TransmissionPayload, List<TransmissionPayload>, Task<bool>> timeoutcorrect = null
            , int? retryOnTimeout = null
            , string channelId = null
            , string entityType = null
            )
        {
            Func<TransmissionPayload, List<TransmissionPayload>, Task> actionPayload = async (m, l) =>
            {
                int start = Environment.TickCount;
                ResourceRequestResult? result = null;
                Guid profileId = ProfileStart(m);
                try
                {
                    var rsMessage = m.Message.ToResponse();

                    rsMessage.ChannelId = m.Message.ResponseChannelId;
                    rsMessage.ChannelPriority = m.Message.ResponseChannelPriority;
                    rsMessage.MessageType = m.Message.MessageType;
                    rsMessage.ActionType = "";

                    var rsPayload = new TransmissionPayload(rsMessage);

                    PersistenceRepositoryHolder<KT, ET> rq = null;
                    PersistenceRepositoryHolder<KT, ET> rs = null;

                    bool hasTimedOut = false;

                    try
                    {
                        RepositoryHolder<KT, ET> rqTemp = m.MessageObject as RepositoryHolder<KT, ET>;
                        //Deserialize the incoming payloadRq request
                        if (rqTemp == null)
                            rqTemp = PayloadSerializer.PayloadDeserialize<RepositoryHolder<KT, ET>>(m);

                        rq = new PersistenceRepositoryHolder<KT, ET>(rqTemp);

                        if (rq.Timeout == null)
                            rq.Timeout = TimeSpan.FromSeconds(10);

                        bool preactionFailed = false;

                        try
                        {
                            bool retryExceeded = false;

                            do
                            {
                                int attempt = Environment.TickCount;
                                //Create the payloadRs holder
                                rs = new PersistenceRepositoryHolder<KT, ET>();

                                if (preaction != null && !(await preaction(rq, rs, m, l)))
                                {
                                    preactionFailed = true;
                                    break;
                                }

                                //Call the specific command to process the action, i.e Create, Read, Update, Delete etc.
                                await action(rq, rs, m, l);

                                //Flag if the request times out at any point. 
                                //This may be required later when checking whether the action was actually successful.
                                hasTimedOut |= rs.IsTimeout;

                                //OK, if this is not a time out then it is successful
                                if (!rs.IsTimeout && !rs.ShouldRetry)
                                    break;

                                ProfileRetry(profileId, attempt, rs.ShouldRetry?ResourceRetryReason.Other:ResourceRetryReason.Timeout);

                                mStatistics.RetryIncrement();
                                Logger.LogMessage(LoggingLevel.Info, string.Format("Timeout occured for {0} {1} for request:{2}", EntityType, actionType, rq), "DBTimeout");

                                rq.IsRetry = true;
                                //These should not be counted against the limit.
                                if (!rs.ShouldRetry)
                                    rq.Retry++;
                                rq.IsTimeout = false;

                                retryExceeded = m.Cancel.IsCancellationRequested || rq.Retry > mPersistenceRetryPolicy.GetMaximumRetries(m);
                            }
                            while (!retryExceeded);

                            //Signal to the underlying comms channel that the message has been processed successfully.
                            m.Signal(!retryExceeded);

                            // If we have exceeded the retry limit then Log error
                            if (retryExceeded)
                            {
                                Log(actionType, m, rq, rs, LoggingLevel.Error, string.Format("Retry limit has been exceeded (cancelled ({0})) for {1} {2} for {3}", m.Cancel.IsCancellationRequested, EntityType, actionType, rq), "DBRetry");
                                result = ResourceRequestResult.RetryExceeded;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException(actionType, m, ex, rq, rs);
                            m.SignalFail();
                            result = ResourceRequestResult.Exception;
                        }

                        bool logEventSource = !preactionFailed && logEventOnSuccess && rs.IsSuccess;

                        if (!rs.IsSuccess && hasTimedOut && timeoutcorrect != null && result != ResourceRequestResult.Exception)
                        {
                            if (await timeoutcorrect(rq, rs, m, l))
                            {
                                logEventSource = true;
                                Logger.LogMessage(LoggingLevel.Info, string.Format("Recovered timeout sucessfully for {0}-{1} for request:{2} - response:{3}", EntityType, actionType, rq, rs), "DBTimeout");
                            }
                            else
                            {
                                Logger.LogMessage(LoggingLevel.Error, string.Format("Not recovered timeout for {0}-{1} for request:{2} - response:{3}", EntityType, actionType, rq, rs), "DBTimeout");
                            }
                        }

                        if (logEventSource && rs.ShouldLogEventSource)
                            await LogEventSource(actionType, m, rq, rs);

                        if (!preactionFailed && postaction != null)
                            await postaction(rq, rs, m, l);

                        //Serialize the payloadRs
                        var reposHolder = rs.ToRepositoryHolder();
                        rsPayload.MessageObject = reposHolder;
                        rsPayload.Message.Blob = PayloadSerializer.PayloadSerialize(reposHolder);

                        rsPayload.Message.Status = "200";

                        if (!result.HasValue)
                            result = ResourceRequestResult.Success;
                    }
                    catch (Exception ex)
                    {
                        m.SignalFail();
                        rsPayload.Message.Status = "500";
                        rsPayload.Message.StatusDescription = ex.Message;
                        Logger.LogException(string.Format("Error processing message (was cancelled({0}))-{1}-{2}-{3}", m.Cancel.IsCancellationRequested, EntityType, actionType, rq), ex);
                        result = ResourceRequestResult.Exception;
                    }

                    // check whether we need to send a response message. If this is async and AsyncResponse is not set to true,
                    // then by default we do not send a response message to cut down on unnecessary traffic.
                    if (rq == null || rq.Settings == null || !rq.Settings.ProcessAsync)
                        l.Add(rsPayload);
                }
                finally
                {
                    ProfileEnd(profileId, start, result ?? ResourceRequestResult.Unknown);
                }
            };

            if (channelId == null)
                channelId = ChannelId ?? string.Empty;

            if (entityType == null)
                entityType=EntityType;

            CommandRegister(
                  channelId.ToLowerInvariant()
                , entityType.ToLowerInvariant()
                , actionType.ToLowerInvariant()
                , actionPayload);
        }
        #endregion

        #region Create
        protected virtual async Task ProcessCreate(
            PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var result = await InternalCreate(rq.Key, rq, rs, prq, prs);

            if (mCacheManager.IsActive && !mCacheManager.IsReadOnly && result.IsSuccess)
                mCacheManager.Write(rq.Key, result);

            ProcessOutputEntity(rq.Key, rq, rs, result);
        }

        protected virtual async Task<IResponseHolder> InternalCreate(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return new ResponseHolderBase() { StatusCode = 501, StatusMessage = "Not implemented." };
        }

        #endregion

        #region Read
        protected virtual async Task ProcessRead(
            PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            IResponseHolder result = null;

            if (mCacheManager.IsActive && rq.Settings.UseCache)
                result = await mCacheManager.Read(rq.Key);

            if (result == null || !result.IsSuccess)
            {
                result = await InternalRead(rq.Key, rq, rs, prq, prs);

                if (mCacheManager.IsActive && !mCacheManager.IsReadOnly && result.IsSuccess)
                    mCacheManager.Write(rq.Key, result);
            }

            ProcessOutputEntity(rq.Key, rq, rs, result);
        }

        protected async virtual Task<IResponseHolder> InternalRead(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return new ResponseHolderBase() { StatusCode = 501, StatusMessage = "Not implemented." };
        }
        #endregion
        #region ReadByRef
        protected virtual async Task ProcessReadByRef(
            PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            rs.ResponseCode = 501;
            rs.ResponseMessage = "Not implemented.";
        }
        #endregion

        #region Update
        protected virtual async Task ProcessUpdate(
            PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var result = await InternalUpdate(rq.Key, rq, rs, prq, prs);

            if (mCacheManager.IsActive && result.IsSuccess)
                mCacheManager.Write(rq.Key, result);

            ProcessOutputEntity(rq.Key, rq, rs, result);
        }

        protected virtual async Task<IResponseHolder> InternalUpdate(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return new ResponseHolderBase() { StatusCode = 501, StatusMessage = "Not implemented." };
        }
        #endregion

        #region Delete
        protected virtual async Task ProcessDelete(
            PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var result = await InternalDelete(rq.Key, rq, rs, prq, prs);

            if (mCacheManager.IsActive && !mCacheManager.IsReadOnly && result.IsSuccess)
                await mCacheManager.Delete(rq.Key);

            ProcessOutputKey(rq, rs, result);
        }

        protected virtual async Task<IResponseHolder> InternalDelete(K key, PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return new ResponseHolderBase() { StatusCode = 501, StatusMessage = "Not implemented." };
        }
        #endregion
        #region DeleteByRef
        protected virtual async Task ProcessDeleteByRef(
            PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            rs.ResponseCode = 501;
            rs.ResponseMessage = "Not implemented.";
        }
        #endregion

        #region Version
        protected virtual async Task ProcessVersion(
            PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            IResponseHolder result = null;

            if (mCacheManager.IsActive)
                result = await mCacheManager.VersionRead(rq.Key);

            if (result == null || !result.IsSuccess)
            {
                result = await InternalVersion(rq.Key, rq, rs, prq, prs);

                if (mCacheManager.IsActive && !mCacheManager.IsReadOnly && result.IsSuccess)
                    mCacheManager.VersionWrite(rq.Key, result);
            }

            ProcessOutputKey(rq, rs, result);
        }

        protected virtual async Task<IResponseHolder> InternalVersion(K key, PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            return new ResponseHolderBase() { StatusCode = 501, StatusMessage = "Not implemented." };
        }
        #endregion
        #region VersionByRef
        protected virtual async Task ProcessVersionByRef(
            PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            rs.ResponseCode = 501;
            rs.ResponseMessage = "Not implemented.";
        }
        #endregion

        #region Search
        protected virtual async Task ProcessSearch(
            PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            rs.ResponseCode = 501;
            rs.ResponseMessage = "Not implemented.";
        }
        #endregion

        #region EntityMaker(string jsonHolder)
        /// <summary>
        /// This is a simple JSON deserialization method that returns an entity from the 
        /// JSON representation from the DocumentDB repository.
        /// </summary>
        /// <param name="data">The string representation of the entity..</param>
        /// <returns>The object to return.</returns>
        protected virtual E EntityMaker(string data)
        {
            throw new NotImplementedException("EntityMaker is not implemented.");
        }
        #endregion
        #region KeyStringMaker(K key)
        /// <summary>
        /// This is a very simple key serializer to a string representation.
        /// </summary>
        /// <param name="key">The incoming key object.</param>
        /// <returns>The output string.</returns>
        protected virtual string KeyStringMaker(K key)
        {
            return string.Format("{0}.{1}", mEntityName, key.ToString());
        }
        #endregion
        #region KeyMaker(E entity)
        /// <summary>
        /// This method intercepts and replaces the keymaker in the function has been set in the constructor.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>Returns the key from the entity.</returns>
        protected virtual K KeyMaker(E entity)
        {
            if (mKeyMaker == null)
                throw new NotImplementedException("mKeyMaker has not been set.");

            return mKeyMaker(entity);
        }
        #endregion

        #region ReferenceMaker(E entity)
        /// <summary>
        /// This method intercepts and replaces the keymaker in the function has been set in the constructor.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>Returns the key from the entity.</returns>
        protected virtual IEnumerable<KeyValuePair<string,string>> ReferenceMaker(E entity)
        {
            if (mReferenceMaker == null)
                return new KeyValuePair<string, string>[] { };

            return mReferenceMaker(entity);
        }
        #endregion

        #region ProcessOutputEntity...
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
            {
                rs.Entity = EntityMaker(holderResponse.Content);
                rs.Key = KeyMaker(rs.Entity);
                rs.Settings.VersionId = mVersion?.EntityVersionAsString(rs.Entity);

                rs.KeyReference = new Tuple<string, string>(rs.Key.ToString(), rs.Settings.VersionId);
            }
            else
            {
                if (holderResponse.Ex != null && !rs.IsTimeout)
                    Logger.LogException(string.Format("Error in persistence {0}-{1}", typeof(E).Name, key), holderResponse.Ex);
                else
                    Logger.LogMessage(
                        rs.IsTimeout ? LoggingLevel.Warning : LoggingLevel.Info,
                        string.Format("Error in persistence {0}-{1}-{2}-{3}", typeof(E).Name, rs.ResponseCode, key,
                            holderResponse.Ex != null ? holderResponse.Ex.ToString() : rs.ResponseMessage), typeof(E).Name);
            }
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
                //rs.ResponseCode = holderResponse.IsTimeout?408:404;
            }
        }
        #endregion
    }
}
