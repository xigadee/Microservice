using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Communication.Algorithms
{
    public class TestPriorityHolderMock: IClientPriorityHolderMetrics
    {
        public double CapacityPercentage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? FabricPollWaitTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int? LastPollTickCount => throw new NotImplementedException();

        public int? LastReserved { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan MaxAllowedPollWait { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan MinExpectedPollWait { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long PollAchievedBatch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long PollAttemptedBatch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public decimal PollSuccessRate => throw new NotImplementedException();

        public decimal? PollTimeReduceRatio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long? PriorityCalculated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public long? PriorityQueueLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? PriorityTickCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public decimal PriorityWeighting => throw new NotImplementedException();

        public IResourceRequestRateLimiter RateLimiter => throw new NotImplementedException();

        public int SkipCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool SkipCountDecrement()
        {
            throw new NotImplementedException();
        }
    }

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
