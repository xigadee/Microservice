using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class Channels
    {

        [TestMethod]
        public void TestBasicAddRemove()
        {
            TestsAddRemove(new Channel("Hello", ChannelDirection.Incoming));
            TestsAddRemove(new Channel("Hello", ChannelDirection.Outgoing));
            TestsAddRemove(new Channel("Hello", ChannelDirection.Bidirectional));
        }

        private void TestsAddRemove(Channel c1)
        {
            CommunicationContainer mContainer
                = new CommunicationContainer(new CommunicationPolicy() { AutoCreateChannels = true });

            Assert.IsTrue(mContainer.Add(c1));

            Assert.IsTrue(mContainer.Channels.Count() == 1);

            Assert.IsTrue(mContainer.Exists(c1.Id, c1.Direction));

            Assert.IsTrue(mContainer.Remove(c1));

            Assert.IsFalse(mContainer.Exists(c1.Id, c1.Direction));

            Assert.IsTrue(mContainer.Channels.Count() == 0);
        }


        [TestMethod]
        public void TestChannelAutocreateSuccess()
        {
            TestChannelAutocreateSuccess(ChannelDirection.Incoming);
            TestChannelAutocreateSuccess(ChannelDirection.Outgoing);
            TestChannelAutocreateSuccess(ChannelDirection.Bidirectional);
        }

        private void TestChannelAutocreateSuccess(ChannelDirection direction)
        {
            CommunicationContainer mContainer
                = new CommunicationContainer(new CommunicationPolicy() { AutoCreateChannels = true });

            Channel channel;

            Assert.IsTrue(mContainer.TryGet("Freddy", direction, out channel));

            Assert.IsTrue(mContainer.Exists("Freddy", direction));

            Assert.IsTrue(channel.IsAutoCreated);

            Assert.IsTrue(mContainer.Channels.Count() == 1);

        }

        [TestMethod]
        public void TestChannelAutocreateFail()
        {
            TestChannelAutocreateFail(ChannelDirection.Incoming);
            TestChannelAutocreateFail(ChannelDirection.Outgoing);
            TestChannelAutocreateFail(ChannelDirection.Bidirectional);
        }

        private void TestChannelAutocreateFail(ChannelDirection direction)
        {
            CommunicationContainer mContainer
                = new CommunicationContainer(new CommunicationPolicy() { AutoCreateChannels = false });

            Channel channel = null;

            Assert.IsFalse(mContainer.TryGet("Freddy", direction, out channel));

            Assert.IsTrue(channel == null);

            Assert.IsFalse(mContainer.Exists("Freddy", direction));

            Assert.IsTrue(mContainer.Channels.Count() == 0);

        }
    }
}
