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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class AesEncryptionTests
    {
        private readonly byte[] mKey = Convert.FromBase64String("hNCV1t5sA/xQgDkHeuXYhrSu8kF72p9H436nQoLDC28=");
        private readonly AesEncryptionHandler mAesEncryption;

        public AesEncryptionTests()
        {
            mAesEncryption = new AesEncryptionHandler(mKey, 256, false);
        }

        [TestMethod]
        public void EncryptDecrypt()
        {
            var secret = "I know a secret";
            var encryptedData = mAesEncryption.Encrypt(Encoding.UTF8.GetBytes(secret));
            Assert.AreNotEqual(secret, Encoding.UTF8.GetString(encryptedData));

            // Verify that the string can be decrypted
            var decryptedData = mAesEncryption.Decrypt(encryptedData);
            Assert.AreEqual(secret, Encoding.UTF8.GetString(decryptedData));

            var encryptedData2 = mAesEncryption.Encrypt(Encoding.UTF8.GetBytes(secret));
            Assert.IsFalse(encryptedData.SequenceEqual(encryptedData2));
        }

        [TestMethod]
        public void EncryptDecryptWithCompression()
        {
            var encryption = new AesEncryptionHandler(mKey, 256);
            var secret = "I know a secret";
            var encryptedData = encryption.Encrypt(Encoding.UTF8.GetBytes(secret));
            Assert.AreNotEqual(secret, Encoding.UTF8.GetString(encryptedData));

            // Verify that the string can be decrypted
            var decryptedData = encryption.Decrypt(encryptedData);
            Assert.AreEqual(secret, Encoding.UTF8.GetString(decryptedData));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidKeySize()
        {
            var encryption = new AesEncryptionHandler(mKey, 128);
            Assert.Fail("Key size is incorrect so should have thrown an exception");
        }
    }
}
