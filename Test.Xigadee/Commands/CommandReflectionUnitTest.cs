using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
namespace Test.Xigadee.Commands
{
    [TestClass]
    public class CommandReflectionUnitTest
    {
        ICommand mSimples;

        [TestInitialize]
        public void TearUp()
        {
            mSimples = new SimpleCommand2();
        }

        [TestCleanup]
        public void TearDown()
        {
            mSimples = null;
        }

        [TestMethod]
        public void TestMethod1()
        {
            var info1 = mSimples.CommandMethodSignatures(false);
            var info2 = mSimples.CommandMethodAttributeSignatures();
            //var signature = new CommandMethodSignature(mSimples, info);
        }
    }
}
