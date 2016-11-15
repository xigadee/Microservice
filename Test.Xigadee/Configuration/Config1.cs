using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
namespace Test.Xigadee
{
    [TestClass]
    public class Config1
    {
        [TestMethod]
        public void Test1()
        {
            var msp = new MicroservicePipeline();
            msp.ConfigurationOverrideSet("override", "one");
            var value1 = msp.Configuration.PlatformOrConfigCache("override");
            Assert.IsTrue(value1 == "one");

            msp.ConfigurationOverrideSet("override", "two");
            var value2 = msp.Configuration.PlatformOrConfigCache("override");
            Assert.IsTrue(value2 == "two");

            msp.Start();

            msp.Stop();
            
        }
    }
}
