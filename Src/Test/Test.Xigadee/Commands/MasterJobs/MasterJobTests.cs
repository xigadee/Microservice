using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
            public void Record(object o, MasterJobStateChangeEventArgs s)
            {
                Debug.Write(s.Debug());
                Log.Add(s);

                if (s.StateNew != MasterJobState.Active)
                    return;

                if (MasterName != null || !(o is TestMasterJobCommand))
                    Assert.Fail();

                MasterName = ((TestMasterJobCommand)o).OriginatorId.Name;
                Start(MasterName);

                Mre.Set();
            }

            public List<MasterJobEventArgsBase> Log { get; } = new List<MasterJobEventArgsBase>();

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
            public ConcurrentQueue<ValueTuple<DateTime, string>> Timestamps { get; } = new ConcurrentQueue<ValueTuple<DateTime, string>>();
            /// <summary>
            /// This is a set of services.
            /// </summary>
            public Dictionary<string, MicroservicePipeline> Services { get; }  = new Dictionary<string, MicroservicePipeline>();

            private void Enqueue(string id)
            {
                Timestamps.Enqueue((DateTime.UtcNow, id));
            }

            /// <summary>
            /// Logs the start time.
            /// </summary>
            /// <param name="name">The service name.</param>
            public void Start(string name)
            {
                Enqueue($"{name} start");
                MasterName = name;
            }
            /// <summary>
            /// Logs the stop time.
            /// </summary>
            /// <param name="name">The service name.</param>
            public void Stop(string name)
            {
                Enqueue($"{name} stop");
                MasterName = null;
                Services[name].Stop();
            }

            public void Create(string id
                , CommunicationBridge bridgeOut, CommunicationBridge bridgeIn, CommunicationBridge bridgeMaster
                , TestMasterJobCommand masterjob
                , out PersistenceClient<Guid, BridgeMe> init, out DebugMemoryDataCollector memcollector)
            {
                var pipeline = new MicroservicePipeline(id)
                    .AdjustPolicyTaskManagerForDebug()
                    .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                    .AddDebugMemoryDataCollector(out memcollector)
                    .AddChannelIncoming("local", internalOnly: true)
                        .AttachCommand(masterjob)
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
                        .AssignMasterJob(masterjob)
                        .Revert()
                    ;

                Add(id, pipeline, masterjob);
            }

            public void Add(string id, MicroservicePipeline pipeline, TestMasterJobCommand masterjob)
            {
                Services.Add(id, pipeline);

                masterjob.OnMasterJobStateChange += (o, e) => Record(o, e);
                masterjob.OnMasterJobNegotiation += (o, e) => Log.Add(e);
            }
        }

        [Ignore]
        [TestMethod]
        public void MasterJobNegotiation()
        {
            var ctx = new EnqueueContext();

            var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
            var bridgeIn = new CommunicationBridge(CommunicationBridgeMode.Broadcast);
            var bridgeMaster = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

            try
            {
                PersistenceClient<Guid, BridgeMe> init1, init3;
                DebugMemoryDataCollector memp1, memp2, memp3;

                var mast1 = new TestMasterJobCommand();
                var mast2 = new TestMasterJobCommand();
                var mast3 = new TestMasterJobCommand();

                ctx.Create("Sender1", bridgeOut, bridgeIn, bridgeMaster, mast1, out init1, out memp1);

                ctx.Create("Sender3", bridgeOut, bridgeIn, bridgeMaster, mast3, out init3, out memp3);

                ctx.Add("Receiver1"
                    , new MicroservicePipeline("Receiver1")
                        .AdjustPolicyTaskManagerForDebug()
                        .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                        .AddDebugMemoryDataCollector(out memp2)
                        .AddChannelIncoming("local", internalOnly: true)
                            .AttachCommand(mast2)
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
                    , mast2);

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
                //Ok, next service take over
                Assert.IsNotNull(ctx.MasterName);
                ctx.Mre.Reset();
                var holdme1 = ctx.MasterName;
                ctx.Stop(holdme1);

                //Ok, final service take over
                ctx.Mre.WaitOne();
                Assert.IsNotNull(ctx.MasterName);
                ctx.Mre.Reset();
                var holdme2 = ctx.MasterName;
                ctx.Stop(holdme2);

                ctx.Mre.WaitOne();
                var holdme3 = ctx.MasterName;
                ctx.Stop(ctx.MasterName);
                Assert.IsNotNull(holdme3);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
