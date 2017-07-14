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
    public partial class PipelineTest2
    {
        [Contract("internalIn", "franky", "johnny5")]
        public interface IPipelineTest2: IMessageContract { }

        [TestMethod]
        public void Pipeline2()
        {
            try
            {
                DebugMemoryDataCollector collector1, collector2;
                ICommandInitiator init = null;

                IPipelineChannelIncoming<MicroservicePipeline> cpipeIn = null;
                IPipelineChannelOutgoing<MicroservicePipeline> cpipeOut = null;

                var bridgeOut = new CommunicationBridge(CommunicationBridgeMode.RoundRobin);
                var bridgeReturn = new CommunicationBridge(CommunicationBridgeMode.Broadcast);

                //bridgeReturn.Agent.OnReceive += (o, e) => { if (e.Payload.Extent.Days == 42) init.ToString(); };
                //bridgeReturn.Agent.OnException += (o, e) => { if (e.Payload.Extent.Days == 42) init.ToString(); };

                var pClient = new MicroservicePipeline("Client");
                var pServer = new MicroservicePipeline("Server");
                    
                pClient
                    .AdjustPolicyTaskManagerForDebug()
                    .AddDebugMemoryDataCollector(out collector1)
                    .AddPayloadSerializerDefaultJson()
                    .AddChannelIncoming("return")
                        .AttachListener(bridgeReturn.GetListener())
                        .AttachMessageProcessPriorityOverride()
                        .AttachICommandInitiator(out init)
                        .Revert()
                    .AddChannelOutgoing("internalIn", internalOnly: false
                        , autosetPartition01: false
                        , assign: (p,c) => cpipeOut = p)
                        .AttachPriorityPartition(0, 1)
                        .AttachSender(bridgeOut.GetSender())
                        .Revert();

                pServer
                    .AdjustPolicyTaskManagerForDebug()
                    .AddDebugMemoryDataCollector(out collector2)
                    .AddPayloadSerializerDefaultJson()
                    .AddChannelIncoming("internalIn", internalOnly: false
                        , autosetPartition01: false
                        , assign: (p, c) => cpipeIn = p)
                        .AttachPriorityPartition(0, 1)
                        .AttachListener(bridgeOut.GetListener())
                        .AttachCommand((CommandInlineContext ctx) =>
                        {
                            var payload = ctx.DtoGet<Blah>();
                            ctx.ResponseSet(200, payload.Message);
                            return Task.FromResult(0);
                        }, ("franky", "johnny5"))
                        .Revert()
                    .AddChannelOutgoing("return")
                        .AttachSender(bridgeReturn.GetSender())
                        .Revert();
                    ;

                pClient.Start();
                pServer.Start();

                var result1 = init.Process<IPipelineTest2, Blah, string>(new Blah() { Message = "hello1" }).Result;
                var result2 = init.Process<Blah, string>("internalIn", "franky", "johnny5", new Blah() { Message = "hello2" }).Result;

                pClient.Stop();
                pServer.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
