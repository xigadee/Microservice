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
            var container = new SerializationContainer(new[] { new JsonContractSerializer() });

            var test = new Blah { Message = "Hmm" };
            var blob = container.PayloadSerialize(test);

            var resolve = container.PayloadDeserialize<Blah>(blob);

            Assert.AreEqual(test, resolve);
        }
    }
}
