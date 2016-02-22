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
    /// This the the base Json serializer.
    /// </summary>
    public class JsonTransportSerializer<E> : TransportSerializer<E>
    {
        private readonly JsonSerializerSettings mJsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        public JsonTransportSerializer():base()
        {
            MediaType = "application/json";
        }

        public override E GetObjectInternal(byte[] data, Encoding encoding = null)
        {
            return JsonConvert.DeserializeObject<E>(encoding.GetString(data), mJsonSerializerSettings);
        }

        public override byte[] GetDataInternal(E entity, Encoding encoding)
        {
            return encoding.GetBytes(JsonConvert.SerializeObject(entity, mJsonSerializerSettings));
        }
    }
}
