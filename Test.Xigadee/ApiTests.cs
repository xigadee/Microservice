using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class ApiTests
    {
        //[TestMethod]
        //public void TestMethod1()
        //{
        //    var provider = new ApiProviderAsyncV2<Guid, SomeDataClass>(new Uri("http://localhost:29001/poc/"));

        //}


        public class SomeDataClass
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public string Name { get; set; }
        }
    }
}
