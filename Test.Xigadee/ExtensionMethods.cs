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
                var pipeline = Microservice.Configure();

                //pipeline
                //    .AddChannelIncoming("Incoming")
                //    .AddAzureSBQueueListener(

                ChannelPipelineIncoming cpipeIn = null;
                ChannelPipelineOutgoing cpipeOut = null;
                PersistenceSharedService<Guid, Blah> sharedService = null;
                MemoryLogger logger = null;

                pipeline
                    .AddLogger<MemoryLogger>((l) => logger = l)
                    .AddPayloadSerializerDefaultJson()
                    .AddChannelIncoming("internalIn", internalOnly: true)
                        .AppendResourceProfile(new ResourceProfile("TrackIt"))
                        .AssignPriorityPartition(ListenerPartitionConfig.Init(0, 1))              
                        .Revert((c) => cpipeIn = c)
                    .AddChannelOutgoing("internalOut", internalOnly: true)
                        .AssignPriorityPartition(SenderPartitionConfig.Init(0, 1))
                        .Revert((c) => cpipeOut = c);

                pipeline
                    .AddCommand(new PersistenceBlahMemory(), channelIncoming: cpipeIn)
                    .AddCommand(new PersistenceSharedService<Guid, Blah>(), channelIncoming: cpipeIn, channelResponse: cpipeOut, assignment: (c) => sharedService = c); 


                pipeline.Start();

                Guid cId = Guid.NewGuid();
                var result = sharedService.Create(new Blah() { ContentId = cId, Message = "Hello", VersionId = Guid.NewGuid() }).Result;
                var result2 = sharedService.Read(cId).Result;

                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }
}
