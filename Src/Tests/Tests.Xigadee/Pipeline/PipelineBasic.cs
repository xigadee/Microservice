using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Pipeline
{
    [TestClass]
    public class PipelineBasic
    {
        /// <summary>
        /// This checks the creation of the various pipeline type.s
        /// </summary>
        [TestMethod]
        public void Pipeline()
        {
            var pipe = new MicroservicePipeline();

            pipe.Start();

            pipe.Stop();
        }

        [TestMethod]
        public void Pipeline2()
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

    }
}
