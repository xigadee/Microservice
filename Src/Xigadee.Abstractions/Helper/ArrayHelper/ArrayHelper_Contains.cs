using System;
using System.Collections.Generic;

namespace Xigadee
{
    public static partial class ArrayHelper
    {
        #region Contains<T>(this IEnumerable<T> items, Predicate<T> action)
        /// <summary>
        /// This method scans a collection and returns true when an item is matched.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="items">The enumeration.</param>
        /// <param name="predic">The predicate that returns true when there is a match.</param>
        /// <returns>Returns true if an item is matched in the collection.</returns>
        public static bool Contains<T>(this IEnumerable<T> items, Predicate<T> predic)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predic == null) throw new ArgumentNullException("action");

            foreach (T item in items)
                if (predic(item))
                    return true;

            return false;
        }
        #endregion  
    }
}
