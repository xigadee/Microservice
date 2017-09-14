using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Commands
{
    [TestClass]
    public class ScheduleAttribute
    {
        [TestMethod]
        public void CommandHarnessTest1()
        {
            var cont = new CommandHarness<CommandTest>();
            cont.Start();

            Assert.IsTrue(cont.RegisteredSchedules.Count == 10);
        }

        public class CommandTest: CommandBase
        {
            public CommandTest()
            {

            }
            [JobSchedule(nameof(Schedule1), "01:00:00", "00:00:10")]
            public void Schedule1(Schedule schedule, CancellationToken token)
            {

            }

            [JobSchedule(nameof(Schedule1a))]
            public void Schedule1a()
            {

            }

            [JobSchedule(nameof(Schedule1b))]
            public void Schedule1b(Schedule schedule)
            {

            }

            [JobSchedule(nameof(Schedule1c))]
            public void Schedule1c(CancellationToken token)
            {

            }

            [JobSchedule(nameof(Schedule1d))]
            public void Schedule1d(CancellationToken token, Schedule schedule)
            {

            }

            [JobSchedule(nameof(Schedule2), "00:00:05", "00:00:10")]
            public async Task Schedule2(Schedule schedule, CancellationToken token)
            {

            }

            [JobSchedule(nameof(Schedule2a), "00:00:05", "00:00:10")]
            public async Task Schedule2a()
            {

            }

            [JobSchedule(nameof(Schedule2b), "00:00:05", "00:00:10")]
            public async Task Schedule2b(Schedule schedule)
            {

            }

            [JobSchedule(nameof(Schedule2c), "00:00:05", "00:00:10")]
            public async Task Schedule2c(CancellationToken token)
            {

            }

            [JobSchedule(nameof(Schedule2d), "00:00:05", "00:00:10")]
            public async Task Schedule2d(CancellationToken token, Schedule schedule)
            {

            }
        }
    }
}
