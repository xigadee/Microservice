using System.IO.Compression;
using System.IO;

namespace Xigadee
{
    /// <summary>
    /// This is the GZIP payload compressor.
    /// </summary>
    /// <seealso cref="Xigadee.CompressorBase" />
    public class CompressorGzip: CompressorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompressorGzip"/> class.
        /// </summary>
        public CompressorGzip() : base("gzip") { }

        /// <summary>
        /// Gets the compression stream.
        /// </summary>
        /// <param name="inner">The inner byte stream.</param>
        /// <returns>Returns the compression stream</returns>
        protected override Stream GetCompressionStream(Stream inner)
        {
            return new GZipStream(inner, CompressionMode.Compress, false);
        }
        /// <summary>
        /// Gets the decompression stream.
        /// </summary>
        /// <param name="inner">The inner byte stream.</param>
        /// <returns>Returns the decompression stream.</returns>
        protected override Stream GetDecompressionStream(Stream inner)
        {
            return new GZipStream(inner, CompressionMode.Decompress, false);
        }
    }
}
