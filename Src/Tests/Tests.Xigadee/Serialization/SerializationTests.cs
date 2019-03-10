using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Serialization
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void Test1()
        {
            var container = new ServiceHandlerContainer();

            var ser = new JsonRawSerializer();
            var sr = container.Serialization.Add(ser);

            container.Start();

            var test = new Blah { Message = "Hmm" };

            var blob = container.Serialization.SerializeToBlob(test, ser.Id);

            var resolve = container.Serialization.DeserializeToObject<Blah>(blob, ser.Id);

            Assert.AreEqual(test, resolve);
        }
    }
}
