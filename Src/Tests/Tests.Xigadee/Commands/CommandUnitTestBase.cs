using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    public abstract class CommandUnitTestBase<C> 
        where C: ICommand, new()
    {
        protected C mCommand  = new C();
        protected CommandInitiator mCommandInit;
        protected IPipelineChannelIncoming<MicroservicePipeline> cpipeIn = null;
        protected IPipelineChannelOutgoing<MicroservicePipeline> cpipeOut = null;
        protected DebugMemoryDataCollector mCollector = null;
        protected Microservice service = null;

        protected void DefaultTest()
        {
            var info1 = mCommand.CommandMethodSignatures<CommandContractAttribute,CommandMethodSignature>(true, true);
        }

        protected virtual IPipeline Pipeline()
        {
            var pipeline = new MicroservicePipeline(GetType().Name);

            pipeline
                .AddDebugMemoryDataCollector(out mCollector)
                .AddChannelIncoming("internalIn", internalOnly: true)
                    .AttachCommand(mCommand)
                    .CallOut((c) => cpipeIn = c)
                    .Revert()
                .AddChannelOutgoing("internalOut", internalOnly: true, autosetPartition01:false)
                    .AttachPriorityPartition(0, 1)
                    .CallOut((c) => cpipeOut = c)
                    .Revert()
                .AddChannelIncoming("internalInit", internalOnly: true)
                    .AttachCommandInitiator(out mCommandInit);

            return pipeline;

        }
    }
}
