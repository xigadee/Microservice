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
            var pipeline = Microservice.Configure();

            //pipeline
            //    .AddChannelIncoming("Incoming")
            //    .AddAzureSBQueueListener(

            pipeline
                .AddLogger<MemoryLogger>()
                .AddChannelIncoming("internal", internalOnly:true)
                //.AddCommand((c) => new PersistenceBlahMemory())
                ;
                

            //pipeline
            //    .AddChannelOutgoing("Return");

            ////    .ConfigureServiceBusQueue()
            ////    .ConfigureApiListener();
            //pipeline.AddPayloadSerializerDefaultJson();
            

            pipeline.Start();
        }
    }
}
