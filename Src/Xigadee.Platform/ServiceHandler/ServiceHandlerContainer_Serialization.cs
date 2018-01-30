using System;
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

        #region TryDeserialize(SerializationHolder holder)
        /// <summary>
        /// Tries to deserialize the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the incoming binary payload is successfully deserialized.
        /// </returns>
        public bool TryDeserialize(ServiceHandlerContext holder)
        {
            string id;
            if (!ExtractContentType(holder, out id))
                return false;

            IServiceHandlerSerialization sr = null;
            if (!Serialization.TryGet(id, out sr))
                return false;

            return sr.TryDeserialize(holder);
        }
        #endregion
        #region TrySerialize(SerializationHolder holder)
        /// <summary>
        /// Tries to compress the outgoing holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the Content is serialized correctly to a binary blob.
        /// </returns>
        public bool TrySerialize(ServiceHandlerContext holder)
        {
            string id;
            if (!ExtractContentType(holder, out id))
                return false;

            IServiceHandlerSerialization sr = null;
            if (!Serialization.TryGet(id, out sr))
                return false;

            return sr.TrySerialize(holder);
        }
        #endregion

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

            var items = holder.ContentType.Split(';');

            value = items[0].Trim().ToLowerInvariant();
            return true;
        }
        #endregion

    }
}
