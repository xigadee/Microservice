#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public abstract class SerializerBase
    {
        /// <summary>
        /// Gets the content-type parameter, which can be used to quickly identify the serialization type used.
        /// </summary>
        public abstract string ContentType { get; }

        /// <summary>
        /// Validates that the incoming holder is correctly configured..
        /// </summary>
        /// <param name="holder">The holder.</param>
        protected virtual void CheckIncoming(SerializationHolder holder)
        {
            if (holder == null)
                throw new ArgumentNullException("holder");

            if (!holder.ContentType.Equals(ContentType, StringComparison.InvariantCultureIgnoreCase))
                throw new SerializationHolderContentTypeException(holder, ContentType);
        }
        /// <summary>
        /// Validates that the outgoing holder is correctly configured..
        /// </summary>
        /// <param name="holder">The holder.</param>
        protected virtual void CheckOutgoing(SerializationHolder holder)
        {
            if (holder == null)
                throw new ArgumentNullException("holder");
        }

        public virtual bool TryDeserialize(SerializationHolder holder)
        {
            CheckIncoming(holder);

            return false;
        }

        public virtual bool TrySerialize(SerializationHolder holder)
        {
            CheckOutgoing(holder);

            return false;
        }

        

        #region Deserialize public        
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="blob">The binary blob.</param>
        /// <returns>The deserialized entity.</returns>
        public virtual E Deserialize<E>(byte[] blob)
        {
            return Deserialize<E>(blob, 0, blob.Length);
        }
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="blob">The binary blob.</param>
        /// <param name="start">The array start.</param>
        /// <param name="length">The array length.</param>
        /// <returns>The deserialized entity.</returns>
        public virtual E Deserialize<E>(byte[] blob, int start, int length)
        {
            return (E)Deserialize(blob, start, length);
        }
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <returns>The deserialized entity.</returns>
        public virtual object Deserialize(byte[] blob)
        {
            return Deserialize(blob, 0, blob.Length);
        }
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <param name="start">The array start.</param>
        /// <param name="length">The array length.</param>
        /// <returns>The deserialized entity.</returns>
        public abstract object Deserialize(byte[] blob, int start, int length);
        #endregion
        #region Serialize public        
        /// <summary>
        /// Serializes the specified entity.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The binary blob.</returns>
        public virtual byte[] Serialize<E>(E entity)
        {
            return Serialize((object)entity);
        }
        /// <summary>
        /// Serializes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The binary blob.</returns>
        public abstract byte[] Serialize(object entity);

        #endregion

    }
}
