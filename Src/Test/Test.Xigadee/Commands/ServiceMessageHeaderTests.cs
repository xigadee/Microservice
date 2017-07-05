using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Collections.Generic;

namespace Test.Xigadee.Commands
{
    [TestClass]
    public class ServiceMessageHeaderTests
    {
        [TestMethod]
        public void ValueTuple1()
        {
            ServiceMessageHeader hi1 = "channel/messagetype/actiontype";
            ServiceMessageHeader hi2 = ("channel", "messagetype", "actiontype");

            Assert.IsTrue(hi1.ChannelId == "channel");
            Assert.IsTrue(hi1.MessageType == "messagetype");
            Assert.IsTrue(hi1.ActionType == "actiontype");
            Assert.IsFalse(hi1.IsPartialKey);

            Assert.IsTrue(hi1.Key == "channel/messagetype/actiontype");

            Assert.AreEqual(hi1,hi2);
        }

        [TestMethod]
        public void Equality()
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

        [TestMethod]
        public void Inequality()
        {
            ServiceMessageHeader hi1 = "channel/messagetype/freddy";
            ServiceMessageHeader hi2 = ("channel", "messagetype", "actiontype");

            Assert.IsTrue(hi1 != hi2);
            Assert.IsFalse(hi1 == hi2);
        }

        [TestMethod]
        public void Dictionary()
        {
            var dict = new Dictionary<ServiceMessageHeader,int>();

            ServiceMessageHeader hi1 = "channel/messagetype/freddy";
            ServiceMessageHeader hi2 = ("channel", "messagetype", "actiontype");

            dict.Add(hi1, 1);
            dict.Add(hi2, 2);

            Assert.IsTrue(dict.ContainsKey(hi1));
            Assert.IsTrue(dict.ContainsKey(hi2));

            //Case insensitive equality
            Assert.IsTrue(dict.ContainsKey(("channel", "messagetype", "Actiontype")));
            Assert.IsTrue(dict[("CHANNEL", "messagetype", "FREDDY")] == 1);
            Assert.IsTrue(dict[hi2] == 2);
        }

        [TestMethod]
        public void Match1()
        {
            ServiceMessageHeader hi1 = ("channel", "messagetype", null);
            ServiceMessageHeader hi2 = "channel/messagetype/freddy";

            Assert.IsTrue(hi1.IsMatch(hi2));
            Assert.IsFalse(hi2.IsMatch(hi1));
        }

        [TestMethod]
        public void Match2()
        {
            ServiceMessageHeader hi1 = ("channel", null, null);
            ServiceMessageHeader hi2 = ("channel", "messagetype", null);

            Assert.IsTrue(hi1.IsMatch(hi2));
            Assert.IsFalse(hi2.IsMatch(hi1));
        }

        [TestMethod]
        public void Match3()
        {
            ServiceMessageHeader hi2 = ("channel", "messagetype", null);

            Assert.IsTrue(ServiceMessageHeader.Any.IsMatch(hi2));
            Assert.IsFalse(hi2.IsMatch(ServiceMessageHeader.Any));
        }
    }
}
