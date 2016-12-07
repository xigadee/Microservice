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
                .AddChannelIncoming("freddy")
                
                ;
            pipe.Start();

            pipe.Stop();
        }

        [TestMethod]
        public void PipelineUnityWebApi()
        {
            var pipe = new UnityWebApiMicroservicePipeline();

            pipe
                .AddChannelIncoming("freddy")
                ;

            pipe.Start();

            pipe.Stop();
        }

        [TestMethod]
        public void PipelineWebApi()
        {
            var pipe = new WebApiMicroservicePipeline();

            pipe
                .AddChannelIncoming("freddy")
                ;

            pipe.Start();

            pipe.Stop();
        }
    }
}
