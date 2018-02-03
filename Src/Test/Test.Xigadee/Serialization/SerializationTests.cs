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

            var sr = container.Serialization.Add(new JsonContractSerializer());

            container.Start();

            var test = new Blah { Message = "Hmm" };

            var blob = container.Serialization.SerializeToBlob(test);

            var resolve = container.Serialization.DeserializeToObject<Blah>(blob);

            Assert.AreEqual(test, resolve);
        }
    }
}
