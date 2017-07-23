using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class MasterJobTests
    {
        private MicroservicePipeline CreateServer(string id, Action<TestMasterJobCommand> release
            , CommunicationBridge bridgeOut, CommunicationBridge bridgeIn, CommunicationBridge bridgeMaster
            , out PersistenceClient<Guid, BridgeMe> init, out DebugMemoryDataCollector memp1, out TestMasterJobCommand mast1)
        {
            return new MicroservicePipeline(id)
                .AdjustPolicyTaskManagerForDebug()
                .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                .AddDebugMemoryDataCollector(out memp1)
                .AddChannelIncoming("local", internalOnly: true)
                    .AttachCommand(mast1 = new TestMasterJobCommand(), assign: release)
                    .Revert()
                .AddChannelIncoming("cresponse")
                    .AttachListener(bridgeIn.GetListener())
                    .Revert()
                .AddChannelOutgoing("crequest")
                    .AttachSender(bridgeOut.GetSender())
                    .AttachPersistenceClient("cresponse", out init)
                    .Revert()
                .AddChannelBroadcast("negotiate")
                    .AttachListener(bridgeMaster.GetListener())
                    .AttachSender(bridgeMaster.GetSender())
                    .AssignMasterJob(mast1)
                    .Revert()
                    ;
        }

        [Ignore]
        [TestMethod]
        public void MasterJobNegotiation()
        {
            var services = new Dictionary<string, MicroservicePipeline>();
            string masterName = null;

            ManualResetEvent mre = new ManualResetEvent(false);
            Action<TestMasterJobCommand> release = (c) => c.OnGoingMaster += (object o, string s) => 
            {
                if (masterName != null)
                    Assert.Fail();

                masterName = s;

                mre.Set();
            };

            try
            {
                var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
                var bridgeIn = new CommunicationBridge(CommunicationBridgeMode.Broadcast);
                var bridgeMaster = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

                PersistenceClient<Guid, BridgeMe> init1, init3;
                DebugMemoryDataCollector memp1, memp2, memp3;
                TestMasterJobCommand mast1 = null, mast2 = null, mast3 = null;

                services.Add("Sender1", CreateServer("Sender1", release, bridgeOut, bridgeIn, bridgeMaster, out init1, out memp1, out mast1));
                services.Add("Sender3", CreateServer("Sender3", release, bridgeOut, bridgeIn, bridgeMaster, out init3, out memp3, out mast3));

                services.Add("Receiver2", new MicroservicePipeline("Receiver2")
                    .AdjustPolicyTaskManagerForDebug()
                    .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                    .AddDebugMemoryDataCollector(out memp2)
                    .AddChannelIncoming("local", internalOnly: true)
                        .AttachCommand(mast2 = new TestMasterJobCommand(), assign: release)
                        .Revert()
                    .AddChannelIncoming("crequest")
                        .AttachListener(bridgeOut.GetListener())
                        .AttachCommand(new PersistenceManagerHandlerMemory<Guid, BridgeMe>((e) => e.Id, (s) => new Guid(s)))
                        .Revert()
                    .AddChannelOutgoing("cresponse")
                        .AttachSender(bridgeIn.GetSender())
                        .Revert()
                    .AddChannelBroadcast("negotiate")
                        .AttachListener(bridgeMaster.GetListener())
                        .AttachSender(bridgeMaster.GetSender())
                        .AssignMasterJob(mast2)
                        .Revert()
                        );

                services.Values.ForEach((v) => v.Start());

                int check1 = services["Sender1"].ToMicroservice().Commands.Count();
                int check2 = services["Sender3"].ToMicroservice().Commands.Count();

                //Check that the standard comms are working.
                var entity = new BridgeMe() { Message = "Momma" };
                var rs = init1.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromSeconds(30) }).Result;
                var rs2 = init1.Read(entity.Id).Result;
                var rs3 = init3.Read(entity.Id).Result;
                Assert.IsTrue(rs2.IsSuccess);
                Assert.IsTrue(rs3.IsSuccess);
                Assert.IsTrue(rs2.Entity.Message == "Momma");

                //Wait for one of the services to go master.
                mre.WaitOne();
                Assert.IsNotNull(masterName);

                //Ok, next service take over
                mre.Reset();
                var holdme1 = masterName;
                masterName = null;
                services[holdme1].Stop();
                mre.WaitOne();

                //Ok, final service take over
                mre.Reset();
                var holdme2 = masterName;
                masterName = null;
                services[holdme2].Stop();

                mre.WaitOne();
                Assert.IsNotNull(masterName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
