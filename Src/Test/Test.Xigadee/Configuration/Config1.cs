using System;
using System.Linq;
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

        [TestMethod]
        public void Test2()
        {
            ConfigResolverMemory cr20 = null, cr30 = null;

            var msp = new MicroservicePipeline();
            msp.ConfigResolverSet(20, (ConfigResolverMemory r) => cr20 = r);
            msp.ConfigResolverSet(30, (ConfigResolverMemory r) => cr30 = r);

            Assert.IsTrue(msp.Configuration.Resolvers.Count()==4);

            cr20.Add("valueset20", "one");
            var value20 = msp.Configuration.PlatformOrConfigCache("valueset20");
            Assert.IsTrue(value20 == "one");

            cr30.Add("valueset20", "two");
            var value30 = msp.Configuration.PlatformOrConfigCache("valueset20");
            Assert.IsTrue(value30 == "one");
            msp.Configuration.CacheFlush();
            var value30b = msp.Configuration.PlatformOrConfigCache("valueset20");
            Assert.IsTrue(value30b == "two");

            msp.ConfigurationOverrideSet("valueset20", "three");
            var value30c = msp.Configuration.PlatformOrConfigCache("valueset20");
            Assert.IsTrue(value30c == "three");

        }

        [TestMethod]
        public void Test3()
        {
            ConfigResolverMemory cr20 = null, cr30 = null;

            var msp = new MicroservicePipeline();
            msp.ConfigResolverSet(20, (ConfigResolverMemory r) => cr20 = r);
            msp.ConfigResolverSet(30, (ConfigResolverMemory r) => cr30 = r);

            Assert.IsTrue(msp.Configuration.Resolvers.Count() == 4);

            cr20.Add("valueset20", "one");
            cr30.Add("valueset20", "two");

            var value20 = msp.Configuration.PlatformOrConfigCache("valueset20");
            Assert.IsTrue(value20 == "two");

        }
    }
}
