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
        #region *--> CommandsTearUp()
        /// <summary>
        /// This method sets up the command collection.
        /// </summary>
        protected virtual void CommandsTearUp()
        {
            CommandsRegister();

            if (Policy.CommandReflectionSupported)
                CommandsRegisterReflection();
        }
        #endregion
        #region CommandsRegister()
        /// <summary>
        /// This method should be overridden to populate supported commands.
        /// You should use the CommandRegister method to do this.
        /// </summary>
        protected virtual void CommandsRegister()
        {

        }
        #endregion
        #region CommandsRegisterReflection()
        /// <summary>
        /// This method scans through the command and registers commands that are defined using the metadata tags.
        /// </summary>
        protected void CommandsRegisterReflection()
        {
            foreach (var holder in this.CommandMethodSignatures<CommandContractAttribute,CommandMethodSignature>(true, Policy.CommandContractAttributeInherit))
                CommandRegister(CommandChannelIdAdjust(holder.Attribute)
                    , (rq, rs) => holder.Signature.Action(rq, rs, ServiceHandlers)
                    , referenceId: holder.Reference
                    );
        }
        #endregion
        #region CommandChannelIdAdjust<A>(A attr)
        /// <summary>
        /// This method replaces the channel with the command default if the value specified in the attribute is null.
        /// </summary>
        /// <param name="attr">The incoming attribute whose header channel should be checked.</param>
        /// <returns>Returns a message filter wrapper for the header.</returns>
        protected MessageFilterWrapper CommandChannelIdAdjust<A>(A attr)
            where A : CommandContractAttributeBase
        {
            ServiceMessageHeader header = attr.Header;

            if (header.ChannelId == null)
            {
                if (ChannelId == null)
                    throw new CommandChannelIdNullException($"Command '{FriendlyName}' uses CommandContracts without a Channel id set but its default ChannelId is null.");
                
                header = new ServiceMessageHeader(ChannelId, header.MessageType, header.ActionType);
            }

            return new MessageFilterWrapper(header);
        }
        #endregion

        #region *--> CommandsTearDown()
        /// <summary>
        /// This method tears down the command collection.
        /// </summary>
        protected virtual void CommandsTearDown()
        {
            CommandsNotify(true);
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
        protected void CommandRegister<CM, PM>(
            Func<PM, TransmissionPayload, List<TransmissionPayload>, Task> action,
            Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null)
            where CM : IMessageContract
        {
            Func<TransmissionPayload, List<TransmissionPayload>, Task> actionReduced = async (m, l) =>
            {
                if (!(m.Message?.Holder?.HasObject ?? false))
                    await action(default(PM), m, l);
                else
                    await action((PM)m.Message.Holder.Object, m, l);
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

            CommandRegister((channelId, messageType, actionType), action, exceptionAction, typeof(CM).Name);
        }
        #endregion
        #region CommandRegister...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="header">The service message header</param>
        /// <param name="action">The command method action.</param>
        /// <param name="exceptionAction">An option exception action.</param>
        /// <param name="referenceId">This is the referenceId of the command</param>
        /// <param name="isMasterJob">Specifies whether the command is related to a master job.</param>
        protected void CommandRegister(ServiceMessageHeader header
            , Func<TransmissionPayload, List<TransmissionPayload>, Task> action
            , Func<Exception, TransmissionPayload, List<TransmissionPayload>, Task> exceptionAction = null
            , string referenceId = null
            , bool isMasterJob = false)
        {
            var wrapper = new MessageFilterWrapper(header, null);

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

            switch (Policy.CommandNotify)
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

        #region CommandsNotify(bool isRemoval)
        /// <summary>
        /// This method can be used to notify the command container of all the current keys currently supported.
        /// </summary>
        protected void CommandsNotify(bool isRemoval)
        {
            foreach (var item in mSupported)
                CommandNotify(item.Key, isRemoval, item.Value.IsMasterJob);
        }
        #endregion
        #region CommandNotify(MessageFilterWrapper key, bool remove, bool isMasterJob)
        /// <summary>
        /// This method will notify the command container when a commands is added or removed.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="isRemoval">Set this to true to remove the command mapping, false is default.</param>
        /// <param name="isMasterJob">Specifies whether it is a master job.</param>
        protected void CommandNotify(MessageFilterWrapper key, bool isRemoval, bool isMasterJob)
        {
            FireAndDecorateEventArgs(OnCommandChange, () => new CommandChangeEventArgs(isRemoval, key, isMasterJob));
        } 
        #endregion

        #region CommandUnregister<C>...
        /// <summary>
        /// This method unregisters a command.
        /// </summary>
        /// <typeparam name="C">The message contract type</typeparam>
        protected void CommandUnregister<C>(bool isMasterJob = false) where C : IMessageContract
        {
            string channelId, messageType, actionType;
            if (!ServiceMessageHelper.ExtractContractInfo<C>(out channelId, out messageType, out actionType))
                throw new InvalidMessageContractException(typeof(C));

            CommandUnregister((channelId, messageType, actionType), isMasterJob);
        }
        #endregion
        #region CommandUnregister...
        /// <summary>
        /// This method unregisters a particular command.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="isMasterJob">Specifies whether the command is linked to a master job.</param>
        protected void CommandUnregister(ServiceMessageHeader header, bool isMasterJob = false)
        {
            CommandUnregister(new MessageFilterWrapper(header, null), isMasterJob);
        }

        /// <summary>
        /// This method unregisters a particular command.
        /// </summary>
        /// <param name="key">Message filter wrapper key</param>
        /// <param name="isMasterJob">Specifies whether the command is linked to a master job.</param>
        protected void CommandUnregister(MessageFilterWrapper key, bool isMasterJob)
        {
            if (mSupported.ContainsKey(key))
            {
                var handler = mSupported[key];

                if (handler.IsMasterJob == isMasterJob)
                {
                    mSupported.Remove(key);
                    mCommandCache.Clear();

                    CommandNotify(key, true, handler.IsMasterJob);
                }
            }
        }
        #endregion

        #region --> SupportedMessageTypes()
        /// <summary>
        /// This method retrieves the supported messages enclosed in the MessageHandler.
        /// </summary>
        /// <returns>Returns a list of MessageFilterWrappers</returns>
        public List<MessageFilterWrapper> SupportedMessageTypes()
        {
            return mSupported.Keys.ToList();
        }
        #endregion
        #region --> SupportsMessage(ServiceMessageHeader header)
        /// <summary>
        /// This commands returns true is the command channelId and action are supported.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <returns>Returns true if the message is supported.</returns>
        public bool SupportsMessage(ServiceMessageHeader header)
        {
            H command;

            bool supported = SupportedResolve(header, out command);

            return supported;
        }
        #endregion

        #region SupportedResolve(ServiceMessageHeader header, out H handler)
        /// <summary>
        /// This attempts to match the message header to the command registration collection.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="handler">The command handler as an output.</param>
        /// <returns>Returns true if there is a match.</returns>
        protected bool SupportedResolve(ServiceMessageHeader header, out H handler)
        {
            if (!mCommandCache.TryGetValue(header, out handler))
            {
                SupportedResolveActual(header, out handler);
                mCommandCache.TryAdd(header, handler);
            }

            return handler != null;
        }
        #endregion
        #region SupportedResolveActual(ServiceMessageHeader header, out H command)
        /// <summary>
        /// This attempts to match the message header to the command registration collection.
        /// </summary>
        /// <param name="header">The message header.</param>
        /// <param name="command">The command handler as an output.</param>
        /// <returns>Returns true if there is a match.</returns>
        protected bool SupportedResolveActual(ServiceMessageHeader header, out H command)
        {
            //Fix for BUG 180 - ensuring that trailing slash is on each match for partial key. Moved match logic to struct.
            command = mSupported
                .Where((k) => k.Key.Header.IsMatch(header))
                .Select((k) => k.Value)
                .FirstOrDefault();

            return command != null;
        } 
        #endregion
    }
}