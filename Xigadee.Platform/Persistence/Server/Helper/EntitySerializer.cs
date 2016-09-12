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

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the serializer / deserializer for an entity.
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    public class EntitySerializer<E>
    {
        /// <summary>
        /// Holds the entity serializer
        /// </summary>
        public Func<E, string> Serializer { get; set; }

        /// <summary>
        /// Holds the entity deserializer
        /// </summary>
        public Func<string, E> Deserializer { get; set; }
    
        public EntitySerializer(Func<E, string> serializer, Func<string, E> deserializer)
        {
            Serializer = serializer;
            Deserializer = deserializer;
        }

        public EntitySerializer(Func<E, XElement> serializer, Func<XElement, E> deserializer)
        {
            Serializer = e => serializer(e).ToString();
            Deserializer = s => deserializer(XElement.Parse(s));
        }
    }
}
