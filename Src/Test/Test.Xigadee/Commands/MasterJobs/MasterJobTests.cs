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
    [Ignore]
    public class MasterJobTests
    {
        [TestMethod]
        public void MasterJobNegotiation()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            var services = new Dictionary<string, MicroservicePipeline>();

            string masterName = null;

            Action<object, string> goingMaster = (o,s) =>
            {
                if (masterName != null)
                    Assert.Fail();
                masterName = s;
                
                mre.Set();
            };

            Action < TestMasterJobCommand> release = (c) => 
            {
                c.OnGoingMaster += (object o, string s) => goingMaster(o,s);
            };

            try
            {
                var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
                var bridgein = new CommunicationBridge(CommunicationBridgeMode.Broadcast);
                var bridgeMaster = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

                PersistenceMessageInitiator<Guid, BridgeMe> init, init3;
                DebugMemoryDataCollector memp1, memp2, memp3;
                TestMasterJobCommand mast1 = null, mast2 = null, mast3 = null;

                services.Add("Sender1", new MicroservicePipeline("Sender1")
                    .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                    .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp1 = c)
                    .AddChannelIncoming("local", internalOnly: true)
                        .AttachCommand(mast1 = new TestMasterJobCommand(), assign: release)
                        .Revert()
                    .AddChannelOutgoing("crequest")
                        .AttachSender(bridgeOut.GetSender())
                        .Revert()
                    .AddChannelIncoming("cresponse")
                        .AttachListener(bridgein.GetListener())
                        .AttachPersistenceMessageInitiator(out init, "crequest")
                        .Revert()
                    .AddChannelBroadcast("negotiate")
                        .AttachListener(bridgeMaster.GetListener())
                        .AttachSender(bridgeMaster.GetSender())
                        .AssignMasterJob(mast1)
                        .Revert()
                        );

                services.Add("Sender3", new MicroservicePipeline("Sender3")
                    .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                    .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp3 = c)
                    .AddChannelIncoming("local", internalOnly: true)
                        .AttachCommand(mast3 = new TestMasterJobCommand(), assign: release)
                        .Revert()
                    .AddChannelOutgoing("crequest")
                        .AttachSender(bridgeOut.GetSender())
                        .Revert()
                    .AddChannelIncoming("cresponse")
                        .AttachListener(bridgein.GetListener())
                        .AttachPersistenceMessageInitiator(out init3, "crequest")
                        .Revert()
                    .AddChannelBroadcast("negotiate")
                        .AttachListener(bridgeMaster.GetListener())
                        .AttachSender(bridgeMaster.GetSender())
                        .AssignMasterJob(mast3)
                        .Revert()
                        );

                services.Add("Receiver2", new MicroservicePipeline("Receiver2")
                    .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                    .AddDataCollector((c) => new DebugMemoryDataCollector(), (c) => memp2 = c)
                    .AddChannelIncoming("local", internalOnly: true)
                        .AttachCommand(mast2 = new TestMasterJobCommand(), assign: release)
                        .Revert()
                    .AddChannelIncoming("crequest")
                        .AttachListener(bridgeOut.GetListener())
                        .AttachCommand(new PersistenceManagerHandlerMemory<Guid, BridgeMe>((e) => e.Id, (s) => new Guid(s)))
                        .Revert()
                    .AddChannelOutgoing("cresponse")
                        .AttachSender(bridgein.GetSender())
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

                var entity = new BridgeMe() { Message = "Momma" };
                var rs = init.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                var rs2 = init.Read(entity.Id).Result;
                var rs3 = init3.Read(entity.Id).Result;            

                Assert.IsTrue(rs2.IsSuccess);
                Assert.IsTrue(rs3.IsSuccess);
                Assert.IsTrue(rs2.Entity.Message == "Momma");

                //Wait for one of the services to go master.
                mre.WaitOne();
                Assert.IsNotNull(masterName);

                //Ok, service 2 take over
                mre.Reset();
                var holdme1 = masterName;
                masterName = null;
                services[holdme1].Stop();
                mre.WaitOne();

                //Ok, service 3 take over
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
