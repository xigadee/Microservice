using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xigadee;

namespace Tests.Xigadee
{
    [TestClass]
    public class SerializationJson
    {
        [TestMethod]
        public void SerializationJson1()
        {
            var s1 = new JsonRawSerializer();
            var ex1 = new S1JsonTest();

            Assert.IsTrue(s1.SupportsContentTypeSerialization(ex1.GetType()));

            var holder = new ServiceHandlerContext();

            holder.SetObject(ex1);

            s1.Serialize(holder);

            Assert.IsTrue(holder.HasContentType);
            Assert.IsFalse(holder.HasContentEncoding);
            Assert.IsFalse(holder.HasEncryption);
            Assert.IsFalse(holder.HasAuthentication);

            holder.SetObject(null, false);

            s1.Deserialize(holder);

            Assert.AreEqual(ex1.Item, ((S1JsonTest)holder.Object).Item);

            //OK, now check that these are not the same object.
            ex1.Id = Guid.NewGuid();
            Assert.AreNotEqual(ex1.Item, ((S1JsonTest)holder.Object).Item);

        }

        private class S1JsonTest
        {
            public S1JsonTest()
            {

            }

            public Guid Id { get; set; } = Guid.NewGuid();

            public string Item => Id.ToString("N").ToUpperInvariant();
        }
    }
}
