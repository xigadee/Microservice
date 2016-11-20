using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class CommandUnitTestBase<C> 
        where C: ICommand, new()
    {
        protected C mCommand;
        protected CommandInitiator mCommandInit;
        protected ChannelPipelineIncoming cpipeIn = null;
        protected ChannelPipelineOutgoing cpipeOut = null;
        protected DebugMemoryDataCollector collector = null;
        protected Microservice service = null;

        [TestInitialize]
        public void TearUp()
        {
            mCommand = new C();
        }

        [TestCleanup]
        public void TearDown()
        {
            mCommand = default(C);
        }


        protected void DefaultTest()
        {
            var info1 = mCommand.CommandMethodSignatures(true);
            var info2 = mCommand.CommandMethodAttributeSignatures(true);
        }

        protected virtual MicroservicePipeline Pipeline()
        {
            return PipelineConfigure(Microservice.Create((s) => service = s, serviceName: GetType().Name));
        }

        protected virtual MicroservicePipeline PipelineConfigure(MicroservicePipeline pipeline)
        {
            pipeline
                .AddDataCollector<DebugMemoryDataCollector>((c) => collector = c)
                .AddPayloadSerializerDefaultJson()
                .AddChannelIncoming("internalIn", internalOnly: true)
                    .AttachPriorityPartition(0, 1)
                    .AttachCommand(mCommand)
                    .Revert((c) => cpipeIn = c)
                .AddChannelOutgoing("internalOut", internalOnly: true)
                    .AttachPriorityPartition(0, 1)
                    .Revert((c) => cpipeOut = c)
                .AddCommand(new CommandInitiator() { ResponseChannelId = cpipeOut.Channel.Id }, (c) => mCommandInit = c);

            return pipeline;

        }
    }
}
