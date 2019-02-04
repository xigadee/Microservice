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


        #region GetSerializers<E>...
        /// <summary>
        /// This method extract the attributes and returns the transport serializers.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <returns>Returns a dictionary of transport serializers.</returns>
        public static List<TransportSerializer<E>> GetSerializers<E>()
        {
            return GetSerializers<E>(typeof(E));
        }

        /// <summary>
        /// This method extract the attributes and returns the transport serializers.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <returns>Returns a dictionary of transport serializers.</returns>
        public static List<TransportSerializer<E>> GetSerializers<E>(Type baseType)
        {
            Type entityType = typeof(E);

            var attrs = baseType.GetCustomAttributes(false)
                .OfType<MediaTypeConverterAttribute>()
                .Where((t) => t.EntityType == entityType);

            return attrs.Select(att =>
                {
                    var tSerial = (TransportSerializer<E>)Activator.CreateInstance(att.ConverterType);
                    tSerial.Priority = att.Priority;
                    return tSerial;
                }).ToList();
        } 
        #endregion
    }

    public abstract class TransportSerializer<E> : TransportSerializer, ITransportSerializer<E>
    {
        public TransportSerializer()
        {
            EntityType = typeof(E);
        }

        public abstract E GetObjectInternal(byte[] data, Encoding encoding = null);

        public virtual E GetObject(byte[] data, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            try
            {
                return GetObjectInternal(data, encoding);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public abstract byte[] GetDataInternal(E entity, Encoding encoding = null);

        public virtual byte[] GetData(E entity, Encoding encoding = null)
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
