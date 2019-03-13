using System;
using System.Collections.Generic;
using System.Linq;
namespace Xigadee
{
    public partial class ServiceHandlerContainer
    {
        /// <summary>
        /// Gets the compression collection.
        /// </summary>
        public ServiceHandlerCollection<IServiceHandlerCompression> Compression { get; }

        #region TryDecompress(ServiceHandlerContext holder)
        /// <summary>
        /// Tries to decompress the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the incoming binary payload is successfully decompressed.
        /// </returns>
        public bool TryDecompress(ServiceHandlerContext holder)
        {
            string id;
            if (!ExtractContentEncoding(holder, out id))
                return false;

            IServiceHandlerCompression comp;
            if (!Compression.TryGet(id, out comp))
                return false;

            return comp.TryDecompression(holder);
        }
        #endregion
        #region TryCompress(SerializationHolder holder)
        /// <summary>
        /// Tries to compress the outgoing payload.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the Content is compressed correctly to a binary blob.
        /// </returns>
        public bool TryCompress(ServiceHandlerContext holder)
        {
            string id;
            if (!ExtractContentEncoding(holder, out id))
                return false;

            IServiceHandlerCompression comp;
            if (!Compression.TryGet(id, out comp))
                return false;

            return comp.TryCompression(holder);
        }
        #endregion

        #region ExtractContentEncoding(string contentEncoding, out string value)        
        /// <summary>
        /// Extracts the content encoding in to a matchable format.
        /// </summary>
        /// <param name="holder">The service holder.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns true if it can be extracted.</returns>
        public static bool ExtractContentEncoding(ServiceHandlerContext holder, out string value)
        {
            value = null;

            if (!holder.HasContentEncoding)
                return false;

            value = holder.ContentEncoding.Id;
            return true;
        }
        #endregion

    }


}
