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

namespace Xigadee
{
    /// <summary>
    /// Provide symmetric i.e. Encrypt / Decrypt Functionality
    /// </summary>
    public interface IEncryptionHandler
    {
        /// <summary>
        /// Encrypt a byte array
        /// </summary>
        /// <param name="input">The byte array to encrypt.</param>
        /// <returns>Returns the encrypted byte array.</returns>
        byte[] Encrypt(byte[] input);

        /// <summary>
        /// Decrypt a byte array
        /// </summary>
        /// <param name="input">The byte array to decrypt.</param>
        /// <returns>Returns the decrpypted byte array.</returns>
        byte[] Decrypt(byte[] input);
    }
}
