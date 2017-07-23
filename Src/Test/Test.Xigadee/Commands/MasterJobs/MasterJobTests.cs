using System;
using System.Collections.Concurrent;
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
        private class EnqueueContext
        {
            public void Record(object o, string s)
            {
                if (MasterName != null)
                    Assert.Fail();

                MasterName = s;
                Start(MasterName);

                Mre.Set();
            }

            public string MasterName { get; set; }
            /// <summary>
            /// Use this to thread signalling.
            /// </summary>
            public ManualResetEvent Mre { get; } = new ManualResetEvent(false);
            /// <summary>
            /// This is the start time.
            /// </summary>
            public DateTime Time { get; } = DateTime.UtcNow;
            /// <summary>
            /// This is the environment tick count.
            /// </summary>
            public int Checkpoint { get; }  = Environment.TickCount;
            /// <summary>
            /// This is the set of timestamps.
            /// </summary>
            public ConcurrentQueue<ValueTuple<TimeSpan, string>> Timestamps { get; } = new ConcurrentQueue<ValueTuple<TimeSpan, string>>();
            /// <summary>
            /// This is a set of services.
            /// </summary>
            public Dictionary<string, MicroservicePipeline> Services { get; }  = new Dictionary<string, MicroservicePipeline>();

            private void Enqueue(string id)
            {
                Timestamps.Enqueue((DateTime.UtcNow - Time, id));
            }

            /// <summary>
            /// Logs the start time.
            /// </summary>
            /// <param name="name">The service name.</param>
            public void Start(string name)
            {
                Enqueue($"{name} start");
            }
            /// <summary>
            /// Logs the stop time.
            /// </summary>
            /// <param name="name">The service name.</param>
            public void Stop(string name)
            {
                Enqueue($"{name} stop");
            }

            public void CreateServer(string id, Action<TestMasterJobCommand> release
                , CommunicationBridge bridgeOut, CommunicationBridge bridgeIn, CommunicationBridge bridgeMaster
                , out PersistenceClient<Guid, BridgeMe> init, out DebugMemoryDataCollector memp1, out TestMasterJobCommand mast1)
            {
                Services.Add(id, new MicroservicePipeline(id)
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
                    );
            }
        }

        [Ignore]
        [TestMethod]
        public void MasterJobNegotiation()
        {
            var ctx = new EnqueueContext();
            Action<TestMasterJobCommand> release = (c) => c.OnGoingMaster += (object o, string s) => ctx.Record(o,s);

            try
            {
                var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
                var bridgeIn = new CommunicationBridge(CommunicationBridgeMode.Broadcast);
                var bridgeMaster = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

                PersistenceClient<Guid, BridgeMe> init1, init3;
                DebugMemoryDataCollector memp1, memp2, memp3;
                TestMasterJobCommand mast1 = null, mast2 = null, mast3 = null;

                ctx.CreateServer("Sender1", release, bridgeOut, bridgeIn, bridgeMaster, out init1, out memp1, out mast1);
                ctx.CreateServer("Sender3", release, bridgeOut, bridgeIn, bridgeMaster, out init3, out memp3, out mast3);

                ctx.Services.Add("Receiver1", new MicroservicePipeline("Receiver1")
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

                ctx.Services.Values.ForEach((v) => v.Start());

                //Check that the standard comms are working.
                var entity = new BridgeMe() { Message = "Momma" };
                var rs = init1.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromSeconds(30) }).Result;
                var rs2 = init1.Read(entity.Id).Result;
                var rs3 = init3.Read(entity.Id).Result;
                Assert.IsTrue(rs2.IsSuccess);
                Assert.IsTrue(rs3.IsSuccess);
                Assert.IsTrue(rs2.Entity.Message == "Momma");

                //Wait for one of the services to go master.
                ctx.Mre.WaitOne();
                Assert.IsNotNull(ctx.MasterName);

                //Ok, next service take over
                ctx.Mre.Reset();
                var holdme1 = ctx.MasterName;
                ctx.Stop(ctx.MasterName);

                ctx.MasterName = null;
                ctx.Services[holdme1].Stop();
                ctx.Mre.WaitOne();

                //Ok, final service take over
                ctx.Mre.Reset();
                var holdme2 = ctx.MasterName;
                ctx.Stop(ctx.MasterName);

                ctx.MasterName = null;
                ctx.Services[holdme2].Stop();

                ctx.Mre.WaitOne();
                ctx.Stop(ctx.MasterName);

                Assert.IsNotNull(ctx.MasterName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
