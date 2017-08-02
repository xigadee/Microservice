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
            mSimples = new SimpleCommand1();
        }

        [TestCleanup]
        public void TearDown()
        {
            mSimples = null;
        }

        [TestMethod]
        public void CommandSignatureTest()
        {
            var info1 = mSimples.CommandMethodSignatures<CommandContractAttribute>(false);
            Assert.IsTrue(info1.Count == 2);

            var info2 = mSimples.CommandMethodAttributeSignatures<CommandContractAttribute>();
            Assert.IsTrue(info2.Count == 2);

            var minfo1 = mSimples.CommandMethodSignatures<MasterJobCommandContractAttribute>(false);
            Assert.IsTrue(minfo1.Count == 2);

            var minfo2 = mSimples.CommandMethodAttributeSignatures<MasterJobCommandContractAttribute>();
            Assert.IsTrue(minfo2.Count == 2);
        }
    }
}
