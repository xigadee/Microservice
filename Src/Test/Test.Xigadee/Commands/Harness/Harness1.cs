using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class CommandHarness
    {
        public class CommandHarness1: CommandBase
        {
            public CommandHarness1(CommandPolicy policy = null) :base(policy){}

            [CommandContract("one","two")]
            [return: PayloadOut]
            public async Task<string> Command1([PayloadIn]string inParam
                , TransmissionPayload inPayload, List<TransmissionPayload> outPayload)
            {
                var back = await Outgoing.Process<string, string>(("one", "two", "three"), "Hello");
                return back.Response;
            }

            [JobSchedule("1")]
            public async Task Schedule1()
            {
                var back = await Outgoing.Process<string,string>(("one", "two", "four"),"Hello");
            }
        }

        [TestMethod]
        public void PartialCommandContractWithNoChannelIdSet()
        {
            var policy = new CommandPolicy();
            policy.ChannelId = null;
            CommandHarness<CommandHarness1> harness;

            try
            {
                harness = new CommandHarness<CommandHarness1>(policy:policy);

                harness.Start();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is CommandChannelIdNullException);
            }
        }

        [TestMethod]
        public void PartialCommandContractWithChannelIdSet()
        {
            int countTotal = 0;
            int countOutgoing = 0;
            int countResponse = 0;

            var policy = new CommandPolicy() { ChannelId = "fredo", OutgoingRequestsEnabled = true, ResponseChannelId = "getback" };

            var harness = new CommandHarness<CommandHarness1>(policy);

            harness.Start();

            Assert.IsTrue(harness.RegisteredCommandMethods.Count == 2);

            bool ok = false;
            harness
                .Intercept((ctx) => ok = true, CommandHarnessTrafficDirection.Outgoing, ("one", null,null))
                .Intercept((ctx) => Interlocked.Increment(ref countTotal))
                .Intercept((ctx) => Interlocked.Increment(ref countOutgoing), header: ("one", null, null))
                .Intercept((ctx) => Interlocked.Increment(ref countResponse), header: ("getback", null, null))
                .InterceptOutgoing((c) =>
                {
                    string rString = null;
                    c.RequestPayloadTryGet<string>(out rString);

                    c.ResponseSet<string>(200, "over and out", "Hello mum");
                })
                ;

            harness.ScheduleExecute("1");

            harness.Dispatcher.Process(("fredo", "two", "three"), "Helloe");
            harness.Dispatcher.Process(("one", "two"), "Helloe", responseHeader:("2","3"));
            
            Assert.IsTrue(harness.TrafficPayloadOutgoing.Count == 2);
            Assert.IsTrue(ok);
            Assert.IsTrue(countTotal == 7);
            Assert.IsTrue(countOutgoing == 2);
        }
    }
}
