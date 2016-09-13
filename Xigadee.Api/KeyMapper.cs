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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class KeyMapper
    {
        public static Dictionary<Type, KeyMapper> sMaps;

        static KeyMapper()
        {
            sMaps = new Dictionary<Type, KeyMapper>();

            sMaps.Add(typeof(string), new KeyMapper<string>((s) => s, (s) => s));
            sMaps.Add(typeof(Guid), new KeyMapper<Guid>((s) => new Guid(s), (s) => s.ToString("N")));
            sMaps.Add(typeof(int), new KeyMapper<int>((s) => int.Parse(s), (s) => s.ToString()));
            sMaps.Add(typeof(long), new KeyMapper<long>((s) => long.Parse(s), (s) => s.ToString()));
            sMaps.Add(typeof(double), new KeyMapper<double>((s) => double.Parse(s), (s) => s.ToString()));
        }

        public static KeyMapper<KT> Add<KT>(KeyMapper<KT> mapper)
        {
            sMaps[typeof(KT)] = mapper;
            return mapper;
        }

        public static KeyMapper<KT> Resolve<KT>()
        {
            var type = typeof(KT);

            if (sMaps.ContainsKey(type))
                return sMaps[type] as KeyMapper<KT>;

            //Ok, do attribute lookup

            var attr = type.GetCustomAttributes(false)
                .OfType<KeyMapperAttribute>().FirstOrDefault();

            if (attr != null)
            {
                return Add(Activator.CreateInstance(attr.KeyMapper) as KeyMapper<KT>);
            }

            throw new KeyMapperResolutionException(type.Name);
        }
    }

    public class KeyMapper<K> : KeyMapper, IKeyMapper<K>
    {
        protected Func<string, K> mDeserializer;
        protected Func<K, string> mSerializer;

        public KeyMapper(Func<string, K> deserializer, Func<K, string> serializer)
        {
            mDeserializer = deserializer;
            mSerializer = serializer;
        }

        public K ToKey(string value)
        {
            return mDeserializer(value);
        }

        public string ToString(K value)
        {
            return mSerializer(value);
        }
    }
}
