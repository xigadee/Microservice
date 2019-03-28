#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base transport serializer.
    /// </summary>
    public abstract class TransportSerializer : ITransportSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransportSerializer"/> class.
        /// </summary>
        public TransportSerializer()
        {
            Priority = 0.1;
        }

        /// <summary>
        /// This is the transport media type.
        /// </summary>
        public string MediaType { get; protected set; }
        /// <summary>
        /// This is the priority assigned to the serializer. 
        /// This will set the default serializer when multiple serializers are assigned to a client or server.
        /// </summary>
        public double Priority { get; set; }
        /// <summary>
        /// This is the entity type supported by the transport serializer.
        /// </summary>
        public Type EntityType { get; protected set; }

        protected abstract object GetObjectInternal(Type type, byte[] data, Encoding encoding = null);

        public virtual E GetObject<E>(byte[] data, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            try
            {
                return (E)GetObjectInternal(typeof(E), data, encoding);
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected abstract byte[] GetDataInternal<E>(E entity, Encoding encoding = null);

        public virtual byte[] GetData<E>(E entity, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            try
            {
                return GetDataInternal(entity, encoding);
            }
            catch (Exception ex)
            {
                throw new TransportDeserializationException(
                    string.Format("Cannot deserialize entity of type {0}: {1}", typeof(E).Name, ex.Message), ex);
            }
        }
    }
}
