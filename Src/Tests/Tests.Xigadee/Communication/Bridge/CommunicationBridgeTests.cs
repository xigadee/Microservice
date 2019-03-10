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
        public void ManualFabricTest1()
        {
            try
            {
                var fabric = new ManualFabric();

                var bridgeOut = fabric.Queue;
                var bridgein = fabric.Broadcast;

                var listenerS = bridgein.GetListener();
                var senderS = bridgeOut.GetSender();

                var listenerR = bridgeOut.GetListener();
                var senderR = bridgein.GetSender();

                PersistenceClient<Guid, BridgeMe> init;
                DebugMemoryDataCollector memp1, memp2;

                var p1 = new MicroservicePipeline("Sender")
                    .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                    .AddDebugMemoryDataCollector(out memp1)
                    .AddChannelIncoming("cresponse")
                        .AttachListener(listenerS)
                        .Revert()
                    .AddChannelOutgoing("crequest")
                        .AttachSender(senderS)
                        .AttachPersistenceClient("cresponse", out init)
                        .Revert()
                        ;

                var p2 = new MicroservicePipeline("Receiver")
                    .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                    .AddDebugMemoryDataCollector(out memp2)
                    .AddChannelIncoming("crequest")
                        .AttachListener(listenerR)
                        .AttachCommand(new PersistenceManagerHandlerMemory<Guid, BridgeMe>((e) => e.Id, (s) => new Guid(s)))
                        .Revert()
                    .AddChannelOutgoing("cresponse")
                        .AttachSender(senderR)
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
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [TestMethod]
        public void TestMethodV1()
        {
            try
            {
                var fabric = new ManualFabric();
                var bridgeOut = fabric.Queue;
                var bridgein = fabric.Broadcast;

                var listenerS = bridgein.GetListener();
                var senderS = bridgeOut.GetSender();

                var listenerR = bridgeOut.GetListener();
                var senderR = bridgein.GetSender();

                PersistenceClient<Guid, BridgeMe> init;
                DebugMemoryDataCollector memp1, memp2;

                var p1 = new MicroservicePipeline("Sender")
                    .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                    .AddDebugMemoryDataCollector(out memp1)
                    .AddChannelIncoming("cresponse")
                        .AttachListener(listenerS)
                        .Revert()
                    .AddChannelOutgoing("crequest")
                        .AttachSender(senderS)
                        .AttachPersistenceClient("cresponse", out init)
                        .Revert()
                        ;

                var p2 = new MicroservicePipeline("Receiver")
                    .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                    .AddDebugMemoryDataCollector(out memp2)
                    .AddChannelIncoming("crequest")
                        .AttachListener(listenerR)
                        .AttachCommand(new PersistenceManagerHandlerMemory<Guid, BridgeMe>((e) => e.Id, (s) => new Guid(s)))
                        .Revert()
                    .AddChannelOutgoing("cresponse")
                        .AttachSender(senderR)
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
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        private void CommunicationBridgeTests_OnExecuteBegin(object sender, TransmissionPayloadState e)
        {

        }
    }

    public class BridgeMe
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Message { get; set; }
    }
}
