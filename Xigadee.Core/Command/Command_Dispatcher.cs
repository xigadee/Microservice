#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P>
    {
        #region CommandsRegister()
        /// <summary>
        /// This method should be implemented to populate supported commands.
        /// </summary>
        public virtual void CommandsRegister()
        {
            //Check whether the ResponseId has been set, and if so then register the command.
            if (ResponseId != null)
                CommandRegister(ResponseId, OutgoingRequestResponseProcess);

            if (mPolicy.MasterJobEnabled)
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

        #region Dispatcher In -> ProcessMessage(TransmissionPayload payload, List<TransmissionPayload> responses)
        /// <summary>
        /// This method is called to process an incoming message.
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
    }
}
