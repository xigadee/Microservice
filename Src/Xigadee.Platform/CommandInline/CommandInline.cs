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
        #region Declarations
        MessageFilterWrapper mKey;
        Func<CommandInlineContext, Task> mCommand;
        string mReferenceId;
        #endregion
        /// <summary>
        /// This is the constructor for registering a manual command.
        /// </summary>
        /// <param name="header">The ServiceMessageHeader route for the command.</param>
        /// <param name="command">The command to process.</param>
        /// <param name="referenceId">The optional reference id for tracking.</param>
        /// <param name="policy">The command policy.</param>
        public CommandInline(ServiceMessageHeader header
            , Func<CommandInlineContext, Task> command
            , string referenceId = null
            , CommandPolicy policy = null) :base(policy ?? new CommandPolicy() { OnExceptionCallProcessRequestException = true })
        {
            mKey = new MessageFilterWrapper(header, null);
            mCommand = command;
            mReferenceId = referenceId;
        }

        /// <summary>
        /// This override registers the message.
        /// </summary>
        protected override void CommandsRegister()
        {
            CommandRegister(mKey,
                async (rq, rs) => await mCommand(new CommandInlineContext(rq, rs, PayloadSerializer, Collector, SharedServices, OriginatorId))
                , null
                , mReferenceId);
        }
    }
}
