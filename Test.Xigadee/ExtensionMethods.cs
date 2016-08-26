using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
namespace Test.Xigadee
{
    [TestClass]
    public class ExtensionMethods
    {
        [TestMethod]
        public void TestMethod1()
        {
            var pipeline = Microservice.InitalizePipeline();

            pipeline
                .AddChannelIncoming("Incoming");

            pipeline
                .AddChannelOutgoing("Return");

            //    .ConfigureServiceBusQueue()
            //    .ConfigureApiListener();
            pipeline.AddPayloadSerializerDefaultJson();
            //pipeline.AddEventSource

            pipeline.Service.Start();
        }
    }
}
