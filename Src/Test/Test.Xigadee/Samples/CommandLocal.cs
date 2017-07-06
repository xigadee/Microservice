using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Threading.Tasks;

namespace Test.Xigadee.Samples
{
    [TestClass]
    public class CommandLocal
    {
        [Contract("fredo", "CommandLocal", "1")]
        public interface ITestCommandLocal1 : IMessageContract { }
        [TestMethod]
        public void CommandLocal1()
        {
            try
            {
                DebugMemoryDataCollector memp1;
                ICommandInitiator init;

                var p1 = new MicroservicePipeline(nameof(CommandLocal1), serviceReference: typeof(CommandLocal))
                    .AddDebugMemoryDataCollector(out memp1)
                    //.AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                    .AddICommandInitiator(out init)
                    .AddChannelIncoming("fredo")
                        .AttachCommand(typeof(ITestCommandLocal1), (ctx) =>
                            {
                                var message = ctx.DtoGet<string>();
                                ctx.ResponseSet(200, "howdy");

                                return Task.FromResult(0);
                            }
                        )
                        .Revert();

                p1.Start();

                var ok = init.Process<ITestCommandLocal1, string, string>("Hello").Result;

                p1.Stop();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
