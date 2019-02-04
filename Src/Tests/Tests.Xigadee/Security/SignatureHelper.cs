#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class Test_Sha512SignatureHelper
    {
        [TestMethod]
        public void Test512Signature1()
        {
            var helper = new Sha512SignatureHelper("");

            var signature = helper.Sign("freddy", "got", "fingered");

            Assert.IsTrue(helper.VerifySignature(signature, "freddy", "got", "fingered"));
            Assert.IsFalse(helper.VerifySignature(signature, "Freddy", "got", "fingered"));
        }

        [TestMethod]
        public void Test512Signature2()
        {
            var helper = new Sha512SignatureHelper("");
            Assert.IsTrue(helper.VerifySignature(helper.Sign(), ""));

            var helper2 = new Sha512SignatureHelper("HelloMom");
            Assert.IsTrue(helper2.VerifySignature(helper2.Sign(), ""));
            Assert.IsTrue(helper2.VerifySignature(helper2.Sign()));
            Assert.IsFalse(helper2.VerifySignature(helper2.Sign().ToUpperInvariant(), ""));
        }

        [TestMethod]
        public void Test512Signature3()
        {
            var helper = new Sha512SignatureHelper("123");

            var signature = helper.Sign("freddy", "got", "fingered");

            try
            {
                helper.VerifySignature(null, "freddy", "got", "fingered");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentOutOfRangeException);
                return;
            }

            Assert.Fail();

        }

        [TestMethod]
        public void Test256Signature1()
        {
            var helper = new Sha256SignatureHelper("");

            var signature = helper.Sign("freddy", "got", "fingered");

            Assert.IsTrue(helper.VerifySignature(signature, "freddy", "got", "fingered"));
            Assert.IsFalse(helper.VerifySignature(signature, "Freddy", "got", "fingered"));
        }

        [TestMethod]
        public void Test256Signature2()
        {
            var helper = new Sha256SignatureHelper("");
            Assert.IsTrue(helper.VerifySignature(helper.Sign(), ""));

            var helper2 = new Sha256SignatureHelper("HelloMom");
            Assert.IsTrue(helper2.VerifySignature(helper2.Sign(), ""));
            Assert.IsTrue(helper2.VerifySignature(helper2.Sign()));
            Assert.IsFalse(helper2.VerifySignature(helper2.Sign().ToUpperInvariant(), ""));
        }

        [TestMethod]
        public void Test256Signature3()
        {
            var helper = new Sha256SignatureHelper("123");

            var signature = helper.Sign("freddy", "got", "fingered");

            try
            {
                helper.VerifySignature(null, "freddy", "got", "fingered");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentOutOfRangeException);
                return;
            }

            Assert.Fail();

        }
    }
}
