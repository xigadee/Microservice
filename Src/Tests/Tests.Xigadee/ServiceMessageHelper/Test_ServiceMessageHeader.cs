using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class Test_ServiceMessageHeader
    {
        [TestMethod]
        public void TestBothNull()
        {
            ServiceMessageHeader left = null;
            ServiceMessageHeader right = null;

            Assert.IsTrue(left == right);
        }

        [TestMethod]
        public void TestLeftNull()
        {
            ServiceMessageHeader left = null;
            ServiceMessageHeader right = ("1", "2", "3");

            Assert.IsFalse(left == right);
        }

        [TestMethod]
        public void TestRightNull()
        {
            ServiceMessageHeader left = ("1", "2", "3");
            ServiceMessageHeader right = null;

            Assert.IsFalse(left == right);
        }

        [TestMethod]
        public void TestRightNullEqual()
        {
            ServiceMessageHeader left = ("1", "2", "3");
            ServiceMessageHeader right = null;

            Assert.IsFalse(left.Equals(right));
        }

        [TestMethod]
        public void TestDoesNotMatch()
        {
            ServiceMessageHeader left = ("1", "2", "3");
            ServiceMessageHeader right = ("1", "2", "4");

            Assert.IsFalse(left == right);
        }

        [TestMethod]
        public void TestDoesMatch()
        {
            ServiceMessageHeader left = ("1", "2", "3");
            ServiceMessageHeader right = ("1", "2", "3");

            Assert.IsTrue(left == right);
        }
    }
}
