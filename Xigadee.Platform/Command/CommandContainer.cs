#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This container contains all the internal handlers, initiators and jobs that are responsible for 
    /// processing messages on the system.
    /// </summary>
    public class CommandContainer:ServiceBase<CommandContainerStatistics>
    {
        #region Declarations
        /// <summary>
        /// This concurrent dictionary contains the map used to resolve handlers to messages.
        /// </summary>
        protected ConcurrentDictionary<string, List<IMessageHandler>> mMessageMap;
        /// <summary>
        /// This is the container that holds the shared services/
        /// </summary>
        protected SharedServiceContainer mSharedServices;

        protected HandlersCollection mHandlersCollection;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CommandContainer()
        {
            MessageHandlers = new List<IMessageHandler>();
            MessageInitiators = new List<IMessageInitiator>();
            Jobs = new List<IJob>();

            mMessageMap = new ConcurrentDictionary<string, List<IMessageHandler>>();
            mSharedServices = new SharedServiceContainer();

            mHandlersCollection = new HandlersCollection(SupportedMessageTypes);

            mSharedServices.RegisterService<ISupportedMessageTypes>(mHandlersCollection);
        }
        #endregion

        #region MessageHandlers
        /// <summary>
        /// The message handlers for the container.
        /// </summary>
        public virtual IEnumerable<IMessageHandler> MessageHandlers { get; protected set; }
        #endregion
        #region MessageInitiators
        /// <summary>
        /// The message initiators for the container.
        /// </summary>
        public virtual IEnumerable<IMessageInitiator> MessageInitiators { get; protected set; }
        #endregion
        #region Jobs
        /// <summary>
        /// This is the list of jobs active
        /// </summary>
        public virtual IEnumerable<IJob> Jobs { get; protected set; }
        #endregion

        #region Add(IMessageHandler command)
        /// <summary>
        /// This consolidated method is used in preparation of consolidating Jobs, Initiators and Handlers in to a single entity.
        /// </summary>
        /// <param name="command">The command to add to the collection.</param>
        /// <returns>Returns the command that has been added to the collection.</returns>
        public IMessageHandler Add(IMessageHandler command)
        {
            if (command is IJob)
                ((List<IJob>)Jobs).Add((IJob)command);
            else if (command is IMessageInitiator)
                ((List<IMessageInitiator>)MessageInitiators).Add((IMessageInitiator)command);
            else
                ((List<IMessageHandler>)MessageHandlers).Add(command);

            return command;
        } 
        #endregion

        #region StartInternal/StopInternal
        /// <summary>
        /// This override registers the commands for the command handler.
        /// </summary>
        protected override void StartInternal()
        {
            //Ensure that any handlers are registered.
            Commands.ForEach((h) =>
            {
                h.CommandsRegister();

                if (h is IMessageHandlerDynamic)
                {
                    var hDy = h as IMessageHandlerDynamic;
                    hDy.OnCommandChange += Dynamic_OnCommandChange;
                }
            });
        }
        /// <summary>
        /// This override clears the command handlers.
        /// </summary>
        protected override void StopInternal()
        {
            Commands.ForEach((h) =>
            {
                if (h is IMessageHandlerDynamic)
                {
                    var hDy = h as IMessageHandlerDynamic;
                    hDy.OnCommandChange -= Dynamic_OnCommandChange;
                }
            });
        }
        #endregion

        #region Dynamic_OnCommandChange(object sender, CommandChange e)
        /// <summary>
        /// This event is fired when a dymanic command changes the supported commands.
        /// This might happen specifically when a master job becomes active.
        /// </summary>
        /// <param name="sender">The command which changes.</param>
        /// <param name="e">The change parameters.</param>
        private void Dynamic_OnCommandChange(object sender, CommandChange e)
        {
            //Clear the message map cache as the cache is no longer valid due to removal.
            if (e.IsRemoval)
                mMessageMap.Clear();

            //Notify the relevant parties (probably just communication) to refresh what they are doing.
            mHandlersCollection.NotifyChange(SupportedMessageTypes());
        }
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method recalcuates the component statistics.
        /// </summary>
        protected override void StatisticsRecalculate()
        {
            base.StatisticsRecalculate();
            try
            {
                if (SharedServices != null) mStatistics.SharedServices = mSharedServices.Statistics;

                mStatistics.Handlers = Commands.SelectMany((h) => h.Items).Select((i) => i.Statistics).ToList();

                mStatistics.Commands = Commands.OfType<ICommand>().Select((h) => (CommandStatistics)h.StatisticsGet()).ToList();

                mStatistics.Jobs = Commands.OfType<IJob>().Select((h) => h.StatisticsGet() as JobStatistics).Where((s) => s != null).ToList();

                mStatistics.Cache = Commands.OfType<ICacheComponent>().Select((h) => (MessagingStatistics)h.StatisticsGet()).ToList();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Handlers
        /// <summary>
        /// This property returns the classes that support IMessageHandler.
        /// </summary>
        public IEnumerable<IMessageHandler> Commands
        {
            get
            {
                try
                {
                    return MessageHandlers
                        .Union(MessageInitiators)
                        .Union(Jobs.Cast<IMessageHandler>());
                }
                catch (Exception)
                {
                    return new List<IMessageHandler>();
                }
            }
        }
        #endregion

        #region SupportedMessageTypes()
        /// <summary>
        /// This method provides a list of supported message channelId.
        /// This is used by listeners that need to filter on specific message types.
        /// </summary>
        /// <returns>Returns a list of message types.</returns>
        protected virtual List<MessageFilterWrapper> SupportedMessageTypes()
        {
            var list = Commands.SelectMany(mh => mh.SupportedMessageTypes()).ToList();

            return list;
        }
        #endregion

        #region --> Execute(TransmissionPayload requestPayload, List<TransmissionPayload> responseMessages)
        /// <summary>
        /// This method process the message and passes it to the relevant message handlers.
        /// </summary>
        /// <param name="payload">The incoming requestPayload.</param>
        /// <param name="responseMessages">The reponse messages to process.</param>
        /// <returns>Returns true if the collection was processed successfully.</returns>
        public async Task<bool> Execute(TransmissionPayload payload, List<TransmissionPayload> responseMessages)
        {
            //This is the message handler that will process the call.
            List<IMessageHandler> messageHandlers;
            //If the message handler still can't be resolved then quit.
            if (!ResolveMessageHandlers(payload, out messageHandlers))
                return false;

            var requests = messageHandlers.Select((m) => new { handler = m, response = new List<TransmissionPayload>() }).ToArray();

            //OK, then let's call each of the message handlers and catch any errors so that a return message can be logged.
            await Task.WhenAll(requests.Select(async h => await h.handler.ProcessMessage(payload, h.response)));

            responseMessages.AddRange(requests.SelectMany((h) => h.response));

            return true;
        }
        #endregion
        #region --> Resolve(TransmissionPayload payload)
        /// <summary>
        /// This method returns true if there are local message handlers that can process the payload.
        /// </summary>
        /// <param name="payload">The payload to resolve.</param>
        /// <returns>Returns true of there are message handlers that can process the payload.</returns>
        public bool Resolve(TransmissionPayload payload)
        {
            List<IMessageHandler> messageHandlers;
            bool result = ResolveMessageHandlers(payload, out messageHandlers);
            return result;
        }
        #endregion

        #region ResolveMessageHandlers(TransmissionPayload payload, out List<IMessageHandler> messageHandlers)
        /// <summary>
        /// This message resolves any local handlers that can process the message.
        /// </summary>
        /// <param name="payload">The payload to resolve.</param>
        /// <param name="messageHandlers">A list containing the message handlers that can process the message.</param>
        /// <returns>Returns true of there are message handlers that can process the payload.</returns>
        public bool ResolveMessageHandlers(TransmissionPayload payload, out List<IMessageHandler> messageHandlers)
        {
            messageHandlers = null;
            //Check if the message key exisits in the dictionary 
            string messageKey = ServiceMessageHeader.ToKey(payload.Message);

            //Get the message handler
            if (!mMessageMap.TryGetValue(messageKey, out messageHandlers))
                 return ResolveMessageHandlers(payload.Message, out messageHandlers);

            return messageHandlers.Count > 0;
        }
        #endregion
        #region ResolveMessageHandlers(ServiceMessage message, out List<IMessageHandler> handlers)
        /// <summary>
        /// This message resolves the specific handler that can process the incoming message.
        /// </summary>
        /// <param name="message">The incoming message.</param>
        /// <returns>Returns the handler or null.</returns>
        protected virtual bool ResolveMessageHandlers(ServiceMessage message, out List<IMessageHandler> handlers)
        {
            var header = message.ToServiceMessageHeader();

            //Ok, loop through the handlers until one responds
            var newMap = Commands.Where(h => h.SupportsMessage(header)).ToList();

            //Make sure that the handler is queueAdded as a null value to stop further resolution attemps
            mMessageMap.AddOrUpdate(header.ToKey(), newMap, (k, u) => newMap);

            handlers = newMap;

            return newMap.Count > 0; 
        }
        #endregion

        #region SharedServices
        /// <summary>
        /// This collection holds the shared services for the Microservice.
        /// </summary>
        public virtual ISharedService SharedServices {get { return mSharedServices; } }
        #endregion
        #region SharedServicesConnect()
        /// <summary>
        /// This method is used to connect the message handler components to the shared service catalogue.
        /// </summary>
        public virtual void SharedServicesConnect()
        {
            Commands
                .Where((i) => i is IRequireSharedServices)
                .Cast<IRequireSharedServices>()
                .ForEach((s) => s.SharedServices = SharedServices);
        }
        #endregion
    }
}
