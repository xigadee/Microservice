using System;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This class holds the metadata for the service message blob.
    /// </summary>
    public class ServiceHandlerContext
    {
        #region CreateWithObject(object entity)
        /// <summary>
        /// A static constructor that sets the internal object.
        /// /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the new holder.</returns>
        public static ServiceHandlerContext CreateWithObject(object entity)
        {
            return new ServiceHandlerContext().SetObject(entity);
        }
        #endregion
        #region Metadata
        /// <summary>
        /// Gets or sets the metadata context. The context holds any additional metadata from the incoming connection.
        /// </summary>
        public object Metadata { get; set; } 
        #endregion

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
        public ServiceHandlerContext SetBlob(byte[] blob, string contentType = null, string contentEncoding = null, int? maxLength = null)
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
        #region Blob
        /// <summary>
        /// Gets or sets the binary blob.
        /// </summary>
        public byte[] Blob { get; private set; } 
        #endregion

        #region ContentType/HasContentType
        /// <summary>
        /// Gets or sets the BLOB serializer content type identifier. 
        /// If this is set, the specific serializer will be used without attempting to identify the magic bytes at the start of the blob stream.
        /// The object type will also be appended to the Context-Type as a parameter.
        /// </summary>
        public SerializationHandlerId ContentType { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance has a content type set.
        /// </summary>
        public bool HasContentType => ContentType!=null;
        #endregion
        #region ContentEncoding/HasContentEncoding
        /// <summary>
        /// Identifies the blob encoding type, typically 'gzip'.
        /// </summary>
        public CompressionHandlerId ContentEncoding { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance has a content encoding set.
        /// </summary>
        public bool HasContentEncoding => ContentEncoding != null;
        #endregion
        #region Encryption/HasEncryption
        /// <summary>
        /// Identifies the encryption type.
        /// </summary>
        public EncryptionHandlerId Encryption { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance has encryption set.
        /// </summary>
        public bool HasEncryption => Encryption != null;
        #endregion
        #region Authentication/HasAuthentication
        /// <summary>
        /// Identifies the Authentication type.
        /// </summary>
        public AuthenticationHandlerId Authentication { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance has Authentication set.
        /// </summary>
        public bool HasAuthentication => Authentication != null;
        #endregion

        #region HasObject
        /// <summary>
        /// Gets a value indicating whether this instance has content.
        /// </summary>
        public bool HasObject => Object != null;
        #endregion
        #region Object
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public object Object { get; private set; }
        #endregion
        #region ObjectType
        /// <summary>
        /// The object type, which is parsed from the ContentType parameter.
        /// </summary>
        public Type ObjectType { get; private set; } 
        #endregion
        #region SetObject(object incoming)
        /// <summary>
        /// Sets the object and the object type for the holder.
        /// </summary>
        /// <param name="incoming">The incoming object.</param>
        /// <param name="clearBlob">Optional parameter that clears the incoming binary payload. The default behaviour is false.</param>
        /// <returns>Returns this container object to allow for Fluent behaviour.</returns>
        public ServiceHandlerContext SetObject(object incoming, bool clearBlob = false)
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
        /// Performs an implicit conversion from <see cref="ServiceHandlerContext"/> to a byte array./>.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator byte[] (ServiceHandlerContext holder)
        {
            return holder?.Blob;
        }
        /// <summary>
        /// Performs an implicit conversion from a byte array to <see cref="ServiceHandlerContext"/> and sets the content type to application/octet-stream
        /// </summary>
        /// <param name="blob">The BLOB to convert to.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ServiceHandlerContext(byte[] blob)
        {
            return new ServiceHandlerContext().SetBlob(blob, "application/octet-stream");
        } 
        #endregion
    }
}
