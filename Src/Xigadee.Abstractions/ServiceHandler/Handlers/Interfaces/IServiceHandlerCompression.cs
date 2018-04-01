using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by compression algorithms, such as GZip and Deflate.
    /// </summary>
    public interface IServiceHandlerCompression:IServiceHandler
    {
        /// <summary>
        /// A boolean function that returns true if the compression type is supported.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <returns>Returns true when supported.</returns>
        bool SupportsContentEncoding(ServiceHandlerContext holder);

        /// <summary>
        /// Tries to decompress the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the incoming binary payload is successfully decompressed.</returns>
        bool TryDecompression(ServiceHandlerContext holder);
        /// <summary>
        /// Tries to compress the outgoing payload.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the Content is compressed correctly to a binary blob.</returns>
        bool TryCompression(ServiceHandlerContext holder);
    }
}
