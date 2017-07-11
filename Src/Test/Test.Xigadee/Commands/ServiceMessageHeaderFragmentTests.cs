using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Collections.Generic;

namespace Test.Xigadee.Commands
{
    [TestClass]
    public class ServiceMessageHeaderFragmentTests
    {
        [TestMethod]
        public void ValueTuple1()
        {
            ServiceMessageHeaderFragment hi2 = ("messagetype", "actiontype");

            Assert.IsTrue(hi2.MessageType == "messagetype");
            Assert.IsTrue(hi2.ActionType == "actiontype");

        }

        [TestMethod]
        public void Matching()
        {
            ServiceMessageHeader hi1 = "channel/messagetype/";
            ServiceMessageHeader hi2 = ("channel", "messagetype", null);
            ServiceMessageHeader hi3 = ("channel", "messagetype", "");
            ServiceMessageHeader hi4 = ("channel", "messagetype", " ");
            ServiceMessageHeader himprobable = ("channel", "", "Howdy");

            Assert.IsTrue(hi1.ChannelId == "channel");
            Assert.IsTrue(hi1.MessageType == "messagetype");
            Assert.IsTrue(hi1.ActionType == null);
            Assert.IsTrue(hi1.IsPartialKey);

#pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsTrue(hi1 == hi1);
            Assert.IsFalse(hi1 != hi1);
#pragma warning restore CS1718 // Comparison made to same variable

            Assert.IsTrue((hi1 == hi2) && (hi2 == hi3) && (hi3 == hi4));
        }

    }
}
