#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class allows for serialization root functionality to be shared.
    /// </summary>
    [DebuggerDisplay("Type={ContentType}")]
    public abstract class SerializerBase: IPayloadSerializer
    {
        /// <summary>
        /// Gets the content-type parameter, which can be used to quickly identify the serialization type used.
        /// </summary>
        public virtual string ContentType { get; set; }

        /// <summary>
        /// Returns true if the holder can be deserialized.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if it can be deserialized.
        /// </returns>
        /// <exception cref="ArgumentNullException">holder</exception>
        public virtual bool SupportsDeserialization(SerializationHolder holder)
        {
            if (holder == null)
                throw new ArgumentNullException("holder");

            return (holder.ContentType ?? "").Equals(ContentType, StringComparison.InvariantCultureIgnoreCase);
        }
        /// <summary>
        /// Returns true if the Content in the holder can be serialized.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if it can be serialized.
        /// </returns>
        /// <exception cref="ArgumentNullException">holder</exception>
        public virtual bool SupportsSerialization(SerializationHolder holder)
        {
            if (holder == null)
                throw new ArgumentNullException("holder");

            return holder.HasObject;
        }

        /// <summary>
        /// Tries to deserialize the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the incoming binary payload is successfully deserialized to a content object.
        /// </returns>
        public virtual bool TryDeserialize(SerializationHolder holder)
        {
            if (!SupportsDeserialization(holder))
                return false;

            try
            {
                Deserialize(holder);
                return true;
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        /// <summary>
        /// Deserializes the specified byte array in the holder and sets the object with the entity.
        /// </summary>
        /// <param name="holder">The holder.</param>
        public abstract void Deserialize(SerializationHolder holder);
        /// <summary>
        /// Serializes the specified object in the holder and sets the byte array.
        /// </summary>
        /// <param name="holder">The holder.</param>
        public abstract void Serialize(SerializationHolder holder);

        /// <summary>
        /// Tries to serialize the outgoing payload.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the Content is serialized correctly to a binary blob.
        /// </returns>
        public virtual bool TrySerialize(SerializationHolder holder)
        {
            if (!SupportsSerialization(holder))
                return false;

            try
            {
                Serialize(holder);
                return true;
            }
            catch (Exception) { }

            return false;
        }       

        /// <summary>
        /// Returns true if the serializer supports this entity type for serialization.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns true if supported.</returns>
        public abstract bool SupportsContentTypeSerialization(Type entityType);
        /// <summary>
        /// Returns true if the serializer supports this content type for serialization.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if supported.
        /// </returns>
        public virtual bool SupportsContentTypeSerialization(SerializationHolder holder)
        {
            if (!holder.HasObject)
                return false;

            Type cType = holder.ObjectType ?? holder.Object?.GetType();

            if (cType == null)
                return false;

            return SupportsContentTypeSerialization(cType);
        }
    }
}
