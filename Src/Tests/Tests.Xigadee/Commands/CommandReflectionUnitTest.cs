using System;
using System.Linq;
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
            var info1 = mSimples.CommandMethodSignatures<CommandContractAttribute, CommandMethodSignature>(false,true).ToList();
            Assert.IsTrue(info1.Count == 2);

            var minfo1 = mSimples.CommandMethodSignatures<MasterJobCommandContractAttribute, CommandMethodSignature>(false, true).ToList();
            Assert.IsTrue(minfo1.Count == 2);
        }
    }
}
