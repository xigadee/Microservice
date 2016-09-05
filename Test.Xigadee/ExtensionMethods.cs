using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
namespace Test.Xigadee
{
    [TestClass]
    public class ExtensionMethods
    {
        [TestMethod]
        public void TestMethod1()
        {
            try
            {
                Microservice service;
                var pipeline = Microservice.Configure((s) => service = s);

                ChannelPipelineIncoming cpipeIn = null;
                ChannelPipelineOutgoing cpipeOut = null;
                PersistenceSharedService<Guid, Blah> persistence = null;
                MemoryLogger logger = null;
                MemoryBoundaryLogger bLogger = null;

                pipeline
                    .AddLogger<MemoryLogger>((l) => logger = l)
                    .AddLogger<TraceEventLogger>()
                    .AddPayloadSerializerDefaultJson()
                    .AddChannelIncoming("internalIn", internalOnly: true)
                        .AppendResourceProfile(new ResourceProfile("TrackIt"))
                        .AppendBoundaryLogger(new MemoryBoundaryLogger(), (p,bl) => bLogger = bl)
                        .AssignPriorityPartition(0, 1)
                        .AddCommand(new PersistenceBlahMemory())
                        .AddCommand(new PersistenceSharedService<Guid, Blah>(), (c) => persistence = c, cpipeOut)
                        .Revert((c) => cpipeIn = c)
                    .AddChannelOutgoing("internalOut", internalOnly: true)
                        .AssignPriorityPartition(0, 1)
                        .AppendBoundaryLogger(bLogger)
                        .Revert((c) => cpipeOut = c);

                pipeline.Start();

                Guid cId = Guid.NewGuid();
                var result = persistence.Create(new Blah() { ContentId = cId, Message = "Hello", VersionId = Guid.NewGuid() }).Result;
                Assert.IsTrue(result.IsSuccess);

                var result2 = persistence.Read(cId).Result;
                Assert.IsTrue(result2.IsSuccess);

                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }
}
