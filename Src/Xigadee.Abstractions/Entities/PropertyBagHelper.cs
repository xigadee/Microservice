﻿using System;
namespace Xigadee
{
    /// <summary>
    /// This is an extension helper class for the Property Bag.
    /// </summary>
    public static class PropertyBagHelper
    {
        #region PropertiesSet<K>...
        /// <summary>
        /// Sets a property.
        /// </summary>
        /// <param name="bag">The property bag.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="convert">The optional convert method. If this is not passed, the method will resolve via the key manager.</param>
        public static void PropertiesSet<K>(this IPropertyBag bag, string key, K value, Func<K, string> convert = null)
        {
            if (object.Equals(default(K), value))
            {
                bag.Properties[key] = null;
                return;
            }

            if (convert != null)
            {
                bag.Properties[key] = convert(value);
                return;
            }

            var km = RepositoryKeyManager.Resolve<K>();

            bag.Properties[key] = km.Serialize(value);
        }
        #endregion
        #region PropertiesGet<K>...
        /// <summary>
        /// Gets the property for the required key.
        /// </summary>
        /// <param name="bag">The property bag.</param>
        /// <param name="key">The key.</param>
        /// <param name="convert">The optional convert method. If this is not passed, the method will resolve the key manager.</param>
        /// <param name="defaultValue">The default value if not found.</param>
        /// <returns>The key, or null if not found.</returns>
        public static K PropertiesGet<K>(this IPropertyBag bag, string key, Func<string, K> convert = null, K defaultValue = default(K))
        {
            string value;
            if (!bag.Properties.TryGetValue(key, out value) || value == null)
                return defaultValue;

            if (convert != null)
                return convert(value);

            var km = RepositoryKeyManager.Resolve<K>();

            return km.Deserialize(value);
        }
        #endregion

        #region PropertiesSetRaw(string key, string value)
        /// <summary>
        /// Sets a property.
        /// </summary>
        /// <param name="bag">The property bag.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void PropertiesSetRaw(this IPropertyBag bag, string key, string value)
        {
            bag.Properties[key] = value;
        }
        #endregion
        #region PropertiesGetRaw(string key)
        /// <summary>
        /// Gets the property for the required key.
        /// </summary>
        /// <param name="bag">The property bag.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value if not found.</param>
        /// <returns>The key, or defaultValue if not found.</returns>
        public static string PropertiesGetRaw(this IPropertyBag bag, string key, string defaultValue = null)
        {
            string value;
            if (!bag.Properties.TryGetValue(key, out value))
                return defaultValue;

            return value;
        }
        #endregion
    }
}
