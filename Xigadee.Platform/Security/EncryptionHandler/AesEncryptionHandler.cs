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
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Xigadee
{
    /// <summary>
    /// Perform Encryption / Decryption using AES with IV prepended in encrypted output and used in decryption
    /// </summary>
    public class AesEncryptionHandler : IEncryptionHandler
    {
        private readonly byte[] mKey;

        private readonly bool mUseCompression;

        private readonly int mBlockSize;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keySize"></param>
        /// <param name="useCompression"></param>
        /// <param name="blockSize"></param>
        public AesEncryptionHandler(string key, int keySize,  bool useCompression = true, int blockSize = 128)
            :this(Convert.FromBase64String(key), keySize, useCompression, blockSize)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keySize"></param>
        /// <param name="useCompression"></param>
        /// <param name="blockSize"></param>
        public AesEncryptionHandler(byte[] key, int keySize, bool useCompression = true, int blockSize = 128)
        {
            mKey = key;
            mUseCompression = useCompression;
            mBlockSize = blockSize;

            if (key.Length * 8 != keySize)
                throw new ArgumentException($"Key size of {keySize} does not match supplied key which has a size of {(key.Length * 8)}");           
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] input)
        {
            using (AesManaged aesManaged = new AesManaged {Key = mKey, BlockSize = mBlockSize, Mode = CipherMode.CBC })
            using (MemoryStream ms = new MemoryStream())
            using (ICryptoTransform ct = aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV))
            using (CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
            using (var inputStream = new MemoryStream(input, false))
            {
                // Prepend the IV to the memory stream
                ms.Write(aesManaged.IV, 0, aesManaged.IV.Length);

                if (mUseCompression)
                {
                    using (var outputZipStream = new MemoryStream())
                    {
                        using (var zipStream = new GZipStream(outputZipStream, CompressionMode.Compress, true))
                        {
                            inputStream.CopyTo(zipStream);
                        }

                        outputZipStream.Position = 0;
                        outputZipStream.CopyTo(cs);
                    }
                }
                else
                {
                    inputStream.CopyTo(cs);
                }

                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] input)
        {
            // Grab the Initialization Vector out of the input
            var ivBytes = new byte[(mBlockSize / 8)];
            Array.Copy(input, ivBytes, ivBytes.Length);

            // Take the remaining bytes which represent the actual encrypted payload
            var encryptedBytes = new byte[input.LongLength - ivBytes.Length];
            Array.Copy(input, ivBytes.Length, encryptedBytes, 0, encryptedBytes.LongLength);

            using (AesManaged aesManaged = new AesManaged { Key = mKey, BlockSize = mBlockSize, IV = ivBytes, Mode = CipherMode.CBC })
            using (Stream ms = new MemoryStream(encryptedBytes))
            using (ICryptoTransform ct = aesManaged.CreateDecryptor(aesManaged.Key, aesManaged.IV))
            using (CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Read))
            using (MemoryStream outputStream = new MemoryStream())
            {
                cs.CopyTo(outputStream);

                if (!mUseCompression)
                    return outputStream.ToArray();

                outputStream.Position = 0;
                using (var zipStream = new GZipStream(outputStream, CompressionMode.Decompress))
                using (var outputZipStream = new MemoryStream())
                {
                    zipStream.CopyTo(outputZipStream);
                    zipStream.Close();
                    return outputZipStream.ToArray();
                }
            }
        }
    }
}
