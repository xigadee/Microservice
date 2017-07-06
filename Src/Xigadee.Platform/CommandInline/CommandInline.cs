using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <param name="header">The ServiceMessageHeader route for the command.</param>
        /// <param name="command">The command to process.</param>
        /// <param name="referenceId">The optional reference id for tracking.</param>
        public CommandInline(ServiceMessageHeader header
            , Func<CommandInlineContext, Task> command
            , string referenceId = null) 
        {
            var message = new MessageFilterWrapper(header, null);

            mCommandHolder = new CommandHolder(message, async (rq,rs) => await command(new CommandInlineContext(rq, rs, PayloadSerializer, Collector, SharedServices, OriginatorId)), referenceId);
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
