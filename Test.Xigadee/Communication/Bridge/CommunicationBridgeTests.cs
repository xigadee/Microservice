using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class CommunicationBridgeTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
            var bridgein = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

            PersistenceMessageInitiator<Guid,BridgeMe> init;
            DebugMemoryDataCollector memp1,memp2;

            var p1 = new MicroservicePipeline("Sender")
                .AdjustPolicyCommunication((p) => p.BoundaryLogDefaultStatus = true)
                .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp1 = c)
                .AddChannelOutgoing("out")
                    .AttachSender(bridgeOut.GetSender())
                    .Revert()
                .AddChannelIncoming("response")
                    .AttachListener(bridgein.GetListener())
                    .AttachPersistenceMessageInitiator(out init, "out")
                    ;

            var p2 = new MicroservicePipeline("Receiver")
                .AdjustPolicyCommunication((p) => p.BoundaryLogDefaultStatus = true)
                .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp2 = c)
                .AddChannelIncoming("out")
                    .AttachListener(bridgeOut.GetListener())
                    .AttachCommand(new PersistenceManagerHandlerMemory<Guid,BridgeMe>((e) => e.Id,(s) => new Guid(s)))
                    .Revert()
                .AddChannelOutgoing("response")
                    .AttachSender(bridgein.GetSender())
                    ;

            p2.ToMicroservice().Events.ExecuteBegin += CommunicationBridgeTests_OnExecuteBegin;
            p1.Start();
            p2.Start();

            int check1 = p1.ToMicroservice().Commands.Count();
            int check2 = p2.ToMicroservice().Commands.Count();

            var entity = new BridgeMe() { Message = "Momma" };
            var rs = init.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
            var rs2 = init.Read(entity.Id).Result;

            Assert.IsTrue(rs2.IsSuccess);
            Assert.IsTrue(rs2.Entity.Message == "Momma");
        }

        private void CommunicationBridgeTests_OnExecuteBegin(object sender, Microservice.TransmissionPayloadState e)
        {

        }
    }

    public class BridgeMe
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Message { get; set; }
    }
}
