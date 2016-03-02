using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class Redis_Cache
    {
        public class WorkflowTest
        {
            public WorkflowTest()
            {
                When = DateTime.UtcNow;
            }

            public string Id { get; set; }

            public DateTime? When { get; set; }

        }
        [TestMethod]
        public void TestMethod1()
        {
            var manager = RedisCacheManager.Default<string,WorkflowTest>("vrdevredis.redis.cache.windows.net:6380,password=5e3d540IhGPh/uPLTIfIIDaCRo3CIgR1T8rN9vVNlOg=,ssl=True,abortConnect=False");

            var entity1 = new WorkflowTest() { Id = "Paul2" };
            bool success = manager.Write(entity1.Id, "blur").Result;
        }
    }
}
