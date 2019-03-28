#region using
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This the base default Json serializer.
    /// </summary>
    public class JsonTransportSerializer : TransportSerializer
    {
        private readonly JsonSerializerSettings mJsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTransportSerializer"/> class.
        /// </summary>
        public JsonTransportSerializer() : base()
        {
            MediaType = "application/json";
        }

        /// <summary>
        /// Gets the binary JSON data.
        /// </summary>
        /// <typeparam name="E">The entity type</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        /// Returns the byte array.
        /// </returns>
        protected override byte[] GetDataInternal<E>(E entity, Encoding encoding)
        {
            return encoding.GetBytes(JsonConvert.SerializeObject(entity, mJsonSerializerSettings));
        }
        /// <summary>
        /// Gets the deserialized object.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <param name="data">The binary JSON data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        /// Returns the deserialized object.
        /// </returns>
        protected override object GetObjectInternal(Type type, byte[] data, Encoding encoding)
        {
            return JsonConvert.DeserializeObject(encoding.GetString(data), type, mJsonSerializerSettings);
        }
    }
}
