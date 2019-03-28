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

        public JsonTransportSerializer() : base()
        {
            MediaType = "application/json";
        }

        protected override byte[] GetDataInternal<E>(E entity, Encoding encoding = null)
        {
            return encoding.GetBytes(JsonConvert.SerializeObject(entity, mJsonSerializerSettings));
        }

        protected override object GetObjectInternal(Type type, byte[] data, Encoding encoding = null)
        {
            return JsonConvert.DeserializeObject(encoding.GetString(data), type, mJsonSerializerSettings);
        }
    }
}
