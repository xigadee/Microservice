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
    public class Harness1
    {
        public class CommandHarness1: CommandBase
        {
            public CommandHarness1(CommandPolicy policy = null) :base(policy){}

            [CommandContract("one","two")]
            [return: PayloadOut]
            public async Task<string> Command1([PayloadIn]string inParam
                , TransmissionPayload inPayload, List<TransmissionPayload> outPayload)
            {
                var back = Outgoing.Process<string, string>(("one", "two", "three"), "Hello").Result;
                return back.Response;
            }

            [JobSchedule("1")]
            public async Task Schedule1()
            {
                var back = Outgoing.Process<string,string>(("one", "two", "four"),"Hello").Result;
                //Outgoing.Process(("one", "two", "three"), "Hello");
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
                harness = new CommandHarness<CommandHarness1>((p) => new CommandHarness1(p), policy);

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

            var policy = new CommandPolicy() { OutgoingRequestsEnabled = true, ResponseChannelId = "getback" };
            policy.ChannelId = "fredo";

            var harness = new CommandHarness<CommandHarness1>((p) => new CommandHarness1(p), policy);

            harness.Start();

            Assert.IsTrue(harness.RegisteredCommandMethods.Count == 2);

            bool ok = false;
            harness
                .Intercept((ctx) => ok = true, CommandHarnessTrafficDirection.Outgoing, ("one", null,null))
                .Intercept((ctx) => Interlocked.Increment(ref countTotal))
                .Intercept((ctx) => Interlocked.Increment(ref countOutgoing), header: ("one", null, null))
                .InterceptOutgoing((c) =>
                {
                    string rString = null;
                    c.DtoTryGet<string>(out rString);

                    c.ResponseSet<string>(200, "over and out", "Hello mum");
                })
                ;

            harness.ScheduleExecute("1");

            harness.Dispatcher.Process(("fredo", "two", "three"), "Helloe");
            harness.Dispatcher.Process(("fredo", "one", "two"), "Helloe", responseHeader:("1","2","3"));
            
            Assert.IsTrue(harness.Outgoing.Count == 2);
            Assert.IsTrue(ok);
            Assert.IsTrue(countTotal == 7);
            Assert.IsTrue(countOutgoing == 2);
        }
    }
}
