using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Pipeline
{
    [TestClass]
    public class PipelineChannel
    {
        [TestMethod]
        public void Pipeline()
        {
            var pipe = new MicroservicePipeline();

            pipe
                .AddChannelIncoming("freddyin", autosetPartition01: false)
                    .AttachPriorityPartition(0, 1, 2)
                .Revert()
                .AddChannelOutgoing("freddyout", autosetPartition01: false)
                    .AttachPriorityPartition(1, 2)
                ;

            pipe.Start();

            pipe.Stop();
        }

        [TestMethod]
        public void PipelineUnityWebApi()
        {
            var pipe = new UnityWebApiMicroservicePipeline();

            pipe
                .AddChannelIncoming("freddyin", autosetPartition01:false)
                    .AttachPriorityPartition(0,1,2)                                
                .Revert()                
                .AddChannelOutgoing("freddyout", autosetPartition01: false)
                    .AttachPriorityPartition(1, 2)
                ;

            pipe.Start();

            pipe.Stop();
        }

        [TestMethod]
        public void PipelineWebApi()
        {
            var pipe = new WebApiMicroservicePipeline();

            pipe
                .AddChannelIncoming("freddyin", autosetPartition01: false)
                    .AttachPriorityPartition(0, 1, 2)
                .Revert()
                .AddChannelOutgoing("freddyout", autosetPartition01: false)
                    .AttachPriorityPartition(1, 2)
                ;

            pipe.Start();

            pipe.Stop();
        }
    }
}
