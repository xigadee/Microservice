//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Xigadee;

//namespace Test.Xigadee.Azure
//{
//    [TestClass]
//    public class PipelineAzure1
//    {
//        #region TestContext
//        /// <summary>
//        /// All hail the Microsoft test magic man!
//        /// This class can be populated with values through Visual Studio menus -> [Test>Test Settings>Select Test Settings File] and then selecting a file with the extension .runsettings
//        /// See here for details: https://msdn.microsoft.com/en-us/library/jj635153.aspx
//        /// There is a default file default.runsettings that has a set of empty CI injection parameters specified for testing in this project.
//        /// </summary>
//        public TestContext TestContext
//        {
//            get; set;
//        }
//        #endregion

//        DebugMemoryDataCollector mDataCollector;

//        private void ConfigureServiceRoot<P>(P pipe) where P: IPipeline
//        {
//            pipe
//                .AddDataCollector((c) => mDataCollector = new DebugMemoryDataCollector())
//                .AddPayloadSerializerDefaultJson();
//        }

//        private void ChannelInConfigure(IPipelineChannelIncoming<MicroservicePipeline> inPipe)
//        {
//            inPipe
//                .AttachResourceProfile("TrackIt")
//                //.AppendBoundaryLogger(new MemoryBoundaryLogger(), (p, bl) => bLogger = bl)
//                ;
//        }

//        [TestMethod]
//        public void PipelineAI1()
//        {
//            try
//            {
//                var pipeline = new MicroservicePipeline("TestPipeline");

//                IPipelineChannelIncoming<MicroservicePipeline> cpipeIn = null;
//                IPipelineChannelOutgoing<MicroservicePipeline> cpipeOut = null;

//                PersistenceInternalClient<Guid, Blah> persistence = null;
//                PersistenceBlahMemory persistBlah = null;
//                int signalChange = 0;

//                pipeline
//                    .AdjustPolicyTaskManager((t, c) =>
//                    {
//                        t.ConcurrentRequestsMin = 1;
//                        t.ConcurrentRequestsMax = 4;
//                    })
//                    .CallOut(ConfigureServiceRoot)
//                    .AddChannelIncoming("internalIn", internalOnly: true)
//                        .CallOut(ChannelInConfigure)
//                        .AttachCommand(new PersistenceBlahMemory(), assign:(p) => persistBlah = p)
//                        .AttachCommand(new PersistenceInternalClient<Guid, Blah>(), assign:(c) => persistence = c, channelResponse: cpipeOut)
//                        .CallOut((c) => cpipeIn = c)
//                        .Revert()
//                    .AddChannelOutgoing("internalOut", internalOnly: true)
//                        .CallOut((c) => cpipeOut = c)
//                        .Revert();

//                persistBlah.OnEntityChangeAction += ((o, e) => { signalChange++; });

//                pipeline.Start();


//                Guid cId = Guid.NewGuid();
//                var blah = new Blah { ContentId = cId, Message = "Hello", VersionId = Guid.NewGuid() };
//                var result = persistence.Create(blah).Result;
//                Assert.IsTrue(result.IsSuccess);

//                var result2 = persistence.Read(cId).Result;
//                Assert.IsTrue(result2.IsSuccess);

//                blah.VersionId = Guid.NewGuid();
//                var result3 = persistence.Update(blah).Result;
//                Assert.IsTrue(result3.IsSuccess);

//                var result4 = persistence.Delete(blah.ContentId).Result;
//                Assert.IsTrue(result4.IsSuccess);


//                Assert.IsTrue(signalChange == 3);
//                pipeline.Stop();
//            }
//            catch (Exception ex)
//            {
//                Assert.Fail(ex.Message);
//            }

//        }
//    }
//}
