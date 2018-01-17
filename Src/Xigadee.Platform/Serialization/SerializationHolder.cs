using System;

namespace Xigadee
{
    /// <summary>
    /// This class holds the metadata for the service message blob.
    /// </summary>
    public class SerializationHolder
    {
        /// <summary>
        /// A static constructor that sets the internal object.
        /// /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the new holder.</returns>
        public static SerializationHolder CreateWithObject(object entity)
        {
            return new SerializationHolder().SetObject(entity);
        }
        /// <summary>
        /// Gets or sets the metadata context. The context holds any additional metadata from the incoming connection.
        /// </summary>
        public object Metadata { get; set; }

        #region SetBlob(byte[] blob, string contentType = null, int? maxLength = null)
        /// <summary>
        /// Sets the Blob value for the byte array.
        /// </summary>
        /// <param name="blob">The binary blob parameter.</param>
        /// <param name="contentType">This is the optional content type parameter.</param>
        /// <param name="contentEncoding">This is the optional content encoding parameter.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>Returns this container object to allow for Fluent behaviour.</returns>
        /// <exception cref="Xigadee.SerializationBlobLimitExceeededException">Throw if the byte array length exceeds the maximum permitted value.</exception>
        public SerializationHolder SetBlob(byte[] blob, string contentType = null, string contentEncoding = null, int? maxLength = null)
        {
            if (blob != null && maxLength.HasValue && blob.Length > maxLength.Value)
                throw new SerializationBlobLimitExceeededException(maxLength.Value, blob.Length);

            Blob = blob;

            if (contentType != null)
                ContentType = contentType;

            ContentEncoding = contentEncoding;

            return this;
        } 
        #endregion

        /// <summary>
        /// Gets or sets the binary blob.
        /// </summary>
        public byte[] Blob { get; private set; }

        #region ContentType/HasContentType
        /// <summary>
        /// Gets or sets the BLOB serializer content type identifier. 
        /// If this is set, the specific serializer will be used without attempting to identify the magic bytes at the start of the blob stream.
        /// The object type will also be appended to the Context-Type as a parameter.
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance has a content type set.
        /// </summary>
        public bool HasContentType => !string.IsNullOrEmpty(ContentType); 
        #endregion
        #region ContentEncoding/HasContentEncoding
        /// <summary>
        /// Identifies the blob encoding type, typically 'gzip'.
        /// </summary>
        public string ContentEncoding { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance has a content encoding set.
        /// </summary>
        public bool HasContentEncoding => !string.IsNullOrEmpty(ContentEncoding); 
        #endregion

        /// <summary>
        /// Gets a value indicating whether this instance has content.
        /// </summary>
        public bool HasObject => Object != null;
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public object Object { get; private set; }
        /// <summary>
        /// The object type, which is parsed from the ContentType parameter.
        /// </summary>
        public Type ObjectType { get; set; }

        #region SetObject(object incoming)
        /// <summary>
        /// Sets the object and the object type for the holder.
        /// </summary>
        /// <param name="incoming">The incoming object.</param>
        /// <param name="clearBlob">Optional parameter that clears the incoming binary payload. The default behaviour is false.</param>
        /// <returns>Returns this container object to allow for Fluent behaviour.</returns>
        public SerializationHolder SetObject(object incoming, bool clearBlob = false)
        {
            Object = incoming;
            ObjectType = incoming?.GetType();

            if (clearBlob)
                Blob = null;

            return this;
        }
        #endregion

        #region Implicit binary operators ...
        /// <summary>
        /// Performs an implicit conversion from <see cref="SerializationHolder"/> to a byte array./>.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator byte[] (SerializationHolder holder)
        {
            return holder?.Blob;
        }
        /// <summary>
        /// Performs an implicit conversion from a byte array to <see cref="SerializationHolder"/> and sets the content type to application/octet-stream
        /// </summary>
        /// <param name="blob">The BLOB to convert to.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SerializationHolder(byte[] blob)
        {
            return new SerializationHolder().SetBlob(blob, "application/octet-stream");
        } 
        #endregion
    }

    /// <summary>
    /// This exception is thrown when the byte array is larger than the amount permitted. 
    /// This is used for messaging systems that have a specific limit.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class SerializationBlobLimitExceeededException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationBlobLimitExceeededException"/> class.
        /// </summary>
        /// <param name="maxLength">The maximum byte array length.</param>
        /// <param name="length">The actual byte array length.</param>
        public SerializationBlobLimitExceeededException(int maxLength, int length)
            :base($"The byte array of length {length} has exceeded the permitted length of {maxLength}")
        {
        }
    }
}
