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
        /// You should use the CommandRegister method to do this.
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
            if (!ServiceMessageHelper.ExtractContractInfo<CM>(out channelId, out messageType, out actionType))
                throw new InvalidMessageContractException(typeof(CM));

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
        /// <param name="isMasterJob">Specifies whether the command is a master job.</param>
        protected void CommandRegister(string channelId, string messageType, string actionType
            , Func<TransmissionPayload, List<TransmissionPayload>, Task> action
            , Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null
            , string referenceId = null
            , bool isMasterJob = false)
        {
            var wrapper = new MessageFilterWrapper((channelId, messageType, actionType), null);

            CommandRegister(wrapper, action, exceptionAction, referenceId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <param name="exceptionAction"></param>
        /// <param name="referenceId">This is the referenceId of the command</param>
        /// <param name="isMasterJob">Specifies whether the command is a master job.</param>
        protected void CommandRegister(MessageFilterWrapper key
            , Func<TransmissionPayload, List<TransmissionPayload>, Task> action
            , Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null
            , string referenceId = null
            , bool isMasterJob = false)
        {
            var handler = new H();

            handler.Initialise(key, action, exceptionAction, referenceId, isMasterJob);

            mSupported.Add(key, handler);

            switch (mPolicy.CommandNotify)
            {
                case CommandNotificationBehaviour.OnRegistration:
                    CommandNotify(key, false, isMasterJob);
                    break;
                case CommandNotificationBehaviour.OnRegistrationIfStarted:
                    if (Status == ServiceStatus.Running)
                        CommandNotify(key, false, isMasterJob);
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
            foreach (var item in mSupported)
                CommandNotify(item.Key, remove, item.Value.IsMasterJob);
        }
        /// <summary>
        /// This method will notify the command container when a commands is added or removed.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="remove">Set this to true to remove the command mapping, false is default.</param>
        /// <param name="isMasterJob">Specifies whether it is a master job.</param>
        protected virtual void CommandNotify(MessageFilterWrapper key, bool remove, bool isMasterJob)
        {
            try
            {
                OnCommandChange?.Invoke(this, new CommandChangeEventArgs(remove, key, isMasterJob));
            }
            catch (Exception ex)
            {
                Collector?.LogException($"Command {GetType().Name} Change Notification failed", ex);
            }
        }
        #endregion

        #region CommandUnregister<C>...
        /// <summary>
        /// This method unregisters a command.
        /// </summary>
        /// <typeparam name="C">The message contract type</typeparam>
        protected virtual void CommandUnregister<C>(bool isMasterJob = false) where C : IMessageContract
        {
            string channelId, messageType, actionType;
            if (!ServiceMessageHelper.ExtractContractInfo<C>(out channelId, out messageType, out actionType))
                throw new InvalidMessageContractException(typeof(C));
            CommandUnregister(channelId, messageType, actionType, isMasterJob);
        }
        #endregion
        #region CommandUnregister...
        /// <summary>
        /// This method unregisters a particular command.
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="messageType">The command message type</param>
        /// <param name="actionType">The command action type</param>
        protected void CommandUnregister(string channelId, string messageType, string actionType, bool isMasterJob = false)
        {
            CommandUnregister(new MessageFilterWrapper((channelId, messageType, actionType), null), isMasterJob);
        }

        /// <summary>
        /// This method unregisters a particular command.
        /// </summary>
        /// <param name="key">Message filter wrapper key</param>
        protected void CommandUnregister(MessageFilterWrapper key, bool isMasterJob)
        {
            var item = mSupported.Keys.FirstOrDefault((d) => d == key);
            if (item != null)
            {
                mSupported.Remove(item);

                CommandNotify(item, true, isMasterJob);
            }
        }
        #endregion

        #region SupportedResolve...
        /// <summary>
        /// This attemps to match the message header to the command registration collection.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="handler">The command handler as an output.</param>
        /// <returns>Returns true if there is a match.</returns>
        protected virtual bool SupportedResolve(ServiceMessageHeader header, out H handler)
        {
            if (!mCommandCache.TryGetValue(header, out handler))
            {
                SupportedResolveActual(header, out handler);
                mCommandCache.TryAdd(header, handler);
            }

            return handler != null;
        }

        /// <summary>
        /// This attemps to match the message header to the command registration collection.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="command">The command handler as an output.</param>
        /// <returns>Returns true if there is a match.</returns>
        protected virtual bool SupportedResolveActual(ServiceMessageHeader header, out H command)
        {
            //Fix for BUG 180 - ensuring that trailing slash is on each match for partial key. Moved match logic to struct.
            command = mSupported
                .Where((k) => k.Key.Header.IsMatch(header))
                .Select((k) => k.Value)
                .FirstOrDefault();

            return command != null;
        }
        #endregion

        #region --> SupportsMessage(ServiceMessageHeader header)
        /// <summary>
        /// This commands returns true is the command channelId and action are supported.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns>Returns true if the message is supported.</returns>
        public virtual bool SupportsMessage(ServiceMessageHeader header)
        {
            H command;

            bool supported = SupportedResolve(header, out command);

            return supported;
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