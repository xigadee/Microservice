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

                var p1 = new MicroservicePipeline(nameof(CommandLocal1))
                    .AddDebugMemoryDataCollector(out memp1)
                    .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                    .AddChannelIncoming("fredo")
                        .AttachCommand(typeof(ITestCommandLocal1), (rq,rsc,pl) =>
                        {
                            var payload = rq.PayloadUnpack<string>(pl);

                            var rs = rq.ToResponse();

                            rs.PayloadPack<string>(pl, "howdy");
                            
                            rsc.Add(rs);
                            return Task.FromResult(0);
                        }
                        )
                        .AttachICommandInitiator(out init)
                        .Revert()
                        ;

                p1.Start();

                //var ok = init.Process<ITestCommandLocal1, string, string>("Hello", new RequestSettings).Result;

                p1.Stop();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
