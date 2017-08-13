using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This test class is used to validate MasterJob negotiation.
    /// </summary>
    [TestClass]
    public class MasterJobTests
    {
        private class EnqueueContext
        {
            public void MasterJobStateChange(object o, MasterJobStateChangeEventArgs s)
            {
                Debug.Write(s.Debug());
                Log.Add(s);

                if (s.StateNew != MasterJobState.Active)
                    return;

                if (MasterName != null || !(o is TestMasterJobCommand))
                    Assert.Fail();

                MasterName = ((TestMasterJobCommand)o).OriginatorId.Name;
                Start(MasterName);

                MasterJobSignal.Set();
            }

            public List<MasterJobEventArgsBase> Log { get; } = new List<MasterJobEventArgsBase>();

            public string MasterName { get; private set; }
            /// <summary>
            /// Use this to thread signalling.
            /// </summary>
            public ManualResetEvent MasterJobSignal { get; } = new ManualResetEvent(false);
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
                Enqueue($"{name} master job started");
                MasterName = name;
            }
            /// <summary>
            /// Logs the stop time.
            /// </summary>
            /// <param name="name">The service name.</param>
            public void Stop(string name)
            {
                Enqueue($"{name} stopping");
                Services[name].Stop();
                MasterName = null;
                Enqueue($"{name} stopped");
            }

            public void Create(string id
                , ICommunicationBridge bridgeOut, ICommunicationBridge bridgeIn, ICommunicationBridge bridgeMaster
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

                masterjob.OnMasterJobStateChange += (o, e) => MasterJobStateChange(o, e);
                masterjob.OnMasterJobCommunication += (o, e) => Log.Add(e);
            }
        }

        /// <summary>
        /// This test checks that all three Microservices negotiate correctly and take control when one of the others
        /// is stopped.
        /// </summary>
        [TestMethod]
        public void MasterJobNegotiation()
        {
            var ctx = new EnqueueContext();

            var bridgeOut = new ManualCommunicationBridgeAgent(CommunicationBridgeMode.RoundRobin);
            var bridgeIn = new ManualCommunicationBridgeAgent(CommunicationBridgeMode.Broadcast);
            var bridgeMaster = new ManualCommunicationBridgeAgent(CommunicationBridgeMode.Broadcast);

            try
            {
                PersistenceClient<Guid, BridgeMe> init1, init3;
                DebugMemoryDataCollector memp1, memp2, memp3;
                ManualChannelSender incoming = null;

                var mast1 = new TestMasterJobCommand();
                var mast2 = new TestMasterJobCommand();
                var mast3 = new TestMasterJobCommand();

                ctx.Create("Sender1", bridgeOut, bridgeIn, bridgeMaster, mast1, out init1, out memp1);
                ctx.Create("Sender3", bridgeOut, bridgeIn, bridgeMaster, mast3, out init3, out memp3);

                ctx.Add("Receiver2"
                    , new MicroservicePipeline("Receiver2")
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
                            .AttachSender(bridgeIn.GetSender(), (a) => incoming = a as ManualChannelSender)
                            .Revert()
                        .AddChannelBroadcast("negotiate")
                            .AttachListener(bridgeMaster.GetListener())
                            .AttachSender(bridgeMaster.GetSender())
                            .AssignMasterJob(mast2)
                            .Revert()
                        //.AddChannelIncoming("Deadletter")
                        //    .AttachListener(incoming.GetDeadLetterListener())
                        //    .Revert()
                    , mast2);

                ctx.Services.Values.ForEach((v) => v.Start());

                //Check that the standard comms are working.
                var entity = new BridgeMe() { Message = "Momma" };
                var rs = init1.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromSeconds(30)}).Result;
                var rs2 = init1.Read(entity.Id).Result;
                var rs3 = init3.Read(entity.Id).Result;
                Assert.IsTrue(rs2.IsSuccess);
                Assert.IsTrue(rs3.IsSuccess);
                Assert.IsTrue(rs2.Entity.Message == "Momma");

                //Wait for one of the services to go master.
                ctx.MasterJobSignal.WaitOne();
                //Ok, next service take over
                Assert.IsNotNull(ctx.MasterName);
                ctx.MasterJobSignal.Reset();
                var holdme1 = ctx.MasterName;
                ctx.Stop(holdme1);

                //Ok, final service take over
                ctx.MasterJobSignal.WaitOne();
                Assert.IsNotNull(ctx.MasterName);
                ctx.MasterJobSignal.Reset();
                var holdme2 = ctx.MasterName;
                ctx.Stop(holdme2);

                ctx.MasterJobSignal.WaitOne();
                var holdme3 = ctx.MasterName;
                ctx.Stop(ctx.MasterName);
                Assert.IsNotNull(holdme3);

                //Check that the payloads have been successfully signalled.
                Assert.IsTrue(bridgeOut.PayloadsAllSignalled);
                Assert.IsTrue(bridgeIn.PayloadsAllSignalled);
                Assert.IsTrue(bridgeMaster.PayloadsAllSignalled);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
