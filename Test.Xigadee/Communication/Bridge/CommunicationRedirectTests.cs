using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class CommunicationRedirectTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
            var bridgein = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

            PersistenceMessageInitiator<Guid, BridgeMe> init;
            DebugMemoryDataCollector memp1, memp2;

            var p1 = new MicroservicePipeline("Sender")
                .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp1 = c)
                .AddChannelOutgoing("crequest")
                    .AttachSender(bridgeOut.GetSender())
                    .Revert()
                .AddChannelIncoming("cresponse")
                    .AttachListener(bridgein.GetListener())
                    .AttachPersistenceMessageInitiator(out init, "crequest")
                    ;

            var p2 = new MicroservicePipeline("Receiver")
                .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp2 = c)
                .AddChannelIncoming("crequest")
                .AttachMessageRedirectRule(
                      canRedirect:  (p) => p.Message.ChannelId.Equals("bridgeme", StringComparison.InvariantCultureIgnoreCase)
                    , redirect:     (p) => p.Message.ChannelId = "BridgeMe2"
                    )
                    .AttachListener(bridgeOut.GetListener())
                    .AttachCommand(new PersistenceManagerHandlerMemory<Guid, BridgeMe2>((e) => e.Id, (s) => new Guid(s)))
                    .Revert()
                .AddChannelOutgoing("cresponse")
                    .AttachSender(bridgein.GetSender())
                    ;

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

    }

    public class BridgeMe2
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Message { get; set; }
    }
}
