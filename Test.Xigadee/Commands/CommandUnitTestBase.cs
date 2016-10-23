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
            DebugMemoryDataCollector collector = null;
            Microservice service;
            var pipeline = Microservice.Create((s) => service = s, serviceName: nameof(SimpleCommand1UnitTest));

            ChannelPipelineIncoming cpipeIn = null;
            ChannelPipelineOutgoing cpipeOut = null;

            pipeline
                .AddDataCollector<DebugMemoryDataCollector>((c) => collector = c)
                .AddPayloadSerializerDefaultJson()
                .AddChannelIncoming("internalIn", internalOnly: true)
                    .AssignPriorityPartition(0, 1)
                    .AddCommand(mCommand)
                    .Revert((c) => cpipeIn = c)
                .AddChannelOutgoing("internalOut", internalOnly: true)
                    .AssignPriorityPartition(0, 1)
                    .Revert((c) => cpipeOut = c)
                .AddCommand(new CommandInitiator() { ResponseChannelId = cpipeOut.Channel.Id }, (c) => mCommandInit = c);

            return pipeline;
        }
    }
}
