#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
namespace Test.Xigadee
{
    [TestClass]
    public partial class PipelineTest1
    {
        DebugMemoryDataCollector mDataCollector;

        private void ConfigureServiceRoot(MicroservicePipeline pipe)
        {
            pipe
                .AddDataCollector<DebugMemoryDataCollector>((c) => mDataCollector = c)
                .AddLogger<TraceEventLogger>()
                .AddPayloadSerializerDefaultJson();
        }

        private void ChannelInConfigure(ChannelPipelineIncoming inPipe)
        {
            inPipe
                .AttachResourceProfile("TrackIt")
                //.AppendBoundaryLogger(new MemoryBoundaryLogger(), (p, bl) => bLogger = bl)
                ;
        }

        [TestMethod]
        public void Pipeline1()
        {
            try
            {
                Microservice service;
                var pipeline = Microservice.Create((s) => service = s, serviceName: "TestPipeline");

                ChannelPipelineIncoming cpipeIn = null;
                ChannelPipelineOutgoing cpipeOut = null;
                PersistenceSharedService<Guid, Blah> persistence = null;
                PersistenceBlahMemory persistBlah = null;
                int signalChange = 0;

                pipeline
                    .AdjustPolicyTaskManager((t) =>
                    {
                        t.ConcurrentRequestsMin = 1;
                        t.ConcurrentRequestsMax = 4;
                    })
                    .CallOut(ConfigureServiceRoot)
                    .AddChannelIncoming("internalIn", internalOnly: true)
                        .CallOut(ChannelInConfigure)
                        .AttachCommand(new PersistenceBlahMemory(),(p) => persistBlah = p)
                        .AttachCommand(new PersistenceSharedService<Guid, Blah>(), (c) => persistence = c, cpipeOut)
                        .Revert((c) => cpipeIn = c)
                    .AddChannelOutgoing("internalOut", internalOnly: true)
                        .Revert((c) => cpipeOut = c);

                persistBlah.OnEntityChangeAction += ((o, e) => { signalChange++; });

                pipeline.Start();


                Guid cId = Guid.NewGuid();
                var blah = new Blah { ContentId = cId, Message = "Hello", VersionId = Guid.NewGuid() };
                var result = persistence.Create(blah).Result;
                Assert.IsTrue(result.IsSuccess);

                var result2 = persistence.Read(cId).Result;
                Assert.IsTrue(result2.IsSuccess);

                blah.VersionId = Guid.NewGuid();
                var result3 = persistence.Update(blah).Result;
                Assert.IsTrue(result3.IsSuccess);

                var result4 = persistence.Delete(blah.ContentId).Result;
                Assert.IsTrue(result4.IsSuccess);


                Assert.IsTrue(signalChange == 3);
                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }
}
