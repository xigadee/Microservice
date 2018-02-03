using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
namespace Xigadee
{
    public partial class ServiceHandlerContainer
    {
        /// <summary>
        /// Gets the serialization collection.
        /// </summary>
        public ServiceHandlerCollection<IServiceHandlerSerialization> Serialization { get; }

        #region DefaultContentType
        /// <summary>
        /// Gets or sets the default type of the content type. This is based on the first serializer added to the collection.
        /// </summary>
        public string DefaultContentType { get; protected set; }
        #endregion

        private void OnSerializationAdd(IServiceHandlerSerialization handler)
        {
            DefaultContentType = DefaultContentType ?? handler.Id;
        }

        #region ExtractContentType(string contentType, out string value)        
        /// <summary>
        /// Extracts the type of the content in the format type/subtype.
        /// </summary>
        /// <param name="holder">The context.</param>
        /// <param name="value">The value.</param>
        /// <remarks>See: https://www.w3.org/Protocols/rfc1341/4_Content-Type.html </remarks>
        /// <returns>Returns true if the content type can be extracted from the header field.</returns>
        public static bool ExtractContentType(ServiceHandlerContext holder, out string value)
        {
            value = null;

            if (!holder.HasContentType)
                return false;

            value = holder.ContentType.Id;

            return true;
        }
        #endregion

    }

    /// <summary>
    /// This is the extension methods for the service handler collection.
    /// </summary>
    public static partial class ServiceHandlerContainerExtensions
    {
        /// <summary>
        /// Checks that a specific serializer is supported.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="mimetype">The mime type identifier for the serializer.</param>
        /// <returns>Returns true if the serializer is supported.</returns>
        public static bool SupportsSerializer(this ServiceHandlerCollection<IServiceHandlerSerialization> collection, string mimetype)
        {
            return collection.Contains(mimetype);
        }
        /// <summary>
        /// Checks that a specific serializer is supported.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true when the holder ContentType is supported.</returns>
        public static bool SupportsSerializer(this ServiceHandlerCollection<IServiceHandlerSerialization> collection, ServiceHandlerContext holder)
        {
            return collection.Contains(holder.ContentType);
        }
        /// <summary>
        /// Tries to deserialize the incoming holder.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the incoming binary payload is successfully deserialized.</returns>
        public static bool TryDeserialize(this ServiceHandlerCollection<IServiceHandlerSerialization> collection, ServiceHandlerContext holder)
        {
            string id;
            if (!ServiceHandlerContainer.ExtractContentType(holder, out id))
                return false;

            IServiceHandlerSerialization sr = null;
            if (!collection.TryGet(id, out sr))
                return false;

            return sr.TryDeserialize(holder);
        }
        /// <summary>
        /// Tries to compress the outgoing holder.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the Content is serialized correctly to a binary blob.</returns>
        public static bool TrySerialize(this ServiceHandlerCollection<IServiceHandlerSerialization> collection, ServiceHandlerContext holder)
        {
            string id;
            if (!ServiceHandlerContainer.ExtractContentType(holder, out id))
                return false;

            IServiceHandlerSerialization sr = null;
            if (!collection.TryGet(id, out sr))
                return false;

            return sr.TrySerialize(holder);
        }
    }
}
