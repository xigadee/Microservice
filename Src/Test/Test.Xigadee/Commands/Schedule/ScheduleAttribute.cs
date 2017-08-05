using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Commands.Schedule
{
    [TestClass]
    public class ScheduleAttribute
    {
        [TestMethod]
        public void CommandHarnessTest1()
        {
            var cont = new CommandHarness<CommandTest>(() => new CommandTest());

        }


        public class CommandTest: CommandBase
        {
            [JobSchedule("00:00:05","00:00:10",name:"freddy")]

            public void Schedule1()
            {

            }
        }
    }
}
