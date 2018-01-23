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
        /// Initializes a new instance of the <see cref="AesEncryptionHandler"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="key">The base64 encoded AES key.</param>
        /// <param name="useCompression">if set to <c>true</c> [use compression].</param>
        /// <param name="keySize">Size of the key.</param>
        /// <param name="blockSize">Size of the block.</param>
        public AesEncryptionHandler(string id, string key, bool useCompression = true, int? keySize = null, int blockSize = 128)
            :this(id, Convert.FromBase64String(key), useCompression, keySize, blockSize)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AesEncryptionHandler"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="key">The binary key.</param>
        /// <param name="useCompression">if set to <c>true</c> [use compression].</param>
        /// <param name="keySize">Size of the key.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <exception cref="ArgumentNullException">id - id cannot be null</exception>
        public AesEncryptionHandler(string id, byte[] key, bool useCompression = true, int? keySize = null, int blockSize = 128)
        {
            Id = id ?? throw new ArgumentNullException("id", "id cannot be null");

            mKey = key;
            mUseCompression = useCompression;
            mBlockSize = blockSize;

            var calculatedKeySize = key.Length*8;
            if (!keySize.HasValue && calculatedKeySize != 128 && calculatedKeySize != 192 && calculatedKeySize != 256)
                throw new ArgumentException($"Calculated key size of {calculatedKeySize} does not match a valid key size of 128, 192 or 256", nameof(key));

            if (keySize.HasValue && calculatedKeySize != keySize)
                throw new ArgumentException($"Key size of {keySize} does not match supplied key which has a size of {calculatedKeySize}", nameof(key));
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

        /// <summary>
        /// This is the friendly name for the handler.
        /// </summary>
        public string Name => $"{nameof(AesEncryptionHandler)}/{Id}";

        /// <summary>
        /// Gets the encryption identifier used to match a handler with a specific id.
        /// </summary>
        public string Id { get; }
    }
}
