#region using
using System.Collections.Specialized;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
#endregion
namespace Xigadee.Api
{
    /// <summary>
    /// This attribute can be used to decorate a class for transport to specify 
    /// the specific conversion class for the media type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public class MediaTypeConverterAttribute:Attribute
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="mediaType">The media type string.</param>
        /// <param name="converterType">The type for </param>
        public MediaTypeConverterAttribute(Type converterType, Type entityType=null, string mediaType=null, double priority = 0.1)
        {
            if (converterType == null)
                throw new ArgumentNullException("converterType");

            if (!converterType.IsSubclassOf(typeof(TransportSerializer)))
                throw new ArgumentOutOfRangeException("converterType", "Type should be a subclass of TransportSerializerBase");

            if (priority > 1)
                throw new ArgumentOutOfRangeException("priority", "priority can not be greater than 1");

            ConverterType = converterType;
            TransportSerializer serializer = null;

            bool instantiate = entityType == null || mediaType == null;

            if (instantiate)
                serializer = (TransportSerializer)Activator.CreateInstance(converterType);

            EntityType = entityType ?? serializer.EntityType;
            MediaType = mediaType ?? serializer.MediaType;
            Priority = priority;
        } 
        #endregion

        /// <summary>
        /// This is the entity type that the converter supports.
        /// </summary>
        public Type EntityType { get; private set; }
        /// <summary>
        /// This is the priority for the type converter. It is primarily used by the Api client
        /// to decide on the default type converter to use.
        /// </summary>
        public double Priority { get; private set; }
        /// <summary>
        /// The media type.
        /// </summary>
        public string MediaType { get; private set; }
        /// <summary>
        /// This is the converter type.
        /// </summary>
        public Type ConverterType { get; private set; }

    }
}
