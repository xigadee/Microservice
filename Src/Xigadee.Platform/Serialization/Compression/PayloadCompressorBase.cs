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
        protected abstract Stream GetCompressionStream(Stream inner);
        /// <summary>
        /// Gets the decompression stream.
        /// </summary>
        /// <param name="inner">The inner byte stream.</param>
        /// <returns>Returns the decompression stream.</returns>
        protected abstract Stream GetDecompressionStream(Stream inner);

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

        #region TryCompression(SerializationHolder holder)
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
            return HolderChecks(holder) && Compress(holder, GetCompressionStream, ContentEncoding);
        }
        #endregion
        #region TryDecompression(SerializationHolder holder)
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
            return HolderChecks(holder) && Decompress(holder, GetDecompressionStream);
        }
        #endregion

        #region HolderChecks(SerializationHolder holder)
        /// <summary>
        /// Checks the incoming holder to ensure that it is correctly configured.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the checks are passed.</returns>
        /// <exception cref="ArgumentNullException">holder</exception>
        protected virtual bool HolderChecks(SerializationHolder holder)
        {
            if (holder == null)
                throw new ArgumentNullException("holder", "The serialization holder cannot be null.");

            return holder.Blob != null;
        } 
        #endregion

        /// <summary>
        /// Encodes the blobs from uncompressed to compressed.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <param name="getStream">The get compressor stream function.</param>
        /// <param name="contentEncoding">The content encoding parameter.</param>
        /// <returns>Returns true if encoded without error.</returns>
        /// <exception cref="ArgumentNullException">holder</exception>
        protected virtual bool Compress(SerializationHolder holder, Func<Stream,Stream> getStream, string contentEncoding)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                using (Stream compress = getStream(ms))
                {
                    compress.Write(holder.Blob,0, holder.Blob.Length);
                    compress.Close();

                    holder.SetBlob(ms.ToArray(), holder.ContentType, contentEncoding);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Encodes the blobs from compressed to uncompressed.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <param name="getStream">The get decompressor stream function.</param>
        /// <returns>Returns true if encoded without error.</returns>
        protected virtual bool Decompress(SerializationHolder holder, Func<Stream, Stream> getStream)
        {
            try
            {
                using (MemoryStream msIn = new MemoryStream(holder.Blob))
                using (Stream decompress = getStream(msIn))
                using (MemoryStream msOut = new MemoryStream())
                {
                    decompress.CopyTo(msOut);
                    decompress.Close();
                    holder.SetBlob(msOut.ToArray(), holder.ContentType);
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
