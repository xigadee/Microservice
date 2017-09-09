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
        public class CommandHarnessTest1: CommandBase
        {
            public CommandHarnessTest1(CommandPolicy policy = null) :base(policy)
            {

            }

            [CommandContract("one","two")]
            [return: PayloadOut]
            public async Task<string> Command1([PayloadIn]string inParam, TransmissionPayload inPayload, List<TransmissionPayload> outPayload)
            {              
                return "hello";
            }

            [JobSchedule("1")]
            public async Task Schedule1()
            {
                //var back = Outgoing.Process<string,string>(("one", "two", "three"),"Hello").Result;
                Outgoing.Process(("one", "two", "three"), "Hello");
            }
        }

        [TestMethod]
        public void PartialCommandContractWithNoChannelIdSet()
        {
            var policy = new CommandPolicy();
            policy.ChannelId = null;
            CommandHarness<CommandHarnessTest1> harness;

            try
            {
                harness = new CommandHarness<CommandHarnessTest1>(() => new CommandHarnessTest1(policy));

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
            int count = 0;
            var policy = new CommandPolicy() { OutgoingRequestsEnabled = true, ResponseChannelId = "getback" };
            policy.ChannelId = "fredo";

            var harness = new CommandHarness<CommandHarnessTest1>(() => new CommandHarnessTest1(policy));

            harness.Start();

            Assert.IsTrue(harness.RegisteredCommandMethods.Count == 2);

            bool ok = false;
            harness
                .Intercept((h, a) => ok = true, CommandHarnessTrafficDirection.Outgoing, ("one", "two", "three"))
                .Intercept((h, a) => Interlocked.Increment(ref count))
                ;

            harness.Dispatcher.Process(("fredo", "two", "three"), "Helloe");
            harness.Dispatcher.Process(("fredo", "one", "two"), "Helloe", responseHeader:("1","2","3"));

            harness.ScheduleExecute("1");

            Assert.IsTrue(harness.Outgoing.Count() == 1);

            Assert.IsTrue(ok);
            Assert.IsTrue(count == 4);
        }
    }
}
