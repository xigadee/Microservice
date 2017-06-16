using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class PolicyTaskManagerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var policy = new TaskManagerPolicy();

            policy.BulkheadReserve(4,4,4);

            Assert.IsTrue(policy.PriorityLevelReservations.Max((p) => p.Level) == 4);


        }
    }
}
