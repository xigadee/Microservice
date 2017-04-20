using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This command provides simple inline functionality to register a specific command through an action.
    /// This is used primarily for testing the pipeline.
    /// </summary>
    public class CommandInline : CommandBase
    {
        CommandHolder mCommandHolder;

        /// <summary>
        /// This is the constructor for registering a manual command.
        /// </summary>
        /// <param name="channelId">The channel the command is attached to.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
        /// <param name="command">The command to process.</param>
        /// <param name="referenceId">The optional reference id for tracking.</param>
        public CommandInline(
              Func<TransmissionPayload, List<TransmissionPayload>, Task> command
            , string channelId
            , string messageType = null
            , string actionType = null
            , string referenceId = null) : this(new ServiceMessageHeader(channelId, messageType, actionType), command, referenceId){}

        /// <summary>
        /// This is the constructor for registering a manual command.
        /// </summary>
        /// <param name="header">The ServiceMessageHeader route for the command.</param>
        /// <param name="command">The command to process.</param>
        /// <param name="referenceId">The optional reference id for tracking.</param>
        public CommandInline(ServiceMessageHeader header
            , Func<TransmissionPayload, List<TransmissionPayload>, Task> command
            , string referenceId = null) : this(new MessageFilterWrapper(header), command, referenceId){}

        /// <summary>
        /// This is the constructor for registering a manual command.
        /// </summary>
        /// <param name="message">The message filter wrapper route to identify the command.</param>
        /// <param name="command">The command to process.</param>
        /// <param name="referenceId">The optional reference id for tracking.</param>
        public CommandInline(MessageFilterWrapper message
            , Func<TransmissionPayload, List<TransmissionPayload>, Task> command
            , string referenceId = null) : base(null)
        {
            mCommandHolder = new CommandHolder(message, command, referenceId);
        }

        /// <summary>
        /// This override registers the message.
        /// </summary>
        protected override void CommandsRegister()
        {
            CommandRegister(mCommandHolder);
        }
    }
}
