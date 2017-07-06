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
using System.Threading.Tasks;

namespace Test.Xigadee
{
    [TestClass]
    public partial class PipelineTest3
    {
        /// <summary>
        /// This test does basic inline command tests.
        /// </summary>
        [TestMethod]
        public void Pipeline3()
        {
            try
            {
                var pipeline = new MicroservicePipeline("TestPipeline");
                var destination = ServiceMessageHeader.FromKey("internalIn/frankie/benny");

                ICommandInitiator init;
                DebugMemoryDataCollector collector;

                pipeline
                    .AddDebugMemoryDataCollector(out collector)
                    .AdjustPolicyTaskManagerForDebug()
                    .AddChannelIncoming("internalIn", internalOnly: true)
                        .AttachCommand((prq,prs,c) => 
                        {
                            string entity;

                            if ((prq, prs, c).RequestTryGet(out entity))
                            {
                                Assert.AreEqual(entity, "Hello");
                                (prq, prs, c).ResponseSet(200, entity + "Good good", "It's all good");
                            }
                            else
                                (prq, prs, c).ResponseSet(400, description: "It's all messed up.");

                            return Task.FromResult(0);
                        }
                        , destination)
                        .Revert()
                    .AddChannelOutgoing("internalOut", internalOnly: true)
                        .AttachICommandInitiator(out init)
                        .Revert();

                pipeline.Start();

                var rs1 = init.Process<string, string>(destination, "Hello").Result;
                var rs2 = init.Process<string, string>(destination, null).Result;

                Assert.IsTrue(rs1.ResponseCode.Value == 200);
                Assert.IsTrue(rs1.Response == "HelloGood good");
                Assert.IsTrue(rs2.ResponseCode.Value == 400);

                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
