using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by compression algorithms, such as GZip and Deflate.
    /// </summary>
    public interface IPayloadCompressor
    {
        /// <summary>
        /// Gets the content-encoding parameter, which can be used to quickly identify the compression used (if any), i.e. GZIP.
        /// </summary>
        string ContentEncoding { get; set; }

        /// <summary>
        /// A boolean function that returns true if the compression type is supported.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <returns>Returns true when supported.</returns>
        bool SupportsContentEncoding(SerializationHolder holder);

        /// <summary>
        /// Tries to decompress the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the incoming binary payload is successfully decompressed.</returns>
        bool TryDecompression(SerializationHolder holder);
        /// <summary>
        /// Tries to compress the outgoing payload.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the Content is compressed correctly to a binary blob.</returns>
        bool TryCompression(SerializationHolder holder);
    }
}
