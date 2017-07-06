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
    public partial class Test3Bug180
    {

        [TestMethod]
        public void Bug180()
        {
            try
            {
                ServiceMessageHeader destination = ("internalIn/frankie/benny");

                ICommandInitiator init;
                DebugMemoryDataCollector collector;

                var pipeline = new MicroservicePipeline("TestPipeline")
                    .AdjustPolicyTaskManagerForDebug()
                    .AddDebugMemoryDataCollector(out collector)
                    .AddICommandInitiator(out init)
                    .AddChannelIncoming("internalIn", internalOnly: true)
                        .AttachCommand((prq, prs, c) => 
                        {
                            return Task.FromResult(0);
                        }
                        , destination)
                        .AttachCommand((prq, prs, c) =>
                        {
                            return Task.FromResult(0);
                        }
                        , ("internalIn/frankie4fingers/benny"))
                        .Revert()
                    .AddChannelOutgoing("internalOut", internalOnly: true)
                        .Revert();

                pipeline.Start();

                var rs = init.Process<string,string>(destination, "hello").Result;

                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }
}
