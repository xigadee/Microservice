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
            msp
                .ConfigurationOverrideSet("override", "one")
                .ConfigurationOverrideSet("override", "two")
                
                ;

            var value = msp.Configuration.PlatformOrConfigCache("override");

            Assert.IsTrue(value == "two");

            msp.Start();

            msp.Stop();
            
        }
    }
}
