using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    [TestClass]
    public class CommunicationBridgeReroute
    {
        [Contract("fredo", "Do", "Something1")]
        public interface IContractInitial : IMessageContract { }

        [Contract("crequest", "Do", "Something1")]
        public interface IContractFinal : IMessageContract { }

        /// <summary>
        /// This test is used to check that a message can be rerouted to another channel using a command
        /// </summary>
        [TestMethod]
        public void TestReroute()
        {
            var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
            var bridgein = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

            PersistenceMessageInitiator<Guid, BridgeMeReroute> init;
            DebugMemoryDataCollector memp1,memp2;

            var p1 = new MicroservicePipeline("Sender")
                .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp1 = c)
                .AddChannelIncoming("fredo")
                    .AttachCommand(typeof(IContractInitial), async (rq,rst) => rst.Add(new TransmissionPayload(rq.Message.Clone().SetDestination<IContractFinal>())))
                    .Revert()
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
                    .AttachListener(bridgeOut.GetListener())
                    .AttachCommand(new PersistenceManagerHandlerMemory<Guid, BridgeMeReroute>((e) => e.Id,(s) => new Guid(s)))
                    .Revert()
                .AddChannelOutgoing("cresponse")
                    .AttachSender(bridgein.GetSender())
                    ;

            p2.ToMicroservice().Events.ExecuteBegin += CommunicationBridgeTests_OnExecuteBegin;
            p1.Start();
            p2.Start();

            int check1 = p1.ToMicroservice().Commands.Count();
            int check2 = p2.ToMicroservice().Commands.Count();

            var entity = new BridgeMeReroute() { Message = "Momma" };
            var rs = init.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
            var rs2 = init.Read(entity.Id).Result;

            Assert.IsTrue(rs2.IsSuccess);
            Assert.IsTrue(rs2.Entity.Message == "Momma");
        }

        private void CommunicationBridgeTests_OnExecuteBegin(object sender, Microservice.TransmissionPayloadState e)
        {

        }
    }

    public class BridgeMeReroute
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Message { get; set; }
    }
}
