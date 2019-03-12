using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Xigadee
{
    internal class TypeConverterRepositoryKeyManager<K> : RepositoryKeyManager<K>
        where K : IEquatable<K>
    {
        TypeConverter _converter;

        public TypeConverterRepositoryKeyManager(TypeConverter converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));

            if (!converter.CanConvertFrom(typeof(string)))
                throw new ArgumentOutOfRangeException();

            if (!converter.CanConvertTo(typeof(string)))
                throw new ArgumentOutOfRangeException();

            Serialize = (key) => _converter.ConvertToString(key);
            Deserialize = (s) => (K)_converter.ConvertFromString(s);

            TryDeserialize = (s) =>
            {
                try
                {
                    K key = Deserialize(s);
                    return (true, key);
                }
                catch (Exception)
                {
                    return (false, default(K));
                }
            };
        }
    }

}
