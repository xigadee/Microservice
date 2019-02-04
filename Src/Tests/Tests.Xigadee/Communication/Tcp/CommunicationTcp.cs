using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Net;

namespace Test.Xigadee
{
    [Ignore]
    [TestClass]
    public class CommunicationBridgeTcp
    {
        [Ignore]
        [TestMethod]
        public void Tcp1()
        {
            try
            {
                var bridgeOut = new TcpCommunicationBridgeAgent(new IPEndPoint(IPAddress.Loopback, 8088));
                var bridgein = new TcpCommunicationBridgeAgent(new IPEndPoint(IPAddress.Loopback, 8088));

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
                        .Revert()
                        ;

                var p2 = new MicroservicePipeline("Receiver")
                    .AdjustPolicyCommunicationBoundaryLoggingActive()
                    .AddDebugMemoryDataCollector(out memp2)
                    .AddChannelIncoming("crequest")
                        .AttachListener(bridgeOut.GetListener())
                        .AttachPersistenceManagerHandlerMemory((BridgeMe e) => e.Id, (s) => new Guid(s))
                        .Revert()
                    .AddChannelOutgoing("cresponse")
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
            catch (Exception ex)
            {
                throw;
            }
        }

        private void CommunicationBridgeTests_OnExecuteBegin(object sender, Microservice.TransmissionPayloadState e)
        {

        }

        public class BridgeMe
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public string Message { get; set; }
        }

    }
}
