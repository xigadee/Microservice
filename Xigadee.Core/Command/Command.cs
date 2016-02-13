#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This command is the base implementation that allows multiple commands to be handled 
    /// within a single container.
    /// </summary>
    public abstract partial class CommandBase<S>: ServiceBase<S>, ICommand
        where S : CommandStatistics, new()
    {
        #region Declarations
        /// <summary>
        /// This is the command policy.
        /// </summary>
        protected CommandPolicy mPolicy;
        /// <summary>
        /// This is the concurrent dictionary that contains the supported commands.
        /// </summary>
        protected Dictionary<MessageFilterWrapper, CommandHandler> mSupported;
        /// <summary>
        /// This event is used by the component container to discover when a command is registered or deregistered.
        /// Implement IMessageHandlerDynamic to enable this feature.
        /// </summary>
        public event EventHandler<CommandChange> OnCommandChange;
        /// <summary>
        /// This is the job timer
        /// </summary>
        protected List<Schedule> mSchedules;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor that calls the CommandsRegister function.
        /// </summary>
        public CommandBase(CommandPolicy policy = null)
        {
            mPolicy = policy ?? new CommandPolicy();
            mSupported = new Dictionary<MessageFilterWrapper, CommandHandler>();
            mSchedules = new List<Schedule>();

            if (mPolicy.IsMasterJob)
                MasterJobInitialise();

            if (mPolicy.HasTimerPoll)
                TimerPollSchedulesRegister();
        }
        #endregion

        #region StartInternal/StopInternal
        protected override void StartInternal()
        {
            try
            {
                CommandRequestTimeoutStart();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void StopInternal()
        {
            CommandRequestTimeoutStop();
        }
        #endregion

        #region CommandsRegister()
        /// <summary>
        /// This method should be implemented to populate supported commands.
        /// </summary>
        public virtual void CommandsRegister()
        {
            //Check whether the ResponseId has been set, and if so then register the command.
            if (ResponseId != null)
                CommandRegister(ResponseId, ProcessResponse);

            if (mPolicy.IsMasterJob)
            {
                CommandRegister(NegotiationChannelId, NegotiationMessageType, null, MasterJobStateNotificationIncoming);
            }
        }
        #endregion

        #region CommandRegister<C>...
        /// <summary>
        /// This method registers a command and sets the specific payloadRq for the command.
        /// </summary>
        /// <typeparam name="C">The contract channelId.</typeparam>
        /// <typeparam name="P">The payloadRq DTO channelId. This will be deserialized using the ServiceMessage binary payloadRq.</typeparam>
        /// <param name="action">The process action.</param>
        /// <param name="exceptionAction">The optional action call if an exception is thrown.</param>
        protected virtual void CommandRegister<C, P>(
            Func<P, TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> deadLetterAction = null,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null)
            where C : IMessageContract
        {
            Func<TransmissionPayload, List<TransmissionPayload>, Task> actionReduced = async (m, l) =>
            {
                P payload = PayloadSerializer.PayloadDeserialize<P>(m);
                await action(payload, m, l);
            };

            CommandRegister<C>(actionReduced, deadLetterAction, exceptionAction);
        }
        /// <summary>
        /// This method register a command.
        /// </summary>
        /// <typeparam name="C">The contract channelId.</typeparam>
        /// <param name="action">The process action.</param>
        /// <param name="exceptionAction">The optional action call if an exception is thrown.</param>
        protected virtual void CommandRegister<C>(
            Func<TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> deadLetterAction = null,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null)
            where C : IMessageContract
        {
            string channelId, messageType, actionType;
            ServiceMessageHelper.ExtractContractInfo<C>(out channelId, out messageType, out actionType);

            CommandRegister(channelId, messageType, actionType, action, deadLetterAction, exceptionAction);
        }
        #endregion
        #region CommandRegister...
        /// <summary>
        /// This method registers a particular command.
        /// </summary>
        /// <param name="channelId">The message channelId</param>
        /// <param name="messageType">The command channelId</param>
        /// <param name="actionType">The command action</param>
        /// <param name="command">The action delegate to execute.</param>
        protected void CommandRegister(string type, string messageType, string actionType,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> deadLetterAction = null,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null)
        {
            var key = new ServiceMessageHeader(type, messageType, actionType);
            var wrapper = new MessageFilterWrapper(key);

            CommandRegister(wrapper, action, deadLetterAction, exceptionAction);
        }
        /// <summary>
        /// This method registers a particular command.
        /// </summary>
        /// <param name="channelId">The message channelId</param>
        /// <param name="messageType">The command channelId</param>
        /// <param name="actionType">The command action</param>
        /// <param name="command">The action delegate to execute.</param>
        protected void CommandRegister(MessageFilterWrapper key,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> deadLetterAction = null,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null)
        {
            if (key == null)
                throw new ArgumentNullException("CommandRegister: key cannot be null");

            Func<TransmissionPayload, List<TransmissionPayload>, Task> command = async (sm, lsm) =>
            {
                bool error = false;
                Exception actionEx = null;
                try
                {
                    if (sm.IsDeadLetterMessage && deadLetterAction != null)
                        await deadLetterAction(sm, lsm);
                    else
                        await action(sm, lsm);
                }
                catch (Exception ex)
                {
                    if (exceptionAction == null)
                        throw;
                    error = true;
                    actionEx = ex;
                }

                try
                {
                    if (error)
                        await exceptionAction(actionEx, sm, lsm);
                }
                catch (Exception ex)
                {
                    throw;
                }
            };

            if (key.Header.IsPartialKey && key.Header.ChannelId == null)
                throw new Exception("You must supply a channel when using a partial key.");

            mSupported.Add(key, new CommandHandler(GetType().Name, key, command));

            if (OnCommandChange != null)
                OnCommandChange(this, new CommandChange(false, key));
        }
        #endregion

        #region CommandUnregister<C>...
        /// <summary>
        /// This method unregisters a command.
        /// </summary>
        /// <typeparam name="C">The message contract type</typeparam>
        protected virtual void CommandUnregister<C>() where C : IMessageContract
        {
            string channelId, messageType, actionType;
            ServiceMessageHelper.ExtractContractInfo<C>(out channelId, out messageType, out actionType);
            CommandUnregister(channelId, messageType, actionType);
        }
        #endregion
        #region CommandUnregister...
        /// <summary>
        /// This method unregisters a particular command.
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="messageType">The command message type</param>
        /// <param name="actionType">The command action type</param>
        protected void CommandUnregister(string channelId, string messageType, string actionType)
        {
            CommandUnregister(new MessageFilterWrapper(new ServiceMessageHeader(channelId, messageType, actionType)));
        }

        /// <summary>
        /// This method unregisters a particular command.
        /// </summary>
        /// <param name="key">Message filter wrapper key</param>
        protected void CommandUnregister(MessageFilterWrapper key)
        {
            mSupported.Remove(key);

            if (OnCommandChange != null)
                OnCommandChange(this, new CommandChange(true, key));
        }
        #endregion

        #region In -> ProcessMessage(TransmissionPayload payload, List<TransmissionPayload> responses)
        /// <summary>
        /// This method is called to process and incoming message.
        /// </summary>
        /// <param name="request">The message to process.</param>
        /// <param name="responses">The return path for the message.</param>
        public virtual async Task ProcessMessage(TransmissionPayload payload, List<TransmissionPayload> responses)
        {
            int start = mStatistics.ActiveIncrement();
            try
            {
                var header = payload.Message.ToServiceMessageHeader();

                CommandHandler handler;
                if (!SupportedResolve(header, out handler))
                {
                    throw new NotSupportedException(string.Format("This command is not supported: '{0}' in {1}", header, GetType().Name));
                }

                //Call the registered command.
                await handler.Execute(payload, responses);
            }
            catch (Exception)
            {
                mStatistics.ErrorIncrement();
                throw;
            }
            finally
            {
                mStatistics.ActiveDecrement(start);
            }
        }
        #endregion

        #region SupportedResolve...
        protected virtual bool SupportedResolve(MessageFilterWrapper inWrapper, out CommandHandler command)
        {
            return SupportedResolve(inWrapper.Header, out command);
        }

        protected virtual bool SupportedResolve(ServiceMessageHeader header, out CommandHandler command)
        {
            foreach (var item in mSupported)
            {
                if (item.Key.Header.IsPartialKey)
                {
                    string partialkey = item.Key.Header.ToPartialKey();

                    if (header.ToKey().StartsWith(partialkey))
                    {
                        command = item.Value;
                        return true;
                    }
                }
                else if (item.Key.Header.Equals(header))
                {
                    command = item.Value;
                    return true;
                }
            }

            command = null;
            return false;
        }
        #endregion

        #region SupportsMessage(ServiceMessageHeader header)
        /// <summary>
        /// This commands returns true is the command channelId and action are supported.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns>Returns true if the message is supported.</returns>
        public virtual bool SupportsMessage(ServiceMessageHeader header)
        {
            CommandHandler command;
            return SupportedResolve(header, out command);
        }
        #endregion
        #region SupportedMessageTypes()
        /// <summary>
        /// This method retrieves the supported messages enclosed in the MessageHandler.
        /// </summary>
        /// <returns>Returns a list of MessageFilterWrappers</returns>
        public virtual List<MessageFilterWrapper> SupportedMessageTypes()
        {
            return mSupported.Keys.ToList();
        }
        #endregion

        #region PayloadSerializer
        /// <summary>
        /// This is the requestPayload serializer used across the system.
        /// </summary>
        public IPayloadSerializationContainer PayloadSerializer
        {
            get;
            set;
        }
        #endregion

        #region EventSource
        /// <summary>
        /// This is the event source writer.
        /// </summary>
        public IEventSource EventSource
        {
            get;
            set;
        }
        #endregion
        #region OriginatorId
        /// <summary>
        /// This is the service originator Id.
        /// </summary>
        public string OriginatorId
        {
            get;
            set;
        }
        #endregion
        #region Logger
        /// <summary>
        /// This is the logger for the message handler.
        /// </summary>
        public ILoggerExtended Logger
        {
            get;
            set;
        }
        #endregion
        #region Scheduler
        /// <summary>
        /// This is the scheduler. It is needed to process request timeouts.
        /// </summary>
        public virtual IScheduler Scheduler
        {
            get; set;
        }
        #endregion

        #region Items
        /// <summary>
        /// This returns the list of handlers for logging purposes.
        /// </summary>
        public IEnumerable<CommandHandler> Items
        {
            get
            {
                return mSupported.Values;
            }
        }
        #endregion
        #region Priority
        /// <summary>
        /// This is the message handler priority used when starting up.
        /// </summary>
        public int Priority
        {
            get; set;
        }
        #endregion

    }
}


