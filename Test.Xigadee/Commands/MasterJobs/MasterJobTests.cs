using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class MasterJobTests
    {
        [TestMethod]
        public void Startup()
        {
            try
            {

                var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
                var bridgein = new CommunicationBridge(CommunicationBridgeMode.Broadcast);
                var bridgeMaster = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

                PersistenceMessageInitiator<Guid, BridgeMe> init, init3;
                DebugMemoryDataCollector memp1, memp2, memp3;

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

                var p3 = new MicroservicePipeline("Sender")
                    .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                    .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp3 = c)
                    .AddChannelOutgoing("crequest")
                        .AttachSender(bridgeOut.GetSender())
                        .Revert()
                    .AddChannelIncoming("cresponse")
                        .AttachListener(bridgein.GetListener())
                        .AttachPersistenceMessageInitiator(out init3, "crequest")
                        ;

                var p2 = new MicroservicePipeline("Receiver")
                    .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                    .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp2 = c)
                    .AddChannelIncoming("crequest")
                        .AttachListener(bridgeOut.GetListener())
                        .AttachCommand(new PersistenceManagerHandlerMemory<Guid, BridgeMe>((e) => e.Id, (s) => new Guid(s)))
                        .Revert()
                    .AddChannelOutgoing("cresponse")
                        .AttachSender(bridgein.GetSender())
                        ;


                p1.Start();
                p2.Start();
                p3.Start();

                int check1 = p1.ToMicroservice().Commands.Count();
                int check2 = p2.ToMicroservice().Commands.Count();

                var entity = new BridgeMe() { Message = "Momma" };
                var rs = init.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                var rs2 = init.Read(entity.Id).Result;
                var rs3 = init3.Read(entity.Id).Result;

                Assert.IsTrue(rs2.IsSuccess);
                Assert.IsTrue(rs3.IsSuccess);
                Assert.IsTrue(rs2.Entity.Message == "Momma");

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
