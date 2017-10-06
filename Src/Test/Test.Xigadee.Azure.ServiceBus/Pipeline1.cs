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

namespace Test.Xigadee.Azure.ServiceBus
{
    /// <summary>
    /// These test validate the pipeline.
    /// </summary>
    [TestClass]
    public partial class PipelineTest1
    {
        [TestMethod]
        public void Pipeline1()
        {
            var fabric = new ManualFabricBridge();

            var server = new MicroservicePipeline()
                .AdjustPolicyTaskManagerForDebug()
                .AddChannelIncoming("incoming")
                    .AttachListener(fabric.Queue.GetListener())
                    .AttachCommand((ctx) => 
                    {
                        ctx.ResponseSet(200, "Howdy");
                        return Task.FromResult(0);
                    }
                    , ("one","two"))
                    .Revert()
                .AddChannelOutgoing("outgoing")
                    .AttachSender(fabric.Broadcast.GetSender())
                    .Revert()
                ;

            ICommandInitiator init;
            var client = new MicroservicePipeline()
                .AdjustPolicyTaskManagerForDebug()
                .AddChannelOutgoing("incoming")
                    .AttachSender(fabric.Queue.GetSender())
                    .Revert()
                .AddChannelIncoming("outgoing")
                    .AttachListener(fabric.Broadcast.GetListener())
                    .AttachICommandInitiator(out init)
                    .Revert()
                ;

            server.Start();
            client.Start();

            int t1 = Environment.TickCount;
            var result1 = init.Process<string, string>(("incoming", "one", "two"), "Hello").Result;
            int t2 = Environment.TickCount;
            var result2 = init.Process<string, string>(("incoming", "one", "two"), "Hello").Result;
            int t3 = Environment.TickCount;
            var result3 = init.Process<string, string>(("incoming", "one", "two"), "Hello").Result;
            int t4 = Environment.TickCount;
        }
    }
}
