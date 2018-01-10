using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace Xigadee
{
    /// <summary>
    /// This is the GZIP payload compressor.
    /// </summary>
    /// <seealso cref="Xigadee.PayloadCompressorBase" />
    public class PayloadCompressorGzip: PayloadCompressorBase
    {
        /// <summary>
        /// Gets the content encoding, gzip for this compressor.
        /// </summary>
        public override string ContentEncoding { get; } = "gzip";

        /// <summary>
        /// Gets the compression stream.
        /// </summary>
        /// <param name="inner">The inner byte stream.</param>
        /// <returns>Returns the compression stream</returns>
        public override Stream GetCompressionStream(Stream inner)
        {
            return new GZipStream(inner, CompressionMode.Compress, false);
        }
        /// <summary>
        /// Gets the decompression stream.
        /// </summary>
        /// <param name="inner">The inner byte stream.</param>
        /// <returns>Returns the decompression stream.</returns>
        public override Stream GetDecompressionStream(Stream inner)
        {
            return new GZipStream(inner, CompressionMode.Decompress, false);
        }
    }
}
