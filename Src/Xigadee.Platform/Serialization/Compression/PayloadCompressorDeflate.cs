using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace Xigadee
{
    /// <summary>
    /// This is the DEFLATE payload compressor.
    /// </summary>
    /// <seealso cref="Xigadee.PayloadCompressorBase" />
    public class PayloadCompressorDeflate: PayloadCompressorBase
    {
        /// <summary>
        /// Gets the content encoding, deflate for this compressor.
        /// </summary>
        public override string ContentEncoding { get; } = "deflate";

        /// <summary>
        /// Gets the compression stream.
        /// </summary>
        /// <param name="inner">The inner byte stream.</param>
        /// <returns>Returns the compression stream</returns>
        protected override Stream GetCompressionStream(Stream inner)
        {
            return new DeflateStream(inner, CompressionMode.Compress, true);
        }
        /// <summary>
        /// Gets the decompression stream.
        /// </summary>
        /// <param name="inner">The inner byte stream.</param>
        /// <returns>Returns the decompression stream.</returns>
        protected override Stream GetDecompressionStream(Stream inner)
        {
            return new DeflateStream(inner, CompressionMode.Decompress, true);
        }
    }
}
