using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Threading.Tasks;
using System.Threading;

namespace Test.Xigadee
{
    [TestClass]
    public class CommunicationBridgeReroute
    {
        [Contract("fredo", "Do", "Something1")]
        public interface IContractInitial : IMessageContract { }

        [Contract("crequest", "Did", "SomethingElse")]
        public interface IContractFinal : IMessageContract { }

        /// <summary>
        /// This test is used to check that a message can be rerouted to another channel using a message Clone and SetDestination command.
        /// </summary>
        [TestMethod]
        public void TestReroute()
        {
            var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
            bool success = false;
            ManualResetEvent mre = new ManualResetEvent(false);
            DebugMemoryDataCollector memp1,memp2;

            var p1 = new MicroservicePipeline("Sender")
                .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                .AddDebugMemoryDataCollector(out memp1)
                .AddChannelIncoming("fredo")
                    .AttachCommand(typeof(IContractInitial), async (rq,rst,ps) =>
                    {                  
                        rst.Add(new TransmissionPayload(rq.Message.Clone().SetDestination<IContractFinal>()));
                    })
                    .Revert()
                .AddChannelOutgoing("crequest")
                    .AttachSender(bridgeOut.GetSender())
                    .Revert()
                    ;

            var p2 = new MicroservicePipeline("Receiver")
                .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                .AddDebugMemoryDataCollector(out memp2)
                .AddChannelIncoming("crequest")
                    .AttachListener(bridgeOut.GetListener())
                    .AttachCommand(typeof(IContractFinal), async (rq,rst,ps) =>
                    {
                        var value = ps.PayloadDeserialize<string>(rq);
                        success = value == "Hello";
                        mre.Set();
                    })
                    .Revert()
                    ;

            p1.Start();
            p2.Start();

            int check1 = p1.ToMicroservice().Commands.Count();
            int check2 = p2.ToMicroservice().Commands.Count();

            p1.ToMicroservice().Dispatch.Process<IContractInitial>("Hello");

            mre.WaitOne();

            Assert.IsTrue(success);

            p1.Stop();
            p2.Stop();
        }

    }
}
