using System;
using System.Dynamic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Xigadee;

namespace Test.Xigadee.Serialization
{
    [TestClass]
    public class TestCompressors
    {
        [TestMethod]
        public void TestMethod1()
        {
            dynamic message = new ExpandoObject();

            message.Title = "Mr.";
            message.Name = "Sid";
            message.City = "London";
            message.TimeStamp = DateTime.UtcNow;
            message.Id = 42;

            var jsonIn = JsonConvert.SerializeObject(message, Formatting.None);

            byte[] data = Encoding.UTF8.GetBytes(jsonIn);

            SerializationHolder holder = data;

            var comp = new PayloadCompressorDeflate();

            bool successCompress = comp.TryCompression(holder);

            bool successDecompress = comp.TryDecompression(holder);

            string jsonOut = Encoding.UTF8.GetString(holder.Blob);

            Assert.IsTrue(jsonIn == jsonOut);
        }
    }
}
