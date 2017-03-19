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
    /// <summary>
    /// These test validate the pipeline.
    /// </summary>
    [TestClass]
    public partial class PipelineTest1
    {
        DebugMemoryDataCollector mDataCollector;
        bool? calloutIn = null, calloutOut = null, calloutDefault = null;

        private void ConfigureServiceRoot<P>(P pipe) where P:MicroservicePipeline
        {
            pipe
                .AddDataCollector((c) => mDataCollector = new DebugMemoryDataCollector())
                .AddPayloadSerializerDefaultJson();
        }

        private void ChannelInConfigure(IPipelineChannelIncoming<MicroservicePipeline> inPipe)
        {
            inPipe
                .AttachResourceProfile("TrackIt")
                //.AppendBoundaryLogger(new MemoryBoundaryLogger(), (p, bl) => bLogger = bl)
                ;

            calloutIn = true;
        }

        private void CallOutDefault(IPipeline pipe)
        {
            calloutDefault = true;
        }

        private void ChannelOutConfigure(IPipelineChannelOutgoing<MicroservicePipeline> inPipe)
        {
            calloutOut = true;
        }

        [TestMethod]
        public void Pipeline1()
        {
            try
            {
                var pipeline = new MicroservicePipeline("TestPipeline");

                IPipelineChannelIncoming<MicroservicePipeline> cpipeIn = null;
                IPipelineChannelOutgoing<MicroservicePipeline> cpipeOut = null;
                PersistenceInternalService<Guid, Blah> persistence = null;
                PersistenceBlahMemory persistBlah = null;
                int signalChange = 0;

                pipeline
                    .AdjustPolicyTaskManager((t) =>
                    {
                        t.ConcurrentRequestsMin = 1;
                        t.ConcurrentRequestsMax = 4;
                    })
                    .CallOut(ConfigureServiceRoot)
                    .CallOut(CallOutDefault)
                    .AddChannelIncoming("internalIn", internalOnly: true)
                        .CallOut(ChannelInConfigure, (c) => true)
                        .AttachCommand(new PersistenceBlahMemory(), assign:(p) => persistBlah = p)
                        .AttachCommand(new PersistenceInternalService<Guid, Blah>(), assign:(c) => persistence = c, channelResponse: cpipeOut)
                        .CallOut((c) => cpipeIn = c)
                        .Revert()
                    .AddChannelOutgoing("internalOut", internalOnly: true)
                        .CallOut(ChannelOutConfigure, (c) => false)
                        .CallOut((c) => cpipeOut = c)
                        .Revert();

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

                Assert.IsTrue(calloutDefault.HasValue);
                Assert.IsTrue(calloutIn.HasValue);
                Assert.IsFalse(calloutOut.HasValue);

                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }
}
