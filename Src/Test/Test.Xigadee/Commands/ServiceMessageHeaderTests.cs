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

            Assert.IsTrue(hi1.ToKey() == "channel/messagetype/actiontype");

            Assert.AreEqual(hi1,hi2);
        }

        [TestMethod]
        public void Equality()
        {
            ServiceMessageHeader hi1 = "channel/messagetype/";
            ServiceMessageHeader hi2 = ("channel", "messagetype", null);
            ServiceMessageHeader hi3 = ("channel", "messagetype", "");
            ServiceMessageHeader hi4 = ("channel", "messagetype", " ");

            Assert.IsTrue(hi1.ChannelId == "channel");
            Assert.IsTrue(hi1.MessageType == "messagetype");
            Assert.IsTrue(hi1.ActionType == null);
            Assert.IsTrue(hi1.IsPartialKey);

            Assert.IsTrue(hi1 == hi1);
            Assert.IsFalse(hi1 != hi1);

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
    }
}
