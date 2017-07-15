using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Communication.Algorithms
{
    [TestClass]
    public class MultipleClientPollSlotAllocationAlgorithmTests
    {
        [TestMethod]
        public void CheckOverrideChange()
        {
            var algo = new MultipleClientPollSlotAllocationAlgorithm();

            Assert.IsTrue(algo.SupportPassDueScan);
            Assert.IsTrue(algo.Name == "MultipleClientPollSlotAllocationAlgorithm");
        }
    }
}
