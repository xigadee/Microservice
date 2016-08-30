using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using Unity.WebApi;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static Microservice sExtensionService = null;

        static void ExtensionMicroserviceStart()
        {
            try
            {
                var pipeline = Microservice.Configure((s) => sExtensionService = s);

                ChannelPipelineIncoming cpipeIn = null;
                ChannelPipelineOutgoing cpipeOut = null;
                PersistenceSharedService<Guid, Blah> persistence = null;
                MemoryLogger logger = null;
                MemoryBoundaryLogger bLogger = null;

                pipeline
                    .AddLogger<MemoryLogger>((l) => logger = l)
                    .AddLogger<TraceEventLogger>()
                    .AddPayloadSerializerDefaultJson()
                    .AddChannelIncoming("internalIn")
                        .AppendResourceProfile(new ResourceProfile("TrackIt"))
                        .AppendBoundaryLogger(new MemoryBoundaryLogger(), (bl) => bLogger = bl)
                        .AssignPriorityPartition(0, 1)
                        .AttachAzureServiceBusQueueListener("Myqueue")
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
                //Assert.IsTrue(result.IsSuccess);

                var result2 = persistence.Read(cId).Result;
                //Assert.IsTrue(result2.IsSuccess);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        static void ExtensionMicroserviceStop()
        {
            sExtensionService.Stop();
            sExtensionService = null;
        }
    }
}
