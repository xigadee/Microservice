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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {
        #region CommandsRegister()
        /// <summary>
        /// This method should be overridden to populate supported commands.
        /// </summary>
        protected virtual void CommandsRegister()
        {

        }
        #endregion

        #region CommandRegister<C>...

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="CM"></typeparam>
        /// <typeparam name="PM"></typeparam>
        /// <param name="action"></param>
        /// <param name="exceptionAction"></param>
        protected virtual void CommandRegister<CM, PM>(
            Func<PM, TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null)
            where CM : IMessageContract
        {
            Func<TransmissionPayload, List<TransmissionPayload>, Task> actionReduced = async (m, l) =>
            {
                PM payload = PayloadSerializer.PayloadDeserialize<PM>(m);
                await action(payload, m, l);
            };

            CommandRegister<CM>(actionReduced, exceptionAction);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="CM"></typeparam>
        /// <param name="action"></param>
        /// <param name="exceptionAction"></param>
        protected virtual void CommandRegister<CM>(
            Func<TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null)
            where CM : IMessageContract
        {
            string channelId, messageType, actionType;
            ServiceMessageHelper.ExtractContractInfo<CM>(out channelId, out messageType, out actionType);

            CommandRegister(channelId, messageType, actionType, action, exceptionAction, typeof(CM).Name);
        }
        #endregion
        #region CommandRegister...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="messageType"></param>
        /// <param name="actionType"></param>
        /// <param name="action"></param>
        /// <param name="exceptionAction"></param>
        /// <param name="referenceId">This is the referenceId of the command</param>
        protected void CommandRegister(string channelId, string messageType, string actionType,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null,
            string referenceId = null)
        {
            var key = new ServiceMessageHeader(channelId, messageType, actionType);
            var wrapper = new MessageFilterWrapper(key);

            CommandRegister(wrapper, action, exceptionAction, referenceId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <param name="exceptionAction"></param>
        /// <param name="referenceId">This is the referenceId of the command</param>
        protected void CommandRegister(MessageFilterWrapper key,
            Func<TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null, 
            string referenceId = null)
        {
            Func<TransmissionPayload, List<TransmissionPayload>, Task> command = async (rq, rs) =>
            {
                bool error = false;
                Exception actionEx = null;
                try
                {
                    await action(rq, rs);
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
                        await exceptionAction(actionEx, rq, rs);
                }
                catch (Exception ex)
                {
                    throw;
                }
            };


            CommandRegister(new CommandHolder(key, command, referenceId));
        }

        protected void CommandRegister(CommandHolder cHolder)
        {           
            mSupported.Add(cHolder, CommandHandlerCreate(cHolder));

            switch (mPolicy.CommandNotify)
            {
                case CommandNotificationBehaviour.OnRegistration:
                    CommandNotify(cHolder, false);
                    break;
                case CommandNotificationBehaviour.OnRegistrationIfStarted:
                    if (Status == ServiceStatus.Running)
                        CommandNotify(cHolder, false);
                    break;
            }
        }
        #endregion

        #region CommandsNotify..
        /// <summary>
        /// This method can be used to nofity the command container of all the current keys currently supported.
        /// </summary>
        protected virtual void CommandsNotify(bool remove = false)
        {
            foreach (var key in mSupported.Keys)
                CommandNotify(key, remove);
        }
        /// <summary>
        /// This method will notify the command container when a commands is added or removed.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="remove">Set this to true to remove the command mapping, false is default.</param>
        protected virtual void CommandNotify(CommandHolder key, bool remove = false, bool isQuorum = false)
        {
            try
            {
                OnCommandChange?.Invoke(this, new CommandChange(remove, key.Message, isQuorum));
            }
            catch (Exception ex)
            {
                Logger?.LogException($"Command {GetType().Name} Change Notification failed", ex);
            }
        }
        #endregion

        #region CommandHandlerCreate(MessageFilterWrapper key, Func<TransmissionPayload, List<TransmissionPayload>, Task> action)
        /// <summary>
        /// This method creates the command handler. You can override this method to set additional properties.
        /// </summary>
        /// <param name="holder">The command holder</param>
        /// <returns>Returns the handler.</returns>
        protected virtual H CommandHandlerCreate(CommandHolder holder)
        {
            var handler = new H();

            handler.Initialise(holder);

            return handler;
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
            var item = mSupported.Keys.FirstOrDefault((d) => d.Message == key);
            if (item != null)
            {
                mSupported.Remove(item);

                CommandNotify(item, true);
            }
        }
        #endregion

        #region --> ProcessMessage(TransmissionPayload payload, List<TransmissionPayload> responses)
        /// <summary>
        /// This method is called to process an incoming message.
        /// </summary>
        /// <param name="requestPayload">The message to process.</param>
        /// <param name="responses">The return path for the message.</param>
        public virtual async Task ProcessMessage(TransmissionPayload requestPayload, List<TransmissionPayload> responses)
        {
            int start = StatisticsInternal.ActiveIncrement();
            try
            {
                var header = requestPayload.Message.ToServiceMessageHeader();

                H handler;
                if (!SupportedResolve(header, out handler))
                    throw new CommandNotSupportedException(requestPayload.Id, header, GetType());

                //Call the registered command.
                await handler.Execute(requestPayload, responses);
            }
            catch (Exception)
            {
                StatisticsInternal.ErrorIncrement();
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(start);
            }
        }
        #endregion

        #region SupportedResolve...
        /// <summary>
        /// This attemps to match the message header to the command reigstration collection.
        /// </summary>
        /// <param name="header">The message header</param>
        /// <param name="command">THe command handler as an output.</param>
        /// <returns>Returns true if there is a match.</returns>
        protected virtual bool SupportedResolve(ServiceMessageHeader header, out H command)
        {
            if (!mCommandCache.TryGetValue(header, out command))
            {
                SupportedResolveActual(header, out command);
                mCommandCache.TryAdd(header, command);
            }

            return command != null;
        }

        /// <summary>
        /// This attemps to match the message header to the command reigstration collection.
        /// </summary>
        /// <param name="header">The message header</param>
        /// <param name="command">THe command handler as an output.</param>
        /// <returns>Returns true if there is a match.</returns>
        protected virtual bool SupportedResolveActual(ServiceMessageHeader header, out H command)
        {
            foreach (var item in mSupported)
            {
                if (item.Key.Message.Header.IsPartialKey)
                {
                    string partialkey = item.Key.Message.Header.ToPartialKey();

                    if (header.ToKey().StartsWith(partialkey))
                    {
                        command = item.Value;
                        return true;
                    }
                }
                else if (item.Key.Message.Header.Equals(header))
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
            H command;
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
            return mSupported.Keys.Select((h) => h.Message).ToList();
        }
        #endregion
    }
}