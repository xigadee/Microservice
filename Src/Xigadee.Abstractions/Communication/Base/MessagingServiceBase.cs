#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xigadee;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the generic base class for messaging services.
    /// </summary>
    /// <typeparam name="C">The client class type.</typeparam>
    /// <typeparam name="M">The client message class type.</typeparam>
    /// <typeparam name="H">The client holder class type.</typeparam>
    public abstract class MessagingServiceBase<C, M, H>: ServiceBase<StatusBase>, IMessaging
        where C: class
        where H: ClientHolder<C, M>, new()
    {
        #region Declarations
        /// <summary>
        /// This is the client collection.
        /// </summary>
        protected ConcurrentDictionary<int, H> mListenerClients= new ConcurrentDictionary<int, H>();
        /// <summary>
        /// This is the default priority. 1 if present
        /// </summary>
        protected int? mDefaultPriority;
        /// <summary>
        /// This method is used to name the client based on the priority.
        /// </summary>
        protected Func<string, int, string> mPriorityClientNamer = (s, i) => string.Format("{0}{1}", s, i == 1 ? "" : i.ToString());

        private int mClientStarted = 0;
        private int mClientStopped = 0;
        #endregion

        #region ChannelId
        /// <summary>
        /// This is the ChannelId for the messaging service
        /// </summary>
        public string ChannelId
        {
            get;
            set;
        } 
        #endregion

        #region Clients
        /// <summary>
        /// This method returns the clients created for the messaging service.
        /// </summary>
        public IEnumerable<IClientHolder> ListenerClients
        {
            get
            {
                return mListenerClients?.Values;
            }
        } 
        #endregion

        #region SupportsChannel(string channel)
        /// <summary>
        /// This method compares the channel and returns true if it is supported.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>Returns true if the channel is supported.</returns>
        public bool SenderSupportsChannel(string channel)
        {
            return string.Equals(channel, ChannelId, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region SettingsValidate()
        /// <summary>
        /// This method validates the configuration and settings for the connection.
        /// </summary>
        protected virtual void SettingsValidate()
        {
            if (ChannelId == null)
                throw new CommunicationAgentStartupException("ChannelId", "ChannelId cannot be null");

        } 
        #endregion

        #region StartInternal()
        /// <summary>
        /// This is the default start method for both listeners and senders.
        /// </summary>
        protected override void StartInternal()
        {
            //SettingsValidate();

            //try
            //{
            //    TearUp();

            //    //Start the client in either listener or sender mode.
            //    foreach (var priority in ListenerPriorityPartitions)
            //    {
            //        var client = ClientCreate(priority);

            //        mClients.AddOrUpdate(priority.Priority, client, (i,h) => h);

            //        if (client.CanStart)
            //            ClientStart(client);
            //        else
            //            Collector?.LogMessage(string.Format("Client not started: {0} :{1}/{2}", client.Type, client.Name, client.Priority));

            //        if (priority.Priority == 1)
            //            mDefaultPriority = 1;
            //    }

            //    //If the incoming priority cannot be reconciled we set it to the default
            //    //which is 1, unless 1 is not present and then we set it to the max value.
            //    if (!mDefaultPriority.HasValue)
            //        mDefaultPriority = ListenerPriorityPartitions.Select((p) => p.Priority).Max();
            //}
            //catch (Exception ex)
            //{
            //    LogExceptionLocation("StartInternal", ex);
            //    throw;
            //}
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This is the default stop for all listeners and senders
        /// </summary>
        protected override void StopInternal()
        {
            try
            {
                mListenerClients.Values.ForEach((c)=>ClientStop(c));
                mListenerClients.Clear();
            }
            catch (Exception ex)
            {
                LogExceptionLocation("StopInternal", ex);
            }
            mListenerClients = null;
            TearDown();
        }
        #endregion

        #region TearUp()
        /// <summary>
        /// This override can be used to add additional logic during the start up phase.
        /// This method is called before the clients are created.
        /// </summary>
        protected virtual void TearUp()
        {
        } 
        #endregion
        #region TearDown()
        /// <summary>
        /// This method can be used to clean up any additional communication methods.
        /// It is called after the clients have been closed.
        /// </summary>
        protected virtual void TearDown()
        {

        } 
        #endregion

        #region ClientStart(H client)
        /// <summary>
        /// This method creates and starts the client.
        /// </summary>
        public virtual void ClientStart(H client)
        {
            client.FabricInitialize();
            client.Start();
            Interlocked.Increment(ref mClientStarted);
        }
        #endregion
        //#region ClientCreate()
        ///// <summary>
        ///// This is the default client create logic.
        ///// </summary>
        ///// <returns>Returns the client.</returns>
        //protected virtual H ClientCreate(P partition)
        //{
        //    var client = new H();

        //    //Set the Data Collector
        //    client.Collector = Collector;
        //    //Set the Serializer.
        //    client.ServiceHandlers = ServiceHandlers;

        //    client.BoundaryLoggingActive = BoundaryLoggingActive ?? false;

        //    client.ClientRefresh = () => { };

        //    client.ChannelId = ChannelId;

        //    client.Priority = partition.Priority;

        //    client.QueueLength = () => (int?)null;

        //    client.FabricInitialize = () => { };

        //    client.Start = () => { };

        //    client.Stop = () =>
        //    {
        //        if (client?.Client == null)
        //            return;

        //        client.IsActive = false;
        //        client.ClientClose();
        //    };

        //    client.ClientReset = (ex) => ClientReset(client, ex);

        //    return client;
        //}
        //#endregion
        #region ClientStop(H client)
        /// <summary>
        /// This method stops the client.
        /// </summary>
        protected virtual void ClientStop(H client)
        {
            client.Stop?.Invoke();
            Interlocked.Increment(ref mClientStopped);
        }
        #endregion
        #region ClientReset(H client, Exception mex)
        /// <summary>
        /// This method closes and reset the fabric and the client.
        /// </summary>
        /// <param name="client">The client to reset.</param>
        /// <param name="mex">The messaging exception.</param>
        protected virtual void ClientReset(H client, Exception mex)
        {
            try
            {
                client.Stop();
            }
            catch (Exception ex)
            {
                LogExceptionLocation(string.Format("ClientReset (Close) failed - {0}", client.Name), ex);
            }

            int? attemps = 0;
            do
            {
                try
                {
                    client.FabricInitialize();
                    client.Start();
                    attemps = null;
                }
                catch (Exception ex)
                {
                    LogExceptionLocation("ClientReset (Create)", ex);
                    attemps++;
                    //Stand off with each attempts
                    Thread.Sleep(100 * attemps.Value);
                }
            }
            while (attemps.HasValue);
        }
        #endregion
        #region ClientResolve(int priority)
        /// <summary>
        /// This method resolves the client based on the priority set.
        /// If then priority cannot be matched, it will use the default priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns>The specific client holder.</returns>
        protected virtual H ClientResolve(int priority)
        {
            if ((mListenerClients?.Count??0) == 0)
                throw new ClientsUndefinedMessagingException($"No Clients are defined for {ChannelId}");

            if (mListenerClients.ContainsKey(priority))
                return mListenerClients[priority];

            if (!mDefaultPriority.HasValue)
                throw new ClientsUndefinedMessagingException($"Channel={ChannelId} Priority={priority} cannot be found and a default priority value has not been set.");

            return mListenerClients[mDefaultPriority.Value];
        }
        #endregion

        //Microservice properties set
        #region ServiceHandlers
        /// <summary>
        /// This container is used to contain the service handler collection.
        /// </summary>
        public IServiceHandlers ServiceHandlers
        {
            get;
            set;
        } 
        #endregion
        #region OriginatorId
        /// <summary>
        /// This is the OriginatorId from the parent container.
        /// </summary>
        public virtual MicroserviceId OriginatorId
        {
            get;
            set;
        }
        #endregion
        #region Collector
        /// <summary>
        /// This is the system wide data collector
        /// </summary>
        public IDataCollection Collector
        {
            get;set;
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

        #region LogExceptionLocation(string method)
        /// <summary>
        /// This helper method provides a class name and method name for debugging exceptions. 
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>A combination string.</returns>
        protected void LogExceptionLocation(string method, Exception ex)
        {
            Collector?.LogException($"{GetType().Name}/{method}", ex);
        }
        #endregion

        #region BoundaryLoggingActive
        /// <summary>
        /// This property specifies whether the boundary logger is active.
        /// </summary>
        public bool? BoundaryLoggingActive { get; set; } 
        #endregion
    }
}
