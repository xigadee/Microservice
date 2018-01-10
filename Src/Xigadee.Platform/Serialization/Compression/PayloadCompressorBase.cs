using System;
using System.IO;

namespace Xigadee
{
    /// <summary>
    /// This abstract class is used to provide compression support for payloads.
    /// </summary>
    /// <seealso cref="Xigadee.IPayloadCompressor" />
    public abstract class PayloadCompressorBase: IPayloadCompressor
    {
        /// <summary>
        /// Gets the content encoding.
        /// </summary>
        public abstract string ContentEncoding { get; }
        /// <summary>
        /// Gets the compression stream.
        /// </summary>
        /// <param name="inner">The inner byte stream.</param>
        /// <returns>Returns the compression stream</returns>
        public abstract Stream GetCompressionStream(Stream inner);
        /// <summary>
        /// Gets the decompression stream.
        /// </summary>
        /// <param name="inner">The inner byte stream.</param>
        /// <returns>Returns the decompression stream.</returns>
        public abstract Stream GetDecompressionStream(Stream inner);

        #region SupportsContentEncoding(SerializationHolder holder)
        /// <summary>
        /// A boolean function that returns true if the compression type is supported.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <returns>
        /// Returns true when supported.
        /// </returns>
        /// <exception cref="ArgumentNullException">holder</exception>
        public virtual bool SupportsContentEncoding(SerializationHolder holder)
        {
            if (holder == null)
                throw new ArgumentNullException("holder");

            return holder.ContentEncoding != null
                && string.Equals(holder.ContentEncoding, ContentEncoding, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        /// <summary>
        /// Tries to compress the outgoing payload.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the Content is compressed correctly to a binary blob.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual bool TryCompression(SerializationHolder holder)
        {
            return SwitchBlobs(holder, GetCompressionStream, ContentEncoding);
        }

        /// <summary>
        /// Tries to decompress the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the incoming binary payload is successfully decompressed.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual bool TryDecompression(SerializationHolder holder)
        {
            return SwitchBlobs(holder, GetDecompressionStream, null);
        }

        /// <summary>
        /// Switches the blobs from compressed to uncompressed or visa versa.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <param name="getCompression">The get compressor stream function.</param>
        /// <param name="contentEncoding">The content encoding parameter.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">holder</exception>
        protected virtual bool SwitchBlobs(SerializationHolder holder, Func<Stream,Stream> getCompression, string contentEncoding)
        {
            if (holder == null)
                throw new ArgumentNullException("holder");

            if (holder.Blob == null)
                return false;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                using (Stream compress = getCompression(ms))
                {
                    compress.Write(holder.Blob, 0, holder.Blob.Length);
                    compress.Flush();

                    ms.Flush();

                    holder.SetBlob(ms.ToArray(), holder.ContentType, contentEncoding);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
