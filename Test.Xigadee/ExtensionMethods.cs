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
            var service = new Microservice();

            service.AddChannelIncoming("CRM");

            service.AddChannelOutgoing("API");

            //    .ConfigureServiceBusQueue()
            //    .ConfigureApiListener();


            service.Start();
        }
    }
}
