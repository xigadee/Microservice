using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
namespace Test.Xigadee.Dispatcher
{
    [TestClass]
    public class DispatcherRetry
    {
        [TestMethod]
        public void TestMethod1()
        {
            var fabric = new ManualFabric();
            var incoming = fabric[ManualCommunicationFabricMode.Queue];
            var response = fabric[ManualCommunicationFabricMode.Broadcast];

            ICommandInitiator init;

            var ms1 = new MicroservicePipeline("server")
                .AdjustPolicyTaskManagerForDebug()
                .AddChannelIncoming("freddy")
                    .AttachListener(incoming.GetListener())
                    .AttachCommand((ctx) => throw new Exception("All messed up"),("one","two"))
                    .Revert()
                .AddChannelOutgoing("response")
                    .AttachSender(response.GetSender())
                    .Revert();

            var ms2 = new MicroservicePipeline("client")
                .AdjustPolicyTaskManagerForDebug()
                .AddChannelOutgoing("freddy")
                    .AttachSender(incoming.GetSender())
                    .Revert()
                .AddChannelIncoming("response")
                    .AttachListener(response.GetListener())
                    .AttachICommandInitiator(out init)
                    .Revert()
                    ;

            ms1.Start();
            ms2.Start();

            var rs = init.Process<string,string>(("freddy", "one","two"), "hello").Result;

            Assert.IsTrue(rs.ResponseCode == 500);
        }
    }
}
