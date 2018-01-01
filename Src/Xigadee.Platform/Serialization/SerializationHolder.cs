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
            var holder = new SerializationHolder();
            holder.SetObject(entity);
            return holder;
        }
        /// <summary>
        /// Gets or sets the metadata context. The context holds any additional metadata from the incoming connection.
        /// </summary>
        public object Metadata { get; set; }
        /// <summary>
        /// Gets or sets the BLOB.
        /// </summary>
        public byte[] Blob { get; set; }
        /// <summary>
        /// Gets or sets the BLOB serializer content type identifier. 
        /// If this is set, the specific serializer will be used without attempting to identify the magic bytes at the start of the blob stream.
        /// The object type will also be appended to the Context-Type as a parameter.
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// Identifies the blob encoding type, typically 'gzip'.
        /// </summary>
        public string ContentEncoding { get; set; }

        /// <summary>
        /// The object type, which is parsed from the ContentType parameter.
        /// </summary>
        public Type ObjectType { get; set; }
        /// <summary>
        /// This optional identifier is added by the serialization container and specifies the id of the deserialized object stored in the object registry.
        /// </summary>
        public Guid? ObjectRegistryId { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance has content.
        /// </summary>
        public bool HasObject => Object != null;
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public object Object { get; set; }

        /// <summary>
        /// Sets the object and the object type for the holder.
        /// </summary>
        /// <param name="incoming">The incoming object.</param>
        public void SetObject(object incoming)
        {
            Object = incoming;
            ObjectType = incoming?.GetType();
        }
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
            return new SerializationHolder() { Blob = blob, ContentType = "application/octet-stream" };
        }
    }
}
