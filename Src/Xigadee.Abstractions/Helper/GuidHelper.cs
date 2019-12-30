using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace Xigadee
{
    /// <summary>
    /// This helper class is used to create deterministic Guids safely.
    /// </summary>
    public static class GuidHelper
    {
        #region Create(Guid namespaceId, string name, int version)
        /// <summary>
        /// Creates a name-based UUID using the algorithm from RFC 4122 §4.3.
        /// </summary>
        /// <param name="namespaceId">The ID of the namespace.</param>
        /// <param name="name">The name (within that namespace).</param>
        /// <param name="version">The version number of the UUID to create; this value must be either
        /// 3 (for MD5 hashing) or 5 (for SHA-1 hashing) or 6 (for SHA-256 hashing).</param>
        /// <returns>A UUID derived from the namespace and name.</returns>
        /// <remarks>See <a href="http://code.logos.com/blog/2011/04/generating_a_deterministic_guid.html">Generating a deterministic GUID</a>.</remarks>
        public static Guid Create(Guid namespaceId, string name, int version = 6)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            var hashAlgorithmType = GetHashType(version);

            // convert the name to a sequence of octets (as defined by the standard or conventions of its namespace) (step 3)
            // ASSUME: UTF-8 encoding is always appropriate
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);

            // convert the namespace UUID to network order (step 3)
            byte[] namespaceBytes = namespaceId.ToByteArray();
            SwapByteOrder(namespaceBytes);

            // computes the hash of the name space ID concatenated with the name (step 4)
            byte[] hash;
            using (var incrementalHash = IncrementalHash.CreateHash(hashAlgorithmType))
            {
                incrementalHash.AppendData(namespaceBytes);
                incrementalHash.AppendData(nameBytes);
                hash = incrementalHash.GetHashAndReset();
            }

            // most bytes from the hash are copied straight to the bytes of the new GUID (steps 5-7, 9, 11-12)
            byte[] newGuid = new byte[16];
            Array.Copy(hash, 0, newGuid, 0, 16);

            // set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
            newGuid[6] = (byte)((newGuid[6] & 0x0F) | (version << 4));

            // set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively (step 10)
            newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

            // convert the resulting UUID to local byte order (step 13)
            SwapByteOrder(newGuid);
            return new Guid(newGuid);
        }
        #endregion

        static HashAlgorithmName GetHashType(int version)
        {
            switch (version)
            {
                case 3:
                    return HashAlgorithmName.MD5;
                case 5:
                    return HashAlgorithmName.SHA1;
                case 6:
                    return HashAlgorithmName.SHA256;
                default:
                    throw new ArgumentOutOfRangeException("version", "version must be either 3 (md5) or 5 (sha1), or 6 (sha256).");
            }
        }

        // Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
        static void SwapByteOrder(byte[] guid)
        {
            SwapBytes(guid, 0, 3);
            SwapBytes(guid, 1, 2);
            SwapBytes(guid, 4, 5);
            SwapBytes(guid, 6, 7);
        }

        static void SwapBytes(byte[] guid, int left, int right)
        {
            byte temp = guid[left];
            guid[left] = guid[right];
            guid[right] = temp;
        }
    }
}
