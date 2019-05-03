using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xigadee;

namespace Tests.Xigadee.AspNetCore
{
    [TestClass]
    public class EnumDefinitionTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            //https://dev.to/donbavand/integration-testing-in-net-core-20
            var test1 = new EnumDefinition(typeof(SomeEnumOrOther), TimeSpan.FromDays(10));
            var test2 = new EnumDefinition(typeof(MoreTests));

        }

        [LocalizedDescription("Hello and welcome",lang:"en-gb")]
        public enum MoreTests : long
        {
            One=1,
            Two=2,
            [System.ComponentModel.Description("Big")]
            Max = int.MaxValue,
            [LocalizedDescription("Really Big", "Code99", "en-us")]
            TotalMax = long.MaxValue

        }

    }
}
