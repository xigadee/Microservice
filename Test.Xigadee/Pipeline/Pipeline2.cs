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
    public partial class PipelineTest2
    {
        [TestMethod]
        public void Pipeline2()
        {
            try
            {
                DebugMemoryDataCollector collector = null;
                Microservice service;
                CommandInitiator init = null;
                var pipeline = Microservice.Create((s) => service = s, serviceName: "TestPipeline");

                ChannelPipelineIncoming cpipeIn = null;
                ChannelPipelineOutgoing cpipeOut = null;

                pipeline
                    .AddDataCollector<DebugMemoryDataCollector>((c) => collector = c)
                    .AddPayloadSerializerDefaultJson()
                    .AddChannelIncoming("internalIn", internalOnly: true)
                        .AssignPriorityPartition(0, 1)
                        .AddCommand(new SimpleCommand())
                        .Revert((c) => cpipeIn = c)
                    .AddChannelOutgoing("internalOut", internalOnly: true)
                        .AssignPriorityPartition(0, 1)
                        .Revert((c) => cpipeOut = c)
                    .AddCommand(new CommandInitiator(), (c) => init = c);

                pipeline.Start();

                var result1 = init.Process<IDoSomething1, Blah, string>(new Blah() { Message = "hello" }).Result;
                var result2 = init.Process<Blah,string>("internalIn", "franky", "johnny5", new Blah() { Message = "hello" }).Result;

                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
