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
            try
            {
                var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
                var bridgein = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

                PersistenceClient<Guid, BridgeMe> init;
                DebugMemoryDataCollector memp1, memp2;

                var p1 = new MicroservicePipeline("Sender")
                    .AdjustPolicyCommunicationBoundaryLoggingActive()
                    .AddDebugMemoryDataCollector(out memp1)
                    .AddChannelIncoming("cresponse")
                        .AttachListener(bridgein.GetListener())
                        .Revert()
                    .AddChannelOutgoing("crequest")
                        .AttachSender(bridgeOut.GetSender())
                        .AttachPersistenceClient("cresponse", out init)
                        ;

                var p2 = new MicroservicePipeline("Receiver")
                    .AdjustPolicyCommunicationBoundaryLoggingActive()
                    .AddDebugMemoryDataCollector(out memp2)
                    .AddChannelIncoming("crequest")
                        .AttachMessageRedirectRule(
                              canRedirect: (p) => p.Message.MessageType.Equals("bridgeme", StringComparison.InvariantCultureIgnoreCase)
                            , redirect: (p) => p.Message.MessageType = "BridgeMe2"
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
                var rs = init.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromSeconds(20) }).Result;

                Assert.IsTrue(!rs.IsSuccess && rs.ResponseCode == 422);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class BridgeMe2
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Message { get; set; }
    }
}
