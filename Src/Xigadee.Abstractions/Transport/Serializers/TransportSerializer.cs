using System;
using System.Text;
namespace Xigadee
{
    /// <summary>
    /// This is the base transport serializer.
    /// </summary>
    public abstract class TransportSerializer
    {
        /// <summary>
        /// This is the transport media type.
        /// </summary>
        public string MediaType { get; protected set; }
        /// <summary>
        /// This is the priority assigned to the serializer. 
        /// This will set the default serializer when multiple serializers are assigned to a client or server.
        /// </summary>
        public double Priority { get; set; } = 0.1;
        /// <summary>
        /// This is the entity type supported by the transport serializer.
        /// </summary>
        public Type EntityType { get; protected set; }

        #region GetObject<E>(byte[] data, Encoding encoding = null)
        /// <summary>
        /// Gets the deserialized object.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Returns the object of type E.</returns>
        /// <exception cref="TransportDeserializationException"></exception>
        public virtual E GetObject<E>(byte[] data, Encoding encoding = null)
        {
            try
            {
                return (E)GetObjectInternal(typeof(E), data, encoding ?? Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new TransportDeserializationException($"{MediaType} - cannot deserialize entity of type {typeof(E).Name}: {ex.Message}", ex);
            }
        }
        #endregion
        #region GetData<E>(E entity, Encoding encoding = null)
        /// <summary>
        /// Gets the binary serialized data.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Returns the binary data.</returns>
        /// <exception cref="TransportSerializationException"></exception>
        public virtual byte[] GetData<E>(E entity, Encoding encoding = null)
        {
            try
            {
                return GetDataInternal(entity, encoding ?? Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new TransportSerializationException($"{MediaType} - cannot serialize entity of type {typeof(E).Name}: {ex.Message}", ex);
            }
        }
        #endregion

        /// <summary>
        /// Gets the deserialized object.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <param name="data">The data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Returns the deserialized object.</returns>
        protected abstract object GetObjectInternal(Type type, byte[] data, Encoding encoding);

        /// <summary>
        /// Gets the binary data.
        /// </summary>
        /// <typeparam name="E">The entity type</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Returns the byte array.</returns>
        protected abstract byte[] GetDataInternal<E>(E entity, Encoding encoding);


    }
}
