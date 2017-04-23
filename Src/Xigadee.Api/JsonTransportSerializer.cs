#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
    /// This the the base default Json serializer.
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
