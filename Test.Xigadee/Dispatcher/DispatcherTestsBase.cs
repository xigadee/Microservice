using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public abstract class DispatcherTestsBase<C>
        where C: class, ICommand, new()
    {
        protected CommandInitiator mCommandInit;
        protected IPipelineChannelIncoming<MicroservicePipeline> cpipeIn = null;
        protected IPipelineChannelOutgoing<MicroservicePipeline> cpipeOut = null;
        protected DebugMemoryDataCollector collector = null;
        protected IPipeline mPipeline = null;
        protected C mDCommand = null;

        protected ManualChannelListener mListener = null;
        protected ManualChannelSender mSender = null;

        protected virtual IPipeline PipelineConfigure()
        {
            try
            {
                mDCommand = new C();
                var pipeline = new MicroservicePipeline(GetType().Name)
                    .AddDataCollector((c) => collector = new DebugMemoryDataCollector())
                    .AddPayloadSerializerDefaultJson()
                    .AddChannelIncoming("internalIn", internalOnly: false, autosetPartition01:false)
                        .AttachPriorityPartition(0, 1)
                        .AttachListener((c) => new ManualChannelListener (), action: (s) => mListener = s)
                        .AttachCommand(mDCommand)
                        .CallOut((c) => cpipeIn = c)
                        .Revert()
                    .AddChannelOutgoing("internalOut", internalOnly: false, autosetPartition01: false)
                        .AttachPriorityPartition(0, 1)
                        .AttachSender((c) => new ManualChannelSender(),action: (s) => mSender = s)
                        .CallOut((c) => cpipeOut = c)
                        .Revert()
                    .AddCommand(new CommandInitiator() { ResponseChannelId = cpipeOut.Channel.Id }, assign:(c) => mCommandInit = c);

                return pipeline;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
