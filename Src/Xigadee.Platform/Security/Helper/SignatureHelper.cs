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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base class used by specific signature helpers with different hash implementations.
    /// </summary>
    /// <typeparam name="H">The has algorithm type.</typeparam>
    public abstract class SignatureHelper<H>
        where H: HashAlgorithm
    {
        #region Declarations
        protected readonly string mSalt;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="salt">The case sensitive string that serves as the salt value for the signature.</param>
        public SignatureHelper(string salt)
        {
            mSalt = salt ?? string.Empty;
        }
        #endregion

        #region Sign(params string[] signatureValues)
        /// <summary>
        /// This method creates a hash from the incoming values against the salt.
        /// </summary>
        /// <param name="signatureValues">The set of string values to hash with the salt.</param>
        /// <returns>Returns the base64 encoded hash.</returns>
        public string Sign(params string[] signatureValues)
        {
            byte[] hash = CreateHash(signatureValues);
            return Convert.ToBase64String(hash);
        }
        #endregion
        #region VerifySignature(string base64signature, params string[] signatureValues)
        /// <summary>
        /// This method verifies an existing signature against the set of string parameters passed.
        /// </summary>
        /// <param name="base64signature">The existing base64 encoded signature.</param>
        /// <param name="signatureValues">The parameter collection.</param>
        /// <returns>Returns true if the computed hash matches the one passed.</returns>
        public bool VerifySignature(string base64signature, params string[] signatureValues)
        {
            if (string.IsNullOrEmpty(base64signature))
                throw new ArgumentOutOfRangeException("signature", "signature cannot be null or empty");

            byte[] comparand = Convert.FromBase64String(base64signature);
            byte[] hash = CreateHash(signatureValues);

            return StructuralComparisons.StructuralEqualityComparer.Equals(comparand, hash);
        }
        #endregion
        #region CreateHash(IEnumerable<string> signatureValues)
        /// <summary>
        /// This private method creates a base64 hash from the salt and the parameters passed.
        /// </summary>
        /// <param name="signatureValues">The signature values to hash.</param>
        /// <returns>Returns a byte array.</returns>
        private byte[] CreateHash(IEnumerable<string> signatureValues)
        {
            string data = string.Format("{0}:{1}", mSalt, string.Join(":", signatureValues.Select(s => String.IsNullOrEmpty(s) ? string.Empty : s)));

            using (H provider = CreateProvider())
            {
                byte[] binData = Encoding.UTF8.GetBytes(data);

                byte[] hash = provider.ComputeHash(binData, 0, binData.Length);

                return hash;
            }
        } 
        #endregion

        /// <summary>
        /// This override is used to create the specific provider.
        /// </summary>
        /// <returns></returns>
        protected abstract H CreateProvider();
    }
}
