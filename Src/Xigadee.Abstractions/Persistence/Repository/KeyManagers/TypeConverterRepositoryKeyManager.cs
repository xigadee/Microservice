using System;
using System.ComponentModel;

namespace Xigadee
{
    internal class TypeConverterRepositoryKeyManager<K> : RepositoryKeyManager<K>
    {
        TypeConverter _converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConverterRepositoryKeyManager{K}"/> class.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <exception cref="ArgumentNullException">The converter cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown if the type passed cannot be converted to a string and back again.
        /// </exception>
        public TypeConverterRepositoryKeyManager(TypeConverter converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));

            if (!converter.CanConvertFrom(typeof(string)))
                throw new ArgumentOutOfRangeException($"{KeyType} cannot be converted to a string using the type converter passed.");

            if (!converter.CanConvertTo(typeof(string)))
                throw new ArgumentOutOfRangeException($"{KeyType} cannot be converted from a string using the type converter passed.");

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
