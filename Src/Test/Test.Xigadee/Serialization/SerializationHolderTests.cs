using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Serialization
{
    [TestClass]
    public class SerializationHolderTests
    {
        [TestMethod]
        public void TestCastToHolder()
        {
            SerializationHolder holder = new byte[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Assert.IsNotNull(holder?.Blob);
            Assert.IsTrue(holder.ContentType.Equals("application/octet-stream", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(holder?.Blob.Length == 10);

        }

        [TestMethod]
        public void TestCastFromHolder()
        {
            var holder = new SerializationHolder();

            holder.Blob = new byte[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            byte[] blob = holder;

            Assert.IsNotNull(blob);
            Assert.IsTrue(blob.Length == 10);

        }
    }
}
