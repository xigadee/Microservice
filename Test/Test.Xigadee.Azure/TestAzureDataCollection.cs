using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Azure
{
    [TestClass]
    public class TestAzureDataCollection
    {
        [TestMethod]
        public void TestMethod1()
        {
            var pipeline = new MicroservicePipeline("AzureTest");

            pipeline.Start();

            pipeline.Stop();
        }
    }
}
