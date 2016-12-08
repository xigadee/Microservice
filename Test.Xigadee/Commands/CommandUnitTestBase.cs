using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class CommandUnitTestBase<C> 
        where C: ICommand, new()
    {
        protected C mCommand  = new C();
        protected CommandInitiator mCommandInit;
        protected IPipelineChannelIncoming<MicroservicePipeline> cpipeIn = null;
        protected IPipelineChannelOutgoing<MicroservicePipeline> cpipeOut = null;
        protected DebugMemoryDataCollector mCollector = null;
        protected Microservice service = null;

        //[TestInitialize]
        //public void TearUp()
        //{
        //    mCommand
        //}

        //[TestCleanup]
        //public void TearDown()
        //{
        //    mCommand = default(C);
        //}

        protected void DefaultTest()
        {
            var info1 = mCommand.CommandMethodSignatures(true);
            var info2 = mCommand.CommandMethodAttributeSignatures(true);
        }

        protected virtual IPipeline Pipeline()
        {
            var pipeline = new MicroservicePipeline(GetType().Name);

            pipeline
                .AddDataCollector((c) => mCollector = new DebugMemoryDataCollector())
                .AddPayloadSerializerDefaultJson()
                .AddChannelIncoming("internalIn", internalOnly: true)
                    .AttachCommand(mCommand)
                    .CallOut((c) => cpipeIn = c)
                    .Revert()
                .AddChannelOutgoing("internalOut", internalOnly: true, autosetPartition01:false)
                    .AttachPriorityPartition(0, 1)
                    .CallOut((c) => cpipeOut = c)
                    .Revert()
                .AddCommand(new CommandInitiator() { ResponseChannelId = cpipeOut.Channel.Id },assign: (c) => mCommandInit = c);

            return pipeline;

        }
    }
}
