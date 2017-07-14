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

        /// <summary>
        /// This WebApi Unity pipeline.
        /// </summary>
        [TestMethod]
        public void PipelineUnityWebApi()
        {
            var pipe = new UnityWebApiMicroservicePipeline();

            //pipe.AddSharedService(
            pipe.Start();

            pipe.Stop();
        }
        /// <summary>
        /// The WebApi pipeline.
        /// </summary>
        [TestMethod]
        public void PipelineWebApi()
        {
            var pipe = new WebApiMicroservicePipeline();

            pipe.Start();

            pipe.Stop();
        }
    }
}
