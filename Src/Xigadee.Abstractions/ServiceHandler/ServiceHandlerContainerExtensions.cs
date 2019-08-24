using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the extension methods for the service handler collection.
    /// </summary>
    public static partial class ServiceHandlerContainerExtensions
    {
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

            if (holder.ContentType == null)
                return false;

            value = holder.ContentType.Id;

            return true;
        }

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
            if (!ExtractContentType(holder, out id))
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
            if (!ExtractContentType(holder, out id))
                return false;

            IServiceHandlerSerialization sr = null;
            if (!collection.TryGet(id, out sr))
                return false;

            return sr.TrySerialize(holder);
        }

        /// <summary>
        /// Tries to compress the outgoing holder.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="serializationId">The serialization id.</param>
        /// <returns>Returns true if the Content is serialized correctly to a binary blob.</returns>
        public static byte[] SerializeToBlob(this ServiceHandlerCollection<IServiceHandlerSerialization> collection, object item, string serializationId)
        {
            var context = ServiceHandlerContext.CreateWithObject(item);
            context.ContentType = serializationId;
            if (collection.TrySerialize(context))
                return context.Blob;

            throw new PayloadSerializationException();
        }

        /// <summary>
        /// Tries to compress the outgoing holder.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="blob">The binary array.</param>
        /// <returns>Returns true if the Content is serialized correctly to a binary blob.</returns>
        public static O DeserializeToObject<O>(this ServiceHandlerCollection<IServiceHandlerSerialization> collection, byte[] blob, string serializationId)
        {
            ServiceHandlerContext context = blob;
            context.ContentType = serializationId;
            context.ContentType.ObjectType = typeof(O).FullName;
            if (collection.TryDeserialize(context))
                return (O)context.Object;

            throw new PayloadDeserializationException();
        }

        /// <summary>
        /// This methood clones the serive handler context.
        /// </summary>
        /// <param name="incoming">The incoming context.</param>
        /// <returns>The cloned context.</returns>
        public static ServiceHandlerContext Clone(this ServiceHandlerContext incoming)
        {
            if (incoming == null)
                return null;

            var toReturn = new ServiceHandlerContext();

            if (incoming.HasObject)
                toReturn.SetObject(JsonHelper.Clone(incoming.Object));
            else if (incoming.Blob != null)
                toReturn.SetBlob(incoming.Blob);

            if (incoming.Metadata != null)
                toReturn.Metadata = JsonHelper.Clone(incoming.Metadata);

            toReturn.ContentEncoding = JsonHelper.Clone(incoming.ContentEncoding);
            toReturn.Authentication = JsonHelper.Clone(incoming.Authentication);
            toReturn.ContentType = JsonHelper.Clone(incoming.ContentType);
            toReturn.Encryption = JsonHelper.Clone(incoming.Encryption);

            return toReturn;
        }
    }
}
