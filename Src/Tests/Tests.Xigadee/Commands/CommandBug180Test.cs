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
                        .AttachCommand((c) => 
                        {
                            c.ResponseSet(200);
                            return Task.FromResult(0);
                        }, ("frankie","benny"))
                        .AttachCommand((ctx) =>
                        {
                            ctx.ResponseSet(200);
                            return Task.FromResult(0);
                        }, ("internalIn","frankie4fingers","benny"))
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
