using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This command provides simple inline functionality to register a specific command through an action.
    /// This is used primarily for testing the pipeline.
    /// </summary>
    public class CommandMethodInline : CommandBase
    {
        #region Declarations
        MessageFilterWrapper mKey;
        Func<CommandMethodInlineContext, Task> mCommand;
        string mReferenceId;
        #endregion
        /// <summary>
        /// This is the constructor for registering a manual command.
        /// </summary>
        /// <param name="header">The ServiceMessageHeader route for the command.</param>
        /// <param name="command">The command to process.</param>
        /// <param name="referenceId">The optional reference id for tracking.</param>
        /// <param name="policy">The command policy.</param>
        public CommandMethodInline(ServiceMessageHeader header
            , Func<CommandMethodInlineContext, Task> command
            , string referenceId = null
            , CommandPolicy policy = null) :base(policy ?? new CommandPolicy() { OnProcessRequestException = ProcessRequestExceptionBehaviour.SignalSuccessAndSend500ErrorResponse })
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
                async (rq, rs) => await mCommand(new CommandMethodInlineContext(rq, rs, PayloadSerializer, Collector, SharedServices, OriginatorId, OutgoingRequest))
                , null
                , mReferenceId);
        }
    }
}
