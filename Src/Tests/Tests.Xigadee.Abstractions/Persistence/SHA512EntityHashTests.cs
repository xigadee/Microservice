using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Tests.Xigadee
{
    [TestClass]
    public class SHA512EntityHashTests
    {
        [TestInitialize]
        public void Init()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Sha512()
        {
            var hashPolicy = new Sha512SignaturePolicyWrapper();

            var rootPolicy = new TestClassSignaturePolicy();

            hashPolicy.RegisterChildPolicy(rootPolicy);

            var c1 = new TestClass();

            var sig = hashPolicy.Calculate(c1);

            Assert.IsTrue(hashPolicy.Verify(c1, sig));

            c1.Name = "Freddy";
            c1.VersionId = Guid.NewGuid();

            Assert.IsFalse(hashPolicy.Verify(c1, sig));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AesSha512()
        {
            var hashPolicy = AesSha512SignaturePolicyWrapper.CreateTestPolicy();

            var rootPolicy = new TestClassSignaturePolicy();

            hashPolicy.RegisterChildPolicy(rootPolicy);

            var c1 = new TestClass();

            var sig = hashPolicy.Calculate(c1);

            Assert.IsTrue(hashPolicy.Verify(c1, sig));

            c1.Name = "Freddy";
            c1.VersionId = Guid.NewGuid();

            Assert.IsFalse(hashPolicy.Verify(c1, sig));

        }
    }
}
