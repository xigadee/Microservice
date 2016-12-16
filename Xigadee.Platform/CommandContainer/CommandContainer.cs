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
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This container contains all the internal handlers, initiators and jobs that are responsible for 
    /// processing messages on the system.
    /// </summary>
    public class CommandContainer:ServiceContainerBase<CommandContainerStatistics, CommandContainerPolicy>
    {
        #region Declarations
        //public event EventHandler<> OnCommandStatusChange;
        /// <summary>
        /// This concurrent dictionary contains the map used to resolve handlers to messages.
        /// </summary>
        protected ConcurrentDictionary<string, List<ICommand>> mMessageMap;
        /// <summary>
        /// This is the container that holds the shared services/
        /// </summary>
        protected SharedServiceContainer mSharedServices;
        /// <summary>
        /// This is the command collection which is used as a container to hold the supported message type,s
        /// </summary>
        protected HandlersCollection mHandlersCollection;
        /// <summary>
        /// This is the list of registered commands.
        /// </summary>
        protected List<ICommand> mCommands;

        protected bool mNotificationsActive = false;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CommandContainer(CommandContainerPolicy policy = null):base(policy)
        {
            mCommands = new List<ICommand>();

            mMessageMap = new ConcurrentDictionary<string, List<ICommand>>();
            mSharedServices = new SharedServiceContainer();

            mHandlersCollection = new HandlersCollection(SupportedMessageTypes);

            mSharedServices.RegisterService<ISupportedMessageTypes>(mHandlersCollection);
        }
        #endregion

        #region Add(IMessageHandler command)
        /// <summary>
        /// This consolidated method is used in preparation of consolidating Jobs, Initiators and Handlers in to a single entity.
        /// </summary>
        /// <param name="command">The command to add to the collection.</param>
        /// <returns>Returns the command that has been added to the collection.</returns>
        public ICommand Add(ICommand command)
        {
            mCommands.Add(command);

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
                h.OnCommandChange += Dynamic_OnCommandChange;
            });
        }
        /// <summary>
        /// This override clears the command handlers.
        /// </summary>
        protected override void StopInternal()
        {
            Commands.ForEach((h) =>
            {
                h.OnCommandChange -= Dynamic_OnCommandChange;
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

            if (mNotificationsActive)
                //Notify the relevant parties (probably just communication) to refresh what they are doing.
                mHandlersCollection.NotifyChange(SupportedMessageTypes());
        }
        #endregion

        #region StatisticsRecalculate(CommandContainerStatistics stats)
        /// <summary>
        /// This method recalcuates the component statistics.
        /// </summary>
        protected override void StatisticsRecalculate(CommandContainerStatistics stats)
        {
            base.StatisticsRecalculate(stats);

            if (SharedServices != null)
                stats.SharedServices = mSharedServices.Statistics;

            stats.Commands = Commands
                .OrderByDescending((c) => c.StartupPriority)
                .Select((h) => (CommandStatistics)h.StatisticsGet()).ToList();
        }
        #endregion

        #region Commands
        /// <summary>
        /// This property returns the classes that support IMessageHandler.
        /// </summary>
        public IEnumerable<ICommand> Commands
        {
            get
            {
                try
                {
                    return mCommands;
                }
                catch (Exception)
                {
                    return new List<ICommand>();
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
            List<ICommand> messageHandlers;

            //If the message handler can't be resolve then quit.
            if (!ResolveMessageHandlers(payload, out messageHandlers))
                return false;

            var requests = messageHandlers.Select((m) => new { handler = m, response = new List<TransmissionPayload>() }).ToArray();

            //OK, then let's call each of the message handlers and catch any errors so that a return message can be logged.
            await Task.WhenAll(requests.Select(async h => await h.handler.ProcessRequest(payload, h.response)));

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
            List<ICommand> messageHandlers;
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
        public bool ResolveMessageHandlers(TransmissionPayload payload, out List<ICommand> messageHandlers)
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
        protected virtual bool ResolveMessageHandlers(ServiceMessage message, out List<ICommand> matchedCommands)
        {
            var header = message.ToServiceMessageHeader();

            //Ok, loop through the handlers until one responds
            var newMap = Commands.Where(h => h.SupportsMessage(header)).ToList();

            //Make sure that the handler is queueAdded as a null value to stop further resolution attemps
            mMessageMap.AddOrUpdate(header.ToKey(), newMap, (k, u) => newMap);

            matchedCommands = newMap;

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

        #region CommandsStart(Action<ICommand> serviceStart)
        /// <summary>
        /// This method starts the commands
        /// </summary>
        public void CommandsStart(Action<ICommand> serviceStart)
        {
            var commands = Commands.ToList();

            //Start the handlers in decending priority
            int[] commandPriorities = commands.Select((c) => c.StartupPriority)
                .Distinct()
                .OrderByDescending((k) => k)
                .ToArray();

            foreach (int priorityGroup in commandPriorities)
            {
                Task.WaitAll(CommandsStart(priorityGroup, commands, serviceStart));
            }

            mNotificationsActive = true;

            //Notify the relevant parties (probably just communication) to refresh what they are doing.
            mHandlersCollection.NotifyChange(SupportedMessageTypes());
        }
        /// <summary>
        /// This method starts the commands in parallel for the same priority.
        /// </summary>
        /// <param name="priority">The command priority.</param>
        /// <param name="commands">The command list.</param>
        /// <param name="serviceStart">The action to start the command.</param>
        /// <returns>Returns a set of tasks that can be executed in parallel.</returns>
        protected Task[] CommandsStart(int priority, List<ICommand> commands, Action<ICommand> serviceStart)
        {
            return commands
                .Where((c) => c.StartupPriority == priority)
                .Select((c) => Task.Run(() => serviceStart(c)))
                .ToArray();
        }
        #endregion
        #region CommandsStop(Action<ICommand> serviceStop)
        /// <summary>
        /// This method stops the commands in the collection.
        /// </summary>
        /// <param name="serviceStop">The action to stop the command.</param>
        public void CommandsStop(Action<ICommand> serviceStop)
        {
            //Ok, stop the commands.
            Commands
                .OrderBy((h) => h.StartupPriority)
                .ForEach(h => serviceStop(h));

            mNotificationsActive = false;

        } 
        #endregion
    }
}
